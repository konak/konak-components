using Konak.Dac.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Konak.Dac.Core.Extensions
{
    /// <summary>
    /// Class containing extension methods to manage DAC configuration
    /// </summary>
    public static class Configuration
    {
        /// <summary>
        /// Extension used to initialize Konak.Dac component by providing configuration 
        /// </summary>
        /// <param name="app">Instance of allication builder used to configure application</param>
        /// <param name="configuration">Set of key/value application configuration properies</param>
        /// <returns>Instance of allication builder used to configure application</returns>
        public static IApplicationBuilder UseKonakDac(this IApplicationBuilder app, IConfiguration configuration)
        {
            UseKonakDac(configuration);

            return app;
        }

        /// <summary>
        /// Extension used to initialize Konak.Dac component by providing configuration 
        /// </summary>
        /// <param name="configuration">Set of key/value application configuration properies</param>
        /// <returns>Instance of allication builder used to configure application</returns>
        public static void UseKonakDac(IConfiguration configuration)
        {
            IConfigurationSection connectionStringSection = configuration.GetSection("ConnectionStrings");
            IConfigurationSection konakDacSettingsSection = configuration.GetSection(ConfigSection.ConfigSectionName);

            IEnumerable<IConfigurationSection> listOfConnectionStringSections = connectionStringSection.GetChildren();

            List<KeyValuePair<string, string>> connectionList = new List<KeyValuePair<string, string>>();

            foreach (IConfigurationSection cs in listOfConnectionStringSections)
                connectionList.Add(new KeyValuePair<string, string>(cs.Key, cs.Value));

            string defaultConnectionStringKey = konakDacSettingsSection[DACSettings.DefaultConnectionStringAttributeName];

            DAC.Init(connectionList, defaultConnectionStringKey);
        }
    }
}
