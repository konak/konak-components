using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Konak.Common.Helpers
{
    public static class IsEmptyExtension
    {
        public static bool IsEmpty(this ICollection param)
        {
            return (param == null || param.Count == 0);
        }

        public static bool IsEmpty(this IEnumerable param)
        {
            return (param == null || !param.GetEnumerator().MoveNext());
        }

        public static bool IsEmpty(this Array param)
        {
            return (param == null || param.Length == 0);
        }

        public static bool IsEmpty(this string param)
        {
            return IsEmpty(param, true);
        }

        public static bool IsEmpty(this string param, bool ignoreWhiteSpaces)
        {
            return ignoreWhiteSpaces ? string.IsNullOrWhiteSpace(param) : string.IsNullOrEmpty(param);
        }

        public static bool IsEmpty(this Guid param)
        {
            return Guid.Empty.Equals(param);
        }
    }
}
