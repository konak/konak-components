﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Konak.Common.Helpers
{
    public static class RandomKeyHelpers
    {
        public static RNGCryptoServiceProvider Rng = new RNGCryptoServiceProvider();

        /// <summary>
        /// Get string of hexadecimal representation of array with randomly generated bytes
        /// </summary>
        /// <param name="length">length of array to generate.</param>
        /// <param name="withDashes">
        /// Property indicating whether you want bytes in resulting string be separated with dashes or not.
        /// By default resulting string will not contain dashes
        /// </param>
        /// <returns></returns>
        public static string GetString(int length, bool withDashes = true)
        {
            byte[] buff = GetyBytes(length);

            if (withDashes)
            {
                StringBuilder sb = new StringBuilder(length * 2);

                for (int i = 0; i < length; i++)
                    sb.Append(BitConverter.ToString(buff, i, 0));

                return sb.ToString();
            }

            return BitConverter.ToString(buff);
        }

        /// <summary>
        /// Get array of random bytes
        /// </summary>
        /// <param name="length">Length of array to generate</param>
        /// <returns></returns>
        public static byte[] GetyBytes(int length)
        {
            byte[] buff = new byte[length];

            Rng.GetBytes(buff);

            return buff;
        }
    }
}
