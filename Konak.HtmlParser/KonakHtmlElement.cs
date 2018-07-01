using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// done

namespace Konak.HtmlParser
{
    /// <summary>
    /// The abstract base class for all HTML elements of HTML document
    /// </summary>
    public abstract class KonakHtmlElement
    {
        #region public properties
        /// <summary>
        /// Returns text only data of the current element and its subnodes
        /// </summary>
        public abstract string InnerText { get; }

        /// <summary>
        /// Returns inner HTML data of current element
        /// </summary>
        public abstract string InnerHtml { get; }

        /// <summary>
        /// Returns HTML data of current element
        /// </summary>
        public abstract string OuterHtml { get; }

        /// <summary>
        /// The parent of this element
        /// </summary>
        public KonakHtmlElement Parent { get; protected set; }

        /// <summary>
        /// The main document element or null if there is none
        /// </summary>
        public KonakHtmlDocument Document { get; private set; }

        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of KonakHtmlNode class
        /// </summary>
        public KonakHtmlElement()
        {
            Parent = null;
            Document = null;
        }

        /// <summary>
        /// Initializes a new instance of KonakHtmlNode class with provided parent tag node
        /// </summary>
        /// <param name="parent">Parent node of this item</param>
        public KonakHtmlElement(KonakHtmlTagNode parent):this()
        {
            SetParent(parent);
        }
        #endregion

        #region methods
        /// <summary>
        /// internal method to set parent node of the current item
        /// </summary>
        /// <param name="parent"></param>
        /// <returns>Returns the current KonakHtmlElement item for further usage</returns>
        internal KonakHtmlElement SetParent(KonakHtmlElement parent)
        {
            Parent = parent;

            if (parent != null)
                Document = parent.Document;

            return this;
        }

        public void Remove()
        {
            if (Parent == null) return;

            KonakHtmlTagNode tagNode = (KonakHtmlTagNode)Parent;

            tagNode.Remove(this);
        }
        #endregion
    }
}
