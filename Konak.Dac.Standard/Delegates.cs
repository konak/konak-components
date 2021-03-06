﻿using System;
using System.Data;

namespace Konak.Dac
{
    /// <summary>
    /// SQL batch to execute on SQL server
    /// </summary>
    /// <typeparam name="T">A generic type of the result returned by batch job</typeparam>
    /// <param name="connection">Connection to SQL server used to execute the batch job</param>
    /// <returns>Result of the batch job execution</returns>
    public delegate T SQLBatch<T>(IDbConnection connection);

    /// <summary>
    /// SQL batch to execute on SQL server within provided transaction
    /// </summary>
    /// <typeparam name="T">Generic type of the object returned by SQLTransactBatch</typeparam>
    /// <param name="transaction">SQL transaction used to execute batch job</param>
    /// <returns>Result of the batch job execution</returns>
    public delegate T SQLTransactBatch<T>(IDbTransaction transaction);

    /// <summary>
    /// SQL batch to execute on SQL server
    /// </summary>
    /// <typeparam name="T">A generic type of the result returned by batch job</typeparam>
    /// <param name="connection">Connection to SQL server used to execute the batch job</param>
    /// <returns>Result of the batch job execution</returns>
    public delegate T SQLAsyncBatch<T>(IDbConnection connection);

    /// <summary>
    /// SQL batch to execute on SQL server within provided transaction
    /// </summary>
    /// <typeparam name="T">Generic type of the object returned by SQLTransactBatch</typeparam>
    /// <param name="transaction">SQL transaction used to execute batch job</param>
    /// <returns>Result of the batch job execution</returns>
    public delegate T SQLAsyncTransactBatch<T>(IDbTransaction transaction);
}
