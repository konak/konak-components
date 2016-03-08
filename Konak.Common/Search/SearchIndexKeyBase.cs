using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Common.Search
{
    public abstract class SearchIndexKeyBase : IComparable
    {
        private string _toString;
        private string _langID;
        private int _category;
        private string[] _orderingFields;

        public string LangID { get { return _langID; } }

        public int Category { get { return _category; } }

        public string[] OrderingFields { get { return _orderingFields; } }

        public FilterSet Filters { get; }

        public SearchIndexKeyBase(int category, FilterSet filters, string langID, string[] orderingFields)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(_langID = langID).Append('_');
            sb.Append(_category = category).Append('_');
            sb.Append(string.Join("_", _orderingFields = orderingFields)).Append('_');
            sb.Append((Filters = filters).ToString());

            _toString = sb.ToString();
        }

        public int CompareTo(object obj)
        {
            return _toString.CompareTo(obj.ToString());
        }

        public override string ToString()
        {
            return _toString;
        }
    }
}
