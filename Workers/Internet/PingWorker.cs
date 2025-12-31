using System.Net.NetworkInformation;

namespace VaninChat2.Workers
{
    public class PingWorker
    {
        public async Task<bool> InternetConnectionCheckAsync()
        {
            var myPing = new Ping();
            var host = "google.com";
            var buffer = new byte[32];
            var timeout = 1000;
            var pingOptions = new PingOptions();

            PingReply reply = null;

            try
            {
                reply = await myPing.SendPingAsync(host, timeout, buffer, pingOptions);
            }
            catch
            {
            }
            
            return reply?.Status == IPStatus.Success;
        }
    }
}