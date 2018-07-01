using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.HtmlParser
{
    /// <summary>
    /// Class for instances of unpaired tag node
    /// </summary>
    public class KonakHtmlTagNodeUnPaired : KonakHtmlTagNode
    {
        #region properties
        /// <summary>
        /// Returns text only data of the current node. For unpaired nodes it is always empty string
        /// </summary>
        public override string InnerText { get { return string.Empty; } }

        /// <summary>
        /// Returns inner HTML data of current node. For unpaired nodes it is always empty string
        /// </summary>
        public override string InnerHtml { get { return string.Empty; } }

        /// <summary>
        /// Returns HTML data of current node.
        /// </summary>
        public override string OuterHtml
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append('<').Append(Name);

                if (Attributes.Count > 0)
                    sb.Append(' ').Append(Attributes.ToString());

                sb.Append(" />");

                return sb.ToString();
            }
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of KonakHtmlTagNodeUnPaired class with provided tag name
        /// </summary>
        /// <param name="name">Name of the tag node</param>
        public KonakHtmlTagNodeUnPaired(string name) : this(name, null) { }
        /// <summary>
        /// Initializes a new instance of KonakHtmlTagNodeUnPaired class with provided tag name and parent tag node
        /// </summary>
        /// <param name="name">Name of the tag node</param>
        /// <param name="parent">The parent of this node</param>
        public KonakHtmlTagNodeUnPaired(string name, KonakHtmlTagNode parent) : base(name, parent) { }
        #endregion
    }
}
