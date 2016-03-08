using Konak.Common.Exceptions;
using System;

namespace Konak.Net.Exceptions
{
    public class SocketGenericException : GenericException
    {
        public SocketGenericException() : base("Unknown socket exception") { }
        public SocketGenericException(string message) : base(message) { }
        public SocketGenericException(Exception ex) : base("Unknown socket exception", ex) { }
        public SocketGenericException(string message, Exception ex) : base(message, ex) { }
    }
}
