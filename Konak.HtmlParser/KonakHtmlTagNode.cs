using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.HtmlParser
{
    /// <summary>
    /// Abstract class for any HTML tag node implementation
    /// </summary>
    public abstract class KonakHtmlTagNode : KonakHtmlElement
    {
        #region public properties

        /// <summary>
        /// Returns text only data of the current node and its subnodes
        /// </summary>
        public override string InnerText
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (KonakHtmlElement nd in Nodes)
                    sb.Append(nd.InnerText);

                return sb.ToString();
            }
        }

        /// <summary>
        /// Returns inner HTML data of current element
        /// </summary>
        public override string InnerHtml
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (KonakHtmlElement nd in Nodes)
                    sb.Append(nd.OuterHtml);

                return sb.ToString();
            }
        }

        /// <summary>
        /// Returns HTML data of current element
        /// </summary>
        public override string OuterHtml
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append('<').Append(Name);

                if (Attributes.Count > 0)
                    sb.Append(' ').Append(Attributes.ToString());

                sb.Append('>');

                foreach (KonakHtmlElement nd in Nodes)
                    sb.Append(nd.OuterHtml);

                sb.Append("</").Append(Name).Append('>');

                return sb.ToString();
            }
        }

        /// <summary>
        /// Attributes collection of HtmlElement
        /// </summary>
        public KonakHtmlTagNodeAttributesCollection Attributes { get; internal set; }

        /// <summary>
        /// SubNodes collection of curent element
        /// </summary>
        public KonakHtmlNodesList Nodes { get; private set; }

        /// <summary>
        /// Name of the HTML element
        /// </summary>
        public string Name { get; internal set; }

        internal void Remove(KonakHtmlElement childNode)
        {
            Nodes.Remove(childNode);
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of KonakHtmlTagNode class
        /// </summary>
        public KonakHtmlTagNode() : base()
        {
            InitProperties();
        }

        /// <summary>
        /// Initializes a new instance of KonakHtmlTagNode class with provided tag name
        /// </summary>
        /// <param name="name">Name of the tag node</param>
        public KonakHtmlTagNode(string name) : base()
        {
            InitProperties();
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of KonakHtmlTagNode class with provided tag name and parent tag node
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        public KonakHtmlTagNode(string name, KonakHtmlTagNode parent) : base(parent)
        {
            InitProperties();
            Name = name;
        }
        #endregion

        #region methods
        #region private methods
        private void InitProperties()
        {
            Attributes = new KonakHtmlTagNodeAttributesCollection();
            Name = string.Empty;
            Nodes = new KonakHtmlNodesList(this);
        }
        #endregion

        #region public methods
        /// <summary>
        /// Set tag node attributes
        /// </summary>
        /// <param name="attributes">Collection of attributes to be added to current tag node</param>
        /// <returns></returns>
        public KonakHtmlTagNode SetAttributes(KonakHtmlTagNodeAttributesCollection attributes)
        {
            if (attributes != null)
                foreach (KonakHtmlTagNodeAttribute attrib in attributes)
                {
                    if (Attributes.Contains(attrib.Name))
                        Attributes[attrib.Name] = attrib;
                    else
                        Attributes.Add(attrib);
                }

            return this;
        }

        #endregion
        #endregion
    }
}
