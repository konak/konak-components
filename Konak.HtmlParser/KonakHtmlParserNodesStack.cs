using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.HtmlParser
{
    /// <summary>
    /// Stack of tag nodes for parsing process
    /// </summary>
    internal class KonakHtmlParserNodesStack
    {
        private List<KonakHtmlTagNodePaired> _nodes;

        #region constructor
        /// <summary>
        /// Initializes a new instance of KonakHtmlParserNodesStack class
        /// </summary>
        internal KonakHtmlParserNodesStack()
        {
            _nodes = new List<KonakHtmlTagNodePaired>();
        }
        #endregion

        #region internal methods
        /// <summary>
        /// Add paired tag node to the stack
        /// </summary>
        /// <param name="tag">Tag to add to stack</param>
        /// <returns>Returns reference to current instance</returns>
        internal KonakHtmlParserNodesStack Add(KonakHtmlTagNodePaired tag)
        {
            _nodes.Add(tag);
            return this;
        }

        /// <summary>
        /// Try to close tag with provided name of the tag
        /// </summary>
        /// <param name="name">Name of the tag to close</param>
        /// <returns>Returns true if close was successful false if tag was not found</returns>
        internal bool Close(string name)
        {
            KonakHtmlTagNodePaired tag;

            for (int i = _nodes.Count-1; i>=0; i--)
            {
                if ((tag = _nodes[i]).Name.Equals(name))
                {
                    _nodes.RemoveAt(i);
                    return tag.IsClosed = true;
                }
            }

            return false;
        }
        #endregion
    }

}
