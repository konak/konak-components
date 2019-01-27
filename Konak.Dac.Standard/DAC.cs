using Konak.Common.Exceptions;
using Konak.Dac.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Dac
{
    /// <summary>
    /// A proxy class for interaction with database
    /// </summary>
    public class DAC
    {
        #region Properties

        /// <summary>
        /// A list of all available connections to databases, listed in the config file of the application
        /// </summary>
        public static SortedList<string, DB> CONNECTIONS { get; private set; }

        /// <summary>
        /// A connection to database used by default
        /// </summary>
        public static DB DEFAULT_CONNECTION { get; private set; }

        #endregion

        #region component Init

        public static void Init(List<KeyValuePair<string, string>> connectionStrings, string defaultConnectionKey)
        {
            DB firstConnection = null;

            CONNECTIONS = new SortedList<string, DB>();

            foreach (KeyValuePair<string, string> item in connectionStrings)
            {
                DB connection = new DB(item.Value);

                if (firstConnection == null)
                    firstConnection = connection;

                CONNECTIONS.Add(item.Key, connection);

                if (item.Key.Equals(defaultConnectionKey))
                    DEFAULT_CONNECTION = connection;
            }

            if (DEFAULT_CONNECTION == null)
                DEFAULT_CONNECTION = firstConnection;

            return;
        }

        #endregion

        #region public methods

        #region ExecuteSQLBatch

        /// <summary>
        /// Execute SQL batch job
        /// </summary>
        /// <typeparam name="T">A generic type of object the batch must return</typeparam>
        /// <param name="batch">SQL batch job object</param>
        /// <param name="throwDBException">Throw SQL execution exceptions or suspend them</param>
        /// <param name="throwGenericException">Throw Generic exceptions or suspend them</param>
        /// <param name="throwSystemException">Throw System exceptions or suspend them</param>
        /// <returns>Batch execution result object</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static T ExecuteSQLBatch<T>(SQLBatch<T> batch, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            return DEFAULT_CONNECTION.ExecuteSQLBatch<T>(batch, throwDBException, throwGenericException, throwSystemException);
        }

        /// <summary>
        /// Execute SQL batch job asynchronously
        /// </summary>
        /// <typeparam name="T">A generic type of object the batch must return</typeparam>
        /// <param name="batch">SQL batch job object</param>
        /// <param name="throwDBException">Throw SQL execution exceptions or suspend them</param>
        /// <param name="throwGenericException">Throw Generic exceptions or suspend them</param>
        /// <param name="throwSystemException">Throw System exceptions or suspend them</param>
        /// <returns>Batch execution result object</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<T> ExecuteSQLBatchAsync<T>(Func<IDbConnection, Task<T>> batch, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            return DEFAULT_CONNECTION.ExecuteSQLBatchAsync<T>(batch, throwDBException, throwGenericException, throwSystemException);
        }

        /// <summary>
        /// Execute SQL batch job
        /// </summary>
        /// <typeparam name="T">A generic type of object the batch must return</typeparam>
        /// <param name="batch">A transactional SQL batch job object</param>
        /// <param name="throwDBException">Throw SQL execution exceptions or suspend them</param>
        /// <param name="throwGenericException">Throw Generic exceptions or suspend them</param>
        /// <param name="throwSystemException">Throw System exceptions or suspend them</param>
        /// <returns>Batch execution result object</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>

        public static T ExecuteTransactionalSQLBatch<T>(SQLTransactBatch<T> batch, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            return DEFAULT_CONNECTION.ExecuteTransactionalSQLBatch<T>(batch, throwDBException, throwGenericException, throwSystemException);
        }

        /// <summary>
        /// Execute SQL batch job asyncronously
        /// </summary>
        /// <typeparam name="T">A generic type of object the batch must return</typeparam>
        /// <param name="batch">A transactional SQL batch job object</param>
        /// <param name="throwDBException">Throw SQL execution exceptions or suspend them</param>
        /// <param name="throwGenericException">Throw Generic exceptions or suspend them</param>
        /// <param name="throwSystemException">Throw System exceptions or suspend them</param>
        /// <returns>Batch execution result object</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<T> ExecuteTransactionalSQLBatchAsync<T>(Func<IDbTransaction, Task<T>> batch, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            return DEFAULT_CONNECTION.ExecuteTransactionalSQLBatchAsync<T>(batch, throwDBException, throwGenericException, throwSystemException);
        }

        #endregion

        #region FillData<T>

        /// <summary>
        /// Execute SQL command or stored procedure and fill that data into the dataOut object
        /// </summary>
        /// <typeparam name="T">A generic type of the object that will be filled with values. Type of objects that can be passed: <list type="bullet"><item><description>DataTable</description></item><item><description>DataSet</description></item></list></typeparam>
        /// <param name="dataOut">Object that will be filled with values. Type of objects that can be passed: <list type="bullet"><item><description>DataTable</description></item><item><description>DataSet</description></item></list></param>
        /// <param name="sql">SQL query, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="throwDBException">Throw SQL execution exceptions or suspend them</param>
        /// <param name="throwGenericException">Throw Generic exceptions or suspend them</param>
        /// <param name="throwSystemException">Throw System exceptions or suspend them</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static void FillData<T>(T dataOut, string sql, SqlParameter[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DEFAULT_CONNECTION.FillData<T>(dataOut, sql, parameters, commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Execute SQL command or stored procedure and fill that data into the dataOut object
        /// </summary>
        /// <typeparam name="T">A generic type of the object that will be filled with values. Type of objects that can be passed: <list type="bullet"><item><description>DataTable</description></item><item><description>DataSet</description></item></list></typeparam>
        /// <param name="dataOut">Object that will be filled with values. Type of objects that can be passed: <list type="bullet"><item><description>DataTable</description></item><item><description>DataSet</description></item></list></param>
        /// <param name="sql">SQL query, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="throwDBException">Throw SQL execution exceptions or suspend them</param>
        /// <param name="throwGenericException">Throw Generic exceptions or suspend them</param>
        /// <param name="throwSystemException">Throw System exceptions or suspend them</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static void FillData<T>(T dataOut, string sql, KeyValuePair<String, Object>[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DEFAULT_CONNECTION.FillData<T>(dataOut, sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Execute SQL command or stored procedure and fill that data into the dataOut object
        /// </summary>
        /// <typeparam name="T">A generic type of the object that will be filled with values. Type of objects that can be passed: <list type="bullet"><item><description>DataTable</description></item><item><description>DataSet</description></item></list></typeparam>
        /// <param name="dataOut">Object that will be filled with values. Type of objects that can be passed: <list type="bullet"><item><description>DataTable</description></item><item><description>DataSet</description></item></list></param>
        /// <param name="sql">SQL query, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="throwDBException">Throw SQL execution exceptions or suspend them</param>
        /// <param name="throwGenericException">Throw Generic exceptions or suspend them</param>
        /// <param name="throwSystemException">Throw System exceptions or suspend them</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static void FillData<T>(T dataOut, string sql, List<KeyValuePair<String, Object>> parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DEFAULT_CONNECTION.FillData<T>(dataOut, sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Execute SQL command or stored procedure and fill that data into the dataOut object
        /// </summary>
        /// <typeparam name="T">A generic type of the object that will be filled with values. Type of objects that can be passed: <list type="bullet"><item><description>DataTable</description></item><item><description>DataSet</description></item></list></typeparam>
        /// <param name="dataOut">Object that will be filled with values. Type of objects that can be passed: <list type="bullet"><item><description>DataTable</description></item><item><description>DataSet</description></item></list></param>
        /// <param name="sql">SQL query, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="throwDBException">Throw SQL execution exceptions or suspend them</param>
        /// <param name="throwGenericException">Throw Generic exceptions or suspend them</param>
        /// <param name="throwSystemException">Throw System exceptions or suspend them</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static void FillData<T>(T dataOut, string sql, dynamic parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DEFAULT_CONNECTION.FillData<T>(dataOut, sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Execute SQL command or stored procedure and fill that data into the dataOut object
        /// </summary>
        /// <typeparam name="T">A generic type of the object that will be filled with values. Type of objects that can be passed: <list type="bullet"><item><description>DataTable</description></item><item><description>DataSet</description></item></list></typeparam>
        /// <typeparam name="TParam">A generic type of the object the properties will be interpreted as parameters to query</typeparam>
        /// <param name="dataOut">Object that will be filled with values. Type of objects that can be passed: <list type="bullet"><item><description>DataTable</description></item><item><description>DataSet</description></item></list></param>
        /// <param name="sql">SQL query, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">object the properties will be transformed to Parameters of the SQL command</param>
        /// <param name="throwDBException">Throw SQL execution exceptions or suspend them</param>
        /// <param name="throwGenericException">Throw Generic exceptions or suspend them</param>
        /// <param name="throwSystemException">Throw System exceptions or suspend them</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static void FillData<T, TParam>(T dataOut, string sql, TParam parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DEFAULT_CONNECTION.FillData<T>(dataOut, sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }
        #endregion

        #region ExecuteReader

        /// <summary>
        /// Execute SQL command and return SqlDataReader object to read data
        /// </summary>
        /// <param name="sql">SQL command text</param>
        /// <returns>Data reader object to read data</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static SqlDataReader ExecuteReader(string sql)
        {
            return DEFAULT_CONNECTION.ExecuteReader(sql, new SqlParameter[0]);
        }

        /// <summary>
        /// Execute SQL command asynchronously and return SqlDataReader object to read data
        /// </summary>
        /// <param name="sql">SQL command text</param>
        /// <returns>Data reader object to read data</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<SqlDataReader> ExecuteReaderAsync(string sql)
        {
            return DEFAULT_CONNECTION.ExecuteReaderAsync(sql, new SqlParameter[0]);
        }

        /// <summary>
        /// Execute SQL command or stored procedure and return SqlDataReader object to read data
        /// </summary>
        /// <param name="sql">SQL command, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Data reader object to read data</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static SqlDataReader ExecuteReader(string sql, SqlParameter[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            return DEFAULT_CONNECTION.ExecuteReader(sql, parameters, commandType, throwDBException, throwGenericException, throwSystemException);
        }

        /// <summary>
        /// Execute SQL command or stored procedure and return SqlDataReader object to read data
        /// </summary>
        /// <param name="sql">SQL command, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Data reader object to read data</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static SqlDataReader ExecuteReader(string sql, KeyValuePair<String, Object>[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            return DEFAULT_CONNECTION.ExecuteReader(sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException);
        }

        /// <summary>
        /// Execute SQL command or stored procedure and return SqlDataReader object to read data
        /// </summary>
        /// <param name="sql">SQL command, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Data reader object to read data</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static SqlDataReader ExecuteReader(string sql, List<KeyValuePair<String, Object>> parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            return DEFAULT_CONNECTION.ExecuteReader(sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException);
        }

        /// <summary>
        /// Execute SQL command or stored procedure and return SqlDataReader object to read data
        /// </summary>
        /// <param name="sql">SQL command, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Data reader object to read data</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static SqlDataReader ExecuteReader(string sql, dynamic parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            return DEFAULT_CONNECTION.ExecuteReader(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException);
        }

        /// <summary>
        /// Execute SQL command or stored procedure and return SqlDataReader object to read data
        /// </summary>
        /// <typeparam name="T">Generic type of the object the properties will be transformed to sql command parameters</typeparam>
        /// <param name="sql">SQL command, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Data reader object to read data</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static SqlDataReader ExecuteReader<T>(string sql, T parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            return DEFAULT_CONNECTION.ExecuteReader(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException);
        }

        /// <summary>
        /// Execute SQL command asynchronously or stored procedure and return SqlDataReader object to read data
        /// </summary>
        /// <param name="sql">SQL command, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Data reader object to read data</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<SqlDataReader> ExecuteReaderAsync(string sql, SqlParameter[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            return DEFAULT_CONNECTION.ExecuteReaderAsync(sql, parameters, commandType, throwDBException, throwGenericException, throwSystemException);
        }

        /// <summary>
        /// Execute SQL command asynchronously or stored procedure and return SqlDataReader object to read data
        /// </summary>
        /// <param name="sql">SQL command, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Data reader object to read data</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<SqlDataReader> ExecuteReaderAsync(string sql, KeyValuePair<String, Object>[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            return DEFAULT_CONNECTION.ExecuteReaderAsync(sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException);
        }

        /// <summary>
        /// Execute SQL command asynchronously or stored procedure and return SqlDataReader object to read data
        /// </summary>
        /// <param name="sql">SQL command, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Data reader object to read data</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<SqlDataReader> ExecuteReaderAsync(string sql, List<KeyValuePair<String, Object>> parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            return DEFAULT_CONNECTION.ExecuteReaderAsync(sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException);
        }

        /// <summary>
        /// Execute SQL command asynchronously or stored procedure and return SqlDataReader object to read data
        /// </summary>
        /// <param name="sql">SQL command, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Data reader object to read data</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<SqlDataReader> ExecuteReaderAsync(string sql, dynamic parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            return DEFAULT_CONNECTION.ExecuteReaderAsync(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException);
        }

        /// <summary>
        /// Execute SQL command asynchronously or stored procedure and return SqlDataReader object to read data
        /// </summary>
        /// <typeparam name="T">Generic type of the object the properties will be transformed to sql command parameters</typeparam>
        /// <param name="sql">SQL command, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Data reader object to read data</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<SqlDataReader> ExecuteReaderAsync<T>(string sql, T parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            return DEFAULT_CONNECTION.ExecuteReaderAsync(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException);
        }
        #endregion

        #region GetDataSet

        /// <summary>
        /// Get new dataset for specified SQL command
        /// </summary>
        /// <param name="sql">SQL command text to execute</param>
        /// <returns>New DataSet of results of SQL command</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static DataSet GetDataSet(string sql)
        {
            return DEFAULT_CONNECTION.GetDataSet(sql, new SqlParameter[0]);
        }

        /// <summary>
        /// Get new dataset for specified SQL command or stored procedure
        /// </summary>
        /// <param name="sql">SQL command text, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>New DataSet of results of SQL command</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static DataSet GetDataSet(string sql, SqlParameter[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            return DEFAULT_CONNECTION.GetDataSet(sql, parameters, commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Get new dataset for specified SQL command or stored procedure
        /// </summary>
        /// <param name="sql">SQL command text, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>New DataSet of results of SQL command</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static DataSet GetDataSet(string sql, KeyValuePair<String, Object>[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            return DEFAULT_CONNECTION.GetDataSet(sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Get new dataset for specified SQL command or stored procedure
        /// </summary>
        /// <param name="sql">SQL command text, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>New DataSet of results of SQL command</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static DataSet GetDataSet(string sql, List<KeyValuePair<String, Object>> parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            return DEFAULT_CONNECTION.GetDataSet(sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Get new dataset for specified SQL command or stored procedure
        /// </summary>
        /// <param name="sql">SQL command text, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>New DataSet of results of SQL command</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static DataSet GetDataSet(string sql, dynamic parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            return DEFAULT_CONNECTION.GetDataSet(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Get new dataset for specified SQL command or stored procedure
        /// </summary>
        /// <typeparam name="T">Generic type of the object the properties will be transformed to sql command parameters</typeparam>
        /// <param name="sql">SQL command text, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>New DataSet of results of SQL command</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static DataSet GetDataSet<T>(string sql, T parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            return DEFAULT_CONNECTION.GetDataSet(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        #endregion

        #region FillDataSet

        /// <summary>
        /// Fill provided DataSet item with values from executed SQL command
        /// </summary>
        /// <param name="ds">A DatSet item that must be filled with data</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static void FillDataSet(DataSet ds, string sql)
        {
            DEFAULT_CONNECTION.FillDataSet(ds, sql, new SqlParameter[0]);
        }

        /// <summary>
        /// Fill provided DataSet item with values from executed SQL command
        /// </summary>
        /// <param name="ds">A DatSet item that must be filled with data</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static void FillDataSet(DataSet ds, string sql, int startRecord, int maxRecords)
        {
            DEFAULT_CONNECTION.FillDataSet(ds, sql, new SqlParameter[0], startRecord: startRecord, maxRecords: maxRecords);
        }

        /// <summary>
        /// Fill provided DataSet item with values from executed SQL command
        /// </summary>
        /// <param name="ds">A DatSet item that must be filled with data</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static void FillDataSet(DataSet ds, string sql, SqlParameter[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DEFAULT_CONNECTION.FillData<DataSet>(ds, sql, parameters, commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Fill provided DataSet item with values from executed SQL command
        /// </summary>
        /// <param name="ds">A DatSet item that must be filled with data</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static void FillDataSet(DataSet ds, string sql, KeyValuePair<String, Object>[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DEFAULT_CONNECTION.FillData<DataSet>(ds, sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Fill provided DataSet item with values from executed SQL command
        /// </summary>
        /// <param name="ds">A DatSet item that must be filled with data</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static void FillDataSet(DataSet ds, string sql, List<KeyValuePair<String, Object>> parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DEFAULT_CONNECTION.FillData<DataSet>(ds, sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Fill provided DataSet item with values from executed SQL command
        /// </summary>
        /// <param name="ds">A DatSet item that must be filled with data</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static void FillDataSet(DataSet ds, string sql, dynamic parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DEFAULT_CONNECTION.FillData<DataSet>(ds, sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Fill provided DataSet item with values from executed SQL command
        /// </summary>
        /// <typeparam name="T">Generic type of the object the properties will be transformed to sql command parameters</typeparam>
        /// <param name="ds">A DatSet item that must be filled with data</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static void FillDataSet<T>(DataSet ds, string sql, T parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DEFAULT_CONNECTION.FillData<DataSet>(ds, sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        #endregion

        #region GetDataTable

        /// <summary>
        /// Get new DataTable for specified sql command
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <returns>New DataTable item with results of the SQL command</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static DataTable GetDataTable(string sql)
        {
            return DEFAULT_CONNECTION.GetDataTable(sql, new SqlParameter[0]);
        }

        /// <summary>
        /// Get new DataTable for specified sql command
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string sql, SqlParameter[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            return DEFAULT_CONNECTION.GetDataTable(sql, parameters, commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Get new DataTable for specified sql command
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string sql, KeyValuePair<String, Object>[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            return DEFAULT_CONNECTION.GetDataTable(sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Get new DataTable for specified sql command
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string sql, List<KeyValuePair<String, Object>> parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            return DEFAULT_CONNECTION.GetDataTable(sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Get new DataTable for specified sql command
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string sql, dynamic parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            return DEFAULT_CONNECTION.GetDataTable(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Get new DataTable for specified sql command
        /// </summary>
        /// <typeparam name="T">Generic type of the object the properties will be transformed to sql command parameters</typeparam>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <returns></returns>
        public static DataTable GetDataTable<T>(string sql, T parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            return DEFAULT_CONNECTION.GetDataTable(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        #endregion

        #region FillDataTable

        /// <summary>
        /// Fill provided DataTable item with SQL command values
        /// </summary>
        /// <param name="dt">A DataTable item</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        public static void FillDataTable(DataTable dt, string sql, int startRecord = 0, int maxRecords = 0)
        {
            DEFAULT_CONNECTION.FillDataTable(dt, sql, new SqlParameter[0], startRecord: startRecord, maxRecords: maxRecords);
        }

        /// <summary>
        /// Fill provided DataTable item with SQL command values
        /// </summary>
        /// <param name="dt">A DataTable item</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static void FillDataTable(DataTable dt, string sql, SqlParameter[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DEFAULT_CONNECTION.FillData<DataTable>(dt, sql, parameters, commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Fill provided DataTable item with SQL command values
        /// </summary>
        /// <param name="dt">A DataTable item</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static void FillDataTable(DataTable dt, string sql, KeyValuePair<String, Object>[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DEFAULT_CONNECTION.FillData<DataTable>(dt, sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Fill provided DataTable item with SQL command values
        /// </summary>
        /// <param name="dt">A DataTable item</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static void FillDataTable(DataTable dt, string sql, List<KeyValuePair<String, Object>> parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DEFAULT_CONNECTION.FillData<DataTable>(dt, sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Fill provided DataTable item with SQL command values
        /// </summary>
        /// <param name="dt">A DataTable item</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static void FillDataTable(DataTable dt, string sql, dynamic parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DEFAULT_CONNECTION.FillData<DataTable>(dt, sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Fill provided DataTable item with SQL command values
        /// </summary>
        /// <typeparam name="T">Generic type of the object the properties will be transformed to sql command parameters</typeparam>
        /// <param name="dt">A DataTable item</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static void FillDataTable<T>(DataTable dt, string sql, T parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DEFAULT_CONNECTION.FillData<DataTable>(dt, sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        #endregion

        #region ExecuteNonQuery

        /// <summary>
        /// Execute SQL query and return the number of affected values
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <returns>Number of affected rows</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static int ExecuteNonQuery(string sql)
        {
            return DEFAULT_CONNECTION.ExecuteNonQuery(sql, new SqlParameter[0]);
        }

        /// <summary>
        /// Execute SQL query asynchronously and return the number of affected values
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <returns>Number of affected rows</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<int> ExecuteNonQueryAsync(string sql)
        {
            return DEFAULT_CONNECTION.ExecuteNonQueryAsync(sql, new SqlParameter[0]);
        }

        /// <summary>
        /// Execute SQL query and return the number of affected values
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Number of affected rows</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static int ExecuteNonQuery(string sql, SqlParameter[] parameters, CommandType commandType = CommandType.Text)
        {
            return DEFAULT_CONNECTION.ExecuteNonQuery(sql, parameters, commandType);
        }

        /// <summary>
        /// Execute SQL query and return the number of affected values
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Number of affected rows</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static int ExecuteNonQuery(string sql, KeyValuePair<String, Object>[] parameters, CommandType commandType = CommandType.Text)
        {
            return DEFAULT_CONNECTION.ExecuteNonQuery(sql, parameters.ToSqlParameters(), commandType);
        }

        /// <summary>
        /// Execute SQL query and return the number of affected values
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Number of affected rows</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static int ExecuteNonQuery(string sql, List<KeyValuePair<String, Object>> parameters, CommandType commandType = CommandType.Text)
        {
            return DEFAULT_CONNECTION.ExecuteNonQuery(sql, parameters.ToSqlParameters(), commandType);
        }

        /// <summary>
        /// Execute SQL query and return the number of affected values
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Number of affected rows</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static int ExecuteNonQuery(string sql, dynamic parameters, CommandType commandType = CommandType.Text)
        {
            return DEFAULT_CONNECTION.ExecuteNonQuery(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType);
        }

        /// <summary>
        /// Execute SQL query and return the number of affected values
        /// </summary>
        /// <typeparam name="T">Generic type of the object the properties will be transformed to sql command parameters</typeparam>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Number of affected rows</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static int ExecuteNonQuery<T>(string sql, T parameters, CommandType commandType = CommandType.Text)
        {
            return DEFAULT_CONNECTION.ExecuteNonQuery(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType);
        }

        /// <summary>
        /// Execute SQL query asynchronously and return the number of affected values
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Number of affected rows</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<int> ExecuteNonQueryAsync(string sql, SqlParameter[] parameters, CommandType commandType = CommandType.Text)
        {
            return DEFAULT_CONNECTION.ExecuteNonQueryAsync(sql, parameters, commandType);
        }

        /// <summary>
        /// Execute SQL query asynchronously and return the number of affected values
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Number of affected rows</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<int> ExecuteNonQueryAsync(string sql, KeyValuePair<String, Object>[] parameters, CommandType commandType = CommandType.Text)
        {
            return DEFAULT_CONNECTION.ExecuteNonQueryAsync(sql, parameters.ToSqlParameters(), commandType);
        }

        /// <summary>
        /// Execute SQL query asynchronously and return the number of affected values
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Number of affected rows</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<int> ExecuteNonQueryAsync(string sql, List<KeyValuePair<String, Object>> parameters, CommandType commandType = CommandType.Text)
        {
            return DEFAULT_CONNECTION.ExecuteNonQueryAsync(sql, parameters.ToSqlParameters(), commandType);
        }

        /// <summary>
        /// Execute SQL query asynchronously and return the number of affected values
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Number of affected rows</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<int> ExecuteNonQueryAsync(string sql, dynamic parameters, CommandType commandType = CommandType.Text)
        {
            return DEFAULT_CONNECTION.ExecuteNonQueryAsync(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType);
        }

        /// <summary>
        /// Execute SQL query asynchronously and return the number of affected values
        /// </summary>
        /// <typeparam name="T">Generic type of the object the properties will be transformed to sql command parameters</typeparam>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Number of affected rows</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<int> ExecuteNonQueryAsync<T>(string sql, T parameters, CommandType commandType = CommandType.Text)
        {
            return DEFAULT_CONNECTION.ExecuteNonQueryAsync(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType);
        }

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// Execute SQL command and return value of first column of the first row from results
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <returns>Value of first column of the first row</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static object ExecuteScalar(string sql)
        {
            return DEFAULT_CONNECTION.ExecuteScalar(sql, new SqlParameter[0]);
        }

        /// <summary>
        /// Execute SQL command asynchronously and return value of first column of the first row from results
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <returns>Value of first column of the first row</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<object> ExecuteScalarAsync(string sql)
        {
            return DEFAULT_CONNECTION.ExecuteScalarAsync(sql, new SqlParameter[0]);
        }

        /// <summary>
        /// Execute SQL command and return value of first column of the first row from results
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Value of first column of the first row</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static object ExecuteScalar(string sql, SqlParameter[] parameters, CommandType commandType = CommandType.Text)
        {
            return DEFAULT_CONNECTION.ExecuteScalar(sql, parameters, commandType);
        }

        /// <summary>
        /// Execute SQL command and return value of first column of the first row from results
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Value of first column of the first row</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static object ExecuteScalar(string sql, KeyValuePair<String, Object>[] parameters, CommandType commandType = CommandType.Text)
        {
            return DEFAULT_CONNECTION.ExecuteScalar(sql, parameters.ToSqlParameters(), commandType);
        }

        /// <summary>
        /// Execute SQL command and return value of first column of the first row from results
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Value of first column of the first row</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static object ExecuteScalar(string sql, List<KeyValuePair<String, Object>> parameters, CommandType commandType = CommandType.Text)
        {
            return DEFAULT_CONNECTION.ExecuteScalar(sql, parameters.ToSqlParameters(), commandType);
        }

        /// <summary>
        /// Execute SQL command and return value of first column of the first row from results
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Value of first column of the first row</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static object ExecuteScalar(string sql, dynamic parameters, CommandType commandType = CommandType.Text)
        {
            return DEFAULT_CONNECTION.ExecuteScalar(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType);
        }

        /// <summary>
        /// Execute SQL command and return value of first column of the first row from results
        /// </summary>
        /// <typeparam name="T">Generic type of the object the properties will be transformed to sql command parameters</typeparam>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Value of first column of the first row</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static object ExecuteScalar<T>(string sql, T parameters, CommandType commandType = CommandType.Text)
        {
            return DEFAULT_CONNECTION.ExecuteScalar(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType);
        }

        /// <summary>
        /// Execute SQL command asynchronously and return value of first column of the first row from results
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Value of first column of the first row</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<object> ExecuteScalarAsync(string sql, SqlParameter[] parameters, CommandType commandType = CommandType.Text)
        {
            return DEFAULT_CONNECTION.ExecuteScalarAsync(sql, parameters, commandType);
        }

        /// <summary>
        /// Execute SQL command asynchronously and return value of first column of the first row from results
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Value of first column of the first row</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<object> ExecuteScalarAsync(string sql, KeyValuePair<String, Object>[] parameters, CommandType commandType = CommandType.Text)
        {
            return DEFAULT_CONNECTION.ExecuteScalarAsync(sql, parameters.ToSqlParameters(), commandType);
        }

        /// <summary>
        /// Execute SQL command asynchronously and return value of first column of the first row from results
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Value of first column of the first row</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<object> ExecuteScalarAsync(string sql, List<KeyValuePair<String, Object>> parameters, CommandType commandType = CommandType.Text)
        {
            return DEFAULT_CONNECTION.ExecuteScalarAsync(sql, parameters.ToSqlParameters(), commandType);
        }

        /// <summary>
        /// Execute SQL command asynchronously and return value of first column of the first row from results
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Value of first column of the first row</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<object> ExecuteScalarAsync(string sql, dynamic parameters, CommandType commandType = CommandType.Text)
        {
            return DEFAULT_CONNECTION.ExecuteScalarAsync(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType);
        }

        /// <summary>
        /// Execute SQL command asynchronously and return value of first column of the first row from results
        /// </summary>
        /// <typeparam name="T">Generic type of the object the properties will be transformed to sql command parameters</typeparam>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Value of first column of the first row</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<object> ExecuteScalarAsync<T>(string sql, T parameters, CommandType commandType = CommandType.Text)
        {
            return DEFAULT_CONNECTION.ExecuteScalarAsync(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType);
        }

        #endregion

        #endregion

    }
}
