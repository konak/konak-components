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
        /// Name of the default conection string attribute in config file
        /// </summary>
        public const string DefaultConnectionStringAttributeName = "default_connection_string";

        /// <summary>
        /// Default connection string name used for SQL execution
        /// </summary>
        [ConfigurationProperty(DefaultConnectionStringAttributeName)]
        public string DefaultConnectionString
        {
            get { return (string)this[DefaultConnectionStringAttributeName]; }
            set { this[DefaultConnectionStringAttributeName] = value; }
        }
    }
}
