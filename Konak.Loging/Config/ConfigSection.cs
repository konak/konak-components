using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Logging.Config
{
    /// <summary>
    /// Class to read log writing settings from section of configuration file
    /// </summary>
    public class ConfigSection : ConfigurationSection
    {
        /// <summary>
        /// FileWriter configuration section
        /// </summary>
        [ConfigurationProperty("FileWriter", DefaultValue = null, IsRequired = false)]
        internal FileWriterSettings FileWriter
        {
            get { return (FileWriterSettings)this["FileWriter"]; }
            set { this["FileWriter"] = value; }
        }

        /// <summary>
        /// static method to read configuration section
        /// </summary>
        /// <returns>returns instance of ConfigurationSection</returns>
        internal static ConfigSection GetSection()
        {
            return (ConfigSection)ConfigurationManager.GetSection("Konak.Logging");
        }
    }
}
