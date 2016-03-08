using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Konak.HttpBrowser;

namespace Konak.WebCrawler
{
    public class CrawledData
    {
        public string Charset { get; internal set; }
        public CookieContainer Coockies { get; internal set; }
        public byte[] DataBytes { get; internal set; }
        public Encoding DataEncoding { get; set; }
        public WebDataType DataType { get; internal set; }
        public string Html { get; set; }
        public ICrawlerJobItem Job { get; internal set; }
        public HttpStatusCode ResponseStatusCode { get; internal set; }
    }
}
