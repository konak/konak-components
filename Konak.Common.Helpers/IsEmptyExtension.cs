using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Konak.Common.Helpers
{
    public static class IsEmptyExtension
    {
        /// <summary>
        /// Extension to check if collection is empty
        /// </summary>
        /// <param name="param">Collection to check</param>
        /// <returns></returns>
        public static bool IsEmpty(this ICollection param)
        {
            return (param == null || param.Count == 0);
        }

        /// <summary>
        /// Extension to check if enumerable object is empty
        /// </summary>
        /// <param name="param">IEnumerable object to check</param>
        /// <returns></returns>
        public static bool IsEmpty(this IEnumerable param)
        {
            return (param == null || !param.GetEnumerator().MoveNext());
        }

        /// <summary>
        /// Extension to check if array is empty
        /// </summary>
        /// <param name="param">Array to check</param>
        /// <returns></returns>
        public static bool IsEmpty(this Array param)
        {
            return (param == null || param.Length == 0);
        }

        /// <summary>
        /// Extension to check if string is empty
        /// </summary>
        /// <param name="param">String to check if it is empty</param>
        /// <param name="ignoreWhiteSpaces">if string contains only whitespaces interpret it as empty</param>
        /// <returns></returns>
        public static bool IsEmpty(this string param, bool ignoreWhiteSpaces = true)
        {
            return ignoreWhiteSpaces ? string.IsNullOrWhiteSpace(param) : string.IsNullOrEmpty(param);
        }

        /// <summary>
        /// Extension to check if Guid is empty
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool IsEmpty(this Guid param)
        {
            return Guid.Empty.Equals(param);
        }
    }
}
