using Konak.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Net
{
    public interface IConnection
    {
        event ConnectionClosedDelegate ConnectionClosedEvent;
        event ClientConnectedDelegate ClientConnectedEvent;
        event ConnectionErrorDelegate ConnectionErrorEvent;
        event ConnectionDataReadDelegate ConnectionDataReadEvent;

        Guid RID { get; }

        void Start();
        void Stop();

        void Send(byte[] data);
        void Send(IEnumerable<byte[]> data);
    }
}
