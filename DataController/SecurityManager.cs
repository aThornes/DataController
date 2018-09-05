using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataController
{
    class SecurityManager
    {
        #region Password security

        #endregion

        #region General security

        #endregion

        #region File security
        public static string EncryptFile(string[] fileContents, string encryptionPassword, string salt)
        {
            string fullString = "";
            foreach (string s in fileContents)
                fullString += s + "@NL#";  //Characters denotes new line
            string toEncrypt = fullString.Substring(0, fullString.Length - 4);

            return AESTwoWayEncryption.Encrypt<TripleDESCryptoServiceProvider>(toEncrypt, encryptionPassword, salt);
        }
        public static string[] DecryptFile(string encryptedContents, string encrpytionPassword, string salt) {
            string decrpytedData = AESTwoWayEncryption.Decrypt<TripleDESCryptoServiceProvider>(encryptedContents, encrpytionPassword, salt);
            string[] seperatedString = decrpytedData.Split(new string[] { "@NL#" }, StringSplitOptions.None);
            return seperatedString;
        }

        #endregion

        #region Generation functions
        public static string GenerateNewSALT(int len = 32)
        {
            byte[] salt = new byte[len];
            string newSalt;
            System.Security.Cryptography.RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetBytes(salt);
            newSalt = Convert.ToBase64String(salt);
            return newSalt;
            //using (var ran = new RNGCryptoServiceProvider()) { ran.GetNonZeroBytes(salt); }
            //return Encoding.UTF8.GetString(salt); ;
        }
        #endregion
    }

    public class AESTwoWayEncryption
    {
        public static string Encrypt<T>(string value, string password, string salt)
        where T : SymmetricAlgorithm, new()
        {
            DeriveBytes rgb = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt));

            SymmetricAlgorithm algorithm = new T();

            byte[] rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
            byte[] rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);

            ICryptoTransform transform = algorithm.CreateEncryptor(rgbKey, rgbIV);

            using (MemoryStream buffer = new MemoryStream())
            {
                using (CryptoStream stream = new CryptoStream(buffer, transform, CryptoStreamMode.Write))
                {
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                    {
                        writer.Write(value);
                    }
                }

                return Convert.ToBase64String(buffer.ToArray());
            }
        }

        public static string Decrypt<T>(string text, string password, string salt)
           where T : SymmetricAlgorithm, new()
        {
            DeriveBytes rgb = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt));

            SymmetricAlgorithm algorithm = new T();

            byte[] rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
            byte[] rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);

            ICryptoTransform transform = algorithm.CreateDecryptor(rgbKey, rgbIV);

            using (MemoryStream buffer = new MemoryStream(Convert.FromBase64String(text)))
            {
                using (CryptoStream stream = new CryptoStream(buffer, transform, CryptoStreamMode.Read))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
    }
}
