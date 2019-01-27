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
        public static async Task<List<T>> GetInstanceList<T>(this SqlDataReader dr)
        {
            List<T> res = new List<T>();

            while(await dr.ReadAsync())
                res.Add(GetInstance<T>(dr));

            return res;
        }

        public static T GetInstance<T>(this SqlDataReader dr)
        {
            PropertyInfo[] propertyInfos = TypeCache.GetProperties(typeof(T));

            T item = Activator.CreateInstance<T>();

            HashSet<string> columnsHashSet = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            for (int i = 0; i < dr.FieldCount; i++)
                columnsHashSet.Add(dr.GetName(i));

            foreach (PropertyInfo prop in propertyInfos)
            {
                if (columnsHashSet.Contains(prop.Name))
                {
                    object val = dr[prop.Name];

                    if (val != DBNull.Value)
                    {
                        Type convertTo = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        prop.SetValue(item, Convert.ChangeType(val, convertTo), null);
                    }
                }
            }

            return item;
        }

    }


}
