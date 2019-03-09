using System;
using System.Collections.Generic;
using System.Text;

namespace Konak.Common.Cryptography
{
    public enum AlgorithmType
    {
        SHA1 = 1,
        SHA256 = 2,
        SHA384 = 3,
        SHA512 = 4,
        HMAC = 5,
        HMACMD5 = 6,
        HMACSHA1 = 7,
        HMACSHA256 = 8,
        HMACSHA384 = 9,
        HMACSHA512 = 10,
        MD5 = 11,
        HMACRIPEMD160 = 12,
        MACTripleDES = 13,
        RIPEMD160 = 14
    }
}
