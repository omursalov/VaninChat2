using Newtonsoft.Json;
using System.Net;
using VaninChat2.Dto;

namespace VaninChat2.Workers.Internet
{
    public class ProxyWorker
    {
        private const string KEY = "ca08cb8ae5f2b6582064f7c66a142590";
        private const string API_URL = "https://api.best-proxies.ru";

        private const int DEFAULT_ATTEMPTS = 5;
        private const int DEFAULT_DELAY_SEC = 5;
        private const int DEFAULT_CACHE_MINUTES = 5;

        private readonly int _attempts;
        private readonly int _delaySec;
        private readonly int _cacheMinutes;

        private WebProxy _cachedProxy;
        private DateTime _cacheDateUtc;

        public ProxyWorker(int? attempts = null, int? delaySec = null, int? cacheMinutes = null)
        {
            _attempts = attempts.HasValue ? attempts.Value : DEFAULT_ATTEMPTS;
            _delaySec = delaySec.HasValue ? delaySec.Value : DEFAULT_DELAY_SEC;
            _cacheMinutes = cacheMinutes.HasValue ? cacheMinutes.Value : DEFAULT_CACHE_MINUTES;
        }

        public async Task<bool> ExecuteAsync(Func<HttpClient, Task<bool>> funcAsync)
        {
            var result = false;

            for (var i = 0; i < _attempts; i++)
            {
                try
                {
                    if (_cachedProxy == null || DateTime.UtcNow.Subtract(_cacheDateUtc).TotalMinutes >= _cacheMinutes)
                    {
                        _cachedProxy = (await GetRandomAsync()).TryGetHttpProxy();
                        _cacheDateUtc = DateTime.UtcNow;
                    }

                    using var httpClientHandler = new HttpClientHandler
                    {
                        Proxy = _cachedProxy
                    };

                    using var httpClient = new HttpClient(httpClientHandler)
                    {
                        Timeout = TimeSpan.FromSeconds(10)
                    };

                    result = await funcAsync(httpClient);
                }
                catch (Exception ex)
                {
                    _cachedProxy = null;
                }
                finally
                {
                    await Task.Delay(_delaySec * 1000);
                }

                if (result)
                    break;
            }

            return result;
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