using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntegrationManagement;
using System;
using Notes.Models;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;

namespace IntegrationManagement.Tests
{
    [TestClass()]
    public class iMigratableTests
    {
        string _connectionString = "Data Source=jacob-g5\\sqlexpress;Initial Catalog=NotesDB;Persist Security Info=True;User ID=TestUser;Password=TestUser";
        [TestInitialize]
        public void CreateTestEnvironment()
        {
            string CreateDBQuery = System.IO.File.ReadAllText(@"..\\..\\..\\CreateDBQuery.txt");
            ExecuteQuery(CreateDBQuery);
            string CreateUserLoginTable = System.IO.File.ReadAllText(@"..\\..\\..\\CreateUserLoginTable.txt");
            ExecuteQuery(CreateUserLoginTable);
            string TestData = System.IO.File.ReadAllText(@"..\\..\\..\\TestData.txt");
            ExecuteQuery(TestData);

            using (RijndaelManaged myRijndael = new RijndaelManaged())
            {
                myRijndael.GenerateKey();
                myRijndael.GenerateIV();
                EncryptionManagement.InitializeEncryption(myRijndael.IV, myRijndael.Key);
            }

        }

        public Boolean ExecuteQuery(string queryString)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
                return true;
            }
        }
        [TestCleanup]
        public void CleanUpTestEnvironment()
        {
            string CleanupDB = System.IO.File.ReadAllText(@"..\\..\\..\\CleanupDB.txt");
            ExecuteQuery(CleanupDB);
        }

        [TestMethod()]
        public void SaveTest()
        {
            UserLogin login = new UserLogin();
            login.ID = Guid.NewGuid();
            login.USERNAME = "GoodBoi";
            login.PASSWORD = "Password1";
            login.LOGINTIME = DateTime.Now;
            Boolean testResult = login.Save();
            UserLogin testLogin = new UserLogin();
            testLogin.ID = login.ID;
            Boolean testLoad = testLogin.Load();
            Assert.AreEqual(testResult, true);
            Assert.AreEqual(testLoad, true);
        }

        [TestMethod()]
        public void LoadTest()
        {
            UserLogin login = new UserLogin();
            login.ID = Guid.Parse("fa89bfff-8fba-4bfa-a7df-84ad7ddc7181");
            Boolean testResult = login.Load();
            Assert.AreEqual(testResult, true);
        }

        [TestMethod()]
        public void DeleteTest()
        {
            UserLogin login = new UserLogin();
            login.ID = Guid.NewGuid();
            login.USERNAME = "GoodBoi";
            login.PASSWORD = "Password1";
            login.LOGINTIME = DateTime.Now;
            Boolean testResult = login.Save();
            UserLogin testLogin = new UserLogin();
            testLogin.ID = login.ID;
            Boolean testLoad = testLogin.Load();
            Assert.AreEqual(testResult, true);
            Assert.AreEqual(testLoad, true);
            Boolean testDelete = login.Delete();
            Assert.AreEqual(testDelete, true);
        }
    }
}