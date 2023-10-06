using BlitzkriegSoftware.AsymmetricCryptoNative;
using Faker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;

namespace BlitzkriegSoftware.Test.AsymmetricCrypto
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class AsymmetricCryptoNaitiveTest
    {
        #region "Boilerplate and read in private RSA key"

        private static TestContext _testContext;
        private static byte[] _privateKeyBytes;

        [ClassInitialize]
        public static void SetupTests(TestContext testContext)
        {
            _testContext = testContext;
            //Generate a public/private key pair.  
            RSA rsa = RSA.Create();
            _privateKeyBytes = rsa.ExportRSAPrivateKey();
        }

        #endregion

        [TestMethod]
        [TestCategory("Unit")]
        public void SimpleTest1()
        {
            string expected = "xyz";
            _testContext.WriteLine($"[{expected.Length}]: {expected}");

            using (var cg = new AsymmetricCryptoClientNative(_privateKeyBytes))
            {
                var ct = cg.Encrypt(expected);
                var pt = cg.Decrypt(ct);
                Assert.AreEqual(expected, pt);
            }
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void SimpleTest2()
        {
            string expected = "The whole house is spinning because of a tornado.";
            _testContext.WriteLine($"[{expected.Length}]: {expected}");

            using (var cg = new AsymmetricCryptoClientNative(_privateKeyBytes))
            {
                var ct = cg.Encrypt(expected);
                var pt = cg.Decrypt(ct);
                Assert.AreEqual(expected, pt);
            }
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(System.Security.Cryptography.CryptographicException))]
        public void SizingTest1()
        {
            string expected = string.Empty;
            for (int i = 1; i < 260; i++)
            {
                expected = new string(Lorem.Letters(i).ToArray());
                _testContext.WriteLine($"[{expected.Length}]: {expected}");

                try
                {
                    using (var cg = new AsymmetricCryptoClientNative(_privateKeyBytes))
                    {
                        var ct = cg.Encrypt(expected);
                        var pt = cg.Decrypt(ct);
                        Assert.AreEqual(expected, pt);
                    }
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    _testContext.WriteLine($"{ex.Message}");
                    break;
                }
            }
            _testContext.WriteLine($"Maximim Length {expected.Length - 1}");
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BadCtor1()
        {
            _ = new AsymmetricCryptoClientNative(keyPrivate: null);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(System.Security.Cryptography.CryptographicException))]
        public void BadCtor2()
        {
            _ = new AsymmetricCryptoClientNative(secret: null);
        }
        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BadEncrypt1()
        {
            using (var cg = new AsymmetricCryptoClientNative(_privateKeyBytes))
            {
                _ = cg.Encrypt(null);
            }
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void BadEncrypt2()
        {
            using (var cg = new AsymmetricCryptoClientNative("this is not a RSA Private Key"))
            {

            }
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(System.Security.Cryptography.CryptographicException))]
        public void BadEncrypt3()
        {
            using (var cg = new AsymmetricCryptoClientNative($"this is{Environment.NewLine} not a{Environment.NewLine} RSA Private Key"))
            {

            }
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BadDecrypt1()
        {
            using (var cg = new AsymmetricCryptoClientNative(_privateKeyBytes))
            {
                _ = cg.Decrypt(null);
            }
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(System.FormatException))]
        public void BadDecrypt2()
        {
            var cipherText = "This is not ciphered text";
            using (var cg = new AsymmetricCryptoClientNative(_privateKeyBytes))
            {
                _ = cg.Decrypt(cipherText);
            }
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(System.Security.Cryptography.CryptographicException))]
        public void BadDecrypt3()
        {
            using (var cg = new AsymmetricCryptoClientNative(_privateKeyBytes))
            {
                var cipherText = Convert.ToBase64String(cg.ByteConverter.GetBytes("This is not ciphered text"));
                _ = cg.Decrypt(cipherText);
            }
        }

    }
}
