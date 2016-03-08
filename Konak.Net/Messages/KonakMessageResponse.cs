using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Net.Messages
{
    public class KonakMessageResponse : KonakMessageBase
    {
        public KonakMessageResult Result { get; private set; }

        public KonakMessageResponse(Guid id) : base(id)
        {
            Type = KonakMessageType.response;
        }
    }
}
