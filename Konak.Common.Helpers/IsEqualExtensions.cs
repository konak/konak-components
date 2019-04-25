using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Konak.Common.Helpers
{
    public static class IsEqualExtensions
    {
        /// <summary>
        /// Extension to compare equality of two byte arrays
        /// </summary>
        /// <param name="firstArray">Firs array to be compared</param>
        /// <param name="secondArray">Second array to be compared</param>
        /// <returns></returns>
        public static bool IsEqual(this byte[] firstArray, byte[] secondArray)
        {
            if (firstArray == null || secondArray == null) return firstArray == secondArray;

            if (firstArray.Length != secondArray.Length) return false;

            for (int i = 0, len = firstArray.Length; i < len; i++)
                if (firstArray[i] != secondArray[i])
                    return false;

            return true;
        }
    }
}
