using Newtonsoft.Json;
using VaninChat2.Common;
using VaninChat2.Dto;

namespace VaninChat2.Helpers.Internet
{
    public class ProxyHelper
    {
        private const string API_URL = "https://api.best-proxies.ru";

        private readonly string _key;
        private readonly int _httpClientTimeoutSec;

        public ProxyHelper(string key = null, int httpClientTimeoutSec = 25)
        {
            _key = !string.IsNullOrWhiteSpace(key) ? key : AppSettings.BEST_PROXIES_KEY;
            _httpClientTimeoutSec = httpClientTimeoutSec;
        }

        public static async Task<bool> CheckApiKey(string key)
            => await new AttemptHelper(number: 3, delaySec: 2).ExecuteAsync(async () =>
            {
                var url = $"{API_URL}/key.txt?key={key}";
                using var httpClient = new HttpClient();
                using var getResponse = await httpClient.GetAsync(url);
                return getResponse.StatusCode == System.Net.HttpStatusCode.OK;
            });

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
                $"{API_URL}/proxylist.json?key={_key}&type=https&level=1,2&speed=1,2,3&google=1&limit=0");
            using var response = await httpClient.SendAsync(request);
            var value = await response.Content.ReadAsStringAsync();
            var array = JsonConvert.DeserializeObject<ProxyDto[]>(value)
                .Where(x => x.country_code.ToLower() != "ru").ToArray();
            return array[new Random().Next(0, array.Length)];
        }
    }
}