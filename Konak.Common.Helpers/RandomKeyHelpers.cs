using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Konak.Common.Helpers
{
    public static class RandomKeyHelpers
    {
        public static string GetString(int bytelength)
        {
            byte[] buff = GetyBytes(bytelength);

            StringBuilder sb = new StringBuilder(bytelength * 2);

            for (int i = 0; i < buff.Length; i++)
                sb.Append(string.Format("{0:X2}", buff[i]));

            return sb.ToString();
        }

        public static byte[] GetyBytes(int bytelength)
        {
            byte[] buff = new byte[bytelength];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            rng.GetBytes(buff);

            return buff;
        }
    }
}
