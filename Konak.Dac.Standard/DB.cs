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
    public class DB
    {
        private readonly string _connectionString;

        /// <summary>
        /// Get connection string of <see cref="DB"/> connection
        /// </summary>
        public string ConnectionString { get { return _connectionString; } }

        #region constructor
        public DB(string connectionString)
        {
            this._connectionString = connectionString;
        }
        #endregion

        #region ExecuteSQLBatch

        /// <summary>
        /// Execute transactional SQL batch job
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
        public T ExecuteTransactionalSQLBatch<T>(SQLTransactBatch<T> batch, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            T res = default(T);
            SqlConnection connection = null;
            SqlTransaction transaction = null;

            try
            {
                connection = new SqlConnection(this._connectionString);
                connection.Open();
                transaction = connection.BeginTransaction();

                res = batch(transaction);

                transaction.Commit();
            }
            catch (SqlException ex)
            {
                if (throwDBException)
                    throw new SqlExecutionException(ex);
            }
            catch (SqlExecutionReturnedErrorCodeException)
            {
                throw;
            }
            catch (GenericException)
            {
                if (throwGenericException)
                    throw;
            }
            catch (System.Exception ex)
            {
                if (throwSystemException)
                    throw new GenericException(Resources.Exceptions.Messages.SYSTEM_EXCEPTION_ON_EXECUTE_SQL_BATCH_LEVEL, ex);
            }
            finally
            {
                if (connection != null) connection.Close();
            }

            return res;
        }

        /// <summary>
        /// Async version of <see cref="ExecuteTransactionalSQLBatch"/> to execute transactional SQL batch job
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
        public async Task<T> ExecuteTransactionalSQLBatchAsync<T>(Func<IDbTransaction, Task<T>> batch, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            T res = default(T);
            SqlConnection connection = null;
            SqlTransaction transaction = null;

            try
            {
                connection = new SqlConnection(this._connectionString);

                await connection.OpenAsync();
                transaction = connection.BeginTransaction();
                res = await batch(transaction);
                transaction.Commit();
            }
            catch (SqlException ex)
            {
                if (throwDBException)
                    throw new SqlExecutionException(ex);
            }
            catch (SqlExecutionReturnedErrorCodeException)
            {
                throw;
            }
            catch (GenericException)
            {
                if (throwGenericException)
                    throw;
            }
            catch (System.Exception ex)
            {
                if (throwSystemException)
                    throw new GenericException(Resources.Exceptions.Messages.SYSTEM_EXCEPTION_ON_EXECUTE_SQL_BATCH_LEVEL, ex);
            }
            finally
            {
                if (connection != null) connection.Close();
            }

            return res;
        }

        /// <summary>
        /// Execute SQL batch job
        /// </summary>
        /// <typeparam name="T">A generic type of object the batch must return</typeparam>
        /// <param name="batch">SQL batch job object</param>
        /// <param name="throwDBException">Throw SQL execution exceptions or suspend them</param>
        /// <param name="throwGenericException">Throw Generic exceptions or suspend them</param>
        /// <param name="throwSystemException">Throw System exceptions or suspend them</param>
        /// <param name="closeConnection">Close connection after batch job execution</param>
        /// <returns>Batch execution result object</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public T ExecuteSQLBatch<T>(SQLBatch<T> batch, bool closeConnection = true, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            T res = default(T);
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(this._connectionString);
                connection.Open();
                res = batch(connection);
            }
            catch (SqlException ex)
            {
                if (throwDBException)
                    throw new SqlExecutionException(ex);
            }
            catch (SqlExecutionReturnedErrorCodeException)
            {
                throw;
            }
            catch (GenericException)
            {
                if (throwGenericException)
                    throw;
            }
            catch (System.Exception ex)
            {
                if (throwSystemException)
                    throw new GenericException(Resources.Exceptions.Messages.SYSTEM_EXCEPTION_ON_EXECUTE_SQL_BATCH_LEVEL, ex);
            }
            finally
            {
                if (closeConnection && connection != null) connection.Close();
            }

            return res;
        }

        /// <summary>
        /// Async version of <see cref="ExecuteSQLBatch"/> to execute SQL batch job
        /// </summary>
        /// <typeparam name="T">A generic type of object the batch must return</typeparam>
        /// <param name="batch">SQL batch job object</param>
        /// <param name="throwDBException">Throw SQL execution exceptions or suspend them</param>
        /// <param name="throwGenericException">Throw Generic exceptions or suspend them</param>
        /// <param name="throwSystemException">Throw System exceptions or suspend them</param>
        /// <param name="closeConnection">Close connection after batch job execution</param>
        /// <returns>Batch execution result object</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public async Task<T> ExecuteSQLBatchAsync<T>(Func<IDbConnection, Task<T>> batch, bool closeConnection = true, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            T res = default(T);
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(this._connectionString);
                await connection.OpenAsync();
                res = await batch(connection);
            }
            catch (SqlException ex)
            {
                if (throwDBException)
                    throw new SqlExecutionException(ex);
            }
            catch (SqlExecutionReturnedErrorCodeException)
            {
                throw;
            }
            catch (GenericException)
            {
                if (throwGenericException)
                    throw;
            }
            catch (System.Exception ex)
            {
                if (throwSystemException)
                    throw new GenericException(Resources.Exceptions.Messages.SYSTEM_EXCEPTION_ON_EXECUTE_SQL_BATCH_LEVEL, ex);
            }
            finally
            {
                if (closeConnection && connection != null) connection.Close();
            }

            return res;
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
        public void FillData<T>(T dataOut, string sql, List<KeyValuePair<String, Object>> parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            FillData(dataOut, sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
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
        public void FillData<T>(T dataOut, string sql, dynamic parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            FillData(dataOut, sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Execute SQL command or stored procedure and fill that data into the dataOut object
        /// </summary>
        /// <typeparam name="T">A generic type of the object that will be filled with values. Type of objects that can be passed: <list type="bullet"><item><description>DataTable</description></item><item><description>DataSet</description></item></list></typeparam>
        /// <typeparam name="TParam">Generic type of the object the properties will be transformed to sql command parameters</typeparam>
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
        public void FillData<T, TParam>(T dataOut, string sql, TParam parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            FillData(dataOut, sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
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
        public void FillData<T>(T dataOut, string sql, KeyValuePair<String, Object>[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            FillData(dataOut, sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
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
        public void FillData<T>(T dataOut, string sql, SqlParameter[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            SqlCommand cmd = null;
            SqlDataAdapter da = null;

            if (typeof(T) != typeof(DataTable) && typeof(T) != typeof(DataSet))
                throw new GenericException(Konak.Dac.Resources.Messages.FILL_DATA_INVALID_TYPE_PASSED + typeof(T).ToString());

            try
            {
                cmd = new SqlCommand(sql, new SqlConnection(this._connectionString));
                cmd.CommandType = commandType;

                SqlParameter rv = new SqlParameter("@return_value", SqlDbType.Int);
                rv.Direction = ParameterDirection.ReturnValue;

                cmd.Parameters.Add(rv);

                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                da = new SqlDataAdapter(cmd);

                if (typeof(T) == typeof(DataTable))
                {
                    if (maxRecords == 0)
                        da.Fill(dataOut as DataTable);
                    else
                        da.Fill(startRecord, maxRecords, new DataTable[] { dataOut as DataTable });
                }

                if (typeof(T) == typeof(DataSet))
                {
                    if (maxRecords == 0)
                        da.Fill(dataOut as DataSet);
                    else
                        da.Fill(dataOut as DataSet, startRecord, maxRecords, string.Empty);
                }

                int retVal = (int)rv.Value;

                if (retVal != 0) throw new SqlExecutionReturnedErrorCodeException(retVal, dataOut);

            }
            catch (SqlException ex)
            {
                if (throwDBException)
                    throw new SqlExecutionException(ex);
            }
            catch (SqlExecutionReturnedErrorCodeException)
            {
                throw;
            }
            catch (GenericException)
            {
                if (throwGenericException)
                    throw;
            }
            catch (Exception ex)
            {
                if (throwSystemException)
                    throw new GenericException("System Exception on ExecuteSQLBatch level", ex);
            }
            finally
            {
                if (cmd != null) cmd.Connection.Close();
            }
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
        public SqlDataReader ExecuteReader(string sql)
        {
            return ExecuteReader(sql, new SqlParameter[0]);
        }

        /// <summary>
        /// Execute SQL command asyncronously and return SqlDataReader object to read data
        /// </summary>
        /// <param name="sql">SQL command text</param>
        /// <returns>Data reader object to read data</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public Task<SqlDataReader> ExecuteReaderAsync(string sql)
        {
            return ExecuteReaderAsync(sql, new SqlParameter[0]);
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
        public SqlDataReader ExecuteReader(string sql, SqlParameter[] parameters)
        {
            return ExecuteReader(sql, parameters, CommandType.Text, true, true, true);
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
        public SqlDataReader ExecuteReader(string sql, KeyValuePair<String, Object>[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            return ExecuteReader(sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException);
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
        public SqlDataReader ExecuteReader(string sql, List<KeyValuePair<String, Object>> parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            return ExecuteReader(sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException);
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
        public SqlDataReader ExecuteReader(string sql, dynamic parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            return ExecuteReader(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException);
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
        public SqlDataReader ExecuteReader<T>(string sql, T parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            return ExecuteReader(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException);
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
        public SqlDataReader ExecuteReader(string sql, SqlParameter[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            SQLBatch<SqlDataReader> b = delegate (IDbConnection connection)
            {
                SqlConnection conn = connection as SqlConnection;
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.CommandType = commandType;

                SqlParameter rv = new SqlParameter("@return_value", SqlDbType.Int);
                rv.Direction = ParameterDirection.ReturnValue;
                rv.IsNullable = false;

                cmd.Parameters.Add(rv);

                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                SqlDataReader res = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                int retVal = 0;

                if (rv.Value != null)
                    retVal = (int)rv.Value;

                if (retVal != 0) throw new SqlExecutionReturnedErrorCodeException(retVal, res);

                return res;
            };

            return ExecuteSQLBatch<SqlDataReader>(b, false, throwDBException, throwGenericException, throwSystemException);
        }

        /// <summary>
        /// Private Method to execute SQL command or stored procedure and return SqlDataReader object to read data 
        /// </summary>
        /// <param name="sql">SQL command, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Data reader object to read data</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        private async Task<SqlDataReader> ExecuteReaderAsyncInternal(IDbConnection connection, string sqlQuery, SqlParameter[] parameters, CommandType commandType = CommandType.Text)
        {
            SqlConnection conn = connection as SqlConnection;
            SqlCommand cmd = new SqlCommand(sqlQuery, conn);

            cmd.CommandType = commandType;

            SqlParameter rv = new SqlParameter("@return_value", SqlDbType.Int);
            rv.Direction = ParameterDirection.ReturnValue;
            rv.IsNullable = false;

            cmd.Parameters.Add(rv);

            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            SqlDataReader res = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

            int retVal = 0;

            if (rv.Value != null)
                retVal = (int)rv.Value;

            if (retVal != 0) throw new SqlExecutionReturnedErrorCodeException(retVal, res);

            return res;
        }

        /// <summary>
        /// Async version of <see cref="ExecuteReader"/> to execute SQL command or stored procedure and return SqlDataReader object to read data
        /// </summary>
        /// <param name="sql">SQL command, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Data reader object to read data</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public Task<SqlDataReader> ExecuteReaderAsync(string sql, KeyValuePair<String, Object>[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            Func<IDbConnection, Task<SqlDataReader>> executeReaderAsyncFunction = connection => ExecuteReaderAsyncInternal(connection, sql, parameters.ToSqlParameters(), commandType);

            return ExecuteSQLBatchAsync<SqlDataReader>(executeReaderAsyncFunction, false, throwDBException, throwGenericException, throwSystemException);
        }


        /// <summary>
        /// Async version of <see cref="ExecuteReader"/> to execute SQL command or stored procedure and return SqlDataReader object to read data
        /// </summary>
        /// <param name="sql">SQL command, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Data reader object to read data</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public Task<SqlDataReader> ExecuteReaderAsync(string sql, List<KeyValuePair<String, Object>> parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            Func<IDbConnection, Task<SqlDataReader>> executeReaderAsyncFunction = connection => ExecuteReaderAsyncInternal(connection, sql, parameters.ToSqlParameters(), commandType);

            return ExecuteSQLBatchAsync<SqlDataReader>(executeReaderAsyncFunction, false, throwDBException, throwGenericException, throwSystemException);
        }

        /// <summary>
        /// Async version of <see cref="ExecuteReader"/> to execute SQL command or stored procedure and return SqlDataReader object to read data
        /// </summary>
        /// <param name="sql">SQL command, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Data reader object to read data</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public Task<SqlDataReader> ExecuteReaderAsync(string sql, dynamic parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            Func<IDbConnection, Task<SqlDataReader>> executeReaderAsyncFunction = connection => ExecuteReaderAsyncInternal(connection, sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType);

            return ExecuteSQLBatchAsync<SqlDataReader>(executeReaderAsyncFunction, false, throwDBException, throwGenericException, throwSystemException);
        }

        /// <summary>
        /// Async version of <see cref="ExecuteReader"/> to execute SQL command or stored procedure and return SqlDataReader object to read data
        /// </summary>
        /// <typeparam name="T">Generic type of the object the properties will be transformed to sql command parameters</typeparam>
        /// <param name="sql">SQL command, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Data reader object to read data</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public Task<SqlDataReader> ExecuteReaderAsync<T>(string sql, T parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            Func<IDbConnection, Task<SqlDataReader>> executeReaderAsyncFunction = connection => ExecuteReaderAsyncInternal(connection, sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType);

            return ExecuteSQLBatchAsync<SqlDataReader>(executeReaderAsyncFunction, false, throwDBException, throwGenericException, throwSystemException);
        }

        /// <summary>
        /// Async version of <see cref="ExecuteReader"/> to execute SQL command or stored procedure and return SqlDataReader object to read data
        /// </summary>
        /// <param name="sql">SQL command, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Data reader object to read data</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public Task<SqlDataReader> ExecuteReaderAsync(string sql, SqlParameter[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true)
        {
            Func<IDbConnection, Task<SqlDataReader>> executeReaderAsyncFunction = connection => ExecuteReaderAsyncInternal(connection, sql, parameters, commandType);

            return ExecuteSQLBatchAsync<SqlDataReader>(executeReaderAsyncFunction, false, throwDBException, throwGenericException, throwSystemException);
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
        public DataSet GetDataSet(string sql)
        {
            return GetDataSet(sql, new SqlParameter[0]);
        }

        /// <summary>
        /// Get new dataset for specified SQL command or stored procedure
        /// </summary>
        /// <param name="sql">SQL command text, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="throwDBException">Throw SQL execution exceptions or suspend them</param>
        /// <param name="throwGenericException">Throw Generic exceptions or suspend them</param>
        /// <param name="throwSystemException">Throw System exceptions or suspend them</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <returns>New DataSet of results of SQL command</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public DataSet GetDataSet(string sql, SqlParameter[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DataSet ds = new DataSet();

            FillData<DataSet>(ds, sql, parameters, commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);

            return ds;
        }

        /// <summary>
        /// Get new dataset for specified SQL command or stored procedure
        /// </summary>
        /// <param name="sql">SQL command text, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="throwDBException">Throw SQL execution exceptions or suspend them</param>
        /// <param name="throwGenericException">Throw Generic exceptions or suspend them</param>
        /// <param name="throwSystemException">Throw System exceptions or suspend them</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <returns>New DataSet of results of SQL command</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public DataSet GetDataSet(string sql, KeyValuePair<String, Object>[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DataSet ds = new DataSet();

            FillData<DataSet>(ds, sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);

            return ds;
        }

        /// <summary>
        /// Get new dataset for specified SQL command or stored procedure
        /// </summary>
        /// <param name="sql">SQL command text, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="throwDBException">Throw SQL execution exceptions or suspend them</param>
        /// <param name="throwGenericException">Throw Generic exceptions or suspend them</param>
        /// <param name="throwSystemException">Throw System exceptions or suspend them</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <returns>New DataSet of results of SQL command</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public DataSet GetDataSet(string sql, List<KeyValuePair<String, Object>> parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DataSet ds = new DataSet();

            FillData<DataSet>(ds, sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);

            return ds;
        }

        /// <summary>
        /// Get new dataset for specified SQL command or stored procedure
        /// </summary>
        /// <param name="sql">SQL command text, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="throwDBException">Throw SQL execution exceptions or suspend them</param>
        /// <param name="throwGenericException">Throw Generic exceptions or suspend them</param>
        /// <param name="throwSystemException">Throw System exceptions or suspend them</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <returns>New DataSet of results of SQL command</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public DataSet GetDataSet(string sql, dynamic parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DataSet ds = new DataSet();

            FillData<DataSet>(ds, sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);

            return ds;
        }

        /// <summary>
        /// Get new dataset for specified SQL command or stored procedure
        /// </summary>
        /// <typeparam name="T">Generic type of the object the properties will be transformed to sql command parameters</typeparam>
        /// <param name="sql">SQL command text, stored procedure or table name</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="throwDBException">Throw SQL execution exceptions or suspend them</param>
        /// <param name="throwGenericException">Throw Generic exceptions or suspend them</param>
        /// <param name="throwSystemException">Throw System exceptions or suspend them</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <returns>New DataSet of results of SQL command</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public DataSet GetDataSet<T>(string sql, T parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DataSet ds = new DataSet();

            FillData<DataSet>(ds, sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);

            return ds;
        }

        #endregion

        #region FillDataSet

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
        public void FillDataSet(DataSet ds, string sql, int startRecord = 0, int maxRecords = 0)
        {
            FillDataSet(ds, sql, new SqlParameter[0], startRecord: startRecord, maxRecords: maxRecords);
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
        public void FillDataSet(DataSet ds, string sql, SqlParameter[] parameters)
        {
            FillDataSet(ds, sql, parameters);
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
        public void FillDataSet(DataSet ds, string sql, SqlParameter[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            FillData<DataSet>(ds, sql, parameters, commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
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
        public void FillDataSet(DataSet ds, string sql, KeyValuePair<String, Object>[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            FillData<DataSet>(ds, sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
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
        public void FillDataSet(DataSet ds, string sql, List<KeyValuePair<String, Object>> parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            FillData<DataSet>(ds, sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
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
        public void FillDataSet(DataSet ds, string sql, dynamic parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            FillData<DataSet>(ds, sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
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
        public void FillDataSet<T>(DataSet ds, string sql, T parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            FillData<DataSet>(ds, sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        #endregion

        #region GetDataTable

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
        public DataTable GetDataTable(string sql, int startRecord = 0, int maxRecords = 0)
        {
            return GetDataTable(sql, new SqlParameter[0], startRecord: startRecord, maxRecords: maxRecords);
        }

        /// <summary>
        /// Get new DataTable for specified sql command
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="throwDBException">Throw SQL execution exceptions or suspend them</param>
        /// <param name="throwGenericException">Throw Generic exceptions or suspend them</param>
        /// <param name="throwSystemException">Throw System exceptions or suspend them</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <returns>New DataTable item with results of the SQL command</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public DataTable GetDataTable(string sql, SqlParameter[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DataTable dt = new DataTable("Table0");

            FillData<DataTable>(dt, sql, parameters, commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);

            return dt;
        }

        /// <summary>
        /// Get new DataTable for specified sql command
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="throwDBException">Throw SQL execution exceptions or suspend them</param>
        /// <param name="throwGenericException">Throw Generic exceptions or suspend them</param>
        /// <param name="throwSystemException">Throw System exceptions or suspend them</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <returns>New DataTable item with results of the SQL command</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public DataTable GetDataTable(string sql, KeyValuePair<String, Object>[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DataTable dt = new DataTable("Table0");

            FillData<DataTable>(dt, sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);

            return dt;
        }

        /// <summary>
        /// Get new DataTable for specified sql command
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="throwDBException">Throw SQL execution exceptions or suspend them</param>
        /// <param name="throwGenericException">Throw Generic exceptions or suspend them</param>
        /// <param name="throwSystemException">Throw System exceptions or suspend them</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <returns>New DataTable item with results of the SQL command</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public DataTable GetDataTable(string sql, List<KeyValuePair<String, Object>> parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DataTable dt = new DataTable("Table0");

            FillData<DataTable>(dt, sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);

            return dt;
        }

        /// <summary>
        /// Get new DataTable for specified sql command
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="throwDBException">Throw SQL execution exceptions or suspend them</param>
        /// <param name="throwGenericException">Throw Generic exceptions or suspend them</param>
        /// <param name="throwSystemException">Throw System exceptions or suspend them</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <returns>New DataTable item with results of the SQL command</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public DataTable GetDataTable(string sql, dynamic parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DataTable dt = new DataTable("Table0");

            FillData<DataTable>(dt, sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);

            return dt;
        }

        /// <summary>
        /// Get new DataTable for specified sql command
        /// </summary>
        /// <typeparam name="T">Generic type of the object the properties will be transformed to sql command parameters</typeparam>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="throwDBException">Throw SQL execution exceptions or suspend them</param>
        /// <param name="throwGenericException">Throw Generic exceptions or suspend them</param>
        /// <param name="throwSystemException">Throw System exceptions or suspend them</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <returns>New DataTable item with results of the SQL command</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public DataTable GetDataTable<T>(string sql, T parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            DataTable dt = new DataTable("Table0");

            FillData<DataTable>(dt, sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);

            return dt;
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
        public void FillDataTable(DataTable dt, string sql, int startRecord = 0, int maxRecords = 0)
        {
            FillDataTable(dt, sql, new SqlParameter[0], startRecord: startRecord, maxRecords: maxRecords);
        }


        /// <summary>
        /// Fill provided DataTable item with SQL command values
        /// </summary>
        /// <param name="dt"> DataTable item</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public void FillDataTable(DataTable dt, string sql, SqlParameter[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            FillData<DataTable>(dt, sql, parameters, commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Fill provided DataTable item with SQL command values
        /// </summary>
        /// <param name="dt"> DataTable item</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public void FillDataTable(DataTable dt, string sql, KeyValuePair<String, Object>[] parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            FillData<DataTable>(dt, sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Fill provided DataTable item with SQL command values
        /// </summary>
        /// <param name="dt"> DataTable item</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public void FillDataTable(DataTable dt, string sql, List<KeyValuePair<String, Object>> parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            FillData<DataTable>(dt, sql, parameters.ToSqlParameters(), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Fill provided DataTable item with SQL command values
        /// </summary>
        /// <param name="dt"> DataTable item</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public void FillDataTable(DataTable dt, string sql, dynamic parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            FillData<DataTable>(dt, sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
        }

        /// <summary>
        /// Fill provided DataTable item with SQL command values
        /// </summary>
        /// <typeparam name="T">Generic type of the object the properties will be transformed to sql command parameters</typeparam>
        /// <param name="dt"> DataTable item</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public void FillDataTable<T>(DataTable dt, string sql, T parameters, CommandType commandType = CommandType.Text, bool throwDBException = true, bool throwGenericException = true, bool throwSystemException = true, int startRecord = 0, int maxRecords = 0)
        {
            FillData<DataTable>(dt, sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);
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
        public int ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(sql, new SqlParameter[0], CommandType.Text);
        }

        /// <summary>
        /// Execute SQL query asynchronously and return the number of affected values
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <returns>Number of affected rows</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public Task<int> ExecuteNonQueryAsync(string sql)
        {
            return ExecuteNonQueryAsync(sql, new SqlParameter[0], CommandType.Text);
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
        public int ExecuteNonQuery(string sql, KeyValuePair<String, Object>[] parameters, CommandType commandType = CommandType.Text)
        {
            return ExecuteNonQuery(sql, parameters.ToSqlParameters(), commandType);
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
        public int ExecuteNonQuery(string sql, List<KeyValuePair<String, Object>> parameters, CommandType commandType = CommandType.Text)
        {
            return ExecuteNonQuery(sql, parameters.ToSqlParameters(), commandType);
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
        public int ExecuteNonQuery(string sql, dynamic parameters, CommandType commandType = CommandType.Text)
        {
            return ExecuteNonQuery(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType);
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
        public int ExecuteNonQuery<T>(string sql, T parameters, CommandType commandType = CommandType.Text)
        {
            return ExecuteNonQuery(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType);
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
        public int ExecuteNonQuery(string sql, SqlParameter[] parameters, CommandType commandType = CommandType.Text)
        {
            SQLBatch<int> b = delegate (IDbConnection connection)
            {
                SqlConnection conn = connection as SqlConnection;
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.CommandType = commandType;

                SqlParameter rv = new SqlParameter("@return_value", SqlDbType.Int);
                rv.Direction = ParameterDirection.ReturnValue;

                cmd.Parameters.Add(rv);

                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                int res = cmd.ExecuteNonQuery();

                int retVal = (int)rv.Value;

                if (retVal != 0) throw new SqlExecutionReturnedErrorCodeException(retVal, res);

                return res;
            };

            return ExecuteSQLBatch<int>(b);
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
        public Task<int> ExecuteNonQueryAsync(string sql, KeyValuePair<String, Object>[] parameters, CommandType commandType = CommandType.Text)
        {
            return ExecuteNonQueryAsync(sql, parameters.ToSqlParameters(), commandType);
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
        public Task<int> ExecuteNonQueryAsync(string sql, List<KeyValuePair<String, Object>> parameters, CommandType commandType = CommandType.Text)
        {
            return ExecuteNonQueryAsync(sql, parameters.ToSqlParameters(), commandType);
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
        public Task<int> ExecuteNonQueryAsync(string sql, dynamic parameters, CommandType commandType = CommandType.Text)
        {
            return ExecuteNonQueryAsync(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType);
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
        public Task<int> ExecuteNonQueryAsync<T>(string sql, T parameters, CommandType commandType = CommandType.Text)
        {
            return ExecuteNonQueryAsync(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType);
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
        public Task<int> ExecuteNonQueryAsync(string sql, SqlParameter[] parameters, CommandType commandType = CommandType.Text)
        {
            Func<IDbConnection, Task<int>> func = async delegate (IDbConnection connection)
            {
                SqlConnection conn = connection as SqlConnection;
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.CommandType = commandType;

                SqlParameter rv = new SqlParameter("@return_value", SqlDbType.Int);
                rv.Direction = ParameterDirection.ReturnValue;

                cmd.Parameters.Add(rv);

                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                int res = await cmd.ExecuteNonQueryAsync();

                int retVal = (int)rv.Value;

                if (retVal != 0) throw new SqlExecutionReturnedErrorCodeException(retVal, res);

                return res;
            };

            return ExecuteSQLBatchAsync<int>(func);
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
        public object ExecuteScalar(string sql)
        {
            return ExecuteScalar(sql, new SqlParameter[0]);
        }

        /// <summary>
        /// Execute SQL command asynchronously and return value of first column of the first row from results
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <returns>Value of first column of the first row</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public Task<object> ExecuteScalarAsync(string sql)
        {
            return ExecuteScalarAsync(sql, new SqlParameter[0]);
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
        public object ExecuteScalar(string sql, KeyValuePair<String, Object>[] parameters, CommandType commandType = CommandType.Text)
        {
            return ExecuteScalar(sql, parameters.ToSqlParameters(), commandType);
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
        public object ExecuteScalar(string sql, List<KeyValuePair<String, Object>> parameters, CommandType commandType = CommandType.Text)
        {
            return ExecuteScalar(sql, parameters.ToSqlParameters(), commandType);
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
        public object ExecuteScalar(string sql, dynamic parameters, CommandType commandType = CommandType.Text)
        {
            return ExecuteScalar(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType);
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
        public object ExecuteScalar<T>(string sql, T parameters, CommandType commandType = CommandType.Text)
        {
            return ExecuteScalar(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType);
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
        public object ExecuteScalar(string sql, SqlParameter[] parameters, CommandType commandType = CommandType.Text)
        {
            SQLBatch<object> b = delegate (IDbConnection connection)
            {
                SqlConnection conn = connection as SqlConnection;
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.CommandType = commandType;

                SqlParameter rv = new SqlParameter("@return_value", SqlDbType.Int);
                rv.Direction = ParameterDirection.ReturnValue;

                cmd.Parameters.Add(rv);

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                object res = cmd.ExecuteScalar();

                int retVal = (int)rv.Value;

                if (retVal != 0)
                    throw new SqlExecutionReturnedErrorCodeException(retVal, res);

                return res;
            };

            return ExecuteSQLBatch<object>(b);
        }

        /// <summary>
        /// Async version of <see cref="ExecuteScalar"/> to execute SQL command and return value of first column of the first row from results
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Value of first column of the first row</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public Task<object> ExecuteScalarAsync(string sql, KeyValuePair<String, Object>[] parameters, CommandType commandType = CommandType.Text)
        {
            return ExecuteScalarAsync(sql, parameters.ToSqlParameters(), commandType);
        }

        /// <summary>
        /// Async version of <see cref="ExecuteScalar"/> to execute SQL command and return value of first column of the first row from results
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Value of first column of the first row</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public Task<object> ExecuteScalarAsync(string sql, List<KeyValuePair<String, Object>> parameters, CommandType commandType = CommandType.Text)
        {
            return ExecuteScalarAsync(sql, parameters.ToSqlParameters(), commandType);
        }

        /// <summary>
        /// Async version of <see cref="ExecuteScalar"/> to execute SQL command and return value of first column of the first row from results
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Value of first column of the first row</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public Task<object> ExecuteScalarAsync(string sql, dynamic parameters, CommandType commandType = CommandType.Text)
        {
            return ExecuteScalarAsync(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType);
        }

        /// <summary>
        /// Async version of <see cref="ExecuteScalar"/> to execute SQL command and return value of first column of the first row from results
        /// </summary>
        /// <typeparam name="T">Generic type of the object the properties will be transformed to sql command parameters</typeparam>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Value of first column of the first row</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public Task<object> ExecuteScalarAsync<T>(string sql, T parameters, CommandType commandType = CommandType.Text)
        {
            return ExecuteScalarAsync(sql, DacSqlParametersExtensions.ToSqlParameters(parameters), commandType);
        }

        /// <summary>
        /// Async version of <see cref="ExecuteScalar"/> to execute SQL command and return value of first column of the first row from results
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="commandType">SQL command type to execute</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Value of first column of the first row</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public Task<object> ExecuteScalarAsync(string sql, SqlParameter[] parameters, CommandType commandType = CommandType.Text)
        {
            Func<IDbConnection, Task<object>> b = async delegate (IDbConnection connection)
            {
                SqlConnection conn = connection as SqlConnection;
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.CommandType = commandType;

                SqlParameter rv = new SqlParameter("@return_value", SqlDbType.Int);
                rv.Direction = ParameterDirection.ReturnValue;

                cmd.Parameters.Add(rv);

                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                object res = await cmd.ExecuteScalarAsync();

                int retVal = (int)rv.Value;

                if (retVal != 0)
                    throw new SqlExecutionReturnedErrorCodeException(retVal, res);

                return res;
            };

            return ExecuteSQLBatchAsync<object>(b);
        }

        #endregion

    }
}
