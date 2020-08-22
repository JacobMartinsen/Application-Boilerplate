using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntegrationManagement;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace IntegrationManagement.Tests
{
    [TestClass()]
    public class EncryptionManagementTests
    {
        [TestInitialize]
        public void InitializeEncryptionDecryption()
        {
            using (RijndaelManaged myRijndael = new RijndaelManaged())
            {
                myRijndael.GenerateKey();
                myRijndael.GenerateIV();
                EncryptionManagement.InitializeEncryption(myRijndael.IV, myRijndael.Key);
            }
        }

        [TestMethod()]
        public void EncryptTest()
        {
            Assert.AreEqual("Hi Frens!", EncryptionManagement.Decrypt(EncryptionManagement.Encrypt("Hi Frens!")));
        }
    }
}