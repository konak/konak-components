using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.HtmlParser
{
    /// <summary>
    /// Comments node of style HTML node
    /// </summary>
    public class KonakHtmlStyleCommentNode : KonakHtmlPlainTextNode
    {
        #region public properties
        /// <summary>
        /// Returns HTML data of current element.
        /// </summary>
        public override string OuterHtml { get { return "/*" + _text + "*/"; } }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of KonakHtmlStyleCommentNode class
        /// </summary>
        public KonakHtmlStyleCommentNode() : base() { }

        /// <summary>
        /// Initializes a new instance of KonakHtmlStyleCommentNode class with provided parent html tag node
        /// </summary>
        /// <param name="parent"></param>
        public KonakHtmlStyleCommentNode(KonakHtmlTagNode parent) : base(parent) { }
        #endregion

        #region public methods
        /// <summary>
        /// Setts text data of plain text node
        /// </summary>
        /// <param name="text">text data to be set</param>
        /// <returns>returns current instance of KonakHtmlStyleCommentNode class</returns>
        public new KonakHtmlStyleCommentNode SetText(string text)
        {
            _text = text;

            return this;
        }
        #endregion

    }
}
