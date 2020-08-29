using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BlitzkriegSoftware.AsymmetricCrypto.Test
{
    public class AsymmetricCrypto : IDisposable
    {
        #region "Vars, Constants, Utility"
        public const int MaxCharsSupported = 250;

#pragma warning disable CA1805 // Do not initialize unnecessarily
        bool disposed = false;
#pragma warning restore CA1805 // To conform to dispose pattern

        private RSA rsa = RSA.Create();

        private readonly string _keyPrivate = string.Empty;
        private readonly UnicodeEncoding byteConverter = new UnicodeEncoding();

        private static string KeyConverter(string key)
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

        #endregion

        #region "CTOR"
        private AsymmetricCrypto() { }

        public AsymmetricCrypto(string keyPrivate)
        {
            if (string.IsNullOrWhiteSpace(keyPrivate)) throw new ArgumentNullException(nameof(keyPrivate));
            _keyPrivate = KeyConverter(keyPrivate);
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(_keyPrivate), out _);
        }

        #endregion

        public string Encrypt(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(nameof(text));
            if (text.Length > MaxCharsSupported) throw new ArgumentOutOfRangeException(nameof(text), $"Text length of {text.Length} exceeds maximum of {MaxCharsSupported}");
            byte[] dataToEncrypt = byteConverter.GetBytes(text);
            byte[] encryptedData = rsa.Encrypt(dataToEncrypt, RSAEncryptionPadding.Pkcs1);
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
