using Konak.Net.Sockets.TCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Konak.Net.Sockets
{
    public class SocketConnection : ConnectionBase<SocketConnection>
    {
        TCPMessage _currentMessage = new TCPMessage();
        List<byte[]> _readData = new List<byte[]>(); // used only in one thread, in procedure StartRaisingConnectionDataReadEvents
        byte[] _processingBuffer = new byte[0];

        public SocketConnection(ConnectionConfig config) : base(config) { }

        protected override void StartRaisingConnectionDataReadEvents(object state)
        {
            byte[] data;
            int dataLength = 0;

            try
            {
                while (_receivedDataQueue.TryDequeue(out data))
                {
                    _readData.Add(data);
                    dataLength += data.Length;
                }

                if (dataLength == 0)
                    return;

                dataLength += _processingBuffer.Length;

                byte[] newBuffer = new byte[dataLength];

                Array.Copy(_processingBuffer, newBuffer, _processingBuffer.Length);

                int ind = _processingBuffer.Length;

                foreach (byte[] d in _readData)
                {
                    Array.Copy(d, 0, newBuffer, ind, d.Length);
                    ind += d.Length;
                }

                _readData.Clear();

                ind = 0;

                while(_currentMessage.TryRead(newBuffer, ind, dataLength, out ind))
                {
                    if(_currentMessage.Complete)
                    {
                        TCPMessage msg = _currentMessage;
                        _currentMessage = new TCPMessage();
                        RaiseConnectionDataReadEvent(msg.GetPayloadDataBytes());
                    }
                }

                if (ind == 0)
                    _processingBuffer = newBuffer;
                else
                {
                    _processingBuffer = new byte[dataLength - ind];
                    Array.Copy(newBuffer, ind, _processingBuffer, 0, _processingBuffer.Length);
                }

                Interlocked.Add(ref _bytesRead, dataLength);
                Interlocked.Add(ref _bytesReadWrite, dataLength);
            }
            finally
            {
                lock (_raisingDataReadSync)
                    _raisingDataReadEvent = false;
            }
        }
    }
}
