using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Konak.Common.Caching;

namespace Konak.Common.Search
{
    public abstract class SearchIndexBase<TValue> : Cache<string, TValue>, ISearchIndex<TValue>, ISearchIndexRepositoryItem
    where TValue : IIndexable
    {
        internal SearchIndexKeyBase Key { get; }

        public SearchIndexBase(SearchIndexKeyBase key)
        {
            Key = key;
        }


        public void ManageItemAddedOrUpdated(object source, Guid key, TValue value)
        {
            AddIndexItem((TValue)value);
        }

        public void ManageItemRemoved(object source, Guid key, TValue value)
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
