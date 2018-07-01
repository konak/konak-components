using System;
using System.Collections.Generic;
using System.Text;

namespace Konak.Common.Exceptions
{
    /// <summary>
    /// Exception fired when the stored procedure returns not zero code,
    /// and it means that an error has accured during storedprocedure execution
    /// </summary>
    public class SqlExecutionReturnedErrorCodeException : SqlExecutionException
    {
        /// <summary>
        /// Code returned after executing SQL stored procedure
        /// </summary>
        public int RetCode { get; private set; }

        /// <summary>
        /// Object returned from the stored procedure
        /// </summary>
        public object ReturnedObject { get; private set; }

        public SqlExecutionReturnedErrorCodeException(int retCode)
            : base(Resources.Exceptions.SqlExecutionReturnedErrorCodeException.SQL_EXECUTION_RETURNED_NON_ZERO_CODE + retCode)
        {
            RetCode = retCode;
        }

        public SqlExecutionReturnedErrorCodeException(int retCode, object returnedObject)
            : base(Resources.Exceptions.SqlExecutionReturnedErrorCodeException.SQL_EXECUTION_RETURNED_NON_ZERO_CODE + retCode)
        {
            RetCode = retCode;
            ReturnedObject = returnedObject;
        }
    }
}
