using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BlitzkriegSoftware.AsymmetricCrypto.Test
{

    /// <summary>
    /// Helper: To Encrypt and Decrypt Text using a public/private key
    /// </summary>
    public class AsymmetricCryptoHelper : IDisposable
    {
        #region "Vars, Constants, Utility"

        /// <summary>
        /// Maximum Support Characters
        /// </summary>
        public const int MaxCharsSupported = 250;

#pragma warning disable CA1805 // Do not initialize unnecessarily
        private bool disposed = false;
#pragma warning restore CA1805 // To conform to dispose pattern

        private RSA rsa;

        private readonly string _keyPrivate = string.Empty;
        private readonly UnicodeEncoding _byteConverter = new();

        /// <summary>
        /// Unicode Byte Converter
        /// </summary>
        public UnicodeEncoding ByteConverter
        {
            get { return _byteConverter; }
        }

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
        [ExcludeFromCodeCoverage]
        private AsymmetricCryptoHelper() { }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="keyPrivate">RSA Private Key</param>
        /// <exception cref="ArgumentNullException">Missing Key</exception>
        /// <exception cref="CryptographicException">Bad Key</exception>
        public AsymmetricCryptoHelper(string keyPrivate)
        {
            if (string.IsNullOrWhiteSpace(keyPrivate)) throw new ArgumentNullException(nameof(keyPrivate));

            rsa = RSA.Create();

            _keyPrivate = KeyConverter(keyPrivate);
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(_keyPrivate), out _);
        }

        #endregion

        /// <summary>
        /// Encrypt Text
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Enciphered Text</returns>
        /// <exception cref="ArgumentNullException">(sic)</exception>
        /// <exception cref="ArgumentOutOfRangeException">(sic)</exception>
        /// <exception cref="CryptographicException">Bad Key</exception>
        public string Encrypt(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(nameof(text));
            if (text.Length > MaxCharsSupported) throw new ArgumentOutOfRangeException(nameof(text), $"Text length of {text.Length} exceeds maximum of {MaxCharsSupported}");

            byte[] dataToEncrypt = _byteConverter.GetBytes(text);
            byte[] encryptedData = rsa.Encrypt(
                    data: dataToEncrypt,
                    padding: RSAEncryptionPadding.Pkcs1
            );

            var cryptoText = Convert.ToBase64String(encryptedData);
            return cryptoText;
        }

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="cryptoText">Text to be decrypted</param>
        /// <returns>Plain text</returns>
        /// <exception cref="ArgumentNullException">(sic)</exception>
        /// <exception cref="CryptographicException">Bad Key or <c>cryptoText</c></exception>
        /// <exception cref="System.FormatException">Bad <c>cryptoText</c></exception>
        public string Decrypt(string cryptoText)
        {
            if (string.IsNullOrWhiteSpace(cryptoText)) throw new ArgumentNullException(nameof(cryptoText));

            byte[] encryptedData = Convert.FromBase64String(cryptoText);
            byte[] data = rsa.Decrypt(
                data: encryptedData,
                padding: RSAEncryptionPadding.Pkcs1
            );

            var text = _byteConverter.GetString(data);
            return text;
        }

        #region "IDisposable"

        /// <summary>
        /// (sic)
        /// </summary>
        public void Dispose()
        {
            rsa = null;
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// (sic)
        /// </summary>
        /// <param name="disposing">flag</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                rsa?.Clear();
                rsa = null;
            }

            disposed = true;
        }

        #endregion
    }
}
