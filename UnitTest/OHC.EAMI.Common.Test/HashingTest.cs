using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OHC.EAMI.Common;

namespace OHC.EAMI.Common.Test
{
    [TestClass]
    public class EAMIHashingTest
    {
        [TestMethod]
        public void TestHashingAndTextCompare()
        {
            string textToHashAndCompare = "password";
            
            string generatedHash = EAMIHashGenerator.ComputeHash(textToHashAndCompare, "SHA512", null);

            bool isComputedHashValid = EAMIHashGenerator.VerifyHash(textToHashAndCompare, "SHA512", generatedHash);

            Assert.IsTrue(isComputedHashValid);
        }
    }
}
