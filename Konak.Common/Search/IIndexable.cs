using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Common.Search
{
    /// <summary>
    /// Interface applyed to items of cache that can be searched, sorted and must be indexed in the system
    /// </summary>
    public interface IIndexable<TKey>
    {
        /// <summary>
        /// ID of the object
        /// </summary>
        TKey ID { get; }

        bool PassFilters(FilterSet filterSet);

        string GetKey(string[] orderingFields);
    }
}
