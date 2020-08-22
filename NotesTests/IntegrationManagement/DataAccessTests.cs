using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntegrationManagement;
using System;
using Notes.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace IntegrationManagement.Tests
{
    [TestClass()]
    public class DataAccessTests
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
          
        }
        [TestCleanup]
        public void CleanUpTestEnvironment()
        {
            string CleanupDB = System.IO.File.ReadAllText(@"..\\..\\..\\CleanupDB.txt");
            ExecuteQuery(CleanupDB);
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
        [TestMethod()]
        public void LoadByPKTest()
        {
            UserLogin login = new UserLogin();
            login.ID = Guid.Parse("BD1236E2-E469-47E6-A330-522C411DA8CE");
            DataAccess<UserLogin> DAC = new DataAccess<UserLogin>(_connectionString);
            if (DAC.LoadSingleRowFromPK(login))
            {
                Assert.AreEqual(login.USERNAME, "Howdy Frens");
                Assert.AreEqual(login.PASSWORD, "12345");
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void LoadByColumnTest()
        {
            UserLogin login = new UserLogin();
            login.USERNAME = "hmoortond";
            DataAccess<UserLogin> DAC = new DataAccess<UserLogin>(_connectionString);
            if (DAC.LoadSingleRowByColumn(login, DAC.GetFields(login).Find(r => r.COLUMN.Equals("USERNAME"))))
            {
                Assert.AreEqual(login.PASSWORD, "axE8oeQSHUfk");
                Assert.AreEqual(login.ID, Guid.Parse("a76fe576-87b2-4847-8180-138d99972dd1"));
                Assert.AreEqual(login.LOGINTIME, DateTime.Parse("2019-01-12 21:10:42"));
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}