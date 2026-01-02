using System.Security.Cryptography;
using System.Text;

namespace VaninChat2.Helpers.Crypto
{
    public class CryptoHelper
    {
        private const string HASH_ALGORITHM = "SHA256";
        private const int PASSWORD_ITERATIONS = 2;
        private const string INIT_VECTOR = "!1A3g2D4s9K556g7";
        private const int KEY_SIZE = 256;

        private readonly string _passPhrase;
        private readonly string _saltValue;
        private readonly Encoding _encoding;

        public CryptoHelper(string passPhrase, string saltValue)
        {
            _passPhrase = passPhrase;
            _saltValue = saltValue;
            _encoding = Encoding.UTF8;
        }

        public string Encrypt(string text)
        {
            var initVectorBytes = _encoding.GetBytes(INIT_VECTOR);
            var saltValueBytes = _encoding.GetBytes(_saltValue);
            var textBytes = _encoding.GetBytes(text);
            using var password = new PasswordDeriveBytes(_passPhrase, saltValueBytes, HASH_ALGORITHM, PASSWORD_ITERATIONS);
            var keyBytes = password.GetBytes(KEY_SIZE / 8);
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
            var initVectorBytes = _encoding.GetBytes(INIT_VECTOR);
            var saltValueBytes = _encoding.GetBytes(_saltValue);
            var textBytes = Convert.FromBase64String(text);
            using var password = new PasswordDeriveBytes(_passPhrase, saltValueBytes, HASH_ALGORITHM, PASSWORD_ITERATIONS);
            var keyBytes = password.GetBytes(KEY_SIZE / 8);
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