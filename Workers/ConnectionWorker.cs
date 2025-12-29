using Newtonsoft.Json;
using System.Net.Http.Headers;
using VaninChat2.Dto;

namespace VaninChat2.Workers
{
    public class ConnectionWorker
    {
        private const string BIN = "ZgNmTY05XjQgNgBc";
        private const string ZIP_NAME = "OZFpyfTGOIERdzQ5";
        private const string TXT_FILE_NAME = "info.txt";
        private const string INTERNAL_PASS = "o0nZZZ0000ZZzZZ__)(FZ*7y-R}V?n3N&&^^&9";

        private readonly DateTime _startDateTimeUtc;
        private readonly int _minutes;
        private readonly string _postfix;

        private readonly string _myName;
        private readonly string _myPass;
        private readonly string _companionName;

        private readonly string _bin;
        private readonly string _myZipName;
        private readonly string _companionZipName;
        private readonly string _internalPass;

        private ConnectionInfo _info;

        public ConnectionInfo Info => _info;

        public ConnectionWorker(
            string myName, string myPass, string companionName, int minutes = 3)
        {
            _startDateTimeUtc = DateTime.UtcNow;
            _minutes = minutes;
            _postfix = _startDateTimeUtc.AddMinutes(minutes).ToString("yyyy-MM-dd-HH-mm");
            _myName = myName;
            _myPass = myPass;
            _companionName = companionName;
            _bin = $"{BIN}-{_postfix}";
            _myZipName = $"{ZIP_NAME}-{_myName}-{_postfix}.zip";
            _companionZipName = $"{ZIP_NAME}-{_companionName}-{_postfix}.zip";
            _internalPass = $"{INTERNAL_PASS}-{_postfix}";
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
            using (var zipObj = new ZipWorker(_internalPass)
                .CreateTxtFileAndPutToArchieve(_myZipName, TXT_FILE_NAME, _myPass))
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
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                using var response = await httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                var files = JsonConvert.DeserializeObject<BinDto>(responseContent).files;
                var companionZip = files.FirstOrDefault(x => x.filename.Equals(_companionName, StringComparison.OrdinalIgnoreCase));

                if (companionZip == null)
                    throw new Exception("waiting for companion zip..");

                return true;
            });

        private bool IsExpired()
        {
            return true;
        }
        #endregion
    }
}