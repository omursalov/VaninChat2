using Newtonsoft.Json;
using System.Net.Http.Headers;
using VaninChat2.Dto;
using VaninChat2.Models;
using VaninChat2.Workers.Crypto;
using VaninChat2.Workers.Internet;

namespace VaninChat2.Workers
{
    public class ConnectionWorker : IDisposable
    {
        private const string CONTROL_STRING = "06a9c0bf5bd7285e2c6b38d6cfefa5d0";
        private const string API_URL = "https://filebin.net";

        private readonly string _myName;
        private readonly string _myPass;
        private readonly string _companionName;

        private readonly string _bin;
        private readonly string _myTxtFileName;
        private readonly string _companionTxtFileName;

        private readonly ProxyWorker _proxyWorker;
        private readonly CryptoWorker _cryptoWorker;

        private FileObj _fileObj;

        public ConnectionWorker(
            string myName, string myPass, string companionName)
        {
            _myName = myName;
            _myPass = myPass;
            _companionName = companionName;

            _bin = SymbolShuffling(_myName, _companionName, CONTROL_STRING);
            _myTxtFileName = $"{SymbolShuffling(_myName, CONTROL_STRING)}.txt";
            _companionTxtFileName = $"{SymbolShuffling(_companionName, CONTROL_STRING)}.txt";

            _proxyWorker = new ProxyWorker(attempts: 20, delaySec: 10, cacheMinutes: 10);
            _cryptoWorker = new CryptoWorker();
        }

        public async Task<ConnectionInfo> ExecuteAsync()
        {
            if (await SendMyInfoAsync() && await WaitCompanionAsync())
            {
                return await GetCompanionInfoAsync();
            }

            return null;
        }

        public void Dispose()
        {
            _fileObj?.Dispose();
            _cryptoWorker?.Dispose();
        }

        #region Private
        private async Task<bool> SendMyInfoAsync()
            => await _proxyWorker.ExecuteAsync(async (httpClient) =>
            {
                var message = new MessageWorker(_cryptoWorker).DefinePassword(_myPass);
                using (_fileObj = new FileWorker(_cryptoWorker).CreateEncryptedTxtFile(_myTxtFileName, message))
                {
                    using var requestContent = new MultipartFormDataContent();
                    using var content = new ByteArrayContent(_fileObj.Bytes);
                    content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                    httpClient.DefaultRequestHeaders.Add("filename", _fileObj.Name);
                    requestContent.Add(content, _fileObj.Name, _fileObj.Name);

                    var url = $"{API_URL}/{_bin}/{_fileObj.Name}";
                    using var response = await httpClient.PostAsync(url, requestContent);

                    var responseContent = await response.Content.ReadAsStringAsync();
                    return response.StatusCode == System.Net.HttpStatusCode.Created;
                }
            });

        private async Task<bool> WaitCompanionAsync()
            => await _proxyWorker.ExecuteAsync(async (httpClient) =>
            {
                var url = $"{API_URL}/{_bin}";
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                using var response = await httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                var files = JsonConvert.DeserializeObject<BinDto>(responseContent).files;
                var companionTxtFile = files.FirstOrDefault(x => x.filename.Equals(_companionTxtFileName, StringComparison.OrdinalIgnoreCase));

                if (companionTxtFile == null)
                {
                    throw new Exception("waiting for companion txt file..");
                }

                return true;
            });

        private async Task<ConnectionInfo> GetCompanionInfoAsync()
        {
            string companionPass = null;

            var result = await _proxyWorker.ExecuteAsync(async (httpClient) =>
            {
                var url = $"{API_URL}/{_bin}/{_companionTxtFileName}";
                httpClient.DefaultRequestHeaders.Add("Cookie", "verified=2024-05-24");
                using var response = await httpClient.GetAsync(url);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("get companion info error..");
                }

                var result = await response.Content.ReadAsStringAsync();
                companionPass = new MessageWorker(_cryptoWorker).ExtractPassword(result);

                return true;
            });

            return result ? new ConnectionInfo(_myPass, companionPass) : null;
        }

        private string SymbolShuffling(params string[] values)
        {
            var characters = string.Concat(values).ToArray();
            Array.Sort(characters);
            return new string(characters);
        }
        #endregion
    }
}