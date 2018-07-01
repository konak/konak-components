using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Konak.HtmlParser
{
    /// <summary>
    /// Text node of HTML document
    /// </summary>
    public class KonakHtmlTextNode : KonakHtmlPlainTextNode
    {
        #region overriden properties
        /// <summary>
        /// Returns decoded text data of the current node
        /// </summary>
        public override string InnerText { get { return HttpUtility.HtmlDecode(_text); } }

        /// <summary>
        /// Returns HTML code of inner elements.
        /// </summary>
        public override string InnerHtml { get { return _text; } }

        /// <summary>
        /// Returns HTML code of current element.
        /// </summary>
        public override string OuterHtml { get { return _text; } }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of KonakHtmlTextNode class
        /// </summary>
        public KonakHtmlTextNode() : base() { }

        /// <summary>
        /// Initializes a new instance of KonakHtmlTextNode class with provided parent tag node
        /// </summary>
        /// <param name="parent">Parent element of current node</param>
        public KonakHtmlTextNode(KonakHtmlTagNode parent) : base(parent) { }
        #endregion

        #region public methods
        /// <summary>
        /// Set text data of node
        /// </summary>
        /// <param name="text">Text to be set</param>
        /// <returns>Returns current KonakHtmlTextNode node item for further use</returns>
        public new KonakHtmlTextNode SetText(string text)
        {
            _text = text;
            return this;
        }
        #endregion
    }
}
