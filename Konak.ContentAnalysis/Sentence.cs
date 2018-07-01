using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.ContentAnalysis
{
    public class Sentence
    {
        private static readonly string[] WORD_DELIMITERS = new string[] { " ", Convert.ToChar(160).ToString(), "\t", "\r", "\n", ",", Convert.ToChar(8212).ToString(), " - ", "(", ")", "/", "|", "\"" };

        public List<Word> Words { get; }
        public string Text { get; set; }
        public Sentence()
        {
            Words = new List<Word>();
            Text = string.Empty;
        }

        public Sentence(string text) : this()
        {
            AddText(text);
        }

        internal void AddText(string text)
        {
            Text += text + " ";

            string[] words = text.Split(WORD_DELIMITERS, StringSplitOptions.RemoveEmptyEntries);

            foreach(string word in words)
                Words.Add(new Word(word));

        }
    }
}
