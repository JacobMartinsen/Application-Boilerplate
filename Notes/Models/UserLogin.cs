using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using IntegrationManagement;

namespace Notes.Models
{
    [Table("UserLogin", "ID", "NotesDB")]
    public class UserLogin : iMigratable<UserLogin>
    {
        [Field("ID", DataType.UUID, false)]
        public Guid ID;
        [Field("USERNAME", DataType.VARCHAR, false)]
        public string USERNAME;
        [Field("PASSWORD", DataType.VARCHAR, false)]
        public string PASSWORD;
        [Field("LOGINTIME", DataType.DATETIME, true)]
        public DateTime LOGINTIME;

        public UserLogin () 
        {

        }
        public Boolean Authenticate(string plainTextPassword)
        {
            return SafeStringEquals(PadString(plainTextPassword, 128), PadString(plainTextPassword, 128));
        }

        private string PadString(string text, int length)
        {
            string dummyStr = "".PadRight(length);
            StringBuilder dummy = new StringBuilder();
            StringBuilder realPassword = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                if (i < text.Length)
                {
                    realPassword.Append(text[i]);
                }
                else
                {
                    dummy.Append(dummyStr[i]);
                }
            }
            return realPassword.ToString();
        }

        private Boolean SafeStringEquals(string text1, string text2)
        {
            Boolean notEqual = true;
            Boolean equal = true; 
            for (int i = 0; i < 128; i++)
            {
                if(text1[i] != text2[i])
                {
                    equal = false;
                }
                else
                {
                    notEqual = true; 
                }
            }
            return equal && notEqual;
        }



    }
}
