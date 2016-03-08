using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.HttpBrowser.Configuration
{
    internal class BrowserSettings : ConfigurationElement
    {
        private const string _BROWSER_USER_AGENT = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
        private const string _BROWSER_ACCEPT_LANGUAGE = "ru,en-US;q=0.8,en;q=0.6,hy;q=0.4";

        [ConfigurationProperty("user_agent", DefaultValue = _BROWSER_USER_AGENT, IsRequired = false)]
        internal string UserAgent
        {
            get { return (string)this["user_agent"]; }
            set { this["user_agent"] = value; }
        }

        [ConfigurationProperty("accept_language", DefaultValue = _BROWSER_ACCEPT_LANGUAGE, IsRequired = false)]
        internal string AcceptLanguage
        {
            get { return (string)this["accept_language"]; }
            set { this["accept_language"] = value; }
        }

        [ConfigurationProperty("keep_alive", DefaultValue = true, IsRequired = false)]
        internal bool KeepAlive
        {
            get { return (bool)this["keep_alive"]; }
            set { this["keep_alive"] = value; }
        }


        [ConfigurationProperty("request_timeout", DefaultValue = 60, IsRequired = false)]
        internal int RequestTimeout
        {
            get { return (int)this["request_timeout"]; }
            set { this["request_timeout"] = value; }
        }

        [ConfigurationProperty("response_timeout", DefaultValue = 60, IsRequired = false)]
        internal int ResponseTimeout
        {
            get { return (int)this["response_timeout"]; }
            set { this["response_timeout"] = value; }
        }

    }
}
