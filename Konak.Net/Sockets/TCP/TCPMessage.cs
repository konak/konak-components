using Konak.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Net.Sockets.TCP
{
    internal class TCPMessage
    {
        private FrameOperationCode _operationCode;
        private bool _complete;
        private List<TCPFrame> _frames;

        internal FrameOperationCode OperationCode { get { return _operationCode; } }
        internal bool Complete { get { return _complete; } }

        internal TCPMessage()
        {
            _operationCode = FrameOperationCode.continuation;
            _complete = false;
            _frames = new List<TCPFrame>();
        }
        internal TCPMessage(FrameOperationCode operationCode, byte[] data) : this()
        {
            _operationCode = operationCode;
            _complete = true;

            CreateFramesPrivate(data, true);
        }

        internal TCPMessage(FrameOperationCode operationCode, IList<byte[]> arrayList)
        {
            _operationCode = operationCode;
            _complete = true;

            for (int i = 0; i < arrayList.Count; i++)
                CreateFramesPrivate(arrayList[i], i == arrayList.Count - 1);
        }

        private void CreateFramesPrivate(byte[] data, bool setFinBit)
        {
            if(data.Length > TCPFrame.MAX_SIZE)
            {
                int ind = 0;
                int cnt = 0;

                while(ind<data.Length)
                {
                    cnt = data.Length - ind;

                    if (cnt > TCPFrame.MAX_SIZE)
                        cnt = TCPFrame.MAX_SIZE;

                    byte[] payloadData = new byte[cnt];

                    Array.Copy(data, ind, payloadData, 0, cnt);

                    TCPFrame frame = new TCPFrame(payloadData);

                    if (_frames.Count == 0)
                        frame.OperationCode = _operationCode;

                    ind += cnt;

                    if (ind == data.Length && setFinBit)
                        frame.ControlByte |= FrameControlByte.finbit;

                    _frames.Add(frame);
                }
            }
            else
            {
                _frames.Add(new TCPFrame(data) { ControlByte = setFinBit ? FrameControlByte.finbit : FrameControlByte.none, OperationCode = _operationCode });
            }
        }

        internal IEnumerable<byte[]> GetBytes()
        {
            foreach (TCPFrame frame in _frames)
                yield return frame.GetBytes();
        }

        internal bool TryRead(byte[] data, int start, int count, out int ind)
        {
            TCPFrame frame = TCPFrame.TryRead(data, start, count, out ind);

            if (frame == null)
                return false;

            if (_frames.Count == 0)
                _operationCode = frame.OperationCode;

            _frames.Add(frame);

            _complete = frame.ControlByte.HasFlag(FrameControlByte.finbit);

            return true;
        }

        internal byte[] GetPayloadDataBytes()
        {
            int dataLength = 0;

            foreach (TCPFrame frame in _frames)
                dataLength += frame.PayloadDataLength;

            byte[] res = new byte[dataLength];

            int ind = 0;

            foreach (TCPFrame frame in _frames)
            {
                Array.Copy(frame.PayloadData, 0, res, ind, frame.PayloadData.Length);
                ind += frame.PayloadData.Length;
            }

            return res;
        }
    }
}
