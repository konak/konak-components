using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Net.Exceptions
{
    public class SocketMessageFrameException : SocketGenericException
    {
        public SocketMessageFrameException() : base("Socket frame unknown exception") { }
        public SocketMessageFrameException(string message) : base(message) { }
        public SocketMessageFrameException(Exception ex) : base("Socket frame unknown exception", ex) { }
        public SocketMessageFrameException(string message, Exception ex) : base(message, ex) { }
    }
}
