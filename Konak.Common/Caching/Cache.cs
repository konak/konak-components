using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Konak.Common.Search;

namespace Konak.Common.Caching
{
    /// <summary>
    /// A base class for cache list for provided type of objects. Implemented as proxy class for <see cref="System.Collections.Generic.SortedList"/>
    /// </summary>
    /// <remarks>
    /// On server side applications <see cref="Konak.ServerCommon.Caching.Cache<T>"/>
    /// class should be used for caching where data loading from the database must be implemented.
    /// On client side applications <see cref="Konak.ClientCommon.Caching.Cache<T>"/>
    /// class should be used for caching, where data loading from the server must be implemented.
    /// </remarks>
    /// <typeparam name="TKey">Type of the key of the item</typeparam>
    /// <typeparam name="TValue">Type of the objects that will be stored in the cache</typeparam>
    abstract public class Cache<TKey, TValue> : ICache, System.Collections.IEnumerable
    {

        public delegate void CacheItemAddedOrUpdatedDelegate(Cache<TKey, TValue> source, TKey key, TValue value);
        public delegate void CacheItemRemovedDelegate(Cache<TKey, TValue> source, TKey key, TValue value);

        /// <summary>
        /// Event fired on adding or updating an item in the cache
        /// </summary>
        public event CacheItemAddedOrUpdatedDelegate CacheItemAddedOrUpdatedEvent;

        /// <summary>
        /// Event fired on removing an item from the cache
        /// </summary>
        public event CacheItemRemovedDelegate CacheItemRemovedEvent;

        /// <summary>
        /// private repository of items
        /// </summary>
        private SortedList<TKey, TValue> _cache;

        /// <summary>
        /// Syncronisation object for concurent threads
        /// </summary>
        public object SyncRoot { get; private set; }

        /// <summary>
        /// Get the count of items in the cache
        /// </summary>
        public int Count { get { return this._cache.Count; } }


        public Cache()
        {
            this.SyncRoot = new object();
            this._cache = new SortedList<TKey, TValue>();
        }


        /// <summary>
        /// Method to fire event when an item was removed from the cache
        /// </summary>
        /// <param name="key">key of the added item</param>
        /// <param name="value">an item added to the cache</param>
        private void RaiseCacheItemRemovedEvent(TKey key, TValue value)
        {
            CacheItemRemovedDelegate evt = CacheItemRemovedEvent;

            if (evt == null) return;

            foreach (Delegate d in evt.GetInvocationList())
            {
                try
                {
                    d.DynamicInvoke(this, key, value);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }
        }


        /// <summary>
        /// Method to fire event when new item is added to cache
        /// </summary>
        /// <param name="key">key of the added item</param>
        /// <param name="value">an item added to the cache</param>
        private void RaiseCacheItemAddedOrUpdatedEvent(TKey key, TValue value)
        {
            CacheItemAddedOrUpdatedDelegate evt = CacheItemAddedOrUpdatedEvent;

            if (evt == null) return;

            foreach (Delegate d in evt.GetInvocationList())
            {
                try
                {
                    d.DynamicInvoke(this, key, value);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Threadsafe implementation of get / set / update item in the cache
        /// </summary>
        /// <param name="key">Key of the item to be got / seted or updated</param>
        /// <returns>item from the cache</returns>
        public TValue this[TKey key]
        {
            get
            {
                TValue res;

                lock (this.SyncRoot)
                {
                    if (this._cache.TryGetValue(key, out res))
                        return res;

                    return default(TValue);
                }
            }

            set
            {
                lock (this.SyncRoot)
                {
                    this._cache[key] = value;
                }

                RaiseCacheItemAddedOrUpdatedEvent(key, value);
            }
        }

        public TValue Remove(TKey key)
        {
            TValue res;

            bool isRemoved = false;

            lock (this.SyncRoot)
            {
                if (!this._cache.TryGetValue(key, out res))
                {
                    return default(TValue);
                }

                isRemoved = this._cache.Remove(key);
            }

            if(isRemoved)
                RaiseCacheItemRemovedEvent(key, res);

            return res;
        }

        /// <summary>
        /// Get subset of items from the cache
        /// </summary>
        /// <param name="page">The page of the items to get</param>
        /// <param name="pageLength">Number of items in one page</param>
        /// <returns>List of items from cahe</returns>
        public List<TValue> GetSubset(int page, int pageLength, out int pagesCount, SortDirection direction)
        {
            int startPosition = page * pageLength;
            int lastPosition = startPosition + pageLength;

            if(direction == SortDirection.desc)
            {
                startPosition = this.Count - startPosition;
                lastPosition = this.Count - lastPosition;
            }

            List<TValue> res = new List<TValue>();


            if (direction == SortDirection.asc)
            {
                lock (this.SyncRoot)
                {
                    if (lastPosition > this.Count)
                        lastPosition = this.Count;

                    for (int i = startPosition; i < lastPosition; i++)
                        res.Add(this[this._cache.Keys[i]]);
                }
            }
            else
            {
                lock (this.SyncRoot)
                {
                    if (lastPosition < 0)
                        lastPosition = 0;

                    for (int i = startPosition - 1; i >= lastPosition; i--)
                        res.Add(this[this._cache.Keys[i]]);
                }
            }
            

            int cnt = this.Count;

            pagesCount = cnt % pageLength == 0 ? cnt / pageLength : cnt / pageLength + 1;

            return res;
        }

        public TValue[] GetValues()
        {
            lock (this.SyncRoot)
            {
                return this._cache.Values.ToArray<TValue>();
            }
        }


        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._cache.GetEnumerator();
        }

        /// <summary>
        /// Get the value item from cache by it's index in values collection. <b>NOTE</b> index of the item in values collection is not the same index of it's key in keys collection.
        /// </summary>
        /// <param name="index">index of item in values collection</param>
        /// <returns></returns>
        public TValue GetValueByIndex(int index)
        {
            lock (SyncRoot)
                return _cache.Values[index];
        }

        /// <summary>
        /// Get the key of the item from cache by it's index in keys collection. <b>NOTE</b> index of the key in keys collection is not the same index of the item in values collection.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TKey GetKeyByIndex(int index)
        {
            lock (SyncRoot)
                return _cache.Keys[index];
        }
    }
}
