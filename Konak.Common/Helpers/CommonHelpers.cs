using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Konak.Common.Search;

namespace Konak.Common.Helpers
{
    

    public sealed class CH // Common Helpers
    {
        public static readonly Guid BlankRID = Guid.Empty;

        #region IsEmpty methods
        public static bool IsEmpty(object param)
        {
            if (param == null ||
                (param.GetType() == typeof(string) &&
                    IsEmpty(param.ToString())))
                return true;

            return false;
        }

        public static bool IsEmpty(DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue ||
                (dateTime.Year <= 1 &&
                 dateTime.Month <= 1 &&
                 dateTime.Day <= 1))
                return true;

            return false;
        }

        public static bool IsEmpty(ICollection param)
        {
            return (param == null || param.Count == 0);
        }

        public static bool IsEmpty(Array param)
        {
            return (param == null || param.Length == 0);
        }

        public static bool IsEmpty(string param)
        {
            return IsEmpty(param, true);
        }


        public static bool IsEmpty(string param, bool ignoreWhiteSpaces)
        {
            return ignoreWhiteSpaces ? string.IsNullOrWhiteSpace(param) : string.IsNullOrEmpty(param);
        }

        public static bool IsEmpty(Guid param)
        {
            return BlankRID.Equals(param);
        }
        #endregion

        #region FormatDate method

        public static string FormatDate(DateTime dt)
        {
            return FormatDate(dt, true, true);
        }

        public static string FormatDate(DateTime dt, bool includeTime)
        {
            return FormatDate(dt, includeTime, true);
        }

        public static string FormatDate(DateTime dt, bool includeTime, bool includeYear)
        {
            string f = "HH:mm dd/MM/YYYY";

            if (!includeTime) f = "dd/MM/YYYY";
            if (!includeYear) f = f.Replace("/YYYY", string.Empty);

            return dt.ToString(f);
        }
        #endregion

        #region GetRandomKey
        public static string GetRandomKey(int bytelength)
        {
            byte[] buff = new byte[bytelength];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buff);
            StringBuilder sb = new StringBuilder(bytelength * 2);
            for (int i = 0; i < buff.Length; i++)
                sb.Append(string.Format("{0:X2}", buff[i]));
            return sb.ToString();
        }

        #endregion

        #region PassFilterOnField
        public static bool PassFilterOnField(string fieldValue, FilterActionType actionType, string filterValue)
        {
            switch (actionType)
            {
                case FilterActionType.contain:
                    if (!fieldValue.Contains(filterValue))
                        return false;
                    break;

                case FilterActionType.not_contain:
                    if (fieldValue.Contains(filterValue))
                        return false;
                    break;

                case FilterActionType.equal:
                    if (!fieldValue.Equals(filterValue))
                        return false;
                    break;

                case FilterActionType.not_equal:
                    if (fieldValue.Equals(filterValue))
                        return false;
                    break;

                case FilterActionType.greater:
                    if (fieldValue.CompareTo(filterValue) <= 0)
                        return false;
                    break;

                case FilterActionType.greater_or_equal:
                    if (fieldValue.CompareTo(filterValue) < 0)
                        return false;
                    break;

                case FilterActionType.less:
                    if (fieldValue.CompareTo(filterValue) >= 0)
                        return false;
                    break;

                case FilterActionType.less_or_equal:
                    if (fieldValue.CompareTo(filterValue) > 0)
                        return false;
                    break;

                default:
                    throw new NotImplementedException("Action is not implemented: " + actionType.ToString());
            }

            return true;
        }

        public static DateTime ParseDateTime(string dateTimeString)
        {
            return DateTime.Parse(dateTimeString.Replace("GMT", "+0000").Replace("EST", "-0500").Replace("PST", "-0800"));
        }

        public static bool PassFilterOnField(DateTime fieldValue, FilterActionType actionType, DateTime filterValue)
        {
            switch (actionType)
            {
                case FilterActionType.equal:
                    if (!fieldValue.Equals(filterValue))
                        return false;
                    break;

                case FilterActionType.not_equal:
                    if (fieldValue.Equals(filterValue))
                        return false;
                    break;

                case FilterActionType.greater:
                    if (fieldValue <= filterValue)
                        return false;
                    break;

                case FilterActionType.greater_or_equal:
                    if (fieldValue < filterValue)
                        return false;
                    break;

                case FilterActionType.less:
                    if (fieldValue >= filterValue)
                        return false;
                    break;

                case FilterActionType.less_or_equal:
                    if (fieldValue > filterValue)
                        return false;
                    break;

                default:
                    throw new NotImplementedException("Action is not implemented: " + actionType.ToString());
            }

            return true;
        }

        public static bool PassFilterOnField(int fieldValue, FilterActionType actionType, int filterValue)
        {
            switch (actionType)
            {
                case FilterActionType.equal:
                    if (fieldValue != filterValue)
                        return false;
                    break;

                case FilterActionType.not_equal:
                    if (fieldValue == filterValue)
                        return false;
                    break;

                case FilterActionType.greater:
                    if (fieldValue <= filterValue)
                        return false;
                    break;

                case FilterActionType.greater_or_equal:
                    if (fieldValue < filterValue)
                        return false;
                    break;

                case FilterActionType.less:
                    if (fieldValue >= filterValue)
                        return false;
                    break;

                case FilterActionType.less_or_equal:
                    if (fieldValue > filterValue)
                        return false;
                    break;

                default:
                    throw new NotImplementedException("Action is not implemented: " + actionType.ToString());
            }

            return true;
        }


        #endregion

        #region ShortString
        public static string ShortString(string text)
        {
            return ShortString(text, 20);
        }
        public static string ShortString(string text, int length)
        {
            string txt = text.Trim();

            if (txt.Length < length) return txt;

            return txt.Substring(0, length);
        }
        #endregion

        public static bool CompareArrays(byte[] firstArray, byte[] secondArray)
        {
            if (firstArray.Length != secondArray.Length)
                return false;

            for (int i = 0; i < firstArray.Length; i++)
                if (firstArray[i] != secondArray[i])
                    return false;

            return true;
        }

    }
}
