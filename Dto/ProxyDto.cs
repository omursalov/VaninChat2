using System.Net;

namespace VaninChat2.Dto
{
    public class ProxyDto
    {
        public string ip { get; set; }
        public int port { get; set; }
        public string country_code { get; set; }

        public WebProxy TryGetHttpProxy() => new WebProxy
        {
            Address = new Uri($"http://{ip}:{port}"),
            Credentials = null,
            UseDefaultCredentials = false
        };
    }
}