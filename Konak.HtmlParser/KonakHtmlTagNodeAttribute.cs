using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.HtmlParser
{
    /// <summary>
    /// HTML tag node attribute
    /// </summary>
    public class KonakHtmlTagNodeAttribute
    {
        #region public properties
        /// <summary>
        /// Name of the attribute
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value of the attribute
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Quote character of the attribute value is included in.
        /// </summary>
        public char QuoteChar { get; set; }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of KonakHtmlTagNodeAttribute class with provided name, value and quoteChar values
        /// </summary>
        /// <param name="name">Name of the attribute</param>
        /// <param name="value">Value of the attribute</param>
        /// <param name="quoteChar">Quote character of the attribute value is included in.</param>
        public KonakHtmlTagNodeAttribute(string name, string value, char quoteChar) : this(name, value)
        {
            QuoteChar = quoteChar;
        }

        /// <summary>
        /// Initializes a new instance of KonakHtmlTagNodeAttribute class with provided name and value
        /// </summary>
        /// <param name="name">Name of the attribute</param>
        /// <param name="value">Value of the attribute</param>
        public KonakHtmlTagNodeAttribute(string name, string value) : this(name)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of KonakHtmlTagNodeAttribute class with provided name
        /// </summary>
        /// <param name="name">Name of the attribute</param>
        public KonakHtmlTagNodeAttribute(string name) : this()
        {
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of KonakHtmlTagNodeAttribute class
        /// </summary>
        public KonakHtmlTagNodeAttribute()
        {
            QuoteChar = '\"';
        }
        #endregion

        #region public method
        /// <summary>
        /// Return the overriden string representation of the current object
        /// </summary>
        /// <returns>Returns string representation of the current object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name);
            if (!string.IsNullOrEmpty(Value))
                sb.Append('=').Append(QuoteChar).Append(Value).Append(QuoteChar);

            return sb.ToString();
        }
        #endregion
    }
}
