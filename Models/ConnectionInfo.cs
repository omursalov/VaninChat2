namespace VaninChat2.Models
{
    public class ConnectionInfo
    {
        public string CommonPassword { get; }

        public string MyName { get; }
        public string CompanionName { get; }

        public string Bin { get; }

        public DateTime StartUtc {  get; }

        public int ExpiredInMinutes { get; }

        public ConnectionInfo(string myName, string companionName, string bin, params string[] passwords)
        {
            MyName = myName;
            CompanionName = companionName;

            Bin = bin;

            var characters = string.Concat(passwords).ToArray();
            Array.Sort(characters);
            CommonPassword = new string(characters);

            StartUtc = DateTime.UtcNow;
            ExpiredInMinutes = 10;
        }
    }
}