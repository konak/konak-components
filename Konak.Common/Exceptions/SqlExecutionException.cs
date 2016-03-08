using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Common.Exceptions
{
    public class SqlExecutionException : GenericException
    {
        public SqlExecutionException() : base(Resources.Exceptions.SqlExecutionException.SQL_EXECUTION_EXCEPTION) { }
        public SqlExecutionException(System.Exception innerException) : base(Resources.Exceptions.SqlExecutionException.SQL_EXECUTION_EXCEPTION, innerException) { }
        public SqlExecutionException(string message, System.Exception innerException) : base(message, innerException) { }
        public SqlExecutionException(string message) : base(message) { }
    }
}
