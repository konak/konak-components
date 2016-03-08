using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Net
{
    public static class Root
    {
        public static ErrorDelegate ErrorEvent;

        #region RaiseErrorEvent
        public static void RaiseErrorEvent(object source, Exception exception)
        {
            ErrorDelegate eventSubscribers = ErrorEvent;

            if (eventSubscribers == null) return;

            foreach (Delegate d in eventSubscribers.GetInvocationList())
            {
                try
                {
                    d.DynamicInvoke(source, exception);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }
        }
        #endregion


    }
}
