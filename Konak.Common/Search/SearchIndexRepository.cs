using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Konak.Common.Caching;

namespace Konak.Common.Search
{
    public class SearchIndexRepository : Cache<SearchIndexKeyBase, ISearchIndexRepositoryItem>
    {

    }
}
