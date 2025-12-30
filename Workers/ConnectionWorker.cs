using Newtonsoft.Json;
using System.Net.Http.Headers;
using VaninChat2.Dto;

namespace VaninChat2.Workers
{
    public class ConnectionWorker
    {
        private const string CONTROL_STRING = "06a9c0bf5bd7285e2c6b38d6cfefa5d0";
        private const string API_URL = "https://filebin.net";

        private readonly DateTime _startDateTimeUtc;
        private readonly int _minutes;

        private readonly string _myName;
        private readonly string _myPass;
        private readonly string _companionName;

        private readonly string _bin;
        private readonly string _myTxtFileName;
        private readonly string _companionTxtFileName;

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
            _myTxtFileName = $"{SymbolShuffling(_myName, CONTROL_STRING)}.txt";
            _companionTxtFileName = $"{SymbolShuffling(_companionName, CONTROL_STRING)}.txt";
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
            using (var fileObj = new FileWorker()
                .CreateEncryptedTxtFile(_myTxtFileName, $"[[[PASS:{_myPass}]]]"))
            {
                using var requestContent = new MultipartFormDataContent();

                using var content = new ByteArrayContent(fileObj.Bytes);
                content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                httpClient.DefaultRequestHeaders.Add("filename", fileObj.Name);
                requestContent.Add(content, fileObj.Name, fileObj.Name);

                var url = $"{API_URL}/{_bin}/{fileObj.Name}";
                using var response = await httpClient.PostAsync(url, requestContent);

                var responseContent = await response.Content.ReadAsStringAsync();
                return response.StatusCode == System.Net.HttpStatusCode.Created;
            }
        });

        private async Task<bool> WaitCompanionAsync()
            => await new ProxyWorker().ExecuteAsync(async (httpClient) =>
            {
                var url = $"{API_URL}/{_bin}";
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                using var response = await httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                var files = JsonConvert.DeserializeObject<BinDto>(responseContent).files;
                var companionTxtFile = files.FirstOrDefault(x => x.filename.Equals(_companionTxtFileName, StringComparison.OrdinalIgnoreCase));

                if (companionTxtFile == null)
                    throw new Exception("waiting for companion txt file..");

                return true;
            });

        private async Task<bool> GetCompanionInfoAsync()
            => await new ProxyWorker().ExecuteAsync(async (httpClient) =>
            {
                var url = $"{API_URL}/{_bin}/{_myTxtFileName}";
                httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
                using var response = await httpClient.GetAsync(url);
                var bytes = await response.Content.ReadAsByteArrayAsync();

                var result = new FileWorker().Decrypt(bytes);

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