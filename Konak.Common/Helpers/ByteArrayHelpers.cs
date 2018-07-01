using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Common.Helpers
{
    public static class ByteArrayHelpers
    {
        #region GetBytes
        public static byte[] GetBytes(this bool item)
        {
            return BitConverter.GetBytes(item);
        }

        public static byte[] GetBytes(this int item)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] intBytes = BitConverter.GetBytes(item);
                return new byte[4] { intBytes[3], intBytes[2], intBytes[1], intBytes[0] };
            }
            else
                return BitConverter.GetBytes(item);
        }

        public static byte[] GetBytes(this int? item)
        {
            int val = item == null ? int.MinValue : item.Value;

            if (BitConverter.IsLittleEndian)
            {
                byte[] intBytes = BitConverter.GetBytes(val);
                return new byte[4] { intBytes[3], intBytes[2], intBytes[1], intBytes[0] };
            }
            else
                return BitConverter.GetBytes(val);
        }

        public static byte[] GetBytes(this uint item)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] intBytes = BitConverter.GetBytes(item);
                return new byte[4] { intBytes[3], intBytes[2], intBytes[1], intBytes[0] };
            }
            else
                return BitConverter.GetBytes(item);
        }

        public static byte[] GetBytes(this long item)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] intBytes = BitConverter.GetBytes(item);
                return new byte[8] { intBytes[7], intBytes[6], intBytes[5], intBytes[4], intBytes[3], intBytes[2], intBytes[1], intBytes[0] };
            }
            else
                return BitConverter.GetBytes(item);
        }

        public static byte[] GetBytes(this long? item)
        {
            long val = item == null ? long.MinValue : item.Value;

            if (BitConverter.IsLittleEndian)
            {
                byte[] intBytes = BitConverter.GetBytes(val);
                return new byte[8] { intBytes[7], intBytes[6], intBytes[5], intBytes[4], intBytes[3], intBytes[2], intBytes[1], intBytes[0] };
            }
            else
                return BitConverter.GetBytes(val);
        }

        public static byte[] GetBytes(this ulong item)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] intBytes = BitConverter.GetBytes(item);
                return new byte[8] { intBytes[7], intBytes[6], intBytes[5], intBytes[4], intBytes[3], intBytes[2], intBytes[1], intBytes[0] };
            }
            else
                return BitConverter.GetBytes(item);
        }

        public static byte[] GetBytes(this float item)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] intBytes = BitConverter.GetBytes(item);
                return new byte[4] { intBytes[3], intBytes[2], intBytes[1], intBytes[0] };
            }
            else
                return BitConverter.GetBytes(item);
        }

        public static byte[] GetBytes(this float? item)
        {
            float val = item == null ? float.MinValue : item.Value;

            if (BitConverter.IsLittleEndian)
            {
                byte[] intBytes = BitConverter.GetBytes(val);
                return new byte[4] { intBytes[3], intBytes[2], intBytes[1], intBytes[0] };
            }
            else
                return BitConverter.GetBytes(val);
        }

        public static byte[] GetBytes(this double item)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] intBytes = BitConverter.GetBytes(item);
                return new byte[8] { intBytes[7], intBytes[6], intBytes[5], intBytes[4], intBytes[3], intBytes[2], intBytes[1], intBytes[0] };
            }
            else
                return BitConverter.GetBytes(item);
        }

        public static byte[] GetBytes(this double? item)
        {
            double val = item == null ? double.MinValue : item.Value;

            if (BitConverter.IsLittleEndian)
            {
                byte[] intBytes = BitConverter.GetBytes(val);
                return new byte[8] { intBytes[7], intBytes[6], intBytes[5], intBytes[4], intBytes[3], intBytes[2], intBytes[1], intBytes[0] };
            }
            else
                return BitConverter.GetBytes(val);
        }

        public static byte[] GetBytes(this decimal item)
        {
            int[] ia = decimal.GetBits(item);

            return ia.GetBytes();
        }

        public static byte[] GetBytes(this decimal? item)
        {
            decimal itm = item == null ? decimal.MinValue : item.Value;

            int[] ia = decimal.GetBits(itm);

            return ia.GetBytes();
        }

        public static byte[] GetBytes(this Guid item)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] bytes = item.ToByteArray();
                return new byte[16] { bytes[15], bytes[14], bytes[13], bytes[12], bytes[11], bytes[10], bytes[9], bytes[8], bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2], bytes[1], bytes[0] };
            }
            else
                return item.ToByteArray();
        }

        public static byte[] GetBytes(this ushort item)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] intBytes = BitConverter.GetBytes(item);
                return new byte[2] { intBytes[1], intBytes[0] };
            }
            else
                return BitConverter.GetBytes(item);
        }

        public static byte[] GetBytes(this short item)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] intBytes = BitConverter.GetBytes(item);
                return new byte[2] { intBytes[1], intBytes[0] };
            }
            else
                return BitConverter.GetBytes(item);
        }

        public static byte[] GetBytes(this DateTime item)
        {
            return item.ToUniversalTime().Ticks.GetBytes();
        }

        public static byte[] GetBytes(this DateTime? item)
        {
            long ticks = item == null ? DateTime.MinValue.ToUniversalTime().Ticks : item.Value.ToUniversalTime().Ticks;

            return ticks.GetBytes();
        }

        public static byte[] GetBytes(this int[] item)
        {
            byte[] data = new byte[item.Length * 4];
            int ind = 0;

            if (BitConverter.IsLittleEndian)
                for (int i = 0; i < item.Length; i++)
                {
                    byte[] intBytes = BitConverter.GetBytes(item[i]);
                    data[ind++] = intBytes[3];
                    data[ind++] = intBytes[2];
                    data[ind++] = intBytes[1];
                    data[ind++] = intBytes[0];
                }
            else
                for (int i = 0; i < item.Length; i++)
                {
                    byte[] intBytes = BitConverter.GetBytes(item[i]);
                    data[ind++] = intBytes[0];
                    data[ind++] = intBytes[1];
                    data[ind++] = intBytes[2];
                    data[ind++] = intBytes[3];
                }

            return data;
        }

        public static byte[] GetBytes(this uint[] item)
        {
            byte[] data = new byte[item.Length * 4];
            int ind = 0;

            if (BitConverter.IsLittleEndian)
                for (int i = 0; i < item.Length; i++)
                {
                    byte[] intBytes = BitConverter.GetBytes(item[i]);
                    data[ind++] = intBytes[3];
                    data[ind++] = intBytes[2];
                    data[ind++] = intBytes[1];
                    data[ind++] = intBytes[0];
                }
            else
                for (int i = 0; i < item.Length; i++)
                {
                    byte[] intBytes = BitConverter.GetBytes(item[i]);
                    data[ind++] = intBytes[0];
                    data[ind++] = intBytes[1];
                    data[ind++] = intBytes[2];
                    data[ind++] = intBytes[3];
                }

            return data;
        }

        public static byte[] GetBytes(this short[] item)
        {
            byte[] data = new byte[item.Length * 2];
            int ind = 0;

            if (BitConverter.IsLittleEndian)
                for (int i = 0; i < item.Length; i++)
                {
                    byte[] intBytes = BitConverter.GetBytes(item[i]);
                    data[ind++] = intBytes[1];
                    data[ind++] = intBytes[0];
                }
            else
                for (int i = 0; i < item.Length; i++)
                {
                    byte[] intBytes = BitConverter.GetBytes(item[i]);
                    data[ind++] = intBytes[0];
                    data[ind++] = intBytes[1];
                }

            return data;
        }

        public static byte[] GetBytes(this ushort[] item)
        {
            byte[] data = new byte[item.Length * 2];
            int ind = 0;

            if (BitConverter.IsLittleEndian)
                for (int i = 0; i < item.Length; i++)
                {
                    byte[] intBytes = BitConverter.GetBytes(item[i]);
                    data[ind++] = intBytes[1];
                    data[ind++] = intBytes[0];
                }
            else
                for (int i = 0; i < item.Length; i++)
                {
                    byte[] intBytes = BitConverter.GetBytes(item[i]);
                    data[ind++] = intBytes[0];
                    data[ind++] = intBytes[1];
                }

            return data;
        }

        public static byte[] GetBytes(this long[] item)
        {
            byte[] data = new byte[item.Length * 8];
            int ind = 0;

            if (BitConverter.IsLittleEndian)
                for (int i = 0; i < item.Length; i++)
                {
                    byte[] intBytes = BitConverter.GetBytes(item[i]);
                    data[ind++] = intBytes[7];
                    data[ind++] = intBytes[6];
                    data[ind++] = intBytes[5];
                    data[ind++] = intBytes[4];
                    data[ind++] = intBytes[3];
                    data[ind++] = intBytes[2];
                    data[ind++] = intBytes[1];
                    data[ind++] = intBytes[0];
                }
            else
                for (int i = 0; i < item.Length; i++)
                {
                    byte[] intBytes = BitConverter.GetBytes(item[i]);
                    data[ind++] = intBytes[0];
                    data[ind++] = intBytes[1];
                    data[ind++] = intBytes[2];
                    data[ind++] = intBytes[3];
                    data[ind++] = intBytes[4];
                    data[ind++] = intBytes[5];
                    data[ind++] = intBytes[6];
                    data[ind++] = intBytes[7];
                }

            return data;
        }

        public static byte[] GetBytes(this ulong[] item)
        {
            byte[] data = new byte[item.Length * 8];
            int ind = 0;

            if (BitConverter.IsLittleEndian)
                for (int i = 0; i < item.Length; i++)
                {
                    byte[] intBytes = BitConverter.GetBytes(item[i]);
                    data[ind++] = intBytes[7];
                    data[ind++] = intBytes[6];
                    data[ind++] = intBytes[5];
                    data[ind++] = intBytes[4];
                    data[ind++] = intBytes[3];
                    data[ind++] = intBytes[2];
                    data[ind++] = intBytes[1];
                    data[ind++] = intBytes[0];
                }
            else
                for (int i = 0; i < item.Length; i++)
                {
                    byte[] intBytes = BitConverter.GetBytes(item[i]);
                    data[ind++] = intBytes[0];
                    data[ind++] = intBytes[1];
                    data[ind++] = intBytes[2];
                    data[ind++] = intBytes[3];
                    data[ind++] = intBytes[4];
                    data[ind++] = intBytes[5];
                    data[ind++] = intBytes[6];
                    data[ind++] = intBytes[7];
                }

            return data;
        }

        public static byte[] GetBytes(this float[] item)
        {
            byte[] data = new byte[item.Length * 4];
            int ind = 0;

            if (BitConverter.IsLittleEndian)
                for (int i = 0; i < item.Length; i++)
                {
                    byte[] intBytes = BitConverter.GetBytes(item[i]);
                    data[ind++] = intBytes[3];
                    data[ind++] = intBytes[2];
                    data[ind++] = intBytes[1];
                    data[ind++] = intBytes[0];
                }
            else
                for (int i = 0; i < item.Length; i++)
                {
                    byte[] intBytes = BitConverter.GetBytes(item[i]);
                    data[ind++] = intBytes[0];
                    data[ind++] = intBytes[1];
                    data[ind++] = intBytes[2];
                    data[ind++] = intBytes[3];
                }

            return data;
        }

        public static byte[] GetBytes(this double[] item)
        {
            byte[] data = new byte[item.Length * 8];
            int ind = 0;

            if (BitConverter.IsLittleEndian)
                for (int i = 0; i < item.Length; i++)
                {
                    byte[] intBytes = BitConverter.GetBytes(item[i]);
                    data[ind++] = intBytes[7];
                    data[ind++] = intBytes[6];
                    data[ind++] = intBytes[5];
                    data[ind++] = intBytes[4];
                    data[ind++] = intBytes[3];
                    data[ind++] = intBytes[2];
                    data[ind++] = intBytes[1];
                    data[ind++] = intBytes[0];
                }
            else
                for (int i = 0; i < item.Length; i++)
                {
                    byte[] intBytes = BitConverter.GetBytes(item[i]);
                    data[ind++] = intBytes[0];
                    data[ind++] = intBytes[1];
                    data[ind++] = intBytes[2];
                    data[ind++] = intBytes[3];
                    data[ind++] = intBytes[4];
                    data[ind++] = intBytes[5];
                    data[ind++] = intBytes[6];
                    data[ind++] = intBytes[7];
                }

            return data;
        }

        public static byte[] GetBytes(this decimal[] item)
        {
            byte[] data = new byte[item.Length * 16];
            int ind = 0;

            for (int i = 0; i < item.Length; i++)
            {
                Array.Copy(decimal.GetBits(item[i]).GetBytes(), 0, data, ind, 16);
                ind += 16;
            }

            return data;
        }

        public static byte[] GetBytes(this Guid[] item)
        {
            byte[] data = new byte[item.Length * 16];
            int ind = 0;

            if (BitConverter.IsLittleEndian)
                for (int i = 0; i < item.Length; i++)
                {
                    byte[] intBytes = item[i].ToByteArray();
                    data[ind++] = intBytes[15];
                    data[ind++] = intBytes[14];
                    data[ind++] = intBytes[13];
                    data[ind++] = intBytes[12];
                    data[ind++] = intBytes[11];
                    data[ind++] = intBytes[10];
                    data[ind++] = intBytes[9];
                    data[ind++] = intBytes[8];
                    data[ind++] = intBytes[7];
                    data[ind++] = intBytes[6];
                    data[ind++] = intBytes[5];
                    data[ind++] = intBytes[4];
                    data[ind++] = intBytes[3];
                    data[ind++] = intBytes[2];
                    data[ind++] = intBytes[1];
                    data[ind++] = intBytes[0];
                }
            else
                for (int i = 0; i < item.Length; i++)
                {
                    byte[] intBytes = item[i].ToByteArray();
                    data[ind++] = intBytes[0];
                    data[ind++] = intBytes[1];
                    data[ind++] = intBytes[2];
                    data[ind++] = intBytes[3];
                    data[ind++] = intBytes[4];
                    data[ind++] = intBytes[5];
                    data[ind++] = intBytes[6];
                    data[ind++] = intBytes[7];
                    data[ind++] = intBytes[8];
                    data[ind++] = intBytes[9];
                    data[ind++] = intBytes[10];
                    data[ind++] = intBytes[11];
                    data[ind++] = intBytes[12];
                    data[ind++] = intBytes[13];
                    data[ind++] = intBytes[14];
                    data[ind++] = intBytes[15];
                }

            return data;
        }

        #endregion

        public static byte[] WriteBytes(this byte[] byteArray, UInt16 item, int start, out int index)
        {
            byte[] data = item.GetBytes();

            Array.Copy(data, 0, byteArray, start, data.Length);

            index = start + data.Length;

            return byteArray;
        }

        public static byte[] WriteBytes(this byte[] byteArray, int item, int start, out int index)
        {
            byte[] data = item.GetBytes();

            Array.Copy(data, 0, byteArray, start, data.Length);

            index = start + data.Length;

            return byteArray;
        }

        public static byte[] WriteBytes(this byte[] byteArray, int? item, int start, out int index)
        {
            byte[] data = item.GetBytes();

            Array.Copy(data, 0, byteArray, start, data.Length);

            index = start + data.Length;

            return byteArray;
        }

        public static byte[] WriteBytes(this byte[] byteArray, byte item, int start, out int index)
        {
            byteArray[start] = item;
            index = start + 1;
            return byteArray;
        }

        public static byte[] WriteBytes(this byte[] byteArray, long item, int start, out int index)
        {
            byte[] data = item.GetBytes();

            Array.Copy(data, 0, byteArray, start, data.Length);

            index = start + data.Length;

            return byteArray;
        }

        public static byte[] WriteBytes(this byte[] byteArray, long? item, int start, out int index)
        {
            byte[] data = item.GetBytes();

            Array.Copy(data, 0, byteArray, start, data.Length);

            index = start + data.Length;

            return byteArray;
        }

        public static byte[] WriteBytes(this byte[] byteArray, DateTime item, int start, out int index)
        {
            byte[] data = item.GetBytes();

            Array.Copy(data, 0, byteArray, start, data.Length);

            index = start + data.Length;

            return byteArray;
        }

        public static byte[] WriteBytes(this byte[] byteArray, DateTime? item, int start, out int index)
        {
            byte[] data = item.GetBytes();

            Array.Copy(data, 0, byteArray, start, data.Length);

            index = start + data.Length;

            return byteArray;
        }

        public static byte[] WriteBytes(this byte[] byteArray, double item, int start, out int index)
        {
            byte[] data = item.GetBytes();

            Array.Copy(data, 0, byteArray, start, data.Length);

            index = start + data.Length;

            return byteArray;
        }

        public static byte[] WriteBytes(this byte[] byteArray, double? item, int start, out int index)
        {
            byte[] data = item.GetBytes();

            Array.Copy(data, 0, byteArray, start, data.Length);

            index = start + data.Length;

            return byteArray;
        }

        public static byte[] WriteBytes(this byte[] byteArray, Guid item, int start, out int index)
        {
            byte[] data = item.GetBytes();

            Array.Copy(data, 0, byteArray, start, data.Length);

            index = start + data.Length;

            return byteArray;
        }

        public static byte[] WriteBytes(this byte[] byteArray, int[] item, int start, out int index, bool skipLength)
        {
            byte[] data = item.GetBytes();

            if (skipLength)
                index = start;
            else
                byteArray.WriteBytes(data.Length, start, out index);

            Array.Copy(data, 0, byteArray, index, data.Length);

            index += data.Length;

            return byteArray;
        }

        public static byte[] WriteBytes(this byte[] byteArray, long[] item, int start, out int index, bool skipLength)
        {
            if (skipLength)
                index = start;
            else
                byteArray.WriteBytes(item.Length, start, out index);

            byte[] data = item.GetBytes();

            Array.Copy(data, 0, byteArray, index, data.Length);

            index += data.Length;

            return byteArray;
        }

        public static byte[] WriteBytes(this byte[] byteArray, byte[] item, int start, out int index, bool skipLength)
        {
            if (skipLength)
                index = start;
            else
                byteArray.WriteBytes(item.Length, start, out index);

            Array.Copy(item, 0, byteArray, index, item.Length);

            index += item.Length;

            return byteArray;
        }



        public static byte ReadByte(this byte[] data, int start, out int index)
        {
            index = start + 1;
            return data[start];
        }

        public static bool ReadBool(this byte[] data, int start, out int index)
        {
            index = start + 1;
            return BitConverter.ToBoolean(data, start);
        }

        public static UInt16 ReadUInt16(this byte[] data, int start, out int index)
        {
            UInt16 res;

            if (BitConverter.IsLittleEndian)
            {
                byte[] bytes = new byte[2] { data[start + 1], data[start] };
                res = BitConverter.ToUInt16(bytes, 0);
            }
            else
                res = BitConverter.ToUInt16(data, start);

            index = start + 2;

            return res;
        }

        public static int ReadInt(this byte[] data, int start, out int index)
        {
            int res;

            if (BitConverter.IsLittleEndian)
            {
                byte[] bytes = new byte[4] { data[start + 3], data[start + 2], data[start + 1], data[start] };
                res = BitConverter.ToInt32(bytes, 0);
            }
            else
                res = BitConverter.ToInt32(data, start);

            index = start + 4;

            return res;
        }

        public static int? ReadIntNullable(this byte[] data, int start, out int index)
        {
            int res = ReadInt(data, start, out index);

            if (res == int.MinValue) return null;

            return res;
        }

        public static long ReadLong(this byte[] data, int start, out int index)
        {
            long res;

            if (BitConverter.IsLittleEndian)
            {
                byte[] bytes = new byte[8] { data[start + 7], data[start + 6], data[start + 5], data[start + 4], data[start + 3], data[start + 2], data[start + 1], data[start] };
                res = BitConverter.ToInt64(bytes, 0);
            }
            else
                res = BitConverter.ToInt64(data, start);

            index = start + 8;

            return res;
        }

        public static Guid ReadGuid(this byte[] data, int start, out int index)
        {
            Guid res;

            byte[] guidBytes;

            if (BitConverter.IsLittleEndian)
                guidBytes = new byte[16] { data[start + 15], data[start + 14], data[start + 13], data[start + 12], data[start + 11], data[start + 10], data[start + 9], data[start + 8], data[start + 7], data[start + 6], data[start + 5], data[start + 4], data[start + 3], data[start + 2], data[start + 1], data[start] };
            else
                guidBytes = new byte[16] { data[start], data[start + 1], data[start + 2], data[start + 3], data[start + 4], data[start + 5], data[start + 6], data[start + 7], data[start + 8], data[start + 9], data[start + 10], data[start + 11], data[start + 12], data[start + 13], data[start + 14], data[start + 15] };

            res = new Guid(guidBytes);

            index = start + 16;

            return res;
        }

        public static double ReadDouble(this byte[] data, int start, out int index)
        {
            double res;

            if (BitConverter.IsLittleEndian)
            {
                byte[] bytes = new byte[8] { data[start + 7], data[start + 6], data[start + 5], data[start + 4], data[start + 3], data[start + 2], data[start + 1], data[start] };
                res = BitConverter.ToDouble(bytes, 0);
            }
            else
                res = BitConverter.ToDouble(data, start);

            index = start + 8;

            return res;
        }

        public static double? ReadDoubleNullable(this byte[] data, int start, out int index)
        {
            double res;

            if (BitConverter.IsLittleEndian)
            {
                byte[] bytes = new byte[8] { data[start + 7], data[start + 6], data[start + 5], data[start + 4], data[start + 3], data[start + 2], data[start + 1], data[start] };
                res = BitConverter.ToDouble(bytes, 0);
            }
            else
                res = BitConverter.ToDouble(data, start);

            index = start + 8;

            if (res == double.MinValue)
                return null;

            return res;
        }

        public static DateTime ReadDate(this byte[] data, int start, out int index)
        {
            return new DateTime(data.ReadLong(start, out index), DateTimeKind.Utc).ToLocalTime();
        }

        public static DateTime? ReadDateNullable(this byte[] data, int start, out int index)
        {
            DateTime dt = new DateTime(data.ReadLong(start, out index), DateTimeKind.Utc).ToLocalTime();

            if (dt == DateTime.MinValue) return null;

            return dt;
        }

        public static int[] ReadArrayInt(this byte[] data, int start, int length, out int index)
        {
            int cnt = 0;
            int[] res = new int[length / 4];
            byte[] buf = new byte[4];
            index = start;
            int end = start + length;

            if (BitConverter.IsLittleEndian)
            {
                while (index < end)
                {
                    buf[3] = data[index++];
                    buf[2] = data[index++];
                    buf[1] = data[index++];
                    buf[0] = data[index++];
                    res[cnt++] = BitConverter.ToInt32(buf, 0);
                }
            }
            else
            {
                while (index < end)
                {
                    buf[0] = data[index++];
                    buf[1] = data[index++];
                    buf[2] = data[index++];
                    buf[3] = data[index++];
                    res[cnt++] = BitConverter.ToInt32(buf, 0);
                }
            }

            return res;
        }

        public static int[] ReadArrayInt(this byte[] data, int start, out int index)
        {
            int length = data.ReadInt(start, out index);

            return ReadArrayInt(data, index, length, out index);
        }
    }
}
