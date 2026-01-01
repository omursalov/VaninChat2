using System.Security.Cryptography;
using System.Text;

namespace VaninChat2.Helpers.Crypto
{
    public class CryptoHelper
    {
        private readonly Encoding _encoding;

        private readonly string _passPhrase;
        private readonly string _saltValue;
        private readonly string _hashAlgorithm;
        private readonly int _passwordIterations;
        private readonly string _initVector;
        private readonly int _keySize;

        public CryptoHelper(string passPhrase, string saltValue,
            string hashAlgorithm, int passwordIterations,
            string initVector, int keySize)
        {
            _encoding = Encoding.UTF8;
            _passPhrase = passPhrase;
            _saltValue = saltValue;
            _hashAlgorithm = hashAlgorithm;
            _passwordIterations = passwordIterations;
            _initVector = initVector;
            _keySize = keySize;
        }

        public string Encrypt(string text)
        {
            var initVectorBytes = _encoding.GetBytes(_initVector);
            var saltValueBytes = _encoding.GetBytes(_saltValue);
            var textBytes = _encoding.GetBytes(text);
            using var password = new PasswordDeriveBytes(_passPhrase, saltValueBytes, _hashAlgorithm, _passwordIterations);
            var keyBytes = password.GetBytes(_keySize / 8);
            using var symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            using var encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(textBytes, 0, textBytes.Length);
            cryptoStream.FlushFinalBlock();
            var resultBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(resultBytes);
        }

        public string Decrypt(string text)
        {
            var initVectorBytes = _encoding.GetBytes(_initVector);
            var saltValueBytes = _encoding.GetBytes(_saltValue);
            var textBytes = Convert.FromBase64String(text);
            using var password = new PasswordDeriveBytes(_passPhrase, saltValueBytes, _hashAlgorithm, _passwordIterations);
            var keyBytes = password.GetBytes(_keySize / 8);
            using var symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            using var decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            using var memoryStream = new MemoryStream(textBytes);
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            var resultBytes = new byte[textBytes.Length];
            var decryptedByteCount = cryptoStream.Read(resultBytes, 0, resultBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return _encoding.GetString(resultBytes, 0, decryptedByteCount);
        }
    }
}