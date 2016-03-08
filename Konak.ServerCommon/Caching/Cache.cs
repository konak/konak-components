using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.ServerCommon.Caching
{
    /// <summary>
    /// Server side implementation of Cache derived from <see cref="Konak.Common.Caching.Cache<T>"/> class
    /// </summary>
    /// <typeparam name="TKey">Type of the key of the item</typeparam>
    /// <typeparam name="TValue">Type of the items stored in the cache</typeparam>
    public abstract class Cache<TKey, TValue> : Konak.Common.Caching.Cache<TKey, TValue>, IServerCache
    {
        public Cache() : base() { }
        public abstract void LoadData();
    }
}
