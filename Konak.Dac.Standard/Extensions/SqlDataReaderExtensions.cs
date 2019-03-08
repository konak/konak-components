using Konak.Dac.Cache;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Dac.Extensions
{
    public static class SqlDataReaderExtensions
    {
        /// <summary>
        /// Get list of instancies by reading them from SqlDataReader records
        /// </summary>
        /// <typeparam name="T">Type of the instance must be read from data reader.</typeparam>
        /// <param name="dr">Data reader, the data must be read from.</param>
        /// <returns>List of items read from data reader.</returns>
        public static List<T> GetInstanceList<T>(this SqlDataReader dr)
        {
            List<T> res = new List<T>();

            List<PropertyInfo> payloadProperties = GetPayloadProperties<T>(dr);

            while (dr.Read())
                res.Add(GetInstance<T>(dr, payloadProperties));

            return res;
        }

        /// <summary>
        /// Get list of instancies by reading them from SqlDataReader records asynchronously
        /// </summary>
        /// <typeparam name="T">Type of the instance must be read from data reader.</typeparam>
        /// <param name="dr">Data reader, the data must be read from.</param>
        /// <returns>List of items read from data reader.</returns>
        public static async Task<List<T>> GetInstanceListAsync<T>(this SqlDataReader dr)
        {
            List<T> res = new List<T>();

            List<PropertyInfo> payloadProperties = GetPayloadProperties<T>(dr);

            while (await dr.ReadAsync())
                res.Add(GetInstance<T>(dr, payloadProperties));

            return res;
        }


        /// <summary>
        /// Get instance of the item by reading data from current row of datareader
        /// </summary>
        /// <typeparam name="T">Type of the instance must be read from data reader.</typeparam>
        /// <param name="dr">Data reader, the data must be read from.</param>
        /// <param name="propertyInfos">Collection of <see cref="PropertyInfo"/> items describing set of columns that must be read from data reader. In case if propertyInfos is null function will try to detect a set of properties available in data reader for provided type of item.</param>
        /// <returns>Instance of provided type read from data reader</returns>
        public static T GetInstance<T>(this SqlDataReader dr, IEnumerable<PropertyInfo> propertyInfos = null)
        {
            T item = Activator.CreateInstance<T>();

            if(propertyInfos == null)
                propertyInfos = GetPayloadProperties<T>(dr);

            foreach (PropertyInfo prop in propertyInfos)
            {
                object val = dr[prop.Name];

                if (val != DBNull.Value)
                {
                    Type convertTo = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    prop.SetValue(item, Convert.ChangeType(val, convertTo), null);
                }
            }

            return item;
        }

        /// <summary>
        /// Detect properties of provided type that have same name columns in data reader
        /// </summary>
        /// <typeparam name="T">Generic type the list of properties must be detected from.</typeparam>
        /// <param name="dr">Data reader containing columns to look for the property name in.</param>
        /// <returns>List of <see cref="PropertyInfo"/> items detected in data reader.</returns>
        private static List<PropertyInfo> GetPayloadProperties<T>(SqlDataReader dr)
        {
            PropertyInfo[] propertyInfos = TypeCache.GetProperties(typeof(T));
            List<PropertyInfo> res = new List<PropertyInfo>(dr.FieldCount);
            PropertyInfo propertyInfo;

            for (int i = 0; i < dr.FieldCount; i++)
            {
                for (int j = 0; j < propertyInfos.Length; j++)
                {
                    propertyInfo = propertyInfos[j];

                    if (dr.GetName(i).Equals(propertyInfo.Name))
                        res.Add(propertyInfo);
                }
            }

            return res;
        }

    }


}
