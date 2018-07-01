using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.HtmlParser
{
    /// <summary>
    /// Html attributes collection of tag node
    /// </summary>
    public class KonakHtmlTagNodeAttributesCollection: IEnumerable<KonakHtmlTagNodeAttribute>
    {
        #region private properties
        private SortedList<string, KonakHtmlTagNodeAttribute> _attributes;
        #endregion

        #region public properties
        /// <summary>
        /// Get tag node attribute by it's name
        /// </summary>
        /// <param name="key">Name of the attribute</param>
        /// <returns></returns>
        public KonakHtmlTagNodeAttribute this[string key]
        {
            get
            {
                KonakHtmlTagNodeAttribute res;

                if (_attributes.TryGetValue(key, out res))
                    return res;

                return null;
            }
            set
            {
                _attributes[key] = value;
            }
        }

        /// <summary>
        /// Returns count of the attributes in collection
        /// </summary>
        public int Count { get { return _attributes.Count; } }
        #endregion

        #region constructor
        /// <summary>
        /// Initializes a new instance of KonakHtmlTagNodeAttributesCollection class
        /// </summary>
        public KonakHtmlTagNodeAttributesCollection()
        {
            _attributes = new SortedList<string, KonakHtmlTagNodeAttribute>();
        }
        #endregion

        #region public methods

        #region IEnumerable interface methods implementation
        public IEnumerator<KonakHtmlTagNodeAttribute> GetEnumerator()
        {
            return _attributes.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _attributes.GetEnumerator();
        }
        #endregion


        /// <summary>
        /// Indicates if the collection contains provided attribute
        /// </summary>
        /// <param name="name">Name of the attribute</param>
        /// <returns>True if the attribute exist in collection, otherway false.</returns>
        public bool Contains(string name)
        {
            return _attributes.Keys.Contains(name);
        }

        /// <summary>
        /// Add provided attribute to collection
        /// </summary>
        /// <param name="attribute">Attribute to add to collection</param>
        /// <returns>returns current tag node attributes collection</returns>
        public KonakHtmlTagNodeAttributesCollection Add(KonakHtmlTagNodeAttribute attribute)
        {
            if (!_attributes.Keys.Contains(attribute.Name))
                _attributes.Add(attribute.Name, attribute);
            
            return this;
        }

        /// <summary>
        /// Set attribute value
        /// </summary>
        /// <param name="name">Name of the attribute</param>
        /// <param name="value">Value of the attribute</param>
        /// <returns>Returns reference to the current instance</returns>
        public KonakHtmlTagNodeAttributesCollection SetValue(string name, string value)
        {
            if(_attributes.ContainsKey(name))
                _attributes[name].Value = value;
            else
                _attributes.Add(name, new KonakHtmlTagNodeAttribute(name, value, '\"'));

            return this;
        }

        /// <summary>
        /// Return the overriden string representation of the current object
        /// </summary>
        /// <returns>Returns string representation of the current object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach(KonakHtmlTagNodeAttribute attr in _attributes.Values)
            {
                sb.Append(attr.Name);

                if (!string.IsNullOrEmpty(attr.Value))
                    sb.Append("=\"").Append(attr.Value).Append('\"');

                sb.Append(' ');
            }

            return sb.ToString().Trim();
        }
        #endregion
    }
}
