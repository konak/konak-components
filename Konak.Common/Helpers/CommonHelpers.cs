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

        public static bool PassFilterOnField(int fieldValue, FilterActionType actionType, int[] filterValue)
        {
            switch (actionType)
            {
                case FilterActionType.equal:
                    foreach (int val in filterValue)
                        if (fieldValue == val)
                            return true;

                    return false;

                case FilterActionType.not_equal:
                    foreach (int val in filterValue)
                        if (fieldValue == val)
                            return false;
                    break;

                case FilterActionType.greater:
                    foreach (int val in filterValue)
                        if (fieldValue <= val)
                            return false;
                    break;

                case FilterActionType.greater_or_equal:
                    foreach (int val in filterValue)
                        if (fieldValue < val)
                            return false;
                    break;

                case FilterActionType.less:
                    foreach (int val in filterValue)
                        if (fieldValue >= val)
                            return false;
                    break;

                case FilterActionType.less_or_equal:
                    foreach (int val in filterValue)
                        if (fieldValue > val)
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
    }
}
