using Dropbox.Api;
using Dropbox.Api.Files;
using System.Text;
using VaninChat2.Common;
using VaninChat2.Helpers;
using VaninChat2.Helpers.Internet;

namespace VaninChat2.Workers
{
    public class DropboxWorker
    {
        private readonly ProxyHelper _proxyHelper;

        public DropboxWorker(int httpClientTimeoutSec = 10)
        {
            _proxyHelper = new ProxyHelper(httpClientTimeoutSec: httpClientTimeoutSec);
        }

        public async Task<bool> CreateTxtFileAsync(string bin, string fileName, string text)
            => await new AttemptHelper(number: 10, delaySec: 2).ExecuteAsync(async () => 
                await _proxyHelper.ExecuteAsync(async (httpClient) =>
                {
                    using (var dbx = new DropboxClient(AppSettings.DROPBOX_ACCESS_TOKEN, new DropboxClientConfig
                    {
                        HttpClient = httpClient
                    }))
                    {
                        var searchSettings = new ListFolderArg("", recursive: false);
                        var folderList = await dbx.Files.ListFolderAsync(searchSettings);

                        if (folderList.Entries.Count(x => x.Name == bin) == 0)
                        {
                            var createFolderResult = await dbx.Files.CreateFolderV2Async($"/{bin}");

                            if (!createFolderResult.Metadata.IsFolder)
                            {
                                throw new Exception("create folder error..");
                            }
                        }

                        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(text)))
                        {
                            var fileMd = await dbx.Files.UploadAsync(
                                $"/{bin}/{fileName}", WriteMode.Overwrite.Instance, body: ms);
                            return fileMd.IsFile;
                        }
                    }
                }));

        public async Task<string> GetTxtFileAsync(string bin, string fileName, bool deleteFlag = false)
            => await new AttemptHelper(number: 25, delaySec: 2).ExecuteAsync(async () => 
                await _proxyHelper.ExecuteAsync(async (httpClient) =>
                {
                    using (var dbx = new DropboxClient(AppSettings.DROPBOX_ACCESS_TOKEN, new DropboxClientConfig
                    {
                        HttpClient = httpClient
                    }))
                    {
                        using (var response = await dbx.Files.DownloadAsync($"/{bin}/{fileName}"))
                        {
                            var responseString = await response.GetContentAsStringAsync();

                            if (deleteFlag)
                            {
                                await dbx.Files.DeleteV2Async($"/{bin}/{fileName}");
                            }

                            return responseString;
                        }
                    }
                }));
    }
}