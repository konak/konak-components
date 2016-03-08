using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.WebCrawler
{
    public delegate void ErrorDelegate(object source, Exception exception);
    public delegate void GetDataDelegate(CrawledData data);
}
