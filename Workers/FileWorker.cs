using System.Text;
using VaninChat2.Models;
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

            using (var fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                var encryptedText = _cryptoWorker.Encrypt(content);
                var bytes = Encoding.UTF8.GetBytes(encryptedText);
                fs.Write(bytes, 0, bytes.Length);
            }

            return result;
        }
    }
}