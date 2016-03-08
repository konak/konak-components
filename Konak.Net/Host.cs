using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Net
{

    [Flags]
    public enum HostMode
    {
        None = 0,
        Client = 1,
        Proxy = 2,
        Server = 4,
        Root = 8
    }
    
    public class Host
    {
    }
}
