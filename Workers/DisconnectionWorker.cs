using VaninChat2.Helpers;
using VaninChat2.Helpers.Crypto;
using VaninChat2.Models;

namespace VaninChat2.Workers
{
    public class DisconnectionWorker
    {
        private readonly ConnectionInfo _connection;
        private readonly CryptoHelper _cryptoWorker;
        private readonly DropboxWorker _dropboxWorker;

        public DisconnectionWorker(ConnectionInfo connection)
        {
            _connection = connection;
            _cryptoWorker = new CryptoHelper(connection.CommonPassword, connection.CommonSalt);
            _dropboxWorker = new DropboxWorker();
        }

        public async Task<bool> ExecuteAsync()
            => await new AttemptHelper(number: 10, delaySec: 0).ExecuteAsync(async () =>
            await _dropboxWorker.CreateTxtFileAsync(_connection.Bin, $"disconnect_{_connection.MyTxtFileName}", string.Empty));
    }
}