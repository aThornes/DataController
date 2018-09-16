using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataController
{
    /// <summary>
    /// Handles all input validation, extend this class for further validation methods or to override existing
    /// </summary>
    class InputValidation
    {
        private static DatabaseInformation dbInfo;

        /// <summary>
        /// Updates database information to be used for validation
        /// </summary>
        public static DatabaseInformation SetInformationClass { set => dbInfo = value; }

        #region Database submission validation

        /// <summary>
        /// Check if all columns exist within the given table
        /// </summary>
        /// <param name="tableName">Name of table in database</param>
        /// <param name="columns">Columns to check</param>  
        public static void ValidateColumns(string tableName, string[] columns, bool ignoreIfNull = false)
        {
            if (ignoreIfNull && columns == null)
                return;
            CheckDatabaseInformation();
            foreach (string col in columns)
            {
                if (!dbInfo.DoesColumnExist(tableName, col))
                    throw new ArgumentNullException("Provided column ," + col + ", does not exist in the database");
            }
        }
        /// <summary>
        /// Check if a column exist within the given table
        /// </summary>
        /// <param name="tableName">Name of table in database</param>
        /// <param name="columns">Columns to check</param>  
        public static void ValidateColumn(string tableName, string column, bool ignoreIfNull = false)
        {
            ValidateColumns(tableName, new string[] { column }, ignoreIfNull);
        }

        /// <summary>
        /// Check if amount of columns match amount found in table
        /// </summary>
        /// <param name="tableName">Name of table</param>
        /// <param name="newColumns">Number of new columns</param>
        public static void ValidateNewEntry(string tableName, int newColumns)
        {
            if (dbInfo.GetColumns(tableName).Count() != newColumns)
                throw new ArgumentException("Provided arguments do not match columns in the " + tableName + " table");
        }

        /// <summary>
        /// Ensure that dbInfo is not null (throws exception if method fails)
        /// </summary>
        private static void CheckDatabaseInformation()
        {
            if (dbInfo == null)
                throw new NullReferenceException("Database information file must be defined before validating data");
        }

        #endregion

        #region Data input validation

        /// <summary>
        /// Validate a user's name (Forename / surname or both) 
        /// <para>Returns null if succesful, or returns with validation error</para>
        /// </summary>
        /// <param name="toValidate">Text to be validates</param>
        /// <param name="minLength">Minimum name length</param>
        /// <param name="maxLength">Maximum name length</param>
        /// <returns></returns>
        public static string ValidateName(string toValidate, int minLength = 4, int maxLength = 20)
        {
            return null;
        }

        /// <summary>
        /// Validate a username
        /// <para>Returns null if succesful, or returns with validation error</para>
        /// </summary>
        /// <param name="toValidate">Text to be validates</param>
        /// <param name="minLength">Minimum name length</param>
        /// <param name="maxLength">Maximum name length</param>
        /// <returns></returns>
        public static string ValidateUsername(string toValidate, int minLength = 3, int maxLength = 32)
        {
            return null;
        }

        public static string ValidatePassword(string toValidate, int minLength = 8, int maxLength = 32)
        {
            return null;
        }

        public static string ValidateEmail(string toValidate, int minLength = 5, int maxLength = 32)
        {
            return null;
        }

        public static string ValidatePhone(string toValidate, int minLength = 8, int maxLength = 32)
        {
            return null;
        }

        public static string ValidateAddress(string toValidate, int minLength = 8, int maxLength = 32)
        {
            return null;
        }

        public static string ValidatePostCode(string toValidate, int minLength = 8, int maxLength = 32)
        {
            return null;
        }

        #endregion
    }

    public static class ValidationExtensionMethods
    {
        #region Useful checks
        /// <summary>
        /// Check if string is null or is empty ("")
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string value)
        {
            if (value == null || value == "")
                return true;
            return false;
        }

        /// <summary>
        /// Check if value length is between given minimum and maximum
        /// </summary>
        /// <param name="value"></param>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static bool HasLengthBetween(this string value, int minLength, int maxLength)
        {
            if (value.IsNullOrEmpty()) return false;
            int length = value.Count();
            if (length >= minLength && length <= maxLength)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Check if string matches the given regex expression
        /// </summary>
        /// <param name="value"></param>
        /// <param name="regexExpression"></param>
        /// <returns></returns>
        public static bool DoesMatchRegex(this string value, string regexExpression)
        {
            Regex regex = new Regex(regexExpression);
            if (regex.IsMatch(value))
                return true;
            else
                return false;
        }
        #endregion

        #region String validation methods
        /// <summary>
        /// Checks if string is a valid email address
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEmail(this string value)
        {
            if (value.IsNullOrEmpty()) return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(value); //This will fail if value is not a valid email
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Check if string is a valid postcode (note: only checks format, not if postcode exists)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsPostcode(this string value)
        {
            //TODO (possible) check if postcode actually exists
            if (value.IsNullOrEmpty()) return false;

            string regExpression = @"^([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|
                                    (([A-Za-z][0-9][A-Za-z])|
                                    ([A-Za-z][A-Ha-hJ-Yj-y][0-9][A-Za-z]?))))\s?[0-9][A-Za-z]{2})?$";
            if (value.DoesMatchRegex(regExpression))
                return true;
            else
                return false;

        }

        /// <summary>
        /// Checks if string is a valid UK phone
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsPhone(this string value)
        {
            if (value.IsNullOrEmpty()) return false;

            string toCheck  = value;
            if (value[0] == '0' && value.Length == 11)
                toCheck = value.Substring(1, value.Length - 1); //Remove first 0 if user wrote full phone (i.e. 07878112912 -> +447878112912)
            toCheck = "+44" + toCheck;
            string regExpression =
                @"^(((\+44\s?\d{4}|\(?0\d{4}\)?)\s?\d{3}\s?\d{3})|((\+44\s?\d{3}|\(?0\d{3}\)?)\s?\d{3}\s?\d{4})|((\+44\s?\d{2}|\(?0\d{2}\)?)\s?\d{4}\s?\d{4}))(\s?\#(\d{4}|\d{3}))?$";

            if (toCheck.DoesMatchRegex(regExpression))            
                return true;            
            else
                return false;
        }

        /// <summary>
        /// Checks if string is a valid password
        /// </summary>
        /// <param name="value"></param>
        /// <
        /// <returns></returns>
        public static bool IsValidPassword(this string value, string expression = null)
        {
            if (value.IsNullOrEmpty()) return false;
            string regExpression = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)$";
            if (!expression.IsNullOrEmpty()) regExpression = @expression;

            if (value.HasLengthBetween(8, 32) && value.DoesMatchRegex(regExpression))
            {
                return true;
            }
            return false;
        }
        #endregion


    }
}
