using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Konak.Common.Helpers
{
    public static class IsEqualExtensions
    {
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);

        /// <summary>
        /// Extension to compare equality of two byte arrays
        /// </summary>
        /// <param name="firstArray">Firs array to be compared</param>
        /// <param name="secondArray">Second array to be compared</param>
        /// <returns></returns>
        public static bool IsEqual(this byte[] firstArray, byte[] secondArray)
        {
            if (firstArray == null || secondArray == null) return firstArray == secondArray;

            return firstArray.Length == secondArray.Length && memcmp(firstArray, secondArray, firstArray.Length) == 0;
        }
    }
}
