using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Konak.Common.Helpers;
using Konak.Common.Search;

namespace Konak.Common.Entity
{
    public class User : GenericItem, IIndexable
    {
        #region IIndexable interface methods implementation
        public virtual string GetKey(string[] orderingFields)
        {
            StringBuilder sb = new StringBuilder(orderingFields == null || orderingFields.Length == 0 ? "_" : string.Empty);

            foreach(string fieldName in orderingFields)
            {
                switch(fieldName.ToUpper())
                {
                    case "NAME":
                        sb.Append("_").Append(Name);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(fieldName, "Unknown field name passed.");
                }
            }

            sb.Append("_").Append(RID);

            return sb.Remove(0, 1).ToString();
        }

        public bool PassFilters(FilterSet filterSet)
        {
            foreach(IFilter filter in filterSet)
            {
                switch(filter.FieldName.ToUpper())
                {
                    case "NAME":
                        if (!CH.PassFilterOnField(Name, filter.ActionType, (string)filter.Value))
                            return false;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(filter.FieldName, "Unknown field name.");
                }
            }

            return true;
        }
        #endregion

    }
}
