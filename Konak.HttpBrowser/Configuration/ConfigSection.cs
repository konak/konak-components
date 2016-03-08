using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.HttpBrowser.Configuration
{
    internal class ConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("settings", IsRequired=false)]
        internal HttpBrowserSettings Settings
        {
            get { return (HttpBrowserSettings)this["settings"]; }
            set { this["settings"] = value; }
        }

        internal ConfigSection()
        {
            this.Settings = new HttpBrowserSettings();
        }

        internal static ConfigSection GetSection()
        {
            ConfigSection res = null;

            try
            {
                res = (ConfigSection)ConfigurationManager.GetSection("Konak.HttpBrowser");
            }
            catch (Exception ex)
            {
                Root.RaiseComponentErrorEvent(res, new Konak.Common.Exceptions.ConfigurationException(Resources.ErrorMessages.CONFIGURATION_UNABLE_TO_LOAD_BROWSER_CONFIGURATION_SECTION + " " + ex.Message, ex));
            }

            if (res == null)
                res = new ConfigSection();

            return res;
        }
    }
}
