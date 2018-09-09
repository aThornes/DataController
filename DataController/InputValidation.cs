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
                    throw new ArgumentNullException("Provided column " + col + " does not exist in the database");
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

        private static bool RegexCheck(string l, string regExpression)
        {
            Regex regex = new Regex(regExpression);
            if (regex.IsMatch(l))
                return true;
            else
                return false;
        }

        private static bool CheckLength(string l, int minLength, int maxLength)
        {
            int length = l.Count();
            if (length >= minLength && length <= maxLength)
                return true;
            else
                return false;
        }
        #endregion
    }

}
