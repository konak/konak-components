using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Konak.Net.Exceptions;

namespace Konak.Net.Sockets
{

    public class Connection : ConnectionBase<Connection>
    {

        #region constructors

        protected Connection() : base() { }

        public Connection(ConnectionConfig config) : base(config) { }
        #endregion

        
        protected override void StartRaisingConnectionDataReadEvents(object state)
        {
            byte[] data;

            try
            {
                while (_receivedDataQueue.TryDequeue(out data))
                {
                    RaiseConnectionDataReadEvent(data);

                    Interlocked.Add(ref _bytesRead, data.Length);
                    Interlocked.Add(ref _bytesReadWrite, data.Length);
                }
            }
            finally
            {
                lock (_raisingDataReadSync)
                    _raisingDataReadEvent = false;
            }
        }
    }
}
