using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.HttpBrowser
{
    public delegate void ComponentErrorEventDelegate(object source, Exception exception);
    public delegate void BrowserNavigationErrorDelegate(WebBrowser browser, Exception exception);
}
