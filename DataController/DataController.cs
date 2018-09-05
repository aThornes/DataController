using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataController
{
    public class DataController
    {
        private static SqlConnection connection;
        /// <summary>
        /// Intialise the database controller, set priminary connection parameters
        /// </summary>
        public static void Initalise(string server, string database, string username, string password, bool trustedConnection = false, int timeoutDelay = 15)
        {
            string connectionString = "user id=" + username + ";" +
                "pwd=" + password + ";" +
                "server=" + server + ";" +
                "Intial Catalog=" + database + ";" +
                "MultipleActiveResultSets=" + "true" + ";" +
                "connection timeout=" + timeoutDelay + ";";
            if (trustedConnection)
                connectionString += "Trusted_Connection=true";

            connection = new SqlConnection(connectionString);
        }
        /// <summary>
        /// Attempt to open database connection
        /// </summary>
        /// <returns></returns>
        public static bool OpenConnection()
        {
            if (connection != null)
            {
                connection.Open();
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Attempt to close database connection
        /// </summary>
        /// <returns></returns>
        public static bool CloseConnection() {
            if (connection != null)
            {                
                connection.Close();
                return true;
            }
            else return false;
        }
    }
}
