using System;
using System.Collections.Generic;

namespace Konak.Common.Caching
{
    /// <summary>
    /// Implementation of data caching for applications
    /// </summary>
    /// <remarks>
    /// Inplemented repository of cached data can be used on server and on client side.
    /// </remarks>
    public sealed class CacheRepository
    {
        #region Private properties
        private object _lockRepository = new object();
        #endregion

        #region Properties
        public SortedList<string, ICache> Repository { get; private set; }
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor to init base properties
        /// </summary>
        public CacheRepository()
        {
            this.Repository = new SortedList<string, ICache>();
        }
        #endregion

        #region Methods

        #region AddRepository method
        /// <summary>
        /// Add a chache to repository with provided name and type
        /// </summary>
        /// <typeparam name="T">Type of items in new cache</typeparam>
        /// <param name="name">Name of the cache</param>
        /// <returns>Created new repository</returns>
        /// <seealso cref="Konak.Common.Caching.Cache"/>
        public TCache AddRepository<TCache, TKey, TValue>(string name) where TCache : Cache<TKey, TValue>
        {

            //TCache cache = (TCache)Activator.CreateInstance(typeof(TCache), new object[] { });
            TCache cache = Activator.CreateInstance<TCache>();

            lock (this._lockRepository)
                this.Repository.Add(name, cache);

            return cache;
        }
        #endregion

        #region RemoveRepository method
        /// <summary>
        /// Remove a cache with provided name from the repository and return it to caller
        /// </summary>
        /// <param name="name">Name of the cache in repository</param>
        /// <returns>Returns a cache if it is found in repository, otherwise null will be returned</returns>
        public ICache RemoveRepository(string name)
        {
            ICache cache;

            lock (this._lockRepository)
                this.Repository.TryGetValue(name, out cache);

            return cache;
        }
        #endregion

        #endregion
    }
}
