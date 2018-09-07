using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataController
{
    class DatabaseCommands
    {
        private bool connectionOpen = false;

        #region Setup commands
        /// <summary>
        /// Initalise the connection parameters and establish a connection to the database
        /// </summary>
        /// <param name="server">Server IP</param>
        /// <param name="database">Name of database</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="trustedConnection">Is connection trusted?</param>
        /// <param name="timeoutDelay">Timeout delay</param>
        /// <returns>If connection was successful</returns>
        public bool InitialiseDatabase(string server, string database, string username, string password, bool trustedConnection = false, int timeoutDelay = 15)
        {
            //Initalise the data controller
            DataController.Initalise(server, database, username, password, trustedConnection, timeoutDelay);

            //Attempt to open a connection to the database
            try
            {
                DataController.OpenConnection();
                connectionOpen = true;
                return true;
            }
            catch {
                return false;
            }
        }

        /// <summary>
        /// Disconnect to the database
        /// </summary>
        /// <returns>If disconnetion was successful</returns>
        public bool DisposeConnection() {
            if (!connectionOpen)
                return false;
            DataController.CloseConnection();
            return true;
        }
        #endregion


    }
}
