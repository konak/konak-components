using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.HttpBrowser.Configuration
{
    internal class HttpBrowserSettings : ConfigurationElement
    {
        [ConfigurationProperty("browser", IsRequired = false)]
        internal BrowserSettings Browser
        {
            get { return (BrowserSettings)this["browser"]; }
            set { this["browser"] = value; }
        }

        internal HttpBrowserSettings()
        {
            this.Browser = new BrowserSettings();
        }
    }
}
