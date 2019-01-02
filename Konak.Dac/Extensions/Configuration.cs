using Konak.Dac.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Konak.Dac.Extensions
{
    /// <summary>
    /// Class containing extension methods to manage DAC configuration
    /// </summary>
    public static class Configuration
    {
        /// <summary>
        /// Extension method to load DAC configuration from configuration file and call DAC init method
        /// </summary>
        /// <param name="httpApplication"></param>
        /// <returns></returns>
        public static HttpApplication UseKonakDac(this HttpApplication httpApplication)
        {
            UseKonakDac();

            return httpApplication;
        }

        /// <summary>
        /// Static method to load DAC configuration from configuration file and call DAC init method
        /// </summary>
        public static void UseKonakDac()
        {
            DACSettings settings = ConfigSection.GetSection().Settings;

            List<KeyValuePair<string, string>> connectionStringsList = new List<KeyValuePair<string, string>>();

            foreach (ConnectionStringSettings item in ConfigurationManager.ConnectionStrings)
                connectionStringsList.Add(new KeyValuePair<string, string>(item.Name, item.ConnectionString));

            DAC.Init(connectionStringsList, settings.DefaultConnectionString);
        }
    }
}
