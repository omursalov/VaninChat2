using System.Text.RegularExpressions;
using VaninChat2.Workers.Crypto;

namespace VaninChat2.Workers
{
    public class MessageWorker
    {
        private const string PREFIX = "[[[PASS:";
        private const string POSTFIX = "]]]";

        private readonly CryptoWorker _cryptoWorker;

        public MessageWorker(CryptoWorker cryptoWorker)
        {
            _cryptoWorker = cryptoWorker;
        }

        public string DefinePassword(string pass)
            => $"{PREFIX}{pass}{POSTFIX}";

        public string ExtractPassword(string value)
        {
            var lines = Regex.Split(value, "\r\n|\r|\n");
            return _cryptoWorker.Decrypt(lines[4])
                .Replace(PREFIX, string.Empty)
                .Replace(POSTFIX, string.Empty);
        }
    }
}