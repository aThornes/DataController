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


    }
}
