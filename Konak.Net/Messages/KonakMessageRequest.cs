using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Konak.Net.Messages
{
    public class KonakMessageRequest : KonakMessageBase
    {
        internal ManualResetEvent SyncEvent;

        internal KonakMessageRequestType RequestType { get; set; }

        public KonakMessageRequest() : base()
        {
            Type = KonakMessageType.request;
            RequestType = KonakMessageRequestType.sync;
        }

    }
}
