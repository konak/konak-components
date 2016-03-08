using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Konak.WebCrawler
{
    public interface ICrawlerJobItem
    {
        //Guid RID { get; set; }
        Uri BaseUri { get; set; }
        bool ClearCoockies { get; set; }
        bool ClearHeaders { get; set; }
        List<KeyValuePair<string, string>> Headers { get; set; }
        CookieContainer Coockies { get; set; }
        string RequestResource { get; set; }
        string Referer { get; set; }
        UrlType Type { get; }
    }
}
