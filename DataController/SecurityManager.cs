using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataController
{
    /// <summary>
    /// Security manager, handles encryption/decrpytion and relevant additional functions
    /// </summary>
    class SecurityManager
    {
        private static string defaultSALT = "KqUvn9D1"; //Not at all secure, only to be used for non-search columns for non-sensitive data

        #region Password security
        /// <summary>
        /// Encrypt password
        /// </summary>
        /// <param name="pass">Original, unencrypted password</param>
        /// <param name="salt">Cryptographically random salt</param>
        /// <returns>Encrypted password string</returns>
        public string EncryptPassword(string pass, string salt)
        {
            using (MD5 crypto = MD5.Create())
            {
                byte[] passByteArray = Encoding.ASCII.GetBytes(pass + salt);
                byte[] hashArray = crypto.ComputeHash(passByteArray);

                return Encoding.ASCII.GetString(hashArray);
            }
        }
        /// <summary>
        /// Check if password matches encrypted pass
        /// </summary>
        /// <param name="encryptedPassword"></param>
        /// <param name="passToCheck"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public bool ComparePassword(string encryptedPassword, string passToCheck, string salt) {
            string encPass = EncryptPassword(passToCheck, salt);
            if (encryptedPassword == encPass)
                return true;
            return false;
        }
        #endregion

        #region General security        
        /// <summary>
        /// Encrypt data to be put into the database (Two way encrpytion) || Not to be used for passwords
        /// </summary>
        /// <param name="data">Data to be encrypted</param>
        /// <param name="encryptionPass">Encrpytion password</param>
        /// <param name="isSearchTerm">Is this a database search term?</param>
        /// <returns></returns>
        public static string EncryptData(string data, string encryptionPass, bool isSearchTerm = false)
        {
            string salt = GenerateNewSALT(8);
            if (isSearchTerm)
                salt = defaultSALT;

            string encryptedString = AESTwoWayEncryption.Encrypt<TripleDESCryptoServiceProvider>(data, encryptionPass, salt);
            if (isSearchTerm) return encryptedString;
            return salt + encryptedString;
        }
        /// <summary>
        /// Decrpyt data from the database (Two way encryption)
        /// </summary>
        /// <param name="encrpytedData">Data to be decrypted</param>
        /// <param name="encryptionPass">Encrpyption password</param>
        /// <param name="isSearchTerm">Is this a database search term?</param>
        /// <returns></returns>
        public static string DecrpytData(string encrpytedData, string encryptionPass, bool isSearchTerm = false)
        {
            if (isSearchTerm)
                return AESTwoWayEncryption.Decrypt<TripleDESCryptoServiceProvider>(encrpytedData, encryptionPass, defaultSALT);

            string salt = encrpytedData.Substring(0, 8);
            string toDecrypt = encrpytedData.Substring(8, encrpytedData.Count() - 8);

            return AESTwoWayEncryption.Decrypt<TripleDESCryptoServiceProvider>(toDecrypt, encryptionPass, salt);
        }
        #endregion

        #region File security
        /// <summary>
        /// Encrypt file contents into multiple lines
        /// </summary>
        /// <param name="fileContents">Original file contents</param>
        /// <param name="encryptionPassword">Encryption password</param>
        /// <param name="salt">Cryptographically genereated salt key</param>
        /// <returns></returns>
        public static string[] EncryptFile(string[] fileContents, string encryptionPassword, string salt)
        {
            string fullString = "";
            foreach (string s in fileContents)
                fullString += s + "@NL#";  //Characters denotes new line
            string toEncrypt = fullString.Substring(0, fullString.Length - 4);

            string encrpytedData = salt + "|SSPP|" + AESTwoWayEncryption.Encrypt<TripleDESCryptoServiceProvider>(toEncrypt, encryptionPassword, salt); //Salt string partion point seperates data and salt

            return SplitStringEveryN(encrpytedData, 20);
        }
        /// <summary>
        /// Decrypt file into original file contents
        /// </summary>
        /// <param name="fileContents">Original file contents</param>
        /// <param name="encryptionPassword">Encryption password</param>
        public static string[] DecryptFile(string[] encryptedContents, string encrpytionPassword)
        {
            string encryptionString = CombineStringArray(encryptedContents);
            string[] parts = encryptionString.Split(new string[] { "|SSPP|" }, StringSplitOptions.None); //Split encrypted data to get string and main contents
            string salt = parts[0];

            string decrpytedData = AESTwoWayEncryption.Decrypt<TripleDESCryptoServiceProvider>(parts[1], encrpytionPassword, salt);

            string[] seperatedString = decrpytedData.Split(new string[] { "@NL#" }, StringSplitOptions.None); //Split string back up into original lines
            return seperatedString;
        }

        #endregion

        #region Generation functions
        /// <summary>
        /// Generates a cyptographically random salt key
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string GenerateNewSALT(int len = 32)
        {
            byte[] salt = new byte[len];
            string newSalt;
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetBytes(salt);
            newSalt = Convert.ToBase64String(salt);
            return newSalt;
            //using (var ran = new RNGCryptoServiceProvider()) { ran.GetNonZeroBytes(salt); }
            //return Encoding.UTF8.GetString(salt); ;
        }
        #endregion

        #region Additional functions
        private static string[] SplitStringEveryN(string toSplit, int nCharacters)
        {
            int lines = (int)Math.Ceiling((double)(toSplit.Count() / nCharacters)); //Number of lines that will be returned
            string[] toReturn = new string[lines];
            int x = 0;
            for (int n = 0; n < toSplit.Count(); n += nCharacters)
            {
                int toAdd = nCharacters;
                if ((n + nCharacters) > toSplit.Count())
                    toAdd = toSplit.Count() - n;
                toReturn[x] = toSplit.Substring(n, toAdd);
                x++;
            }
            return toReturn;
        }

        private static string CombineStringArray(string[] stringArray)
        {
            string combinedString = "";
            foreach (string s in stringArray)
                combinedString += s;
            return combinedString;
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
