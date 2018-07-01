using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Konak.Dac.Configuration
{
    /// <summary>
    /// Settings of DAC
    /// </summary>
    public class DACSettings : ConfigurationElement
    {
        /// <summary>
        /// Default connection string name used for SQL execution
        /// </summary>
        [ConfigurationProperty("default_connection_string")]
        public string DefaultConnectionString
        {
            get { return (string)this["default_connection_string"]; }
            set { this["default_connection_string"] = value; }
        }
    }
}
