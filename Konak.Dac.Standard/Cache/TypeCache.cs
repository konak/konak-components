using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Konak.Dac.Cache
{
    internal static class TypeCache
    {
        private static ConcurrentDictionary<Type, PropertyInfo[]> _typeCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

        internal static PropertyInfo[] GetProperties(Type type)
        {
            if (_typeCache.TryGetValue(type, out PropertyInfo[] value))
                return value;

            PropertyInfo[] propertyInfos = type.GetProperties();

            _typeCache.TryAdd(type, propertyInfos);

            return propertyInfos;
        }
    }
}
