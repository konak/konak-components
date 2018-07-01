using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.HtmlParser
{
    /// <summary>
    /// Class for instances of paired tag nodes
    /// </summary>
    public class KonakHtmlTagNodePaired : KonakHtmlTagNode
    {
        #region properties
        /// <summary>
        /// Property indicating that closed tag is found
        /// </summary>
        internal bool IsClosed { get; set; }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of KonakHtmlTagNodePaired class with provided tag name
        /// </summary>
        /// <param name="name">Name of tag node</param>
        public KonakHtmlTagNodePaired(string name) : base(name)
        {
            IsClosed = false;
        }

        /// <summary>
        /// Initializes a new instance of KonakHtmlTagNodePaired class with provided tag name and parent tag node
        /// </summary>
        /// <param name="name">Name of tag node</param>
        /// <param name="parent">The parrent of the current item</param>
        public KonakHtmlTagNodePaired(string name, KonakHtmlTagNode parent) : base(name, parent)
        {
            IsClosed = false;
        }
        #endregion
    }
}
