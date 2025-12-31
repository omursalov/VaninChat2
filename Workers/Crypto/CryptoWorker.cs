using CryptoNet;
using System.Text;
using VaninChat2.Dto;

namespace VaninChat2.Workers.Crypto
{
    public class CryptoWorker : IDisposable
    {
        private readonly string _privateKey;
        private readonly string _publicKey;

        private readonly FileObj _privateKeyFileObj;
        private readonly FileObj _publicKeyFileObj;

        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly ICryptoNetRsa _cryptoNetRsa;

        public CryptoWorker()
        {
            var key = new KeyWorker().Generate();

            _privateKey = key;
            _publicKey = key;

            // Create keys.
            _cryptoNetRsa = new CryptoNetRsa();

            // Secure the private key and distribute the public key.
            _cryptoNetRsa.SaveKey(new FileInfo(_privateKey), true);
            _cryptoNetRsa.SaveKey(new FileInfo(_publicKey), false);
        }

        public void Encrypt(FileStream fs, string value)
        {
            // Public key is used to encrypt the message.
            var bytes = _cryptoNetRsa.EncryptFromString(value);
            var res = BitConverter.ToString(bytes);
            bytes = _encoding.GetBytes(res);
            fs.Write(bytes, 0, bytes.Length);

            /*var bytes = _cryptoNetRsa.EncryptFromString(value);
            var result = BitConverter.ToString(bytes);*/
            /*bytes = _encoding.GetBytes(result);
            fs.Write(bytes, 0, bytes.Length);*/

            /*ICryptoNetRsa cryptoNet = new CryptoNetRsa();
            cryptoNet.SaveKey(_privateKeyFileObj.FileInfo, true);
            cryptoNet.SaveKey(_publicKeyFileObj.FileInfo, false);

            var bytes = cryptoNet.EncryptFromString(value);
            bytes = _encoding.GetBytes(Convert.ToBase64String(bytes));
            fs.Write(bytes, 0, bytes.Length);*/
        }

        public string Decrypt(string value)
        {
            String[] arr = value.Split('-');
            byte[] array = new byte[arr.Length];
            for (int i = 0; i < arr.Length; i++) 
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