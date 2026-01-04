using VaninChat2.Helpers;
using VaninChat2.Helpers.Crypto;

namespace VaninChat2.Models
{
    public class ConnectionInfo
    {
        public string CommonPassword { get; }
        public string CommonSalt { get; }

        public string MyName { get; }
        public string CompanionName { get; }

        public string Bin { get; }
        public string MyTxtFileName { get; }
        public string MyCompanionTxtFileName { get; }

        public DateTime StartUtc { get; }

        public int ExpiredInMinutes { get; }

        public ConnectionInfo(string myName, string companionName,
            string bin, string myTxtFileName, string companionTxtFileName,
            params string[] passwords)
        {
            MyName = myName;
            CompanionName = companionName;

            Bin = bin;
            MyTxtFileName = myTxtFileName;
            MyCompanionTxtFileName = companionTxtFileName;

            CommonPassword = StringHelper.SortCharacters(passwords);
            CommonSalt = new SaltHelper().Generate();

            StartUtc = DateTime.UtcNow;
            ExpiredInMinutes = 10;
        }
    }
}