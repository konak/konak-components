using Konak.Common.Exceptions;
using Konak.Dac;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Konak.Dac.Core
{
    public class DB
    {
        private readonly string _connectionString;

        #region constructor
        internal DB(string connectionString)
        {
            this._connectionString = connectionString;
        }
        #endregion

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
        public T ExecuteSQLBatch<T>(SQLBatch<T> batch)
        {
            return ExecuteSQLBatch<T>(batch, true, true, true, true);
        }

        /// <summary>
        /// Async version of <see cref="ExecuteSQLBatch"/> to execute SQL batch job
        /// </summary>
        /// <typeparam name="T">A generic type of object the batch must return</typeparam>
        /// <param name="batch">SQL batch job object</param>
        /// <returns>Batch execution result object</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public async Task<T> ExecuteSQLBatchAsync<T>(Func<IDbConnection, Task<T>> batch)
        {
            return await ExecuteSQLBatchAsync<T>(batch, true, true, true, true);
        }

        /// <summary>
        /// Execute SQL batch job
        /// </summary>
        /// <typeparam name="T">A generic type of object the batch must return</typeparam>
        /// <param name="batch">SQL batch job object</param>
        /// <param name="closeConnection">Close connection after batch job execution</param>
        /// <returns>Batch execution result object</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public T ExecuteSQLBatch<T>(SQLBatch<T> batch, bool closeConnection)
        {
            return ExecuteSQLBatch<T>(batch, true, true, true, closeConnection);
        }

        /// <summary>
        /// Async version of <see cref="ExecuteSQLBatch"/> to execute SQL batch job
        /// </summary>
        /// <typeparam name="T">A generic type of object the batch must return</typeparam>
        /// <param name="batch">SQL batch job object</param>
        /// <param name="closeConnection">Close connection after batch job execution</param>
        /// <returns>Batch execution result object</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public async Task<T> ExecuteSQLBatchAsync<T>(Func<IDbConnection, Task<T>> batch, bool closeConnection)
        {
            return await ExecuteSQLBatchAsync<T>(batch, true, true, true, closeConnection);
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
        public T ExecuteSQLBatch<T>(SQLBatch<T> batch, bool throwDBException, bool throwGenericException, bool throwSystemException)
        {
            return ExecuteSQLBatch<T>(batch, throwDBException, throwGenericException, throwSystemException, true);
        }

        /// <summary>
        /// Async version of <see cref="ExecuteSQLBatch"/> to execute SQL batch job
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
        public async Task<T> ExecuteSQLBatchAsync<T>(Func<IDbConnection, Task<T>> batch, bool throwDBException, bool throwGenericException, bool throwSystemException)
        {
            return await ExecuteSQLBatchAsync<T>(batch, throwDBException, throwGenericException, throwSystemException, true);
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
        public T ExecuteTransactionalSQLBatch<T>(SQLTransactBatch<T> batch)
        {
            return ExecuteTransactionalSQLBatch<T>(batch, true, true, true);
        }

        /// <summary>
        /// Async version of <see cref="ExecuteTransactionalSQLBatch"/> Execute SQL batch job
        /// </summary>
        /// <typeparam name="T">A generic type of object the batch must return</typeparam>
        /// <param name="batch">A transactional SQL batch job object</param>
        /// <returns>Batch execution result object</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public async Task<T> ExecuteTransactionalSQLBatchAsync<T>(Func<IDbTransaction, Task<T>> batch)
        {
            return await ExecuteTransactionalSQLBatchAsync<T>(batch, true, true, true);
        }

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
        public T ExecuteTransactionalSQLBatch<T>(SQLTransactBatch<T> batch, bool throwDBException, bool throwGenericException, bool throwSystemException)
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
        public async Task<T> ExecuteTransactionalSQLBatchAsync<T>(Func<IDbTransaction, Task<T>> batch, bool throwDBException, bool throwGenericException, bool throwSystemException)
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
        public T ExecuteSQLBatch<T>(SQLBatch<T> batch, bool throwDBException, bool throwGenericException, bool throwSystemException, bool closeConnection)
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
        public async Task<T> ExecuteSQLBatchAsync<T>(Func<IDbConnection, Task<T>> batch, bool throwDBException, bool throwGenericException, bool throwSystemException, bool closeConnection)
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
        public void FillData<T>(T dataOut, string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters, bool throwDBException, bool throwGenericException, bool throwSystemException, int startRecord, int maxRecords)
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

                if (parameters != null)
                    foreach (KeyValuePair<String, Object> parameter in parameters)
                        cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);

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
            return ExecuteReader(sql, CommandType.Text, null);
        }

        /// <summary>
        /// Execute SQL command asyncronously and return SqlDataReader object to read data
        /// </summary>
        /// <param name="sql">SQL command text</param>
        /// <returns>Data reader object to read data</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public async Task<SqlDataReader> ExecuteReaderAsync(string sql)
        {
            return await ExecuteReaderAsync(sql, CommandType.Text, null);
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
        public SqlDataReader ExecuteReader(string sql, List<KeyValuePair<String, Object>> parameters)
        {
            return ExecuteReader(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// Execute SQL command asyncronously and return SqlDataReader object to read data
        /// </summary>
        /// <param name="sql">SQL command text</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        /// <returns>Data reader object to read data</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public async Task<SqlDataReader> ExecuteReaderAsync(string sql, List<KeyValuePair<String, Object>> parameters)
        {
            return await ExecuteReaderAsync(sql, CommandType.Text, parameters);
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
        public SqlDataReader ExecuteReader(string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters)
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

                if (parameters != null)
                    foreach (KeyValuePair<String, Object> parameter in parameters)
                        cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);

                SqlDataReader res = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                int retVal = 0;

                if (rv.Value != null)
                    retVal = (int)rv.Value;

                if (retVal != 0) throw new SqlExecutionReturnedErrorCodeException(retVal, res);

                return res;
            };

            return ExecuteSQLBatch<SqlDataReader>(b, false);
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
        private async Task<SqlDataReader> ExecuteReaderAsyncInternal(IDbConnection connection, string sqlQuery, CommandType commandType, List<KeyValuePair<String, Object>> parameters)
        {
            SqlConnection conn = connection as SqlConnection;
            SqlCommand cmd = new SqlCommand(sqlQuery, conn);

            cmd.CommandType = commandType;

            SqlParameter rv = new SqlParameter("@return_value", SqlDbType.Int);
            rv.Direction = ParameterDirection.ReturnValue;
            rv.IsNullable = false;

            cmd.Parameters.Add(rv);

            if (parameters != null)
                foreach (KeyValuePair<String, Object> parameter in parameters)
                    cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);

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
        public async Task<SqlDataReader> ExecuteReaderAsync(string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters)
        {
            Func<IDbConnection, Task<SqlDataReader>> executeReaderAsyncFunction = connection => ExecuteReaderAsyncInternal(connection, sql, commandType, parameters);

            return await ExecuteSQLBatchAsync<SqlDataReader>(executeReaderAsyncFunction, true, true, true, false);
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
            return GetDataSet(sql, CommandType.Text, null);
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
        public DataSet GetDataSet(string sql, List<KeyValuePair<String, Object>> parameters)
        {
            return GetDataSet(sql, CommandType.Text, parameters);
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
        public DataSet GetDataSet(string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters)
        {
            return GetDataSet(sql, commandType, parameters, true, true, true, 0, 0);
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
        public DataSet GetDataSet(string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters, bool throwDBException, bool throwGenericException, bool throwSystemException, int startRecord, int maxRecords)
        {
            DataSet ds = new DataSet();

            FillData<DataSet>(ds, sql, commandType, parameters, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);

            return ds;
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
        public void FillDataSet(DataSet ds, string sql)
        {
            FillDataSet(ds, sql, CommandType.Text, null);
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
        public void FillDataSet(DataSet ds, string sql, int startRecord, int maxRecords)
        {
            FillDataSet(ds, sql, CommandType.Text, null, startRecord, maxRecords);
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
        public void FillDataSet(DataSet ds, string sql, List<KeyValuePair<String, Object>> parameters)
        {
            FillDataSet(ds, sql, CommandType.Text, parameters);
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
        public void FillDataSet(DataSet ds, string sql, List<KeyValuePair<String, Object>> parameters, int startRecord, int maxRecords)
        {
            FillDataSet(ds, sql, CommandType.Text, parameters, startRecord, maxRecords);
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
        public void FillDataSet(DataSet ds, string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters)
        {
            FillData<DataSet>(ds, sql, commandType, parameters, true, true, true, 0, 0);
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
        public void FillDataSet(DataSet ds, string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters, int startRecord, int maxRecords)
        {
            FillData<DataSet>(ds, sql, commandType, parameters, true, true, true, startRecord, maxRecords);
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
        public DataTable GetDataTable(string sql)
        {
            return GetDataTable(sql, null);
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
        public DataTable GetDataTable(string sql, int startRecord, int maxRecords)
        {
            return GetDataTable(sql, null, startRecord, maxRecords);
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
        public DataTable GetDataTable(string sql, List<KeyValuePair<String, Object>> parameters)
        {
            return GetDataTable(sql, parameters, 0, 0);
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
        public DataTable GetDataTable(string sql, List<KeyValuePair<String, Object>> parameters, int startRecord, int maxRecords)
        {
            return GetDataTable(sql, CommandType.Text, parameters, true, true, true, startRecord, maxRecords);
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
        /// <returns>New DataTable item with results of the SQL command</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public DataTable GetDataTable(string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters, bool throwDBException, bool throwGenericException, bool throwSystemException)
        {
            return GetDataTable(sql, commandType, parameters, throwDBException, throwGenericException, throwSystemException, 0, 0);
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
        public DataTable GetDataTable(string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters, bool throwDBException, bool throwGenericException, bool throwSystemException, int startRecord, int maxRecords)
        {
            DataTable dt = new DataTable("Table0");

            FillData<DataTable>(dt, sql, commandType, parameters, throwDBException, throwGenericException, throwSystemException, startRecord, maxRecords);

            return dt;
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
        public void FillDataTable(DataTable dt, string sql)
        {
            FillDataTable(dt, sql, CommandType.Text, null);
        }

        /// <summary>
        /// Fill provided DataTable item with SQL command values
        /// </summary>
        /// <param name="dt">A DataTable item</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="startRecord">The zero based record number to start with</param>
        /// <param name="maxRecords">The maximum number of records to retrive</param>
        public void FillDataTable(DataTable dt, string sql, int startRecord, int maxRecords)
        {
            FillDataTable(dt, sql, CommandType.Text, null, startRecord, maxRecords);
        }

        /// <summary>
        /// Fill provided DataTable item with SQL command values
        /// </summary>
        /// <param name="dt">A DataTable item</param>
        /// <param name="sql">SQL command text to be executed</param>
        /// <param name="parameters">Parameters of the SQL command</param>
        public void FillDataTable(DataTable dt, string sql, List<KeyValuePair<String, Object>> parameters)
        {
            FillDataTable(dt, sql, CommandType.Text, parameters);
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
        public void FillDataTable(DataTable dt, string sql, List<KeyValuePair<String, Object>> parameters, int startRecord, int maxRecords)
        {
            FillDataTable(dt, sql, CommandType.Text, parameters, startRecord, maxRecords);
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
        public void FillDataTable(DataTable dt, string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters)
        {
            FillData<DataTable>(dt, sql, commandType, parameters, true, true, true, 0, 0);
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
        public void FillDataTable(DataTable dt, string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters, int startRecord, int maxRecords)
        {
            FillData<DataTable>(dt, sql, commandType, parameters, true, true, true, startRecord, maxRecords);
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
            return ExecuteNonQuery(sql, CommandType.Text, null);
        }

        /// <summary>
        /// Execute SQL query asynchronously and return the number of affected values
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <returns>Number of affected rows</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public async Task<int> ExecuteNonQueryAsync(string sql)
        {
            return await ExecuteNonQueryAsync(sql, CommandType.Text, null);
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
        public int ExecuteNonQuery(string sql, List<KeyValuePair<String, Object>> parameters)
        {
            return ExecuteNonQuery(sql, CommandType.Text, parameters);
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
        public async Task<int> ExecuteNonQueryAsync(string sql, List<KeyValuePair<String, Object>> parameters)
        {
            return await ExecuteNonQueryAsync(sql, CommandType.Text, parameters);
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
        public int ExecuteNonQuery(string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters)
        {
            SQLBatch<int> b = delegate (IDbConnection connection)
            {
                SqlConnection conn = connection as SqlConnection;
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.CommandType = commandType;

                SqlParameter rv = new SqlParameter("@return_value", SqlDbType.Int);
                rv.Direction = ParameterDirection.ReturnValue;

                cmd.Parameters.Add(rv);

                if (parameters != null)
                    foreach (KeyValuePair<String, Object> parameter in parameters)
                        cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);

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
        public async Task<int> ExecuteNonQueryAsync(string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters)
        {
            Func<IDbConnection, Task<int>> func = async delegate (IDbConnection connection)
            {
                SqlConnection conn = connection as SqlConnection;
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.CommandType = commandType;

                SqlParameter rv = new SqlParameter("@return_value", SqlDbType.Int);
                rv.Direction = ParameterDirection.ReturnValue;

                cmd.Parameters.Add(rv);

                if (parameters != null)
                    foreach (KeyValuePair<String, Object> parameter in parameters)
                        cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);

                int res = await cmd.ExecuteNonQueryAsync();

                int retVal = (int)rv.Value;

                if (retVal != 0) throw new SqlExecutionReturnedErrorCodeException(retVal, res);

                return res;
            };

            return await ExecuteSQLBatchAsync<int>(func);
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
            return ExecuteScalar(sql, CommandType.Text, null);
        }

        /// <summary>
        /// Execute SQL command asynchronously and return value of first column of the first row from results
        /// </summary>
        /// <param name="sql">SQL command text to be executed</param>
        /// <returns>Value of first column of the first row</returns>
        /// <exception cref="SqlExecutionException">Throws if any SqlException has accured</exception>
        /// <exception cref="SqlExecutionReturnedErrorCodeException">Throws if SQL query or stored procedure has returned non zero code</exception>
        /// <exception cref="GenericException">Throws if any Generic exception has accured</exception>
        public async Task<object> ExecuteScalarAsync(string sql)
        {
            return await ExecuteScalarAsync(sql, CommandType.Text, null);
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
        public object ExecuteScalar(string sql, List<KeyValuePair<String, Object>> parameters)
        {
            return ExecuteScalar(sql, CommandType.Text, parameters);
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
        public async Task<object> ExecuteScalarAsync(string sql, List<KeyValuePair<String, Object>> parameters)
        {
            return await ExecuteScalarAsync(sql, CommandType.Text, parameters);
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
        public object ExecuteScalar(string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters)
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
                    foreach (KeyValuePair<String, Object> parameter in parameters)
                        cmd.Parameters.AddWithValue(parameter.Key, parameter.Value == null ? DBNull.Value : parameter.Value);

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
        public async Task<object> ExecuteScalarAsync(string sql, CommandType commandType, List<KeyValuePair<String, Object>> parameters)
        {
            Func<IDbConnection, Task<object>> b = async delegate (IDbConnection connection)
            {
                SqlConnection conn = connection as SqlConnection;
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.CommandType = commandType;

                SqlParameter rv = new SqlParameter("@return_value", SqlDbType.Int);
                rv.Direction = ParameterDirection.ReturnValue;

                cmd.Parameters.Add(rv);

                if (parameters != null)
                    foreach (KeyValuePair<String, Object> parameter in parameters)
                        cmd.Parameters.AddWithValue(parameter.Key, parameter.Value == null ? DBNull.Value : parameter.Value);

                object res = await cmd.ExecuteScalarAsync();

                int retVal = (int)rv.Value;

                if (retVal != 0)
                    throw new SqlExecutionReturnedErrorCodeException(retVal, res);

                return res;
            };

            return await ExecuteSQLBatchAsync<object>(b);
        }

        #endregion

    }
}
