using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.HtmlParser
{
    public class KonakHtmlScriptTagNode : KonakHtmlTagNodePaired
    {
        public override string InnerText { get { return string.Empty; } }

        /// <summary>
        /// Creates new instance of KonakHtmlScriptTagNode class with provided name
        /// </summary>
        /// <param name="name">Name of the tag</param>
        public KonakHtmlScriptTagNode(string name) : base(name) { }

        /// <summary>
        /// Creates new instance of KonakHtmlScriptTagNode class with provided name
        /// </summary>
        /// <param name="name">Name of the tag</param>
        /// <param name="parent">The parrent tag node of the current instance of the tag node</param>
        public KonakHtmlScriptTagNode(string name, KonakHtmlTagNode parent) : base(name, parent) { }
    }
}
