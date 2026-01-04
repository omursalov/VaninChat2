using VaninChat2.Helpers;
using VaninChat2.Helpers.Crypto;
using VaninChat2.Models;

namespace VaninChat2.Workers
{
    public class ConnectionWorker
    {
        private const string CONTROL_STRING = "rF2JF67O3JGUuLkaF8SDJdUx";

        private readonly string _myName;
        private readonly string _myPass;
        private readonly string _companionName;

        private readonly string _salt;

        private readonly string _bin;
        private readonly string _myTxtFileName;
        private readonly string _companionTxtFileName;

        private readonly CryptoHelper _cryptoWorker;
        private readonly DropboxWorker _dropboxWorker;

        public ConnectionWorker(
            string myName, string myPass, string companionName)
        {
            _myName = myName;
            _myPass = myPass;
            _companionName = companionName;

            _salt = new SaltHelper().Generate();
            _bin = StringHelper.SortCharacters(_myName, _companionName, CONTROL_STRING, _salt);
            _myTxtFileName = $"{StringHelper.SortCharacters(_myName, CONTROL_STRING, _salt)}.txt";
            _companionTxtFileName = $"{StringHelper.SortCharacters(_companionName, CONTROL_STRING, _salt)}.txt";

            _cryptoWorker = new CryptoHelper(CONTROL_STRING, _salt);
            _dropboxWorker = new DropboxWorker();
        }

        public async Task<ConnectionInfo?> ExecuteAsync()
            => await SendMyPassAsync() ? await GetCompanionPassAsync() : null;

        #region Private
        private async Task<bool> SendMyPassAsync()
        {
            var message = MessageHelper.DefinePassword(_myPass);
            var encryptedText = _cryptoWorker.Encrypt(message);
            return await new AttemptHelper(number: 10, delaySec: 0).ExecuteAsync(async () =>
                await _dropboxWorker.CreateTxtFileAsync(_bin, $"connect_{_myTxtFileName}", encryptedText));
        }

        private async Task<ConnectionInfo?> GetCompanionPassAsync()
        {
            var message = await new AttemptHelper(number: 50, delaySec: 2).ExecuteAsync(async () =>
                await _dropboxWorker.GetTxtFileAsync(_bin, $"connect_{_myTxtFileName}", deleteFlag: true)); // _companionTxtFileName
            var companionPass = MessageHelper.ExtractPassword(message);
            var passwords = new[] { _myPass, companionPass, _salt };
            return new ConnectionInfo(_myName, _companionName,
                _bin, _myTxtFileName, _companionTxtFileName, passwords);
        }
        #endregion
    }
}