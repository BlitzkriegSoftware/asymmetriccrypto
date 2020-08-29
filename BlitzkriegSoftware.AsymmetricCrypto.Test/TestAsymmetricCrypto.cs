using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace BlitzkriegSoftware.AsymmetricCrypto.Test
{
    [TestClass]
    public class TestAsymmetricCrypto
    {
        #region "Boilerplate"

        private static TestContext _testContext;
        private static string PrivateKey;

        [ClassInitialize]
        public static void SetupTests(TestContext testContext)
        {
            _testContext = testContext;
            PrivateKey = File.ReadAllText(@".\rsa4096.private");
        }

        #endregion

        [TestMethod]
        [TestCategory("Unit")]
        public void SimpleTest1()
        {
            string expected = "xyz";
            _testContext.WriteLine($"[{expected.Length}]: {expected}");

            using (var cg = new AsymmetricCrypto(PrivateKey))
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

            using (var cg = new AsymmetricCrypto(PrivateKey))
            {
                var ct = cg.Encrypt(expected);
                var pt = cg.Decrypt(ct);
                Assert.AreEqual(expected, pt);
            }
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void SizingTest1()
        {
            string expected = string.Empty;
            for (int i = 1; i < 260; i++) {
                expected = new string(Faker.Lorem.Letters(i).ToArray());
                _testContext.WriteLine($"[{expected.Length}]: {expected}");

                try
                {
                    using (var cg = new AsymmetricCrypto(PrivateKey))
                    {
                        var ct = cg.Encrypt(expected);
                        var pt = cg.Decrypt(ct);
                        Assert.AreEqual(expected, pt);
                    }
                } catch(ArgumentOutOfRangeException ex)
                {
                    _testContext.WriteLine($"{ex.Message}");
                    break;
                }
            }
            _testContext.WriteLine($"Maximim Length {expected.Length - 1}");
        }

    }
}
