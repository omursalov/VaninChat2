using Newtonsoft.Json;
using VaninChat2.Dto;

namespace VaninChat2.Workers.Internet
{
    public class ProxyWorker
    {
        private const string KEY = "ca08cb8ae5f2b6582064f7c66a142590";
        private const string API_URL = "https://api.best-proxies.ru";

        private const int DEFAULT_ATTEMPTS = 5;
        private const int DEFAULT_DELAY_SEC = 5;

        private readonly int _attempts;
        private readonly int _delaySec;

        public ProxyWorker(int? attempts = null, int? delaySec = null)
        {
            _attempts = attempts.HasValue ? attempts.Value : DEFAULT_ATTEMPTS;
            _delaySec = delaySec.HasValue ? delaySec.Value : DEFAULT_DELAY_SEC;
        }

        public async Task<bool> ExecuteAsync(Func<HttpClient, Task<bool>> funcAsync)
        {
            var result = false;

            for (var i = 0; i < _attempts; i++)
            {
                try
                {
                    using var httpClientHandler = new HttpClientHandler
                    {
                        Proxy = (await GetRandomAsync()).TryGetHttpProxy()
                    };
                    using var httpClient = new HttpClient(httpClientHandler);
                    result = await funcAsync(httpClient);
                }
                catch (Exception ex)
                {
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

        private async Task<ProxyItem> GetRandomAsync()
        {
            using var httpClient = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Get,
                $"{API_URL}/proxylist.json?key={KEY}&type=https&level=1,2&speed=1,2,3&google=1&limit=0");
            using var response = await httpClient.SendAsync(request);
            var value = await response.Content.ReadAsStringAsync();
            var array = JsonConvert.DeserializeObject<ProxyItem[]>(value)
                .Where(x => x.country_code.ToLower() != "ru").ToArray();
            return array[new Random().Next(0, array.Length)];
        }
    }
}