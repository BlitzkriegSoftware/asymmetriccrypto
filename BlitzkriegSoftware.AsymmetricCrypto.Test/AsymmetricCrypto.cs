using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BlitzkriegSoftware.AsymmetricCrypto.Test
{
    public class AsymmetricCrypto : IDisposable
    {
        bool disposed = false;
        private string _keyPublic = string.Empty;
        private string _keyPrivate = string.Empty;
        private RSA rsa = RSA.Create();
        private UnicodeEncoding byteConverter = new UnicodeEncoding();

        private AsymmetricCrypto() { }

        public AsymmetricCrypto(string keyPublic, string keyPrivate)
        {
            _keyPrivate = KeyConverter( keyPrivate );
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(_keyPrivate), out _);
            //_keyPublic = KeyConverter(keyPublic );
            //rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(_keyPublic), out _);
        }

        private string KeyConverter(string key)
        {
            var lines = key.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries
            ).ToList();

            lines.RemoveAt(lines.Count - 1);
            lines.RemoveAt(0);

            var cvt = lines.Aggregate((a, b) => a + b);
            return cvt;
        }

        public string Encrypt(string text)
        {
            byte[] dataToEncrypt = byteConverter.GetBytes(text);
            byte[] encryptedData = rsa.Encrypt(dataToEncrypt,RSAEncryptionPadding.Pkcs1);
            var cryptoText = Convert.ToBase64String(encryptedData);
            return cryptoText;
        }

        public string Decrypt(string cryptoText)
        {
            byte[] encryptedData = Convert.FromBase64String(cryptoText);
            byte[] data = rsa.Decrypt(encryptedData, RSAEncryptionPadding.Pkcs1);
            var text = byteConverter.GetString(data);
            return text;
        }

        #region "IDisposable"

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                rsa = null;
            }

            disposed = true;
        }

        #endregion
    }

}
