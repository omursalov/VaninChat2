using IronZip;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using VaninChat2.Dto;

namespace VaninChat2.Workers
{
    public class ConnectionWorker
    {
        private const string CONTROL_STRING = "Bw8UTFezNR01ma9ZZS8uo68MvYVHrwafeLvnRKl6KCJ";

        private readonly DateTime _startDateTimeUtc;
        private readonly int _minutes;

        private readonly string _myName;
        private readonly string _myPass;
        private readonly string _companionName;

        private readonly string _bin;
        private readonly string _myZipName;
        private readonly string _companionZipName;

        private ConnectionInfo _info;

        public ConnectionInfo Info => _info;

        public ConnectionWorker(
            string myName, string myPass, string companionName, int minutes = 3)
        {
            _startDateTimeUtc = DateTime.UtcNow;
            _minutes = minutes;

            _myName = myName;
            _myPass = myPass;
            _companionName = companionName;

            _bin = SymbolShuffling(_myName, _companionName, CONTROL_STRING);
            _myZipName = $"{SymbolShuffling(_myName, CONTROL_STRING)}.zip";
            _companionZipName = $"{SymbolShuffling(_companionName, CONTROL_STRING)}.zip";
        }

        public async Task<bool> ExecuteAsync()
            => await SendMyInfoAsync()
            ? await WaitCompanionAsync()
            ? await GetCompanionInfoAsync()
            : false : false;

        #region Private
        private async Task<bool> SendMyInfoAsync()
            => await new ProxyWorker().ExecuteAsync(async (httpClient) =>
        {
            using (var zipObj = new ZipWorker(CONTROL_STRING)
                .CreateTxtFileAndPutToArchieve(_myZipName, "info.txt", _myPass))
            {
                using var requestContent = new MultipartFormDataContent();

                using var content = new ByteArrayContent(zipObj.Bytes);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
                httpClient.DefaultRequestHeaders.Add("filename", zipObj.Name);
                requestContent.Add(content, zipObj.Name, zipObj.Name);

                var url = $"https://filebin.net/{_bin}/{zipObj.Name}";
                using var response = await httpClient.PostAsync(url, requestContent);

                var responseContent = await response.Content.ReadAsStringAsync();
                return response.StatusCode == System.Net.HttpStatusCode.Created;
            }
        });

        private async Task<bool> WaitCompanionAsync()
            => await new ProxyWorker().ExecuteAsync(async (httpClient) =>
            {
                var url = $"https://filebin.net/{_bin}";
                httpClient.DefaultRequestHeaders.Add("bin", _bin);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                using var response = await httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                var files = JsonConvert.DeserializeObject<BinDto>(responseContent).files;
                var companionZip = files.FirstOrDefault(x => x.filename.Equals(_companionZipName, StringComparison.OrdinalIgnoreCase));

                if (companionZip == null)
                    throw new Exception("waiting for companion zip..");

                return true;
            });

        private async Task<bool> GetCompanionInfoAsync()
            => await new ProxyWorker().ExecuteAsync(async (httpClient) =>
            {
                var url = $"https://filebin.net/{_bin}/{_companionZipName}";
                httpClient.DefaultRequestHeaders.Add("bin", _bin);
                httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
                using var response = await httpClient.GetAsync(url);
                var bytes = await response.Content.ReadAsByteArrayAsync();
                File.WriteAllBytes(_companionZipName, bytes);

                /*using (var archive = new IronZipArchive(await response.Content.ReadAsByteArrayAsync(), CONTROL_STRING))
                {
                    var dir = Directory.GetCurrentDirectory().TrimEnd('\\', '/');
                    archive.SaveAs(dir + "/" + _companionZipName);
                }*/
                
                /*using (var fs = new FileStream(_companionZipName, FileMode.CreateNew))
                {
                    await response.Content.CopyToAsync(fs);
                }*/

                /*var files = JsonConvert.DeserializeObject<BinDto>(responseContent).files;
                var companionZip = files.FirstOrDefault(x => x.filename.Equals(_companionName, StringComparison.OrdinalIgnoreCase));

                if (companionZip == null)
                    throw new Exception("waiting for companion zip..");*/

                return true;
            });

        private string SymbolShuffling(params string[] values)
        {
            var characters = string.Concat(values).ToArray();
            Array.Sort(characters);
            return new string(characters);
        }

        private bool IsExpired()
        {
            return true;
        }
        #endregion
    }
}