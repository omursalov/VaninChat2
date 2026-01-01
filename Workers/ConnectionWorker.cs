using Newtonsoft.Json;
using VaninChat2.Dto;
using VaninChat2.Helpers;
using VaninChat2.Helpers.Crypto;
using VaninChat2.Helpers.Internet;
using VaninChat2.Models;

namespace VaninChat2.Workers
{
    public class ConnectionWorker
    {
        private const string CONTROL_STRING = "06a9c0bf5bd7285e2c6b38d6cfefa5d0";
        private const string API_URL = "https://filebin.net";
        private const string COOKIE = "verified=2024-05-24";

        private readonly string _myName;
        private readonly string _myPass;
        private readonly string _companionName;

        private readonly string _bin;
        private readonly string _myTxtFileName;
        private readonly string _companionTxtFileName;

        private readonly ProxyHelper _proxyWorker;
        private readonly CryptoHelper _cryptoWorker;
        private readonly FileSharingWorker _fileSharingWorker;

        public ConnectionWorker(
            string myName, string myPass, string companionName)
        {
            _myName = myName;
            _myPass = myPass;
            _companionName = companionName;

            _bin = SymbolShuffling(_myName, _companionName, CONTROL_STRING);
            _myTxtFileName = $"{SymbolShuffling(_myName, CONTROL_STRING)}.txt";
            _companionTxtFileName = $"{SymbolShuffling(_companionName, CONTROL_STRING)}.txt";

            _proxyWorker = new ProxyHelper(attempts: 20,
                delaySec: 3, httpClientTimeoutSec: 10, cacheMinutes: 10);

            var passPhrase = CONTROL_STRING;
            var saltValue = new SaltHelper().Generate();
            var hashAlgorithm = "SHA256";
            var passwordIterations = 2;
            var initVector = "!1A3g2D4s9K556g7";
            var keySize = 256;

            _cryptoWorker = new CryptoHelper(passPhrase, saltValue,
                hashAlgorithm, passwordIterations, initVector, keySize);
        }

        public async Task<ConnectionInfo?> ExecuteAsync()
        {
            if (await SendMyInfoAsync() && await WaitCompanionAsync())
            {
                return await GetCompanionInfoAsync();
            }

            return null;
        }

        #region Private
        private async Task<bool> SendMyInfoAsync()
        {
            var message = new MessageHelper(_cryptoWorker).DefinePassword(_myPass);
            return await _fileSharingWorker.PostAsync(_bin, _myTxtFileName, message);
        }

        private async Task<bool> WaitCompanionAsync()
            => await _proxyWorker.ExecuteAsync(async (httpClient) =>
            {
                var url = $"{API_URL}/{_bin}";
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                using var response = await httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                var files = JsonConvert.DeserializeObject<BinDto>(responseContent).files;
                var companionTxtFile = files.FirstOrDefault(
                    x => x.filename.Equals(_companionTxtFileName, StringComparison.OrdinalIgnoreCase));

                /*if (companionTxtFile == null)
                {
                    throw new Exception("waiting for companion txt file..");
                }*/

                return true;
            });

        private async Task<ConnectionInfo?> GetCompanionInfoAsync()
        {
            string companionPass = null;

            var result = await _proxyWorker.ExecuteAsync(async (httpClient) =>
            {
                var url = $"{API_URL}/{_bin}/{_myTxtFileName}";
                httpClient.DefaultRequestHeaders.Add("Cookie", COOKIE);
                using var getResponse = await httpClient.GetAsync(url);

                if (getResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("get companion info error..");
                }

                var result = await getResponse.Content.ReadAsStringAsync();
                companionPass = new MessageHelper(_cryptoWorker).ExtractPassword(result);

                await httpClient.DeleteAsync(url);

                return true;
            });

            return result ? new ConnectionInfo(_myName, _companionName, _bin, _myPass, companionPass) : null;
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