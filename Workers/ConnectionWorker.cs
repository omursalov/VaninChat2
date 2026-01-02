using VaninChat2.Helpers;
using VaninChat2.Helpers.Crypto;
using VaninChat2.Models;

namespace VaninChat2.Workers
{
    public class ConnectionWorker
    {
        private const string CONTROL_STRING = "rF2JFO3JGUu8xnRpc9OZfGLkaF8SDJdU";

        private readonly string _myName;
        private readonly string _myPass;
        private readonly string _companionName;

        private readonly string _bin;
        private readonly string _myTxtFileName;
        private readonly string _companionTxtFileName;

        private readonly CryptoHelper _cryptoWorker;
        private readonly FileSharingWorker _fileSharingWorker;

        public ConnectionWorker(
            string myName, string myPass, string companionName)
        {
            _myName = myName;
            _myPass = myPass;
            _companionName = companionName;

            var saltValue = new SaltHelper().Generate();

            _bin = StringHelper.SortCharacters(_myName, _companionName, CONTROL_STRING, saltValue);
            _myTxtFileName = $"{StringHelper.SortCharacters(_myName, CONTROL_STRING, saltValue)}.txt";
            _companionTxtFileName = $"{StringHelper.SortCharacters(_companionName, CONTROL_STRING, saltValue)}.txt";

            _cryptoWorker = new CryptoHelper(CONTROL_STRING, saltValue);
            _fileSharingWorker = new FileSharingWorker();
        }

        public async Task<ConnectionInfo?> ExecuteAsync()
            => await SendMyPassAsync() ? await GetCompanionPassAsync() : null;

        #region Private
        private async Task<bool> SendMyPassAsync()
        {
            var message = new MessageHelper(_cryptoWorker).DefinePassword(_myPass);
            var encryptedText = _cryptoWorker.Encrypt(message);
            return await new AttemptHelper(number: 10, delaySec: 2).ExecuteAsync(async () =>
                await _fileSharingWorker.PostAsync(_bin, _myTxtFileName, encryptedText));
        }

        private async Task<ConnectionInfo?> GetCompanionPassAsync()
        {
            var message = await new AttemptHelper(number: 50, delaySec: 3).ExecuteAsync(async () =>
                await _fileSharingWorker.GetAsync(_bin, _myTxtFileName, deleteFlag: true)); // _companionTxtFileName
            var companionPass = new MessageHelper(_cryptoWorker).ExtractPassword(message);
            var passwords = new[] { _myPass, companionPass };
            return new ConnectionInfo(_myName, _companionName, _bin, passwords);
        }
        #endregion
    }
}