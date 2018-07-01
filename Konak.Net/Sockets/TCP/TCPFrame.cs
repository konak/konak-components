using Konak.Common.Helpers;
using Konak.Net.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Net.Sockets.TCP
{
    internal class TCPFrame
    {
        internal const int MAX_SIZE = 1 * 1024 * 1024;

        private byte[] _payloadData;

        internal FrameControlByte ControlByte { get; set; }
        internal FrameOperationCode OperationCode { get; set; }
        internal int PayloadDataLength { get { return _payloadData.Length; } }
        internal byte[] PayloadData { get { return _payloadData; } }

        private TCPFrame()
        {
            ControlByte = FrameControlByte.none;
            OperationCode = FrameOperationCode.continuation;
        }

        internal TCPFrame(byte[] payloadData) : this()
        {
            _payloadData = payloadData;
        }

        internal byte[] GetBytes()
        {
            int pdl = _payloadData.Length;
            byte[] res = new byte[pdl + 6];

            res[0] = (byte)ControlByte;
            res[1] = (byte)OperationCode;

            int ind = 2;

            res.WriteBytes(pdl.GetBytes(), ind, out ind, true);
            res.WriteBytes(_payloadData, ind, out ind, true);

            return res;
        }

        internal static TCPFrame TryRead(byte[] data, int start, int count, out int ind)
        {
            ind = start;

            if (count < start + 6)
                return null;

            TCPFrame res = new TCPFrame();

            res.ControlByte = (FrameControlByte)data[ind++];
            res.OperationCode = (FrameOperationCode)data[ind++];

            int dataLength = data.ReadInt(ind, out ind);

            if (dataLength > MAX_SIZE)
                throw new SocketMessageFrameException("Frame size is too big");

            if(dataLength + ind > count - start)
            {
                ind = start;
                return null;
            }

            res._payloadData = new byte[dataLength];
            Array.Copy(data, ind, res._payloadData, 0, dataLength);

            ind += dataLength;

            return res;
        }
    }
}
