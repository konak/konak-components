using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Konak.Dac.Core.Extensions
{
    public static class Configuration
    {
        /// <summary>
        /// Extension used to initialize Konak.Dac.Core component by providing configuration 
        /// </summary>
        /// <param name="app">Instance of allication builder used to configure application</param>
        /// <param name="configuration">Set of key/value application configuration properies</param>
        /// <returns>Instance of allication builder used to configure application</returns>
        public static IApplicationBuilder UseKonakDac(this IApplicationBuilder app, IConfiguration configuration)
        {
            DAC.Init(configuration);
            return app;
        }

        /// <summary>
        /// Extension used to initialize Konak.Dac.Core component by reading configuration from {assembly_name}.congif file
        /// </summary>
        /// <param name="app">Instance of allication builder used to configure application</param>
        /// <returns>Instance of allication builder used to configure application</returns>
        public static IApplicationBuilder UseKonakDac(this IApplicationBuilder app)
        {
            DAC.Init();
            return app;
        }

        /// <summary>
        /// Extension used to initialize Konak.Dac.Core component by reading configuration from {assembly_name}.congif file
        /// </summary>
        public static void UseKonakDac()
        {
            DAC.Init();
        }

    }
}
