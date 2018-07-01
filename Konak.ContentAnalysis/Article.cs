using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Konak.HtmlParser;

namespace Konak.ContentAnalysis
{
    public class Article
    {
        private static readonly string[] PARAGRAPH_SEPARATOR = new string[] { "\r\n" };
        private string v;

        public List<Paragraph> ParagraphsList { get; }
        public string Text { get; set; }
        public IEnumerable<Word> Words
        {
            get
            {
                List<Word> res = new List<Word>();

                foreach (Paragraph p in ParagraphsList)
                    res.AddRange(p.Words);

                return res;
            }
        }

        public IEnumerable<Sentence> SentenceList
        {
            get
            {
                List<Sentence> res = new List<Sentence>();

                foreach (Paragraph p in ParagraphsList)
                    res.AddRange(p.SentenceList);

                return res;
            }
        }

        private Article()
        {
            ParagraphsList = new List<Paragraph>();
            Text = string.Empty;
        }

        private Article(string text) : this()
        {
            AddText(text);
        }

        public void AddText(string text)
        {
            Text += text + "\r\n";

            string[] paragraphs = text.Split(PARAGRAPH_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);

            foreach(string strP in paragraphs)
                ParagraphsList.Add(new Paragraph(strP));
            
        }

        private static void ReadParagraphs(KonakHtmlNodesList nodes, StringBuilder sb)
        {
            foreach (KonakHtmlElement nd in nodes)
            {
                if (nd is KonakHtmlTagNode)
                {
                    KonakHtmlTagNode tnd = nd as KonakHtmlTagNode;

                    if (tnd.Name.Equals("p")
                        || tnd.Name.Equals("div")
                        || tnd.Name.Equals("br")
                        || tnd.Name.Equals("hr")
                        || tnd.Name.Equals("ul")
                        || tnd.Name.Equals("ol")
                        || tnd.Name.Equals("li")
                        || tnd.Name.Equals("section")
                        || tnd.Name.Equals("blockquote")
                        || tnd.Name.Equals("code")
                        || tnd.Name.Equals("pre")
                        || tnd.Name.Equals("samp")
                        )
                    {
                        sb.Append("\r\n");
                    }

                    if (tnd.Nodes.Count > 0)
                        ReadParagraphs(tnd.Nodes, sb);

                    if (tnd.Name.Equals("p")
                        || tnd.Name.Equals("div")
                        || tnd.Name.Equals("ul")
                        || tnd.Name.Equals("ol")
                        || tnd.Name.Equals("li")
                        || tnd.Name.Equals("section")
                        || tnd.Name.Equals("blockquote")
                        || tnd.Name.Equals("code")
                        || tnd.Name.Equals("pre")
                        || tnd.Name.Equals("samp")
                        )
                    {
                        sb.Append("\r\n");
                    }
                }

                if (nd is KonakHtmlTextNode && !string.IsNullOrWhiteSpace(nd.InnerText))
                    sb.Append(nd.InnerText.Replace("\r\n", " ").Replace('\r', ' ').Replace('\n', ' '));
            }
        }

        public static Article CreateItem(string html)
        {
            KonakHtmlDocument doc = new KonakHtmlDocument(html, new KonakHtmlParserSettings() { RemoveWhiteSpaces = true });

            StringBuilder sb = new StringBuilder();

            ReadParagraphs(doc.Nodes, sb);

            Article article = new Article(sb.ToString());

            return article;
        }
    }
}
