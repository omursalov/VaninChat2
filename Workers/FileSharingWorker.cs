using System.Net.Http.Headers;
using System.Text;
using VaninChat2.Helpers;
using VaninChat2.Helpers.Crypto;
using VaninChat2.Helpers.Internet;

namespace VaninChat2.Workers
{
    public class FileSharingWorker
    {
        private const string API_URL = "https://filebin.net";
        private const string COOKIE = "verified=2024-05-24";

        private readonly ProxyHelper _proxyWorker;
        private readonly CryptoHelper _cryptoWorker;

        public FileSharingWorker(string passPhrase)
        {
            _proxyWorker = new ProxyHelper(attempts: 20,
                delaySec: 3, httpClientTimeoutSec: 10, cacheMinutes: 10);

            var saltValue = new SaltHelper().Generate();
            var hashAlgorithm = "SHA256";
            var passwordIterations = 2;
            var initVector = "!1A3g2D4s9K556g7";
            var keySize = 256;

            _cryptoWorker = new CryptoHelper(passPhrase, saltValue,
                hashAlgorithm, passwordIterations, initVector, keySize);
        }

        public async Task<bool> PostAsync(string bin, string fileName, string text)
            => await _proxyWorker.ExecuteAsync(async (httpClient) =>
            {
                var encryptedText = _cryptoWorker.Encrypt(text);
                var bytes = Encoding.UTF8.GetBytes(encryptedText);

                using var requestContent = new MultipartFormDataContent();
                using var content = new ByteArrayContent(bytes);
                content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                httpClient.DefaultRequestHeaders.Add("filename", fileName);
                requestContent.Add(content, fileName, fileName);

                var url = $"{API_URL}/{bin}/{fileName}";
                using var response = await httpClient.PostAsync(url, requestContent);

                var responseContent = await response.Content.ReadAsStringAsync();
                return response.StatusCode == System.Net.HttpStatusCode.Created;
            });
    }
}
