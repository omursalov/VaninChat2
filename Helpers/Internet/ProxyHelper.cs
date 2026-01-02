using Newtonsoft.Json;
using VaninChat2.Dto;

namespace VaninChat2.Helpers.Internet
{
    public class ProxyHelper
    {
        private const string KEY = "ca08cb8ae5f2b6582064f7c66a142590";
        private const string API_URL = "https://api.best-proxies.ru";

        private readonly int _httpClientTimeoutSec;

        public ProxyHelper(int httpClientTimeoutSec)
            => _httpClientTimeoutSec = httpClientTimeoutSec;

        public async Task<T> ExecuteAsync<T>(Func<HttpClient, Task<T>> funcAsync)
        {
            using var httpClientHandler = new HttpClientHandler
            {
                Proxy = (await GetRandomAsync()).TryGetHttpProxy()
            };

            using var httpClient = new HttpClient(httpClientHandler)
            {
                Timeout = TimeSpan.FromSeconds(_httpClientTimeoutSec)
            };

            return await funcAsync(httpClient);
        }

        private async Task<ProxyDto> GetRandomAsync()
        {
            using var httpClient = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Get,
                $"{API_URL}/proxylist.json?key={KEY}&type=https&level=1,2&speed=1,2,3&google=1&limit=0");
            using var response = await httpClient.SendAsync(request);
            var value = await response.Content.ReadAsStringAsync();
            var array = JsonConvert.DeserializeObject<ProxyDto[]>(value)
                .Where(x => x.country_code.ToLower() != "ru").ToArray();
            return array[new Random().Next(0, array.Length)];
        }
    }
}