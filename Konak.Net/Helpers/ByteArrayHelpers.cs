using Konak.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Net.Helpers
{
    internal static class ByteArrayHelpers
    {
        internal static byte[] GetBytes(this string item, int ttt)
        {
            // do we need to check BitConverter.IsLittleEndian or not?
            // need to check during tests

            return NetworkHelper.DATA_ENCODING.GetBytes(item);
        }

        internal static string ReadString(this byte[] data, int start, out int index)
        {
            int length = data.ReadInt(start, out index);

            string res = NetworkHelper.DATA_ENCODING.GetString(data, index, length);

            index += length;

            return res;
        }

    }
}
