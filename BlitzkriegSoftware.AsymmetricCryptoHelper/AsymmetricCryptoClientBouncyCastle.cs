using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Utilities.IO;
using PgpCore;
using static System.Net.Mime.MediaTypeNames;

namespace BlitzkriegSoftware.AsymmetricCryptoHelper
{
    /// <summary>
    /// For Public/Private GPG generated keys
    /// <para>A PassPhrase is Required!</para>
    /// </summary>
    public class AsymmetricCryptoClientBouncyCastle : IDisposable
    {
        #region "Vars, Constants, Utility"

        /// <summary>
        /// Maximum Support Characters
        /// </summary>
        public const int MaxCharsSupported = 250;

#pragma warning disable CA1805 // Do not initialize unnecessarily
        private bool disposed = false;
#pragma warning restore CA1805 // To conform to dispose pattern

        /// <summary>
        /// Unicode Byte Converter
        /// </summary>
        public UnicodeEncoding ByteConverter { get; } = new();

        /// <summary>
        /// Convert a ASCII Armored GPG Key to a Secret to use in Code
        /// </summary>
        /// <param name="key">(sic)</param>
        /// <returns>(secret)</returns>
        public static byte[] ConvertKeyTextToSecret(string key)
        {
            var lines = key.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries
            ).ToList();

            lines.RemoveAt(lines.Count - 1);
            lines.RemoveAt(0);

            StringBuilder sb = new();
            foreach (var line in lines)
            {
                sb.Append(line.Trim());
            }
            var text = sb.ToString();

            var secret = Base64Replacement.DecodeToArray(text);
            return secret;
        }

        private readonly PGP pgp;

        #endregion

        [ExcludeFromCodeCoverage]
        private AsymmetricCryptoClientBouncyCastle() { }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="keyPrivate">RSA Private Key</param>
        /// <param name="passPhrase">PassPhrase</param>
        /// <exception cref="ArgumentNullException">Missing Key</exception>
        /// <exception cref="CryptographicException">Bad Key</exception>
        public AsymmetricCryptoClientBouncyCastle(string keyPrivate, string passPhrase)
        {
            if(string.IsNullOrWhiteSpace(keyPrivate))
            {
                throw new ArgumentNullException(nameof(keyPrivate));
            }

            if(string.IsNullOrWhiteSpace(passPhrase))
            {
                throw new ArgumentNullException(nameof(passPhrase));
            }

            EncryptionKeys encryptionKeys = new EncryptionKeys(keyPrivate, passPhrase);
            pgp = new(encryptionKeys);
        }

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

            var bytesIn = new MemoryStream(ByteConverter.GetBytes(text));
            using (Stream bytesOut = new MemoryStream())
            {
                pgp.EncryptStream(bytesIn, bytesOut);
                var outText = bytesOut.GetString();
                return outText;
            }
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
            var bytesIn = new MemoryStream(ByteConverter.GetBytes(cryptoText));
            using (Stream bytesOut = new MemoryStream())
            {
                pgp.EncryptStream(bytesIn, bytesOut);
                var outText = bytesOut.GetString();
                return outText;
            }
        }

        #region "IDisposable"

        /// <summary>
        /// (sic)
        /// </summary>
        public void Dispose()
        {
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
                pgp?.Dispose();
            }

            disposed = true;
        }

        #endregion
    }
}
