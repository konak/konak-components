using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Net.Sockets
{
    public interface IKonakNetStream
    {
        IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state);
    }
}
