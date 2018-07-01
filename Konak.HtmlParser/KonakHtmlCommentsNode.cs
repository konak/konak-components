using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.HtmlParser
{
    /// <summary>
    /// Comments node of HTML document
    /// </summary>
    public class KonakHtmlCommentsNode : KonakHtmlPlainTextNode
    {
        #region public properties
        /// <summary>
        /// Returns HTML data of current element
        /// </summary>
        public override string OuterHtml { get { return "<!-- " + _text + " -->"; } }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of KonakHtmlCommentsNode class
        /// </summary>
        public KonakHtmlCommentsNode() : base() { }

        /// <summary>
        /// Initializes a new instance of KonakHtmlCommentsNode class with provided parent tag node
        /// </summary>
        /// <param name="parent"></param>
        public KonakHtmlCommentsNode(KonakHtmlTagNode parent) : base(parent) { }
        #endregion

        #region method
        /// <summary>
        /// Set text data of html comment node
        /// </summary>
        /// <param name="text">text data of comment</param>
        /// <returns>Returns reference to current instance</returns>
        public new KonakHtmlCommentsNode SetText(string text)
        {
            base.SetText(text);

            return this;
        }
        #endregion
    }
}
