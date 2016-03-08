using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Common.Cryptography
{
    public static class HashGenerator
    {
        #region GetHashAlgorithm
        public static HashAlgorithm GetHashAlgorithm(string algorithmName = "MD5")
        {
            return GetHashAlgorithm(algorithmName, new byte[0]);
        }

        public static HashAlgorithm GetHashAlgorithm(string algorithmName = "MD5", string key = null)
        {
            HashAlgorithm res;

            if (string.IsNullOrEmpty(key))
                res = GetHashAlgorithm(algorithmName, new byte[0]);
            else
                res = GetHashAlgorithm(algorithmName, System.Text.Encoding.UTF8.GetBytes(key));

            return res;
        }

        public static HashAlgorithm GetHashAlgorithm(string algorithmName, byte[] key)
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
                    algorithm = Helpers.CH.IsEmpty(key) ? HMACMD5.Create() : new HMACMD5(key);
                    break;

                case "HMACRIPEMD160":
                    algorithm = Helpers.CH.IsEmpty(key) ? HMACRIPEMD160.Create() : new HMACRIPEMD160(key);
                    break;

                case "HMACSHA1":
                    algorithm = Helpers.CH.IsEmpty(key) ? HMACSHA1.Create() : new HMACSHA1(key);
                    break;

                case "HMACSHA256":
                    algorithm = Helpers.CH.IsEmpty(key) ? HMACSHA256.Create() : new HMACSHA256(key);
                    break;

                case "HMACSHA384":
                    algorithm = Helpers.CH.IsEmpty(key) ? HMACSHA384.Create() : new HMACSHA384(key);
                    break;

                case "HMACSHA512":
                    algorithm = Helpers.CH.IsEmpty(key) ? HMACSHA512.Create() : new HMACSHA512(key);
                    break;

                case "MACTripleDES":
                    algorithm = Helpers.CH.IsEmpty(key) ? MACTripleDES.Create() : new MACTripleDES (key);
                    break;

                case "RIPEMD160":
                    algorithm = RIPEMD160.Create();
                    break;

                default:
                    algorithm = MD5.Create();
                    break;
            }

            return algorithm;

        }
        #endregion

        #region GetHashBytes
        public static byte[] GetHashBytes(string data)
        {
            return GetHashBytes(data, "MD5", string.Empty);
        }
        public static byte[] GetHashBytes(string data, string algorithm)
        {
            if (string.IsNullOrEmpty(algorithm))
                algorithm = "MD5";

            return GetHashBytes(data, algorithm, string.Empty);
        }

        public static byte[] GetHashBytes(string data, string algorithm, string key)
        {
            byte[] d = System.Text.Encoding.UTF8.GetBytes(data);
            byte[] k = System.Text.Encoding.UTF8.GetBytes(key);

            if (string.IsNullOrEmpty(algorithm))
                algorithm = "MD5";

            return GetHashBytes(d, algorithm, key);
        }

        public static byte[] GetHashBytes(byte[] data, string algorithm, string key)
        {
            byte[] k = System.Text.Encoding.UTF8.GetBytes(key);

            if (string.IsNullOrEmpty(algorithm))
                algorithm = "MD5";

            return GetHashBytes(data, algorithm, k);
        }

        public static byte[] GetHashBytes(byte[] data, string algorithm, byte[] key)
        {
            HashAlgorithm alg = GetHashAlgorithm(algorithm, key);

            return alg.ComputeHash(data);
        }
        #endregion

        #region GetHashString
        public static string GetHashString(string data)
        {
            return GetHashString(data, "MD5", string.Empty);
        }

        public static string GetHashString(string data, string algorithm)
        {
            return GetHashString(data, algorithm, string.Empty);
        }


        public static string GetHashString(string data, string algorithm, string key)
        {
            byte[] d = System.Text.Encoding.UTF8.GetBytes(data);
            byte[] k = System.Text.Encoding.UTF8.GetBytes(key);

            if (string.IsNullOrEmpty(algorithm))
                algorithm = "MD5";

            return GetHashString(d, algorithm, k);
        }

        public static string GetHashString(byte[] data, string algorithm, string key)
        {
            byte[] k = System.Text.Encoding.UTF8.GetBytes(key);

            if (string.IsNullOrEmpty(algorithm))
                algorithm = "MD5";

            return GetHashString(data, algorithm, k);
        }

        public static string GetHashString(byte[] data, string algorithm, byte[] key)
        {
            byte[] hash = GetHashBytes(data, algorithm, key);

            StringBuilder sb = new StringBuilder();

            foreach (byte b in hash)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
        #endregion
    }
}
