using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.ContentAnalysis
{
    public class Paragraph
    {
        private static readonly string[] SENTENCE_DELIMITERS = new string[] { "...", ".", "?!", "!?", "?", "!", ";", ":" };

        public List<Sentence> SentenceList { get; }
        public string Text { get; private set; }
        public IEnumerable<Word> Words
        {
            get
            {
                List<Word> res = new List<Word>();

                foreach (Sentence sent in SentenceList)
                    res.AddRange(sent.Words);

                return res;
            }
        }

        public Paragraph()
        {
            SentenceList = new List<Sentence>();
            Text = string.Empty; ;
        }

        public Paragraph(string text) : this()
        {
            AddText(text);
        }

        internal void AddText(string text)
        {
            Text += text+". ";

            string[] sentences = text.Split(SENTENCE_DELIMITERS, StringSplitOptions.RemoveEmptyEntries);

            foreach(string sent in sentences)
                SentenceList.Add(new Sentence(sent));

        }
    }
}
