using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Common.Search
{
    public class FilterSet : IEnumerable<IFilter>
    {
        private string _toString;

        protected List<IFilter> _filters;

        public FilterSet(List<IFilter> filters)
        {
            this._filters = filters;

            StringBuilder sb = new StringBuilder();

            foreach (IFilter flt in this._filters)
                sb.Append('_').Append(flt.ToString());

            _toString = _filters.Count == 0 ? "_" : sb.Remove(0, 1).ToString();
        }

        public IEnumerator<IFilter> GetEnumerator()
        {
            return _filters.GetEnumerator();
        }

        public override string ToString()
        {
            return _toString;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _filters.GetEnumerator();
        }
    }
}
