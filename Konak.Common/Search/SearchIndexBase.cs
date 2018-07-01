using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Konak.Common.Caching;

namespace Konak.Common.Search
{
    public abstract class SearchIndexBase<TKey, TValue> : Cache<string, TValue>, ISearchIndex<TKey, TValue>, ISearchIndexRepositoryItem
    where TValue : IIndexable<TKey>
    {
        internal SearchIndexKeyBase Key { get; }

        public SearchIndexBase(SearchIndexKeyBase key)
        {
            Key = key;
        }


        public void ManageItemAddedOrUpdated(object source, TKey key, TValue value)
        {
            AddIndexItem((TValue)value);
        }

        public void ManageItemRemoved(object source, TKey key, TValue value)
        {
            RemoveIndexItem((TValue)value);
        }

        private TValue RemoveIndexItem(TValue item)
        {
            return this.Remove(item.GetKey(Key.OrderingFields));
        }

        private TValue AddIndexItem(TValue item)
        {
            if (item.PassFilters(Key.Filters))
                return this[item.GetKey(Key.OrderingFields)] = item;

            return default(TValue);
        }

        public void Init(IEnumerable<TValue> source)
        {
            foreach (TValue itm in source)
                AddIndexItem(itm);
        }
    }
}
