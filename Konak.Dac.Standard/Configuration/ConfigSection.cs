using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Konak.Dac.Configuration
{
    /// <summary>
    /// Section containing settings for DAC
    /// </summary>
    public class ConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("settings")]
        public DACSettings Settings
        {
            get { return (DACSettings)this["settings"]; }
            set { this["settings"] = value; }
        }

        public static ConfigSection GetSection()
        {
            ConfigSection res = null;

            try
            {
                res = (ConfigSection)ConfigurationManager.GetSection("Konak.Dac");
            }
            catch (Exception ex)
            {
                throw new Konak.Common.Exceptions.ConfigurationException(Resources.Exceptions.Messages.CONFIGURATION_UNABLE_TO_LOAD_DAC_SECTION + " " + ex.Message, ex);
            }

            return res;
        }
    }
}
