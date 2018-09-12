using Konak.Common.Exceptions;
using Konak.Dac.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Dac.Core
{
    /// <summary>
    /// A proxy class for interaction with database
    /// </summary>
    public class DAC
    {
        #region Properties

        /// <summary>
        /// Settings of the DAC
        /// </summary>
        internal static DACSettings SETTINGS { get; private set; }

        /// <summary>
        /// A list of all available connections to databases, listed in the config file of the application
        /// </summary>
        public static SortedList<string, DB> CONNECTIONS { get; private set; }

        /// <summary>
        /// A connection to database used by default
        /// </summary>
        public static DB DEFAULT_CONNECTION { get; private set; }

        #endregion

        #region static constructor
        ///// <summary>
        ///// Static constructor of DAC class.
        ///// </summary>
        ///// <remarks>
        ///// It is called automatically before the first instance is created or any static members are referenced.
        ///// </remarks>
        //static DAC()
        //{
        //    Init();
        //}
        #endregion

        #region private methods
        internal static void Init()
        {
            DB firstConnection = null;

            Exception exception = null;

            CONNECTIONS = new SortedList<string, DB>();

            try
            {
                SETTINGS = ConfigSection.GetSection().Settings;

                foreach (ConnectionStringSettings cs in ConfigurationManager.ConnectionStrings)
                {
                    DB connection = new DB(cs.ConnectionString);

                    if (firstConnection == null)
                        firstConnection = connection;

                    CONNECTIONS.Add(cs.Name, new DB(cs.ConnectionString));
                }

                if (string.IsNullOrEmpty(SETTINGS.DefaultConnectionString))
                    DEFAULT_CONNECTION = firstConnection;
                else
                    DEFAULT_CONNECTION = CONNECTIONS[SETTINGS.DefaultConnectionString];

                return;
            }
            catch (GenericException ex)
            {
                exception = ex;
            }
            catch (Exception ex)
            {
                exception = new GenericException(ex);
            }

            throw exception;
        }

        internal static void Init(IConfiguration configuration)
        {
            DB firstConnection = null;

            CONNECTIONS = new SortedList<string, DB>();

            IConfigurationSection connectionStringSection = configuration.GetSection("ConnectionStrings");
            IConfigurationSection konakDacSettingsSection = configuration.GetSection(ConfigSection.ConfigSectionName);

            IEnumerable<IConfigurationSection> listOfConnectionStrings = connectionStringSection.GetChildren();

            foreach (IConfigurationSection cs in listOfConnectionStrings)
            {
                DB connection = new DB(cs.Value);

                if (firstConnection == null)
                    firstConnection = connection;

                CONNECTIONS.Add(cs.Key, connection);
            }

            if (konakDacSettingsSection.Value == null)
                DEFAULT_CONNECTION = firstConnection;
            else
                DEFAULT_CONNECTION = CONNECTIONS[konakDacSettingsSection[DACSettings.DefaultConnectionStringAttributeName]];
        }
        #endregion

        #region public methods

        #region ExecuteSQLBatch

        /// <summary>
        /// Execute SQL batch job
        /// </summary>
        /// <typeparam name="T">A generic type of object the batch must return</typeparam>
        /// <param name="batch">SQL batch job object</param>
        /// <returns>Batch execution result object</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static T ExecuteSQLBatch<T>(SQLBatch<T> batch)
        {
            return DEFAULT_CONNECTION.ExecuteSQLBatch<T>(batch, true, true, true);
        }

        /// <summary>
        /// Execute SQL batch job asynchronously
        /// </summary>
        /// <typeparam name="T">A generic type of object the batch must return</typeparam>
        /// <param name="batch">SQL batch job object</param>
        /// <returns>Batch execution result object</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<T> ExecuteSQLBatchAsync<T>(Func<IDbConnection, Task<T>> batch)
        {
            return DEFAULT_CONNECTION.ExecuteSQLBatchAsync<T>(batch, true, true, true);
        }

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
        public static T ExecuteSQLBatch<T>(SQLBatch<T> batch, bool throwDBException, bool throwGenericException, bool throwSystemException)
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
        public static Task<T> ExecuteSQLBatchAsync<T>(Func<IDbConnection, Task<T>> batch, bool throwDBException, bool throwGenericException, bool throwSystemException)
        {
            return DEFAULT_CONNECTION.ExecuteSQLBatchAsync<T>(batch, throwDBException, throwGenericException, throwSystemException);
        }

        /// <summary>
        /// Execute SQL batch job
        /// </summary>
        /// <typeparam name="T">A generic type of object the batch must return</typeparam>
        /// <param name="batch">A transactional SQL batch job object</param>
        /// <returns>Batch execution result object</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static T ExecuteTransactionalSQLBatch<T>(SQLTransactBatch<T> batch)
        {
            return DEFAULT_CONNECTION.ExecuteTransactionalSQLBatch<T>(batch, true, true, true);
        }

        /// <summary>
        /// Execute SQL batch job asyncronously
        /// </summary>
        /// <typeparam name="T">A generic type of object the batch must return</typeparam>
        /// <param name="batch">A transactional SQL batch job object</param>
        /// <returns>Batch execution result object</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<T> ExecuteTransactionalSQLBatchAsync<T>(Func<IDbTransaction, Task<T>> batch)
        {
            return DEFAULT_CONNECTION.ExecuteTransactionalSQLBatchAsync<T>(batch, true, true, true);
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
        public static T ExecuteTransactionalSQLBatch<T>(SQLTransactBatch<T> batch, bool throwDBException, bool throwGenericException, bool throwSystemException)
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
        public static Task<T> ExecuteTransactionalSQLBatchAsync<T>(Func<IDbTransaction, Task<T>> batch, bool throwDBException, bool throwGenericException, bool throwSystemException)
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
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static void FillData<T>(T dataOut, string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters, bool throwDBException, bool throwGenericException, bool throwSystemException)
        {
            DEFAULT_CONNECTION.FillData<T>(dataOut, sql, commandType, parameters, throwDBException, throwGenericException, throwSystemException, 0, 0);
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
        public static void FillData<T>(T dataOut, string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters, bool throwDBException, bool throwGenericException, bool throwSystemException, int startRecord, int maxRecords)
        {
            DEFAULT_CONNECTION.FillData<T>(dataOut, sql, commandType, parameters, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
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
            return DEFAULT_CONNECTION.ExecuteReader(sql, CommandType.Text, null);
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
            return DEFAULT_CONNECTION.ExecuteReaderAsync(sql, CommandType.Text, null);
        }

        /// <summary>
        /// Execute SQL command and return SqlDataReader object to read data
        /// </summary>
        /// <param name="sql">SQL command text</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Data reader object to read data</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static SqlDataReader ExecuteReader(string sql, List<KeyValuePair<String, Object>> parameters)
        {
            return DEFAULT_CONNECTION.ExecuteReader(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// Execute SQL command asynchronously and return SqlDataReader object to read data
        /// </summary>
        /// <param name="sql">SQL command text</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Data reader object to read data</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<SqlDataReader> ExecuteReaderAsync(string sql, List<KeyValuePair<String, Object>> parameters)
        {
            return DEFAULT_CONNECTION.ExecuteReaderAsync(sql, CommandType.Text, parameters);
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
        public static SqlDataReader ExecuteReader(string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters)
        {
            return DEFAULT_CONNECTION.ExecuteReader(sql, commandType, parameters);
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
        public static Task<SqlDataReader> ExecuteReaderAsync(string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters)
        {
            return DEFAULT_CONNECTION.ExecuteReaderAsync(sql, commandType, parameters);
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
            return DEFAULT_CONNECTION.GetDataSet(sql, CommandType.Text, null);
        }

        /// <summary>
        /// Get new dataset for specified SQL command
        /// </summary>
        /// <param name="sql">SQL command text to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>New DataSet of results of SQL command</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static DataSet GetDataSet(string sql, List<KeyValuePair<String, Object>> parameters)
        {
            return DEFAULT_CONNECTION.GetDataSet(sql, CommandType.Text, parameters);
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
        public static DataSet GetDataSet(string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters)
        {
            return DEFAULT_CONNECTION.GetDataSet(sql, commandType, parameters);
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
            DEFAULT_CONNECTION.FillDataSet(ds, sql, CommandType.Text, null);
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
            DEFAULT_CONNECTION.FillDataSet(ds, sql, CommandType.Text, null, startRecord, maxRecords);
        }

        /// <summary>
        /// Fill provided DataSet item with values from executed SQL command
        /// </summary>
        /// <param name="ds">A DatSet item that must be filled with data</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static void FillDataSet(DataSet ds, string sql, List<KeyValuePair<String, Object>> parameters)
        {
            DEFAULT_CONNECTION.FillDataSet(ds, sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// Fill provided DataSet item with values from executed SQL command
        /// </summary>
        /// <param name="ds">A DatSet item that must be filled with data</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static void FillDataSet(DataSet ds, string sql, List<KeyValuePair<String, Object>> parameters, int startRecord, int maxRecords)
        {
            DEFAULT_CONNECTION.FillDataSet(ds, sql, CommandType.Text, parameters, startRecord, maxRecords);
        }

        /// <summary>
        /// Fill provided DataSet item with values from executed SQL command
        /// </summary>
        /// <param name="ds">A DatSet item that must be filled with data</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static void FillDataSet(DataSet ds, string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters)
        {
            DEFAULT_CONNECTION.FillData<DataSet>(ds, sql, commandType, parameters, true, true, true, 0, 0);
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
        public static void FillDataSet(DataSet ds, string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters, int startRecord, int maxRecords)
        {
            DEFAULT_CONNECTION.FillData<DataSet>(ds, sql, commandType, parameters, true, true, true, startRecord, maxRecords);
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
            return DEFAULT_CONNECTION.GetDataTable(sql);
        }

        /// <summary>
        /// Get new DataTable for specified sql command
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <returns>New DataTable item with results of the SQL command</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static DataTable GetDataTable(string sql, int startRecord, int maxRecords)
        {
            return DEFAULT_CONNECTION.GetDataTable(sql, null, startRecord, maxRecords);
        }

        /// <summary>
        /// Get new DataTable for specified sql command
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>New DataTable item with results of the SQL command</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static DataTable GetDataTable(string sql, List<KeyValuePair<String, Object>> parameters)
        {
            return DEFAULT_CONNECTION.GetDataTable(sql, parameters);
        }

        /// <summary>
        /// Get new DataTable for specified sql command
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <returns>New DataTable item with results of the SQL command</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static DataTable GetDataTable(string sql, List<KeyValuePair<String, Object>> parameters, int startRecord, int maxRecords)
        {
            return DEFAULT_CONNECTION.GetDataTable(sql, parameters, startRecord, maxRecords);
        }

        /// <summary>
        /// Get new DataTable for specified sql command
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>New DataTable item with results of the SQL command</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static DataTable GetDataTable(string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters)
        {
            return DEFAULT_CONNECTION.GetDataTable(sql, commandType, parameters, true, true, true, 0, 0);
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
        public static DataTable GetDataTable(string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters, int startRecord, int maxRecords)
        {
            return DEFAULT_CONNECTION.GetDataTable(sql, commandType, parameters, true, true, true, startRecord, maxRecords);
        }

        #endregion

        #region FillDataTable

        /// <summary>
        /// Fill provided DataTable item with SQL command values
        /// </summary>
        /// <param name="dt">A DataTable item</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static void FillDataTable(DataTable dt, string sql)
        {
            DEFAULT_CONNECTION.FillDataTable(dt, sql, CommandType.Text, null);
        }

        /// <summary>
        /// Fill provided DataTable item with SQL command values
        /// </summary>
        /// <param name="dt">A DataTable item</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        public static void FillDataTable(DataTable dt, string sql, int startRecord, int maxRecords)
        {
            DEFAULT_CONNECTION.FillDataTable(dt, sql, CommandType.Text, null, startRecord, maxRecords);
        }

        /// <summary>
        /// Fill provided DataTable item with SQL command values
        /// </summary>
        /// <param name="dt">A DataTable item</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        public static void FillDataTable(DataTable dt, string sql, List<KeyValuePair<String, Object>> parameters)
        {
            DEFAULT_CONNECTION.FillDataTable(dt, sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// Fill provided DataTable item with SQL command values
        /// </summary>
        /// <param name="dt">A DataTable item</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static void FillDataTable(DataTable dt, string sql, List<KeyValuePair<String, Object>> parameters, int startRecord, int maxRecords)
        {
            DEFAULT_CONNECTION.FillDataTable(dt, sql, CommandType.Text, parameters, startRecord, maxRecords);
        }

        /// <summary>
        /// Fill provided DataTable item with SQL command values
        /// </summary>
        /// <param name="dt">A DataTable item</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static void FillDataTable(DataTable dt, string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters)
        {
            DEFAULT_CONNECTION.FillData<DataTable>(dt, sql, commandType, parameters, true, true, true, 0, 0);
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
        public static void FillDataTable(DataTable dt, string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters, int startRecord, int maxRecords)
        {
            DEFAULT_CONNECTION.FillData<DataTable>(dt, sql, commandType, parameters, true, true, true, startRecord, maxRecords);
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
            return DEFAULT_CONNECTION.ExecuteNonQuery(sql);
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
            return DEFAULT_CONNECTION.ExecuteNonQueryAsync(sql);
        }

        /// <summary>
        /// Execute SQL query and return the number of affected values
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Number of affected rows</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static int ExecuteNonQuery(string sql, List<KeyValuePair<String, Object>> parameters)
        {
            return DEFAULT_CONNECTION.ExecuteNonQuery(sql, parameters);
        }

        /// <summary>
        /// Execute SQL query asynchronously and return the number of affected values
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Number of affected rows</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<int> ExecuteNonQueryAsync(string sql, List<KeyValuePair<String, Object>> parameters)
        {
            return DEFAULT_CONNECTION.ExecuteNonQueryAsync(sql, parameters);
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
        public static int ExecuteNonQuery(string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters)
        {
            return DEFAULT_CONNECTION.ExecuteNonQuery(sql, commandType, parameters);
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
        public static Task<int> ExecuteNonQueryAsync(string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters)
        {
            return DEFAULT_CONNECTION.ExecuteNonQueryAsync(sql, commandType, parameters);
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
            return DEFAULT_CONNECTION.ExecuteScalar(sql);
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
            return DEFAULT_CONNECTION.ExecuteScalarAsync(sql);
        }

        /// <summary>
        /// Execute SQL command and return value of first column of the first row from results
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Value of first column of the first row</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static object ExecuteScalar(string sql, List<KeyValuePair<String, Object>> parameters)
        {
            return DEFAULT_CONNECTION.ExecuteScalar(sql, parameters);
        }

        /// <summary>
        /// Execute SQL command asynchronously and return value of first column of the first row from results
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Value of first column of the first row</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public static Task<object> ExecuteScalarAsync(string sql, List<KeyValuePair<String, Object>> parameters)
        {
            return DEFAULT_CONNECTION.ExecuteScalarAsync(sql, parameters);
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
        public static object ExecuteScalar(string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters)
        {
            return DEFAULT_CONNECTION.ExecuteScalar(sql, commandType, parameters);
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
        public static Task<object> ExecuteScalarAsync(string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters)
        {
            return DEFAULT_CONNECTION.ExecuteScalarAsync(sql, commandType, parameters);
        }

        #endregion

        #endregion

    }
}
