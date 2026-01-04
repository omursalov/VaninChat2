using VaninChat2.Helpers;
using VaninChat2.Helpers.Crypto;
using VaninChat2.Models;

namespace VaninChat2.Workers
{
    public class DiscussionWorker
    {
        private readonly ConnectionInfo _connection;
        private readonly IList<string> _myMessages;
        private readonly IList<string> _companionMessages;

        private readonly CryptoHelper _cryptoWorker;
        private readonly DropboxWorker _dropboxWorker;

        public DiscussionWorker(ConnectionInfo connection)
        {
            _myMessages = new List<string>();
            _companionMessages = new List<string>();
            _cryptoWorker = new CryptoHelper(connection.CommonPassword, connection.CommonSalt);
            _dropboxWorker = new DropboxWorker();
        }

        public async Task<bool> SendAsync(string text)
        {
            var myTxtFileName = $"msg_{_myMessages.Count}_{_connection.MyTxtFileName}";

            var encryptedText = _cryptoWorker.Encrypt(text);
            var result = await new AttemptHelper(number: 10, delaySec: 2).ExecuteAsync(async () =>
                await _dropboxWorker.CreateTxtFileAsync(_connection.Bin, myTxtFileName, encryptedText));

            if (result)
            {
                _myMessages.Add(text);
            }

            return result;
        }
    }
}