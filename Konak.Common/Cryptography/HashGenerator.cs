using System.Security.Cryptography;
using System.Text;
using Konak.Common.Helpers;

namespace Konak.Common.Cryptography.Net
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
                res = GetHashAlgorithm(algorithmName, Encoding.UTF8.GetBytes(key));

            return res;
        }

        public static HashAlgorithm GetHashAlgorithm(string algorithmName = "MD5", byte[] key = null)
        {
            HashAlgorithm algorithm;

            switch (algorithmName)
            {
                case "HMACRIPEMD160":
                    algorithm = key.IsEmpty() ? HMACRIPEMD160.Create() : new HMACRIPEMD160(key);
                    break;

                case "MACTripleDES":
                    algorithm = key.IsEmpty() ? MACTripleDES.Create() : new MACTripleDES (key);
                    break;

                case "RIPEMD160":
                    algorithm = RIPEMD160.Create();
                    break;

                default:
                    algorithm = Cryptography.HashGenerator.GetHashAlgorithm(algorithmName, key);
                    break;
            }

            return algorithm;

        }
        #endregion
    }
}
