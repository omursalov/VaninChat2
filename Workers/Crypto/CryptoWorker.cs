using CryptoNet;
using System.Text;
using VaninChat2.Dto;

namespace VaninChat2.Workers.Crypto
{
    public class CryptoWorker : IDisposable
    {
        private const string CONTROL_STRING = "nBfvubZ6Nggh76wwQzEy6L6gxQ4eWw";

        private readonly FileObj _privateKeyFileObj;
        private readonly FileObj _publicKeyFileObj;

        private readonly ICryptoNetRsa _cryptoNetRsa;

        public CryptoWorker()
        {
            var key = new KeyWorker().Generate();

            _cryptoNetRsa = new CryptoNetRsa();

            _privateKeyFileObj = new FileObj($"{key}{CONTROL_STRING}");
            _publicKeyFileObj = new FileObj(key);

            _cryptoNetRsa.SaveKey(_privateKeyFileObj.FileInfo, true);
            _cryptoNetRsa.SaveKey(_publicKeyFileObj.FileInfo, false);
        }

        public void Encrypt(FileStream fs, string value)
        {
            var bytes = _cryptoNetRsa.EncryptFromString(value);
            var res = BitConverter.ToString(bytes);
            bytes = Encoding.UTF8.GetBytes(res);
            fs.Write(bytes, 0, bytes.Length);
        }

        public string Decrypt(string value)
        {
            var arr = value.Split('-');
            var array = new byte[arr.Length];

            for (var i = 0; i < arr.Length; i++)
                array[i] = Convert.ToByte(arr[i], 16);

            return _cryptoNetRsa.DecryptToString(array);
        }

        public void Dispose()
        {
            _privateKeyFileObj?.Dispose();
            _publicKeyFileObj?.Dispose();
        }
    }
}