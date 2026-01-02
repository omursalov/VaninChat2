using System.Net.Http.Headers;
using System.Text;
using VaninChat2.Helpers.Internet;

namespace VaninChat2.Workers
{
    public class FileSharingWorker
    {
        private const string API_URL = "https://filebin.net";
        private const string COOKIE = "verified=2024-05-24";

        private readonly ProxyHelper _proxyWorker;

        public FileSharingWorker()
        {
            _proxyWorker = new ProxyHelper(httpClientTimeoutSec: 10);
        }

        public async Task<bool> PostAsync(string bin, string fileName, string text)
            => await _proxyWorker.ExecuteAsync(async (httpClient) =>
            {
                var bytes = Encoding.UTF8.GetBytes(text);

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

        public async Task<string> GetAsync(string bin, string fileName, bool deleteFlag = false)
            => await _proxyWorker.ExecuteAsync(async (httpClient) =>
            {
                var url = $"{API_URL}/{bin}/{fileName}";
                httpClient.DefaultRequestHeaders.Add("Cookie", COOKIE);
                using var getResponse = await httpClient.GetAsync(url);

                if (getResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("get companion info error..");
                }

                var result = await getResponse.Content.ReadAsStringAsync();

                if (deleteFlag)
                {
                    await httpClient.DeleteAsync(url);
                }

                return result;
            });
    }
}