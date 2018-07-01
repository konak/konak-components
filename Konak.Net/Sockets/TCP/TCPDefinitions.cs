using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Net.Sockets.TCP
{
    /// <summary>
    /// Frame control byte
    /// </summary>
    [Flags]
    internal enum FrameControlByte : byte
    {
        /// <summary>
        /// no data, empty control byte
        /// </summary>
        none = 0,
        /// <summary>
        /// Bit that identifying the final frame of the message
        /// </summary>
        finbit = 1
    }

    /// <summary>
    /// Frame operation code
    /// </summary>
    internal enum FrameOperationCode : byte
    {
        /// <summary>
        /// Continuation frame
        /// </summary>
        continuation = 0,
        /// <summary>
        /// Text data / text command frame
        /// </summary>
        text = 1,
        /// <summary>
        /// binary data frame
        /// </summary>
        bin = 2,
        /// <summary>
        /// connection close command frame
        /// </summary>
        close = 8,
        /// <summary>
        /// ping data frame
        /// </summary>
        ping = 9,
        /// <summary>
        /// pong frame
        /// </summary>
        pong = 10
    }
}
