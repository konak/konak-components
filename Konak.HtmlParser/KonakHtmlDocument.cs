using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Konak.HtmlParser
{
    /// <summary>
    /// HTML document class
    /// </summary>
    public class KonakHtmlDocument : KonakHtmlTagNode
    {
        #region properties

        internal KonakHtmlParserNodesStack ParsingNodesStack;
        /// <summary>
        /// settings object for HTML parser
        /// </summary>
        internal KonakHtmlParserSettings ParserSettings { get; private set; }

        /// <summary>
        /// Initial string provided to parser
        /// </summary>
        public string SourceHtmlString { get; private set; }

        /// <summary>
        /// Loopback reference to current instance of main document
        /// </summary>
        public new KonakHtmlDocument Document { get { return this; } }

        public override string OuterHtml
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (KonakHtmlElement nd in Nodes)
                    sb.Append(nd.OuterHtml);

                return sb.ToString();
            }
        }
        #endregion

        #region constructors
        /// <summary>
        /// Constructor that will create an HtmlDocument object by parsing provided html string
        /// </summary>
        /// <param name="html">HTML string to parse</param>
        public KonakHtmlDocument(string html) : this(html, new KonakHtmlParserSettings()) { }

        /// <summary>
        /// Constructor that will create an HtmlDocument object by parsing provided html string
        /// </summary>
        /// <param name="html">HTML string to parse</param>
        /// <param name="settings">Settings for HTML parser</param>
        public KonakHtmlDocument(string html, KonakHtmlParserSettings settings) : base()
        {
            ParsingNodesStack = new KonakHtmlParserNodesStack();

            ParserSettings = settings;
            SourceHtmlString = html;

            int index = 0;
            Parse(ref index, this);
        }
        #endregion

        #region methods

        /// <summary>
        /// parse current scope of the document
        /// </summary>
        /// <param name="index">index of the cursor parsing process has came to</param>
        /// <param name="parent">tag node that will be the parrent for nodes found in current scope</param>
        private void Parse(ref int index, KonakHtmlTagNode parent)
        {
            char ch;

            while (index < SourceHtmlString.Length)
            {
                if(SourceHtmlString[index].Equals('<'))
                {
                    if (index + 1 < SourceHtmlString.Length && SourceHtmlString[index + 1].Equals('/'))
                    {
                        // closing tag
                        index += 2;

                        if (index + parent.Name.Length < SourceHtmlString.Length)
                        {
                            if (SourceHtmlString.Substring(index, parent.Name.Length).Equals(parent.Name))
                            {
                                ParsingNodesStack.Close(parent.Name);
                                index += parent.Name.Length + 1;
                                return;
                            }
                        }

                        ch = ' ';

                        // need some error correction
                        StringBuilder closingTagBuilder = new StringBuilder();

                        while (index < SourceHtmlString.Length)
                        {
                            ch = SourceHtmlString[index];

                            if (ch.Equals('>')) break;

                            index++;

                            if (ch.Equals('<')) break;

                            closingTagBuilder.Append(ch);
                        }

                        string closingTagName = closingTagBuilder.ToString();

                        if (!ParsingNodesStack.Close(closingTagName)) // try to close upper level tag
                        {
                            //upper level tag closing was not successfull so adding that text as a comment
                            parent.Nodes.Add(new KonakHtmlCommentsNode(parent).SetText("</" + closingTagName + (ch.Equals('<') ? ' ' : ch)));
                        }
                    }
                    else
                    {
                        parent.Nodes.Add(ReadTag(ref index, parent));
                    }
                }
                else
                {
                    parent.Nodes.Add(ReadTextNode(ref index, parent));
                }
            }
        }

        /// <summary>
        /// Read text data of the node and return it as KonakHtmlTextNode class item
        /// </summary>
        /// <param name="index"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private KonakHtmlTextNode ReadTextNode(ref int index, KonakHtmlTagNode parent)
        {
            StringBuilder sb = new StringBuilder();
            char ch;
            bool skipWS = false;

            while(index < SourceHtmlString.Length)
            {
                ch = SourceHtmlString[index];

                if (ch.Equals('<'))
                    break;

                if (ParserSettings.RemoveWhiteSpaces)
                {
                    
                    if(KonakHtmlParserSettings.WHITESPACE_CHARS_STRING.Contains(ch))
                    {
                        if(!skipWS)
                        {
                            sb.Append(ch);

                            if (ch.Equals('\r') && index + 1 < SourceHtmlString.Length && (ch = SourceHtmlString[index + 1]).Equals('\n'))
                            {
                                sb.Append(ch);
                                index++;
                            }

                            skipWS = true;
                        }
                    }
                    else
                    {
                        sb.Append(ch);
                        skipWS = false;
                    }
                }
                else
                    sb.Append(ch);

                index++;
            }

            //do
            //{
            //    sb.Append(SourceHtmlString[index++]);
            //}
            //while (index < SourceHtmlString.Length &&
            //    (!SourceHtmlString[index].Equals('<') ||
            //    (index + 1 < SourceHtmlString.Length && KonakHtmlParserSettings.WHITESPACE_CHARS_STRING.Contains(SourceHtmlString[index + 1]))));

            return new KonakHtmlTextNode(parent).SetText(sb.ToString());
        }

        /// <summary>
        /// Parse and read current found tag
        /// </summary>
        /// <param name="index">reference to current index of analysed character of provided HTML code</param>
        /// <param name="parent">The parent tag node, which tag is currently will be parsed</param>
        /// <returns>Returns already parsed Html element</returns>
        private KonakHtmlElement ReadTag(ref int index, KonakHtmlTagNode parent)
        {
            KonakHtmlTagNodeAttributesCollection tagAttributes = null;
            StringBuilder sb = new StringBuilder();
            char ch;
            string tagName;

            while (++index < SourceHtmlString.Length)
            {
                switch (ch = SourceHtmlString[index])
                {
                    case ' ':
                    case '\r':
                    case '\n':
                    case '\t':
                        tagName = sb.ToString();
                        tagAttributes = ReadAttributes(ref index);
                        if (SourceHtmlString[index].Equals('/') && SourceHtmlString[index + 1].Equals('>'))
                        {
                            index += 2;
                            return new KonakHtmlTagNodeUnPaired(tagName, parent).SetAttributes(tagAttributes);
                        }
                        if (SourceHtmlString[index].Equals('>'))
                            goto case '>';

                        break;

                    case '/':
                        break;

                    case '>':
                        tagName = sb.ToString();

                        index++;

                        switch(tagName)
                        {
                            case "script":
                                return ReadScriptingTag(ref index, tagName, parent).SetAttributes(tagAttributes);

                            case "style":
                                return ReadStyleTag(ref index, tagName, parent).SetAttributes(tagAttributes);

                            case "br":
                            case "img":
                            case "input":
                            case "meta":
                            case "link":
                            case "area":
                            case "source":
                            case "base":
                            case "basefont":
                            case "col":
                            case "hr":
                            case "keygen":
                            case "param":
                            case "track":
                            case "embed":
                            case "abbr":

                                if (SourceHtmlString.IndexOf("</" + tagName + ">", index) == -1)
                                    return new KonakHtmlTagNodeUnPaired(tagName, parent).SetAttributes(tagAttributes);
                                else
                                    goto default;

                            default:
                                KonakHtmlTagNodePaired pairedTag = new KonakHtmlTagNodePaired(tagName, parent);
                                ParsingNodesStack.Add(pairedTag);
                                pairedTag.SetAttributes(tagAttributes);
                                Parse(ref index, pairedTag);
                                return pairedTag;
                        }


                    case '!':
                    case '?':
                        if(index+2 < SourceHtmlString.Length)
                        {
                            switch(SourceHtmlString[index+1])
                            {
                                case 'D':
                                case 'x':
                                case 'd':
                                case 'X':
                                    index--;
                                    return ReadServiceTypeNodeData(ref index, parent);

                                case '-':
                                    if (SourceHtmlString[index + 2].Equals('-'))
                                        return ReadCommentsNode(ref index, parent);
                                    else
                                        goto default;

                                default:
                                    // error correction needed
                                    index--;
                                    return ReadErrorDataAsCommentsNode(ref index, parent);
                            }
                        }
                        break;

                    default:
                        sb.Append(ch);
                        break;
                }
            }

            return new KonakHtmlTextNode(parent).SetText("<" + sb.ToString());
        }

        /// <summary>
        /// Parse and read found style tag node
        /// </summary>
        /// <param name="index">reference to current index of the character of provided Html data</param>
        /// <param name="tagName">Name of tag node</param>
        /// <param name="parent">the parent tag node, which node will be currently analyzed</param>
        /// <returns>Returns parsed style tag node</returns>
        private KonakHtmlScriptTagNode ReadStyleTag(ref int index, string tagName, KonakHtmlTagNode parent)
        {
            KonakHtmlScriptTagNode res = new KonakHtmlScriptTagNode(tagName, parent);
            char ch;
            StringBuilder sb = new StringBuilder();

            while(index<SourceHtmlString.Length)
            {
                switch(ch = SourceHtmlString[index])
                {
                    case '/':
                        if (index + 1 < SourceHtmlString.Length && SourceHtmlString[index + 1].Equals('*'))
                        {
                            index += 2;

                            if (sb.Length > 0)
                            {
                                res.Nodes.Add(new KonakHtmlPlainTextNode(res).SetText(sb.ToString()));
                                sb.Clear();
                            }

                            res.Nodes.Add(ReadStyleComment(ref index, res));
                        }
                        else
                            goto default;

                        break;
                    case '<':
                        if (index + 3 + tagName.Length < SourceHtmlString.Length && SourceHtmlString.Substring(index + 1, tagName.Length + 2).Equals("/style>"))
                        {
                            index += 3 + tagName.Length;

                            if (sb.Length > 0)
                            {
                                res.Nodes.Add(new KonakHtmlPlainTextNode(res).SetText(sb.ToString()));
                                sb.Clear();
                            }

                            return res;
                        }
                        else
                            goto default;

                    default:
                        sb.Append(ch);
                        break; 
                }
                index++;
            }

            return res;
        }

        /// <summary>
        /// Read comments inside of style tag node
        /// </summary>
        /// <param name="index">Reference of the index of current character from parsable Html document</param>
        /// <param name="parent">The parent of currently readable comment</param>
        /// <returns>Returns comments of the style node</returns>
        private KonakHtmlStyleCommentNode ReadStyleComment(ref int index, KonakHtmlTagNode parent)
        {
            StringBuilder sb = new StringBuilder();
            char ch;

            while(index < SourceHtmlString.Length)
            {
                if((ch = SourceHtmlString[index]).Equals('*') && index + 1 < SourceHtmlString.Length && SourceHtmlString[index + 1].Equals('/'))
                {
                    index += 2;
                    break;
                }
                else
                    sb.Append(ch);

                index++;
            }

            return new KonakHtmlStyleCommentNode().SetText(sb.ToString());
        }

        /// <summary>
        /// Read found script tag node
        /// </summary>
        /// <param name="index">reference to index of the current character of the parsing Html data</param>
        /// <param name="tagName">Name of the tag</param>
        /// <param name="parent">The parent of the element that will be parsed</param>
        /// <returns>Returns parsed script tag node</returns>
        private KonakHtmlScriptTagNode ReadScriptingTag(ref int index, string tagName, KonakHtmlTagNode parent)
        {
            char ch;
            StringBuilder sb = new StringBuilder();
            KonakHtmlScriptTagNode res = new KonakHtmlScriptTagNode(tagName, parent);

            while (index < SourceHtmlString.Length)
            {
                switch (ch = SourceHtmlString[index])
                {
                    case '\'':
                    case '\"':
                        char qCh = ch;
                        sb.Append(ch);

                        while(++index < SourceHtmlString.Length)
                        {
                            if ((ch = SourceHtmlString[index]).Equals(qCh))
                            {
                                sb.Append(ch);
                                break;
                            }
                            else if(ch.Equals('\\') && index + 1 < SourceHtmlString.Length)
                            {
                                sb.Append(ch).Append(SourceHtmlString[++index]);
                            }
                            else
                                sb.Append(ch);
                        }
                        index++;
                        break;

                    case '<':
                        if (index + 3 + tagName.Length < SourceHtmlString.Length && SourceHtmlString.Substring(index + 1, tagName.Length + 2).Equals("/script>"))
                        {
                            index += 3 + tagName.Length;
                            res.Nodes.Add(new KonakHtmlPlainTextNode(res).SetText(sb.ToString()));
                            sb.Clear();
                            return res;
                        }
                        else
                            goto default;

                    case '/':
                        if (index + 1 < SourceHtmlString.Length && (ch = SourceHtmlString[index + 1]).Equals('/') || ch.Equals('*'))
                        {
                            if(sb.Length > 0)
                            {
                                res.Nodes.Add(new KonakHtmlPlainTextNode(res).SetText(sb.ToString()));
                                sb.Clear();
                            }

                            index++;
                            res.Nodes.Add(ReadScriptCommentsNode(ref index, res));
                        }
                        else
                            goto default;

                        break;

                    default:
                        sb.Append(ch);
                        index++;
                        break;
                }
            }

            if (sb.Length > 0)
                res.Nodes.Add(new KonakHtmlPlainTextNode(res).SetText(sb.ToString()));

            return res;
        }

        /// <summary>
        /// Read comments inside script tag node
        /// </summary>
        /// <param name="index">Reference to index of the character of currently analyzed Html data</param>
        /// <param name="parent">The parent tag element of currently reading script commen</param>
        /// <returns></returns>
        private KonakHtmlScriptCommentNode ReadScriptCommentsNode(ref int index, KonakHtmlTagNode parent)
        {
            StringBuilder sb = new StringBuilder();
            char ch;

            if(SourceHtmlString[index].Equals('/'))
            {
                sb.Append("//");

                while (++index < SourceHtmlString.Length)
                {
                    ch = SourceHtmlString[index];

                    if (ch.Equals('\r') || ch.Equals('\n'))
                    {
                        sb.Append(ch);
                        index++;
                        break;
                    }
                    else if(ch.Equals('<') && index + parent.Name.Length + 3 < SourceHtmlString.Length && SourceHtmlString.Substring(index, parent.Name.Length + 3).Equals("</" + parent.Name + ">"))
                    {
                        break;
                    }
                    else
                        sb.Append(ch);
                }
            }
            else
            {
                sb.Append("/*");

                while (++index < SourceHtmlString.Length)
                {
                    if ((ch = SourceHtmlString[index]).Equals('*'))
                    {
                        if (index + 1 < SourceHtmlString.Length && SourceHtmlString[index + 1].Equals('/'))
                        {
                            index += 2;
                            break;
                        }
                        else
                            sb.Append(ch);
                    }
                    else if (ch.Equals('<') && index + parent.Name.Length + 3 < SourceHtmlString.Length && SourceHtmlString.Substring(index, parent.Name.Length + 3).Equals("</" + parent.Name + ">"))
                    {
                        break;
                    }
                    else
                        sb.Append(ch);
                }
            }

            return new KonakHtmlScriptCommentNode(parent).SetText(sb.ToString());
        }

        /// <summary>
        /// Read data of service type tag node
        /// </summary>
        /// <param name="index">reference to the index of currently analyzed character of parsing Html data</param>
        /// <param name="parent">The parent tag node of currently readint element</param>
        /// <returns>Returns instance of parsed service node</returns>
        private KonakHtmlServiceNode ReadServiceTypeNodeData(ref int index, KonakHtmlTagNode parent)
        {
            StringBuilder sb = new StringBuilder();

            do
            {
                sb.Append(SourceHtmlString[index]);
            } while (!SourceHtmlString[index++].Equals('>'));

            return new KonakHtmlServiceNode(sb.ToString(), parent);
        }

        /// <summary>
        /// Read detected error data as comment
        /// </summary>
        /// <param name="index">Reference to the index of character of currently parsing Html data</param>
        /// <param name="parent">The parent node for currently parsing error data</param>
        /// <returns>Returns error data as comment node</returns>
        private KonakHtmlCommentsNode ReadErrorDataAsCommentsNode(ref int index, KonakHtmlTagNode parent)
        {
            char ch;
            StringBuilder sb = new StringBuilder();

            sb.Append(SourceHtmlString[index]);

            while(++index < SourceHtmlString.Length)
            {
                if((ch = SourceHtmlString[index]).Equals('<'))
                    break;

                if(ch.Equals('>'))
                {
                    index++;
                    break;
                }

                sb.Append(ch);
            }

            return new KonakHtmlCommentsNode(parent).SetText(sb.ToString());
        }

        /// <summary>
        /// Read found comments text of HTML document
        /// </summary>
        /// <param name="index">Reference to the index of character of currently parsing Html data</param>
        /// <param name="parent">The parent node of found comment</param>
        /// <returns>Returns instance of KonakHtmlCommentsNode class</returns>
        private KonakHtmlCommentsNode ReadCommentsNode(ref int index, KonakHtmlTagNode parent)
        {
            StringBuilder sb = new StringBuilder();
            char ch = ' ';

            index += 2;

            if (index < SourceHtmlString.Length && SourceHtmlString[index].Equals('-'))
                index++;

            while(index < SourceHtmlString.Length)
            {
                if((ch = SourceHtmlString[index++]).Equals('-'))
                {
                    if (index + 1 < SourceHtmlString.Length && SourceHtmlString[index].Equals('-') && SourceHtmlString[index + 1].Equals('>'))
                    {
                        index += 2;
                        break;
                    }

                    if (index < SourceHtmlString.Length && SourceHtmlString[index].Equals('>'))
                    {
                        index++;
                        break;
                    }
                }

                sb.Append(ch);
            }

            return new KonakHtmlCommentsNode(parent).SetText(sb.ToString());
        }

        /// <summary>
        /// Read attributes of tag node
        /// </summary>
        /// <param name="index">Reference to index of character of currently parsing Html data</param>
        /// <returns>Returns new instance of the KonakHtmlTagNodeAttributesCollection class</returns>
        private KonakHtmlTagNodeAttributesCollection ReadAttributes(ref int index)
        {
            KonakHtmlTagNodeAttributesCollection res = new KonakHtmlTagNodeAttributesCollection();

            while (++index < SourceHtmlString.Length)
            {
                switch (SourceHtmlString[index])
                {
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                        break;

                    case '/':
                        if (index + 1 < SourceHtmlString.Length && SourceHtmlString[index + 1].Equals('>'))
                            goto case '>';

                        break;

                    case '>':
                        return res;

                    default:
                        res.Add(ReadAttribute(ref index));

                        if (SourceHtmlString[index].Equals('>') || SourceHtmlString[index].Equals('/'))
                            goto case '>';

                        break;
                }
            }

            return res;
        }

        /// <summary>
        /// Read single attribute of tag node
        /// </summary>
        /// <param name="index">Reference to index of character of currently parsing Html data</param>
        /// <returns>Returns new instance of KonakHtmlTagNodeAttribute class</returns>
        private KonakHtmlTagNodeAttribute ReadAttribute(ref int index)
        {
            KonakHtmlTagNodeAttribute res = new KonakHtmlTagNodeAttribute();

            StringBuilder sb = new StringBuilder();
            char ch;

            while(index < SourceHtmlString.Length)
            {
                switch (ch = SourceHtmlString[index])
                {
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':

                        if (sb.Length > 0)
                            goto case '>';

                        index++;
                        break;

                    case '/':
                        if (SourceHtmlString[index + 1].Equals('>'))
                            goto case '>';

                        sb.Append(ch);
                        index++;
                        break;

                    case '>':
                        res.Name = sb.ToString();
                        return res;

                    case '=':
                        while (++index < SourceHtmlString.Length)
                            if (!KonakHtmlParserSettings.WHITESPACE_CHARS_STRING.Contains(SourceHtmlString[index]))
                                break;

                        if ((ch = SourceHtmlString[index]).Equals('\"') || ch.Equals('\''))
                            res.QuoteChar = ch;

                        res.Value = ReadAttributeValue(ref index);
                        goto case '>';

                    default:
                        sb.Append(ch);
                        index++;
                        break;
                }

            }

            res.Name = sb.ToString();

            return res;
        }

        /// <summary>
        /// Read currently parsing attribute value
        /// </summary>
        /// <param name="index">Reference to index of character of currently parsing Html data</param>
        /// <returns>Returns string value of the attribute</returns>
        private string ReadAttributeValue(ref int index)
        {
            StringBuilder sb = new StringBuilder();
            char ch;

            while (index<SourceHtmlString.Length && KonakHtmlParserSettings.WHITESPACE_CHARS_STRING.Contains(SourceHtmlString[index]))
                index++;

            char quoteChar = SourceHtmlString[index++];

            if (quoteChar == '\'' || quoteChar == '\"')
            {
                while (index < SourceHtmlString.Length && quoteChar != (ch = SourceHtmlString[index++]))
                    sb.Append(ch);
            }
            else
            {
                index--;

                while (index < SourceHtmlString.Length)
                    if (KonakHtmlParserSettings.WHITESPACE_CHARS_STRING.Contains(ch = SourceHtmlString[index]) || ch.Equals('>') || ch.Equals('/'))
                    {
                        break;
                    }
                    else
                    {
                        sb.Append(ch);
                        index++;
                    }
            }

            return sb.ToString();
        }
        #endregion

    }
}


