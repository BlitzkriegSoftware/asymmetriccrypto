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

#pragma warning disable CA1805 // Do not initialize unnecessarily
        private bool disposed = false;
#pragma warning restore CA1805 // To conform to dispose pattern

        /// <summary>
        /// Unicode Byte Converter
        /// </summary>
        public UnicodeEncoding ByteConverter { get; } = new();
        private readonly PGP pgp;
        private EncryptionKeys _encryptionKeys;
        private string _passphrase;
        #endregion

        [ExcludeFromCodeCoverage]
        private AsymmetricCryptoClientBouncyCastle() { }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="keyPrivate">RSA Private Key</param>
        /// <param name="keyPublic">RSA Public Key</param>
        /// <param name="passPhrase">PassPhrase</param>
        /// <exception cref="ArgumentNullException">Missing Key</exception>
        /// <exception cref="CryptographicException">Bad Key</exception>
        public AsymmetricCryptoClientBouncyCastle(
            string keyPublic,
            string keyPrivate, 
            string passPhrase
        ) {
            keyPrivate.ThrowIfNullOrEmpty(nameof( keyPrivate ) );
            keyPublic.ThrowIfNullOrEmpty (nameof( keyPublic ) );
            passPhrase.ThrowIfNullOrEmpty(nameof(passPhrase ) );
            _encryptionKeys = new(keyPublic, keyPrivate, passPhrase);
            _passphrase = passPhrase;
            pgp = new(_encryptionKeys);
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
            text.ThrowIfNullOrEmpty(nameof(text));

            var bytesIn = new MemoryStream(ByteConverter.GetBytes(text));
            using (Stream bytesOut = new MemoryStream())
            {
                pgp.EncryptStream(bytesIn, bytesOut);
                bytesOut.Position = 0;
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
            cryptoText.ThrowIfNullOrEmpty(nameof(cryptoText));
            string outText = pgp.DecryptArmoredStringAsync(cryptoText)
                             .ConfigureAwait(false).GetAwaiter().GetResult();
            outText = outText.Replace('\0'.ToString(), "");
            return outText;
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
