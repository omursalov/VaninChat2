namespace VaninChat2.Helpers.Internet
{
    public static class PingHelper
    {
        public static async Task<bool> InternetConnectionCheckAsync()
        {
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(5);
                using var request = new HttpRequestMessage(HttpMethod.Head, "https://www.google.com");
                using var response = await client.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}