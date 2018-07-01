using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.HtmlParser
{
    /// <summary>
    /// Class for parser settings
    /// </summary>
    public class KonakHtmlParserSettings
    {
        internal static string WHITESPACE_CHARS_STRING = " \r\n\t";
        internal static char[] WHITESPACE_CHARS_ARRAY = WHITESPACE_CHARS_STRING.ToCharArray(); // new char[] {' ', '\r', '\n', '\t' };

        /// <summary>
        /// Default doctype header text
        /// </summary>
        internal static string DOCTYPE_HEADER = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">";

        #region properties
        /// <summary>
        /// If true parser wil remove whitespaces form texts
        /// </summary>
        public bool RemoveWhiteSpaces { get; set; }

        /// <summary>
        /// if true parser will remove all comments from html
        /// </summary>
        public bool RemoveComments { get; set; }
        #endregion

        #region constructor
        /// <summary>
        /// Initializes a new instance of KonakHtmlParserSettings class with default settings
        /// </summary>
        public KonakHtmlParserSettings()
        {
            RemoveWhiteSpaces = false;
            RemoveComments = false;
        }
        #endregion
    }
}
