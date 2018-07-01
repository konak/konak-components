using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.HtmlParser
{
    /// <summary>
    /// Comments node of script tag
    /// </summary>
    public class KonakHtmlScriptCommentNode : KonakHtmlPlainTextNode
    {
        private string _openTag;
        private string _closeTag;

        #region public properties
        public override string OuterHtml { get { return _openTag + _text + _closeTag; } }
        #endregion

        #region
        /// <summary>
        /// Initializes a new instance of KonakHtmlScriptCommentNode
        /// </summary>
        public KonakHtmlScriptCommentNode() : this(null) { }

        /// <summary>
        /// Initializes a new instance of KonakHtmlScriptCommentNode with provided parent tag node
        /// </summary>
        /// <param name="parent">The parent of the current node</param>
        public KonakHtmlScriptCommentNode(KonakHtmlTagNode parent) : base(parent)
        {
            _openTag = "/*";
            _closeTag = "*/";
        }
        #endregion

        #region public methods
        /// <summary>
        /// Set text data of script comment node
        /// </summary>
        /// <param name="text">Text data of script comment</param>
        /// <returns>Returns current instance for further use</returns>
        public new KonakHtmlScriptCommentNode SetText(string text)
        {
            _openTag = text.Substring(0, 2);

            if (_openTag[1].Equals('/'))
                _closeTag = "\r\n";
            else
                _closeTag = "*/";

            _text = text.Substring(2);

            return this;
        }
        #endregion

    }
}
