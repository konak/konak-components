using Konak.Dac.Cache;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace Konak.Dac.Extensions
{
    public static class DacSqlParametersExtensions
    {
        /// <summary>
        /// Extension methid to transform <see cref="DacSqlParameters"/> instance to an array of <see cref="SqlParameter"/>
        /// </summary>
        /// <param name="parameters">instance the extension is applyed to</param>
        /// <returns></returns>
        public static SqlParameter[] ToSqlParameters(this DacSqlParameters parameters)
        {
            return ToSqlParameters(parameters.ToArray());
        }

        /// <summary>
        /// Extension methid to transform <see cref="List<KeyValuePair<string, object>>"/> instance to an array of <see cref="SqlParameter"/>
        /// </summary>
        /// <param name="parameters">instance the extension is applyed to</param>
        /// <returns></returns>
        public static SqlParameter[] ToSqlParameters(this List<KeyValuePair<string, object>> parameters)
        {
            return ToSqlParameters(parameters.ToArray());
        }

        /// <summary>
        /// Extension methid to transform <see cref="KeyValuePair<string, object>[]"/> instance to an array of <see cref="SqlParameter"/>
        /// </summary>
        /// <param name="parameters">instance the extension is applyed to</param>
        /// <returns></returns>
        public static SqlParameter[] ToSqlParameters(this KeyValuePair<string, object>[] parameters)
        {
            SqlParameter[] sqlParameters = null;
            int len = parameters.Length;

            if (parameters != null && len > 0)
            {
                sqlParameters = new SqlParameter[len];

                KeyValuePair<string, Object> param;

                for (int i = 0; i < len; i++)
                {
                    param = parameters[i];
                    sqlParameters[i] = new SqlParameter(param.Key, param.Value == null ? DBNull.Value : param.Value);
                }
            }

            return sqlParameters;
        }

        /// <summary>
        /// Extension methid to transform <see cref="dynamic"/> instance to an array of <see cref="SqlParameter"/>
        /// </summary>
        /// <param name="parameters">instance the extension is applyed to</param>
        /// <returns></returns>
        public static SqlParameter[] ToSqlParameters(dynamic parameters)
        {
            SqlParameter[] sqlParameters = null;

            if (parameters != null)
            {
                PropertyInfo[] properties = parameters.GetType().GetProperties();

                sqlParameters = new SqlParameter[properties.Length];

                PropertyInfo propInfo;

                for (int i = 0, len = properties.Length; i < len; i++)
                {
                    propInfo = properties[i];
                    object val = propInfo.GetValue(parameters);
                    sqlParameters[i] = new SqlParameter(propInfo.Name, val == null ? DBNull.Value : val);
                }
            }

            return sqlParameters;
        }

        /// <summary>
        /// Extension methid to transform generic type instance to an array of <see cref="SqlParameter"/>
        /// </summary>
        /// <typeparam name="T">Generic  type of the instance</typeparam>
        /// <param name="parameters">item containing properties interpreted as parameters</param>
        /// <returns></returns>
        public static SqlParameter[] ToSqlParameters<T>(this T parameters)
        {
            SqlParameter[] sqlParameters = null;

            if (parameters != null)
            {
                PropertyInfo[] properties = TypeCache.GetProperties(typeof(T));

                sqlParameters = new SqlParameter[properties.Length];

                PropertyInfo propInfo;

                for (int i = 0, len = properties.Length; i < len; i++)
                {
                    propInfo = properties[i];
                    object val = propInfo.GetValue(parameters);
                    sqlParameters[i] = new SqlParameter(propInfo.Name, val == null ? DBNull.Value : val);
                }
            }

            return sqlParameters;
        }
    }
}
