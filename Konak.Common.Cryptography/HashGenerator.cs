using Konak.Common.Helpers;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Konak.Common.Cryptography
{
    public static class HashGenerator
    {
        #region GetHashAlgorithm

        /// <summary>
        /// Get instance of <see cref="HashAlgorithm"/>
        /// </summary>
        /// <param name="algorithmType">Algorithm type to get instance of</param>
        /// <param name="key">Key to be used during <see cref="HashAlgorithm"/> instance init </param>
        /// <returns></returns>
        public static HashAlgorithm GetHashAlgorithm(AlgorithmType algorithmType = AlgorithmType.MD5, string key = "")
        {
            return GetHashAlgorithm(algorithmType, Encoding.UTF8.GetBytes(key));
        }

        /// <summary>
        /// Get instance of <see cref="HashAlgorithm"/>
        /// </summary>
        /// <param name="algorithmType">Algorithm type to get instance of</param>
        /// <param name="key">Key to be used during <see cref="HashAlgorithm"/> instance init </param>
        /// <returns></returns>
        public static HashAlgorithm GetHashAlgorithm(AlgorithmType algorithmType = AlgorithmType.MD5, byte[] key = null)
        {
            HashAlgorithm algorithm;

            switch (algorithmType)
            {
                case AlgorithmType.MD5:
                    algorithm = MD5.Create();
                    break;

                case AlgorithmType.SHA1:
                    algorithm = SHA1.Create();
                    break;

                case AlgorithmType.SHA256:
                    algorithm = SHA256.Create();
                    break;

                case AlgorithmType.SHA384:
                    algorithm = SHA384.Create();
                    break;

                case AlgorithmType.SHA512:
                    algorithm = SHA512.Create();
                    break;

                case AlgorithmType.HMAC:
                    algorithm = HMAC.Create();
                    break;

                case AlgorithmType.HMACMD5:
                    algorithm = key.IsEmpty() ? HMACMD5.Create() : new HMACMD5(key);
                    break;

                case AlgorithmType.HMACSHA1:
                    algorithm = key.IsEmpty() ? HMACSHA1.Create() : new HMACSHA1(key);
                    break;

                case AlgorithmType.HMACSHA256:
                    algorithm = key.IsEmpty() ? HMACSHA256.Create() : new HMACSHA256(key);
                    break;

                case AlgorithmType.HMACSHA384:
                    algorithm = key.IsEmpty() ? HMACSHA384.Create() : new HMACSHA384(key);
                    break;

                case AlgorithmType.HMACSHA512:
                    algorithm = key.IsEmpty() ? HMACSHA512.Create() : new HMACSHA512(key);
                    break;

                case AlgorithmType.HMACRIPEMD160:
                    throw new NotImplementedException("HMACRIPEMD160 algorithm is not implemented in NetCore.Standard version version. Please use class from Konak.Common.Cryptography.Net namespace.");

                case AlgorithmType.MACTripleDES:
                    throw new NotImplementedException("MACTripleDES algorithm is not implemented in NetCore.Standard version version. Please use class from Konak.Common.Cryptography.Net namespace.");

                case AlgorithmType.RIPEMD160:
                    throw new NotImplementedException("RIPEMD160 algorithm is not implemented in NetCore.Standard version version. Please use class from Konak.Common.Cryptography.Net namespace.");

                default:
                    algorithm = MD5.Create();
                    break;
            }

            return algorithm;

        }
        #endregion

        #region GetHashBytes
        /// <summary>
        /// Extension to get array of bytes of hash, generated for provided data
        /// </summary>
        /// <param name="data">Data to generate hash for</param>
        /// <returns></returns>
        public static byte[] GetHashBytes(this string data)
        {
            byte[] d = Encoding.UTF8.GetBytes(data);

            return GetHashBytes(d, AlgorithmType.MD5, new byte[0]);
        }

        /// <summary>
        /// Extension to get array of bytes of hash, generated for provided data
        /// </summary>
        /// <param name="data">Data to generate hash for</param>
        /// <param name="algorithm">Algorithm to be used during hash generation</param>
        /// <param name="key">Key to be used during hash generation</param>
        /// <returns></returns>
        public static byte[] GetHashBytes(this string data, AlgorithmType algorithm = AlgorithmType.MD5, string key = "")
        {
            byte[] d = Encoding.UTF8.GetBytes(data);
            byte[] k = Encoding.UTF8.GetBytes(key);

            return GetHashBytes(d, algorithm, k);
        }

        /// <summary>
        /// Extension to get array of bytes of hash, generated for provided data
        /// </summary>
        /// <param name="data">Data to generate hash for</param>
        /// <param name="algorithm">Algorithm to be used during hash generation</param>
        /// <param name="key">Key to be used during hash generation</param>
        /// <returns></returns>
        public static byte[] GetHashBytes(this string data, AlgorithmType algorithm = AlgorithmType.MD5, byte[] key = null)
        {
            byte[] d = Encoding.UTF8.GetBytes(data);

            return GetHashBytes(d, algorithm, key);
        }

        /// <summary>
        /// Extension to get array of bytes of hash, generated for provided data
        /// </summary>
        /// <param name="data">Data to generate hash for</param>
        /// <returns></returns>
        public static byte[] GetHashBytes(this byte[] data)
        {
            return GetHashBytes(data, AlgorithmType.MD5, new byte[0]);
        }

        /// <summary>
        /// Extension to get array of bytes of hash, generated for provided data
        /// </summary>
        /// <param name="data">Data to generate hash for</param>
        /// <param name="algorithm">Algorithm to be used during hash generation</param>
        /// <param name="key">Key to be used during hash generation</param>
        /// <returns></returns>
        public static byte[] GetHashBytes(this byte[] data, AlgorithmType algorithm = AlgorithmType.MD5, string key = "")
        {
            byte[] k = Encoding.UTF8.GetBytes(key);

            return GetHashBytes(data, algorithm, k);
        }

        /// <summary>
        /// Extension to get array of bytes of hash, generated for provided data
        /// </summary>
        /// <param name="data">Data to generate hash for</param>
        /// <param name="algorithm">Algorithm to be used during hash generation</param>
        /// <param name="key">Key to be used during hash generation</param>
        /// <returns></returns>
        public static byte[] GetHashBytes(this byte[] data, AlgorithmType algorithm = AlgorithmType.MD5, byte[] key = null)
        {
            if (key == null) key = new byte[0];

            HashAlgorithm alg = GetHashAlgorithm(algorithm, key);

            return alg.ComputeHash(data);
        }
        #endregion

        #region GetHashString
        /// <summary>
        /// Extension to get string representation of hash, generated for provided data
        /// </summary>
        /// <param name="data">Data to generate hash for</param>
        /// <param name="algorithm">Algorithm to be used during hash generation</param>
        /// <param name="key">Key to be used during hash generation</param>
        /// <returns></returns>
        public static string GetHashString(this string data, AlgorithmType algorithm = AlgorithmType.MD5, string key = "")
        {
            byte[] d = Encoding.UTF8.GetBytes(data);
            byte[] k = Encoding.UTF8.GetBytes(key);

            return GetHashString(d, algorithm, k);
        }

        /// <summary>
        /// Extension to get string representation of hash, generated for provided data
        /// </summary>
        /// <param name="data">Data to generate hash for</param>
        /// <param name="algorithm">Algorithm to be used during hash generation</param>
        /// <param name="key">Key to be used during hash generation</param>
        /// <returns></returns>
        public static string GetHashString(this byte[] data, AlgorithmType algorithm = AlgorithmType.MD5, string key = "")
        {
            byte[] k = Encoding.UTF8.GetBytes(key);

            return GetHashString(data, algorithm, k);
        }

        /// <summary>
        /// Extension to get string representation of hash, generated for provided data
        /// </summary>
        /// <param name="data">Data to generate hash for</param>
        /// <param name="algorithm">Algorithm to be used during hash generation</param>
        /// <param name="key">Key to be used during hash generation</param>
        /// <returns></returns>
        public static string GetHashString(this byte[] data, AlgorithmType algorithm = AlgorithmType.MD5, byte[] key = null)
        {
            if (key == null) key = new byte[0];

            byte[] hash = GetHashBytes(data, algorithm, key);

            StringBuilder sb = new StringBuilder();

            for (int i = 0, len = hash.Length; i<len; i++)
                sb.Append(BitConverter.ToString(hash, i, 1));

            return sb.ToString();
        }
        #endregion
    }
}
