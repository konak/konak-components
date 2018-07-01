using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//done

namespace Konak.HtmlParser
{
    /// <summary>
    /// Class for plain text nodes. Text in this nodes are saved as is without any encoding or decoding.
    /// this kind of nodes are used for text in comments, text of style and script tags
    /// </summary>
    public class KonakHtmlPlainTextNode : KonakHtmlElement
    {
        #region properties
        /// <summary>
        /// internal property storing text data of the node
        /// </summary>
        protected string _text;

        /// <summary>
        /// Returns text data of the current node
        /// </summary>
        public override string InnerText { get { return _text; } }

        /// <summary>
        /// Returns inner HTML data of current element.
        /// </summary>
        public override string InnerHtml { get { return _text; } }

        /// <summary>
        /// Returns HTML data of current element.
        /// </summary>
        public override string OuterHtml { get { return _text; } }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of KonakHtmlPlainTextNode class
        /// </summary>
        public KonakHtmlPlainTextNode() : base()
        {
            InitProperties();
        }

        /// <summary>
        /// Initializes a new instance of KonakHtmlPlainTextNode class with provided parent html tag
        /// </summary>
        /// <param name="parent">Parent element of current node</param>
        public KonakHtmlPlainTextNode(KonakHtmlTagNode parent) : base(parent)
        {
            InitProperties();
        }
        #endregion

        #region methods
        /// <summary>
        /// Initializes properties
        /// </summary>
        private void InitProperties()
        {
            _text = string.Empty;
        }

        /// <summary>
        /// Setts text data of plain text node
        /// </summary>
        /// <param name="text">text data to be set</param>
        /// <returns>returns current instance of KonakHtmlPlainTextNode class</returns>
        public virtual KonakHtmlPlainTextNode SetText(string text)
        {
            _text = text;
            return this;
        }
        #endregion

    }
}
