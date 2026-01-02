using VaninChat2.Helpers.Crypto;
using VaninChat2.Helpers.Internet;
using VaninChat2.Models;

namespace VaninChat2.Workers
{
    public class MessageWorker
    {
        private readonly ConnectionInfo _connection;

        private readonly ProxyHelper _proxyWorker;
        private readonly CryptoHelper _cryptoWorker;

        public MessageWorker(ConnectionInfo connection)
        {
            _connection = connection;

            var passPhrase = _connection.CommonPassword;
            var saltValue = new SaltHelper().Generate();
            var hashAlgorithm = "SHA256";
            var passwordIterations = 2;
            var initVector = "ELK5G~wmC5lAjF@{";
            var keySize = 256;

            _cryptoWorker = new CryptoHelper(passPhrase, saltValue);
        }

        public void Send(string text)
        {

        }
    }
}