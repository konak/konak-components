using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Konak.Common.Helpers
{
    public static class CompareExtensions
    {
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);


        public static bool Compare(this byte[] firstArray, byte[] secondArray)
        {
            return firstArray.Length == secondArray.Length && memcmp(firstArray, secondArray, firstArray.Length) == 0;
        }
    }
}
