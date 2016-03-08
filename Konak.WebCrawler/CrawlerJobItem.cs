using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Konak.WebCrawler
{
    public enum UrlType
    {
        rss = 1,
        full_data = 2
    }

    public class CrawlerJobItem<T> : ICrawlerJobItem
    {
        public Uri BaseUri { get; set; }
        public bool ClearCoockies { get; set; }
        public bool ClearHeaders { get; set; }
        public List<KeyValuePair<string, string>> Headers { get; set; }
        public CookieContainer Coockies { get; set; }
        public string RequestResource { get; set; }
        public string Referer { get; set; }
        public UrlType Type { get; private set; }
        public T BoundObject { get; set; }

        private CrawlerJobItem()
        {
            ClearCoockies = false;
            ClearHeaders = false;

            Coockies = null;
            Headers = null;
            Referer = string.Empty;
        }

        public CrawlerJobItem(Uri baseUrl, UrlType type) : this()
        {
            Type = type;
            BaseUri = baseUrl;
            RequestResource = string.Empty;
        }

        public CrawlerJobItem(Uri baseUrl, string requestResource, UrlType type) : this()
        {
            Type = type;
            BaseUri = baseUrl;
            RequestResource = requestResource;
        }

        public CrawlerJobItem(string baseUrl, UrlType type) : this(new Uri(baseUrl), type) { }

        public CrawlerJobItem(string baseUrl, string requestResource, UrlType type) : this(new Uri(baseUrl), requestResource, type) { }

    }
}
