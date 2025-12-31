using CryptoNet;
using System.Text;
using VaninChat2.Dto;

namespace VaninChat2.Workers.Crypto
{
    public class CryptoWorker : IDisposable
    {
        private readonly string _privateKey;
        private readonly FileObj _fileObj;
        private readonly Encoding _encoding;
        private readonly CryptoNetRsa _cryptoNetRsa;

        public CryptoWorker()
        {
            _privateKey = new KeyWorker().Generate();
            _fileObj = new FileObj(_privateKey);
            ICryptoNetRsa cryptoNet = new CryptoNetRsa();
            var key = new KeyWorker().Generate();
            cryptoNet.SaveKey(_fileObj.FileInfo, true);
            _encoding = new UTF8Encoding(true);
            _cryptoNetRsa = new CryptoNetRsa(_fileObj.FileInfo);
        }

        public void Encrypt(FileStream fs, string value)
        {
            var cypher = _cryptoNetRsa.EncryptFromString(value);
            var result = BitConverter.ToString(cypher).Replace("-", string.Empty);
            var info = _encoding.GetBytes(result);
            fs.Write(info, 0, info.Length);
        }

        public string Decrypt(string value)
        {
            var bytes = _encoding.GetBytes(value);
            return _cryptoNetRsa.DecryptToString(bytes);
        }

        public void Dispose()
        {
            _fileObj?.Dispose();
        }
    }
}