using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace BlitzkriegSoftware.AsymmetricCrypto.Test
{
    [TestClass]
    public class Test_AC
    {
        #region "Boilerplate"

        private static TestContext _testContext;
        private static string PrivateKey;
        private static string PublicKey;

        [ClassInitialize]
        public static void SetupTests(TestContext testContext)
        {
            _testContext = testContext;
            PrivateKey = File.ReadAllText(@".\rsa4096.private");
            PublicKey = File.ReadAllText(@".\rsa4096.public");
        }

        #endregion

        [TestMethod]
        [TestCategory("Unit")]
        public void Simple_Test_1()
        {
            string expected = "xyz";
            using (var cg = new AsymmetricCrypto(PublicKey, PrivateKey))
            {
                var ct = cg.Encrypt(expected);
                var pt = cg.Decrypt(ct);
            }
        }

    }
}
