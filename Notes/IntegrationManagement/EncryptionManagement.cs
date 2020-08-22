using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
namespace IntegrationManagement
{
    public class EncryptionManagement
    {
        private static byte[] IV;
        private static byte[] Key;

        public static Boolean InitializeEncryption(byte[] iv, byte[] key)
        {
            IV = iv;
            Key = key;
            return iv != null && key != null;
        }
        /// <summary>
        /// Takes a Unicode string and returns a base-64 encoded encrypted String 
        /// </summary>
        /// <param name="plaintext">Unicode UTF-16 string</param>
        /// <returns>base-64 encrypted string </returns>
        public static string Encrypt(string plaintext)
        {
            try
            {
               byte[] encrypted = EncryptString(plaintext);
               return Convert.ToBase64String(encrypted);
            }
            catch(Exception e)
            {
                //eat error at this point until logging component is working
                return null;
            }
        }

        private static byte[] EncryptString(string plaintext)
        {
            using (AesManaged aes = new AesManaged())
            {
                ICryptoTransform encryptor = aes.CreateEncryptor(Key, IV);
                using (MemoryStream ms = new MemoryStream())
                {
                    using(CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plaintext);
                            sw.Flush();
                        }
                        return ms.ToArray();
                    }
                }
            }
        }
        /// <summary>
        /// Takes a base-64 ciphertext and returns a Unicode plaintext
        /// </summary>
        /// <param name="ciphertext">base-64 encoded ciphertext</param>
        /// <returns>Unicode plaintext</returns>
        public static string Decrypt(string ciphertext)
        {
            try
            {
                byte[] fromBase64 = Convert.FromBase64String(ciphertext);
                return Decrypt(fromBase64);
            }
            catch (Exception e)
            {
                //eat error at this point until logging component is working
                return null;
            }
        }

        private static string Decrypt(byte[] ciphertext)
        {
            using (AesManaged aes = new AesManaged())
            {
                ICryptoTransform decryptor = aes.CreateDecryptor(Key, IV);
                using (MemoryStream ms = new MemoryStream(ciphertext))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }

    }
}
