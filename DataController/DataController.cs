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
        /// <summary>
        /// Execute insert request, add data to the specified table
        /// </summary>
        /// <param name="tablename">Name of table in database</param>
        /// <param name="newData">Data to be added</param>
        /// <param name="insertColumn">Columns to be added to (null for all columns)</param>
        private static void GeneralInsertNonQuery(string tableName, string[] newData, string[] insertColumn = null)
        {
            bool specifiedColumns = false;
            if (insertColumn != null)
                specifiedColumns = true;

            if (specifiedColumns && newData.Count() != insertColumn.Count())
                throw new ConstraintException("Columns do not match data");

            //Construct command string
            string commandString = SQLInsertQueryBuilder(newData.Count());
            SqlCommand cmd = new SqlCommand(commandString, connection);

            cmd.Parameters.Add(GenParameter("table", tableName));
            if (specifiedColumns)
            {
                foreach (SqlParameter sqParam in GenIteratedParameters(insertColumn, "column"))
                    cmd.Parameters.Add(sqParam);
            }
            foreach (SqlParameter sqParam in GenIteratedParameters(newData, "value"))
                cmd.Parameters.Add(sqParam);

            //Execute command
            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// Complete a fetch request for the specified columns via a search term
        /// </summary>
        /// <param name="tableName">Name of table in database</param>
        /// <param name="requestedColumns">Columns to be fetched</param>
        /// <param name="queryColumn">Column to query</param>
        /// <param name="dataQuery">Data to query</param>
        /// <returns>Database response</returns>
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
            foreach (SqlParameter sqParam in GenIteratedParameters(requestedColumns, "column"))
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
        /// <summary>
        /// Complete an update request, edit existing database data
        /// </summary>
        /// <param name="tableName">Name of table in database</param>
        /// <param name="queryColumn">Column to query</param>
        /// <param name="dataQuery">Data to query</param>
        /// <param name="updateColumns">Columns to update</param>
        /// <param name="columnData">New data to replace existing</param>
        private static void GeneralUpdateNonQuery(string tableName, string queryColumn, string dataQuery, string[] updateColumns, string[] columnData)
        {
            //Construct command string
            string commandString = SQLUpdateQueryBuilder(updateColumns.Count());
            SqlCommand cmd = new SqlCommand(commandString, connection);

            //Construct paramters
            string[] parameterNames = new string[3] { "table", "queryColumn", "dataQuery" };
            string[] parameterValues = new string[3] { tableName, queryColumn, dataQuery };
            foreach (SqlParameter sqParam in GenParameters(parameterNames, parameterValues))
                cmd.Parameters.Add(sqParam);

            //Build column and value parameters
            foreach (SqlParameter sqParam in GenIteratedParameters(updateColumns, "column"))
                cmd.Parameters.Add(sqParam);
            foreach (SqlParameter sqParam in GenIteratedParameters(columnData, "newData"))
                cmd.Parameters.Add(sqParam);

            //Execute command
            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// Execute a delete command
        ///</summary>
        /// <param name="tableName">Name of table in database</param>
        /// <param name="queryColumn">Column to query</param>
        /// <param name="dataQuery">Data to query</param>
        private static void GeneralDeleteNonQuery(string tableName, string queryColumn, string dataQuery)
        {
            string commandString = "DELETE FROM @table WHERE @queryColumn = @dataQuery";
            SqlCommand cmd = new SqlCommand(commandString, connection);

            //Construct paramters
            string[] parameterNames = new string[3] { "table", "queryColumn", "dataQuery" };
            string[] parameterValues = new string[3] { tableName, queryColumn, dataQuery };
            foreach (SqlParameter sqParam in GenParameters(parameterNames, parameterValues))
                cmd.Parameters.Add(sqParam);

            //Execute command
            cmd.ExecuteNonQuery();
        }
        #endregion

        #region Communication commands


        #endregion

        #region Supporting functions
        /// <summary>
        /// Build's the SqlCommand string for n amount of columns for a insert request
        /// </summary>
        /// <param name="columnAmount">Number of columns</param>
        ///<param name="specifiedColumns">Does the insert command require specified columns?</param>
        /// <returns>SQL command string</returns>
        private static string SQLInsertQueryBuilder(int columnAmount, bool specifiedColumns = false)
        {
            string SQLCommandString = "INSERT INTO @table ";
            string columns = "(";
            string values = "(";
            for (int n = 0; n < columnAmount; n++)
            {
                columns += "@column" + n.ToString();
                values += "@value" + n.ToString();
                if (n != columnAmount)
                {
                    columns += ", ";
                    values += ", ";
                }
                else
                {
                    columns += ") ";
                    values += ")";
                }
            }
            if (specifiedColumns)
                SQLCommandString += columns;
            SQLCommandString += "VALUES " + values + ";";

            return SQLCommandString;
        }

        /// <summary>
        /// Build's the SqlCommand string for n amount of columns for a fetch request
        /// </summary>
        /// <param name="columnAmount">Number of columns</param>
        /// <param name="allColumns">Is the request for all columns?</param>
        /// <param name="query">Does the command contain a query?</param>
        /// <returns>SQL command string</returns>
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

        /// <summary>
        /// Build's the SqlCommand string for n amuont of columns for an update query
        /// </summary>
        /// <param name="columnAmount">Columns to update</param>
        /// <param name="query">Does the command contain a query?</param>
        /// <returns>SQL command string</returns>
        private static string SQLUpdateQueryBuilder(int columnAmount, bool query = true)
        {
            string SQLCommandString = "UPDATE @table SET ";

            for (int n = 0; n < columnAmount; n++)
            {
                SQLCommandString += "@column" + n.ToString() + " = @newData" + n.ToString();
                if (n != columnAmount)
                    SQLCommandString += ", ";
            }
            if (query) //If select criteria is required
            {
                SQLCommandString += " WHERE @queryColumn = @dataQuery ;";
            }
            return SQLCommandString;
        }

        /// <summary>
        /// Generate a new sql parameter
        /// </summary>
        /// <param name="parameterName">Name of parameter</param>
        /// <param name="parameterValue">Value of parameter</param>
        /// <param name="dataType">Db datatype</param>
        /// <returns>New SqlParamater</returns>
        private static SqlParameter GenParameter(string parameterName, string parameterValue, SqlDbType dataType = SqlDbType.NVarChar)
        {
            SqlParameter param = new SqlParameter(parameterName, SqlDbType.NVarChar)
            {
                Value = parameterValue
            };
            return param;
        }
        /// <summary>
        /// Genereate an array of sql parameters
        /// </summary>
        /// <param name="parameterName">Names of parameters</param>
        /// <param name="parameterValue">Values of parameters</param>
        /// <param name="dataType">Db datatype</param>
        /// <returns></returns>
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
        /// <summary>
        /// Generate a new sql parameter foreach column requested
        /// </summary>
        /// <param name="columns">Columns requested</param>
        /// <returns>Sql parameter array for each column</returns>
        private static SqlParameter[] GenIteratedParameters(string[] columns, string iteratedString)
        {
            string[] paramName = new string[columns.Count()];
            string[] paramValue = new string[columns.Count()];

            for (int n = 0; n < columns.Count(); n++)
            {
                paramName[n] = "column" + n.ToString();
                paramValue[n] = columns[n];
            }

            return GenParameters(paramName, paramValue);
        }
        #endregion
    }

    class DatabaseInformation
    {
        //Store information about database, tables, columns etc.
    }
}
