using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.HtmlParser
{
    /// <summary>
    /// Html nodes list
    /// </summary>
    public class KonakHtmlNodesList : IEnumerable<KonakHtmlElement>
    {
        private List<KonakHtmlElement> _nodesList;
        private KonakHtmlElement _parent;

        #region public properties
        /// <summary>
        /// number of nodes in list
        /// </summary>
        public int Count { get { return _nodesList.Count; } }

        public KonakHtmlElement this[int index]
        {
            get
            {
                return _nodesList[index];
            }
            set
            {
                _nodesList[index] = value;
            }
        }
        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of KonakHtmlNodesList class
        /// </summary>
        internal KonakHtmlNodesList()
        {
            _nodesList = new List<KonakHtmlElement>();
            _parent = null;
        }

        /// <summary>
        /// Initializes a new instance of KonakHtmlNodesList class
        /// </summary>
        /// <param name="parent">The parrent tag node</param>
        internal KonakHtmlNodesList(KonakHtmlElement parent) : this()
        {
            _parent = parent;
        }
        #endregion

        #region method
        
        #region IEnumerable interface methods implementation
        public IEnumerator<KonakHtmlElement> GetEnumerator()
        {
            return _nodesList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _nodesList.GetEnumerator();
        }
        #endregion

        #region Add
        /// <summary>
        /// Add Html element to nodes list
        /// </summary>
        /// <param name="node">Node to add to list</param>
        /// <returns>Returns reference to current instance</returns>
        public KonakHtmlNodesList Add(KonakHtmlElement node)
        {
            if(_parent != null)
                node.SetParent(_parent);

            _nodesList.Add(node);
            return this;
        }
        #endregion

        #region Remove
        /// <summary>
        /// Remove provided node from child nodes list
        /// </summary>
        /// <param name="childNode">Child node to remove</param>
        /// <returns>Returns reference to current instance of nodes list</returns>
        internal KonakHtmlNodesList Remove(KonakHtmlElement childNode)
        {
            _nodesList.Remove(childNode);
            return this;
        }
        #endregion

        #region GetElementsByAttributeNameValue
        /// <summary>
        /// Get list of Html elements by provided attribute name and it's value
        /// </summary>
        /// <param name="name">Name of the attribute</param>
        /// <param name="value">Value of the attribute</param>
        /// <returns>Returns list of elements having provided attribute name and value</returns>
        public KonakHtmlNodesList GetElementsByAttributeNameValue(string name, string value)
        {
            return GetElementsByAttributeNameValue(name, value, true);
        }

        /// <summary>
        /// Get list of Html elements by provided attribute name and it's value
        /// </summary>
        /// <param name="name">Name of the attribute</param>
        /// <param name="value">Value of the attribute</param>
        /// <param name="recursive">True to serchr elements in child nodes too, otherwise False</param>
        /// <returns>Returns list of elements having provided attribute name and value</returns>
        public KonakHtmlNodesList GetElementsByAttributeNameValue(string name, string value, bool recursive)
        {
            KonakHtmlNodesList res = new KonakHtmlNodesList();

            foreach (KonakHtmlElement element in _nodesList)
            {
                KonakHtmlTagNode node = element as KonakHtmlTagNode;

                if (node != null)
                {
                    KonakHtmlTagNodeAttribute attribute = node.Attributes[name];

                    if (attribute != null && attribute.Value.Equals(value))
                        res.Add(node);

                    if (node.Nodes.Count > 0 && recursive)
                        foreach (KonakHtmlTagNode subNode in node.Nodes.GetElementsByAttributeNameValue(name, value, recursive))
                            res.Add(subNode);
                }
            }

            return res;
        }
        #endregion

        #region GetElementsByAttributeName
        /// <summary>
        /// Get list of elements containing provided attribute name
        /// </summary>
        /// <param name="name">Name of the attribute to look for</param>
        /// <returns>Returns list of html tag nodes having provided attribute</returns>
        public KonakHtmlNodesList GetElementsByAttributeName(string name)
        {
            return GetElementsByAttributeName(name, true);
        }

        /// <summary>
        /// Get list of elements containing provided attribute name
        /// </summary>
        /// <param name="name">Name of the attribute to look for</param>
        /// <param name="recursive">True to serchr elements in child nodes too, otherwise False</param>
        /// <returns>Returns list of html tag nodes having provided attribute</returns>
        public KonakHtmlNodesList GetElementsByAttributeName(string name, bool recursive)
        {
            KonakHtmlNodesList res = new KonakHtmlNodesList();

            foreach (KonakHtmlElement element in _nodesList)
            {
                KonakHtmlTagNode node = element as KonakHtmlTagNode;

                if (node != null)
                {
                    if (node.Attributes.Contains(name))
                        res.Add(node);

                    if (node.Nodes.Count > 0 && recursive)
                        foreach (KonakHtmlTagNode subNode in node.Nodes.GetElementsByAttributeName(name, recursive))
                            res.Add(subNode);
                }
            }

            return res;
        }
        #endregion

        #region GetElementsById
        /// <summary>
        /// Get list of elements by provided id attribute value
        /// </summary>
        /// <param name="id">Value of id attribute of tag node</param>
        /// <returns>Returns list of tag nodes with provided value of id attribute</returns>
        public KonakHtmlNodesList GetElementsById(string id)
        {
            return GetElementsById(id, true);
        }

        /// <summary>
        /// Get list of elements by provided id attribute value
        /// </summary>
        /// <param name="id">Value of id attribute of tag node</param>
        /// <param name="recursive">True to search in childs node, False to search only in current list</param>
        /// <returns>Returns list of tag nodes with provided value of id attribute</returns>
        public KonakHtmlNodesList GetElementsById(string id, bool recursive)
        {
            return GetElementsByAttributeNameValue("id", id, recursive);
        }
        #endregion

        #region GetElementsByName
        /// <summary>
        /// Get list of elements by provided name attribute value
        /// </summary>
        /// <param name="name">Value of name attribute of tag node</param>
        /// <returns>Returns list of tag nodes with provided value of name attribute</returns>
        public KonakHtmlNodesList GetElementsByName(string name)
        {
            return GetElementsByName(name, true);
        }

        /// <summary>
        /// Get list of elements by provided name attribute value
        /// </summary>
        /// <param name="name">Value of name attribute of tag node</param>
        /// <param name="recursive">True to search in childs node, False to search only in current list</param>
        /// <returns>Returns list of tag nodes with provided value of name attribute</returns>
        public KonakHtmlNodesList GetElementsByName(string name, bool recursive)
        {
            return GetElementsByAttributeNameValue("name", name, recursive);
        }
        #endregion

        #region GetElementsByClassName
        /// <summary>
        /// Get list of elements containing provided class name in class attribute value
        /// </summary>
        /// <param name="name">Name of the class to find for</param>
        /// <returns>Returns list of tag nodes that has provided class name in it class attribute value</returns>
        public KonakHtmlNodesList GetElementsByClassName(string name)
        {
            return GetElementsByClassName(name, true);
        }

        /// <summary>
        /// Get list of elements containing provided class name in class attribute value
        /// </summary>
        /// <param name="name">Name of the class to find for</param>
        /// <param name="recursive">If True make search in child nodes, otherwise False</param>
        /// <returns>Returns list of tag nodes that has provided class name in it class attribute value</returns>
        public KonakHtmlNodesList GetElementsByClassName(string name, bool recursive)
        {
            KonakHtmlNodesList res = new KonakHtmlNodesList();

            foreach (KonakHtmlElement element in _nodesList)
            {
                KonakHtmlTagNode node = element as KonakHtmlTagNode;

                if (node != null)
                {
                    KonakHtmlTagNodeAttribute attribute = node.Attributes["class"];

                    if (attribute != null)
                    {
                        string[] classList = attribute.Value.Split(KonakHtmlParserSettings.WHITESPACE_CHARS_ARRAY);

                        foreach (string c in classList)
                            if (c.Equals(name))
                            {
                                res.Add(node);
                                break;
                            }
                    }

                    if (node.Nodes.Count > 0 && recursive)
                        foreach (KonakHtmlTagNode subNode in node.Nodes.GetElementsByClassName(name, recursive))
                            res.Add(subNode);
                }
            }

            return res;
        }
        #endregion

        #region GetElementsByTagName
        /// <summary>
        /// Get list of elements by provided name of the tag
        /// </summary>
        /// <param name="name">Name of the tag to look for</param>
        /// <returns>Returns list of elements with provided tag name</returns>
        public KonakHtmlNodesList GetElementsByTagName(string name)
        {
            return GetElementsByTagName(name, true);
        }

        /// <summary>
        /// Get list of elements by provided name of the tag
        /// </summary>
        /// <param name="name">Name of the tag to look for</param>
        /// <param name="recursive">If True make search in child nodes, otherwise False</param>
        /// <returns>Returns list of elements with provided tag name</returns>
        public KonakHtmlNodesList GetElementsByTagName(string name, bool recursive)
        {
            KonakHtmlNodesList res = new KonakHtmlNodesList();

            foreach (KonakHtmlElement element in _nodesList)
            {
                KonakHtmlTagNode node = element as KonakHtmlTagNode;

                if (node != null)
                {
                    if (node.Name.Equals(name))
                        res.Add(node);

                    if (node.Nodes.Count > 0 && recursive)
                        foreach (KonakHtmlTagNode subNode in node.Nodes.GetElementsByTagName(name, recursive))
                            res.Add(subNode);
                }
            }
            
            return res;
        }
        #endregion

        #endregion

    }
}
