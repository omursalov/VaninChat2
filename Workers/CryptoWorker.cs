using System.Security.Cryptography;
using System.Text;

namespace VaninChat2.Workers
{
    public class CryptoWorker
    {
        private const string DEFAULT_KEY = "AAECAwQFBgcICQoLDA0ODw==";
        private const string DEFAULT_IV = "hAC8hMf3N5Zb/DZhFKi6Sg==";

        private readonly byte[] _key;
        private readonly byte[] _iv;

        public CryptoWorker(
            string key = null, 
            string iv = null)
        {
            _key = Convert.FromBase64String(key == null ? DEFAULT_KEY : key);
            _iv = Convert.FromBase64String(iv == null ? DEFAULT_IV : iv);
        }

        public void Encrypt(FileStream fileStream, string content)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _iv;

                fileStream.Write(_iv, 0, _iv.Length);

                using ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (var cryptoStream = new CryptoStream(fileStream, encryptor, CryptoStreamMode.Write))
                {
                    using (var encryptWriter = new StreamWriter(cryptoStream, Encoding.UTF8))
                    {
                        encryptWriter.WriteLine(content);
                    }
                }
            }
        }

        public string Decrypt(byte[] bytes)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                byte[] decryptedBytes;
                using (var msDecrypt = new System.IO.MemoryStream(bytes))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var msPlain = new System.IO.MemoryStream())
                        {
                            csDecrypt.CopyTo(msPlain);
                            decryptedBytes = msPlain.ToArray();
                        }
                    }
                }
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
    }
}