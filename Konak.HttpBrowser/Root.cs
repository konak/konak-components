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
        public static event ComponentErrorEventDelegate ComponentErrorEvent;

        internal static ConfigSection CONFIG;

        static Root()
        {
            CONFIG = ConfigSection.GetSection();
        }

        internal static void RaiseComponentErrorEvent(object source, Exception ex)
        {
            ComponentErrorEventDelegate evt = ComponentErrorEvent;

            if (evt == null) return;

            foreach(Delegate d in evt.GetInvocationList())
            {
                try
                {
                    d.DynamicInvoke(source, ex);
                }
                catch(Exception exx)
                {
                    System.Diagnostics.Debug.WriteLine(exx);
                }
            }
        }
    }
}
