//using Konak.Common.Helpers;
using Konak.HttpBrowser.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.HttpBrowser
{
    
    public static class Root
    {
        internal static ConfigSection CONFIG;

        static Root()
        {
            CONFIG = ConfigSection.GetSection();
        }
    }
}
