using VaninChat2.Dto;
using VaninChat2.Workers.Crypto;

namespace VaninChat2.Workers
{
    public class FileWorker
    {
        private readonly CryptoWorker _cryptoWorker;

        public FileWorker(CryptoWorker cryptoWorker)
        {
            _cryptoWorker = cryptoWorker;
        }

        public FileObj CreateEncryptedTxtFile(string fileName, string content)
        {
            if (!fileName.EndsWith(".txt"))
                fileName = $"{fileName}.txt";

            var result = new FileObj(fileName);

            using (var fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                _cryptoWorker.Encrypt(fileStream, content);
            }

            return result;
        }
    }
}