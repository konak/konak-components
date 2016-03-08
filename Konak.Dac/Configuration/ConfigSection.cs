using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Dac.Configuration
{
    /// <summary>
    /// Section containing settings for DAC
    /// </summary>
    internal class ConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("settings")]
        internal DACSettings Settings
        {
            get { return (DACSettings)this["settings"]; }
            set { this["settings"] = value; }
        }

        internal static ConfigSection GetSection()
        {
            ConfigSection res = null;

            try
            {
                res = (ConfigSection)ConfigurationManager.GetSection("Konak.Dac");
            }
            catch (Exception ex)
            {
                throw new Konak.Common.Exceptions.ConfigurationException(Resources.Exception.Messages.CONFIGURATION_UNABLE_TO_LOAD_DAC_SECTION + " " + ex.Message, ex);
            }

            return res;
        }
    }

    

}
