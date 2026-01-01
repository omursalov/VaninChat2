using System.Text.RegularExpressions;
using VaninChat2.Helpers.Crypto;

namespace VaninChat2.Helpers
{
    public class MessageHelper
    {
        private const string PREFIX = "[[[PASS:";
        private const string POSTFIX = "]]]";

        private readonly CryptoHelper _cryptoWorker;

        public MessageHelper(CryptoHelper cryptoWorker)
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