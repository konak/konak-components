using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Common.Search
{
    public enum FilterActionType
    {
        equal,
        not_equal,
        contain,
        not_contain,
        greater,
        greater_or_equal,
        less,
        less_or_equal
    }

    public interface IFilter
    {
        FilterActionType ActionType { get; }
        object Value { get; }
        string FieldName { get; }
    }

    public class Filter<T> : IFilter
    {
        public FilterActionType ActionType { get; }

        public T Value { get; }
        public string FieldName { get; }

        object IFilter.Value
        {
            get
            {
                return Value;
            }
        }

        private string _toString;

        public Filter(FilterActionType actionType, string fieldName, T value)
        {
            ActionType = actionType;
            FieldName = fieldName;
            Value = value;

            string valueString;

            if(value.GetType().IsArray)
            {
                StringBuilder sb = new StringBuilder();

                foreach(var item in (IEnumerable)value)
                {
                    sb.Append('_').Append(item.ToString());
                }

                valueString = sb.ToString();
            }
            else
            {
                valueString = "_" + value.ToString();
            }

            _toString = fieldName + "_" + ((int)actionType).ToString() + valueString;
        }

        public override string ToString()
        {
            return _toString;
        }
    }
}
