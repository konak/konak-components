using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Common.Search
{
    public enum SortDirection
    {
        asc = 1,
        desc = 2
    }

    public interface ISearchIndex<TValue>
    {
        void ManageItemAddedOrUpdated(object source, Guid key, TValue value);

        void ManageItemRemoved(object source, Guid key, TValue value);

        void Init(IEnumerable<TValue> source);

        List<TValue> GetSubset(int pageNumber, int pageLength, out int pagesCount, SortDirection direction);
    }
}
