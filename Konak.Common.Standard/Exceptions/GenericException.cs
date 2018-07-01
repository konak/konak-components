using System;
using System.Collections.Generic;
using System.Text;

namespace Konak.Common.Exceptions
{
    public class GenericException : Exception
    {
        #region constrctors
        public GenericException() : base() { }
        public GenericException(string message) : base(message) { }
        public GenericException(Exception innerException) : base(innerException.Message, innerException) { }
        public GenericException(string message, Exception innerException) : base(message, innerException) { }
        public GenericException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        #endregion
    }
}
