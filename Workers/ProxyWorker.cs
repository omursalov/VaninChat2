using Newtonsoft.Json;
using VaninChat2.Dto;

namespace VaninChat2.Workers
{
    public class ProxyWorker
    {
        private const string KEY = "ca08cb8ae5f2b6582064f7c66a142590";
        private const string API_URL = "https://api.best-proxies.ru";

        public async Task<bool> ExecuteAsync(Func<HttpClient, Task<bool>> funcAsync, int attempts = 5)
        {
            var result = false;

            for (var i = 0; i < attempts; i++)
            {
                using var httpClientHandler = new HttpClientHandler
                {
                    Proxy = (await GetRandomAsync()).TryGetHttpProxy()
                };
                using var httpClient = new HttpClient(httpClientHandler);

                try
                {
                    result = await funcAsync(httpClient);
                }
                catch (Exception ex)
                {
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