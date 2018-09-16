using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataController
{
    /// <summary>
    /// Handles major commands pased, extend this class for additional database commands
    /// </summary>
    class DataCommand
    {
        /// <summary>
        /// Set up database connection, this is required for all other DataCommands
        /// </summary>
        /// <param name="server">Server IP</param>
        /// <param name="database">Database name</param>
        /// <param name="username">Connection username</param>
        /// <param name="password">Connection password</param>
        /// <param name="trustedConnection">Trusted connection?</param>
        /// <param name="timeoutDelay">Period before timeout (s)</param>
        public static void InitaliseDatabase(string server, string database, string username, string password, bool trustedConnection = false, int timeoutDelay = 15)
        {
            DataController.Initalise(server,database,username,password,trustedConnection, timeoutDelay);
            
        }

        /// <summary>
        /// Attempt to establish connection with specified database
        /// </summary>
        /// <returns>True for successful connection</returns>
        public static bool ConnectToDatabase()
        {
            try {
                DataController.OpenConnection();
                return true;
            }
            catch { return false; }
        }
        #region Primary Commands
        /// <summary>
        /// Generate a new row in the database
        /// </summary>
        /// <param name="tableName">Name of table</param>
        /// <param name="newData">Data to be added</param>
        /// <param name="columnsTargetted">Columns to add data (null for all columns)</param>
        /// <returns>If successful</returns>
        public static string GenerateNewRow(string tableName, string[] newData, string[] columnsTargetted = null)
        {
            return DataController.CreateRowEntry(tableName, newData, columnsTargetted);
        }

        /// <summary>
        /// Fetch data from the database for the given parameters
        /// </summary>
        /// <param name="tableName">Name of table</param>
        /// <param name="queryTerm">Search query column</param>
        /// <param name="queryItem">Column query item</param>
        /// <param name="requestedColumns">Columns to fetch</param>
        /// <returns>string array of all column data found</returns>
        public static string[] FetchDatabaseData(string tableName,  string queryTerm, string queryItem, string[] requestedColumns = null)
        {
            return DataController.FetchDataRow(tableName, queryTerm, queryItem, requestedColumns);
        }
        #endregion
        #region Private base commands


        #endregion


    }
}
