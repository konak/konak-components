using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.HtmlParser
{
    /// <summary>
    /// Class for service tag nodes like DOCTYPE or xml
    /// </summary>
    public class KonakHtmlServiceNode : KonakHtmlElement
    {
        private string _outerHtml;

        #region public properties
        /// <summary>
        /// Name of the sercice tag
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Returns text only data of the current node.
        /// </summary>
        public override string InnerText { get { return string.Empty; } }

        /// <summary>
        /// Returns inner HTML data of current element
        /// </summary>
        public override string InnerHtml { get { return string.Empty; } }

        /// <summary>
        /// Returns HTML data of current element
        /// </summary>
        public override string OuterHtml { get { return _outerHtml; } }
        #endregion

        #region constructor
        /// <summary>
        /// Initializes a new instance of KonakHtmlServiceNode class with provided text data
        /// </summary>
        /// <param name="nodeText">text of the node</param>
        public KonakHtmlServiceNode(string nodeText) : this(nodeText, null) { }

        /// <summary>
        /// Initializes a new instance of KonakHtmlServiceNode class with provided text data and parent tag node
        /// </summary>
        /// <param name="nodeText">Text of the node</param>
        /// <param name="parent">Parent HTML element</param>
        public KonakHtmlServiceNode(string nodeText, KonakHtmlTagNode parent) : base(parent)
        {
            int spaceIndex = nodeText.IndexOf(' ');
            Name = nodeText.Substring(2, spaceIndex - 2);
            _outerHtml = nodeText;
        }
        #endregion
    }
}
