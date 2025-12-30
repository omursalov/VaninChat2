using System.IO;
using VaninChat2.Dto;

namespace VaninChat2.Workers
{
    public class FileWorker
    {
        public FileObj CreateEncryptedTxtFile(string fileName, string content)
        {
            if (!fileName.EndsWith(".txt"))
                fileName = $"{fileName}.txt";

            var result = new FileObj(fileName);

            using (var fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                new CryptoWorker().Encrypt(fileStream, content);
            }

            return result;
        }

        public string Decrypt(byte[] bytes)
        {
            return new CryptoWorker().Decrypt(bytes);
        }
    }
}