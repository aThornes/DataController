using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataController
{
    public class DataController
    {
        private static SqlConnection connection;
        #region Global database commands
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
        public static bool CloseConnection()
        {
            if (connection != null)
            {
                connection.Close();
                return true;
            }
            else return false;
        }
        #endregion

        #region Data communciation
        private static List<string[]> GeneralFetchQuery(string tableName, string[] requestedColumns, string queryColumn, string dataQuery)
        {
            bool allColumns = false;
            if (requestedColumns == null) //If no columns are specified then query will return all columns
                allColumns = true;

            //Construct command string
            string commandString = SQLFetchQueryBuilder(requestedColumns.Count(), allColumns);
            SqlCommand cmd = new SqlCommand(commandString, connection);

            //Construct parameters
            string[] parameterNames = new string[3] { "table", "queryColumn", "dataQuery" };
            string[] parameterValues = new string[3] { tableName, queryColumn, dataQuery };
            foreach (SqlParameter sqParam in GenParameters(parameterNames, parameterValues))
                cmd.Parameters.Add(sqParam);
            //Build column parameters
            foreach (SqlParameter sqParam in GenColumnParameters(requestedColumns))
                cmd.Parameters.Add(sqParam);


            List<string[]> databaseResponse = new List<string[]>();
            //Execute command and read response
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                string[] response = new string[requestedColumns.Count()];
                int n = 0;
                while (n < reader.FieldCount)
                {
                    response[n] = reader[n].ToString();
                    n++;
                }
                databaseResponse.Add(response);
            }
            return databaseResponse;
        }
        #endregion

        #region Supporting functions
        private static string SQLFetchQueryBuilder(int columnAmount, bool allColumns = false, bool query = true)
        {
            string SQLCommandString = "";

            SQLCommandString += "SELECT ";
            if (!allColumns) //If requesting all columns, only need *
            {
                for (int n = 0; n < columnAmount; n++)
                {
                    SQLCommandString += "@column" + n.ToString();
                    if (n != columnAmount) SQLCommandString += ", ";
                }
            }
            else
                SQLCommandString += "* ";
            SQLCommandString += " FROM @table";
            if (query) //If select criteria is required
            {
                SQLCommandString += " WHERE @queryColumn = @dataQuery ;";
            }

            return SQLCommandString;
        }

        private static SqlParameter GenParameter(string parameterName, string parameterValue, SqlDbType dataType = SqlDbType.NVarChar)
        {
            SqlParameter param = new SqlParameter(parameterName, SqlDbType.NVarChar)
            {
                Value = parameterValue
            };
            return param;
        }
        private static SqlParameter[] GenParameters(string[] parameterName, string[] parameterValue, SqlDbType dataType = SqlDbType.NVarChar)
        {
            SqlParameter[] parameters = new SqlParameter[parameterName.Count()];

            if (parameterName.Count() != parameterValue.Count()) throw new ArrayTypeMismatchException("All parameters must hold both name and value");
            for (int n = 0; n < parameterName.Count(); n++)
            {
                SqlParameter param = new SqlParameter(parameterName[n], SqlDbType.NVarChar)
                {
                    Value = parameterValue[n]
                };
                parameters[n] = param;
            }
            return parameters;
        }

        private static SqlParameter[] GenColumnParameters(string[] columns)
        {
            string[] paramName = new string[columns.Count()];
            string[] paramValue = new string[columns.Count()];

            for (int n = 0; n < columns.Count(); n++)
            {
                paramName[n] = columns + n.ToString();
                paramValue[n] = columns[n];
            }

            return GenParameters(paramName, paramValue);
        }
        #endregion
    }
}
