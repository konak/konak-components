using Konak.Common.Helpers;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Konak.Common.Cryptography
{
    public static class HashGenerator
    {
        #region GetHashAlgorithm
        public static HashAlgorithm GetHashAlgorithm(string algorithmName = "MD5", string key = null)
        {
            HashAlgorithm res;

            if (string.IsNullOrEmpty(key))
                res = GetHashAlgorithm(algorithmName, new byte[0]);
            else
                res = GetHashAlgorithm(algorithmName, System.Text.Encoding.UTF8.GetBytes(key));

            return res;
        }

        public static HashAlgorithm GetHashAlgorithm(string algorithmName = "MD5", byte[] key = null)
        {
            HashAlgorithm algorithm;

            switch (algorithmName)
            {
                case "SHA1":
                    algorithm = SHA1.Create();
                    break;

                case "SHA256":
                    algorithm = SHA256.Create();
                    break;

                case "SHA384":
                    algorithm = SHA384.Create();
                    break;

                case "SHA512":
                    algorithm = SHA512.Create();
                    break;

                case "HMAC":
                    algorithm = HMAC.Create();
                    break;

                case "HMACMD5":
                    algorithm = key.IsEmpty() ? HMACMD5.Create() : new HMACMD5(key);
                    break;

                case "HMACSHA1":
                    algorithm = key.IsEmpty() ? HMACSHA1.Create() : new HMACSHA1(key);
                    break;

                case "HMACSHA256":
                    algorithm = key.IsEmpty() ? HMACSHA256.Create() : new HMACSHA256(key);
                    break;

                case "HMACSHA384":
                    algorithm = key.IsEmpty() ? HMACSHA384.Create() : new HMACSHA384(key);
                    break;

                case "HMACSHA512":
                    algorithm = key.IsEmpty() ? HMACSHA512.Create() : new HMACSHA512(key);
                    break;

                default:
                    algorithm = MD5.Create();
                    break;
            }

            return algorithm;

        }
        #endregion

        #region GetHashBytes
        public static byte[] GetHashBytes(this string data, string algorithm = "MD5", string key = "")
        {
            byte[] d = System.Text.Encoding.UTF8.GetBytes(data);
            byte[] k = System.Text.Encoding.UTF8.GetBytes(key);

            return GetHashBytes(d, algorithm, k);
        }

        public static byte[] GetHashBytes(this byte[] data, string algorithm = "MD5", string key = "")
        {
            byte[] k = System.Text.Encoding.UTF8.GetBytes(key);

            return GetHashBytes(data, algorithm, k);
        }

        public static byte[] GetHashBytes(this byte[] data, string algorithm = "MD5", byte[] key = null)
        {
            if (key == null) key = new byte[0];

            HashAlgorithm alg = GetHashAlgorithm(algorithm, key);

            return alg.ComputeHash(data);
        }
        #endregion

        #region GetHashString
        public static string GetHashString(this string data, string algorithm = "MD5", string key = "")
        {
            byte[] d = System.Text.Encoding.UTF8.GetBytes(data);
            byte[] k = System.Text.Encoding.UTF8.GetBytes(key);

            return GetHashString(d, algorithm, k);
        }

        public static string GetHashString(this byte[] data, string algorithm = "MD5", string key = "")
        {
            byte[] k = System.Text.Encoding.UTF8.GetBytes(key);

            return GetHashString(data, algorithm, k);
        }

        public static string GetHashString(this byte[] data, string algorithm = "MD5", byte[] key = null)
        {
            if (key == null) key = new byte[0];

            byte[] hash = GetHashBytes(data, algorithm, key);

            StringBuilder sb = new StringBuilder();

            foreach (byte b in hash)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
        #endregion
    }
}
