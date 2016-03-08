using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Net.Messages
{
    public abstract class KonakMessageBase
    {
        public const string VERSION = "0.01";

        internal const int SECTION_VERSION = 0;
        internal const int SECTION_MESSAGE_TYPE = 1;
        internal const int SECTION_COMMAND_TYPE = 2;
        internal const int SECTION_REQUEST_ID = 3;
        internal const int SECTION_DATA_START = 4;

        internal const byte MESSAGE_DELIMITER = 1;
        internal const byte SECTION_DELIMITER = 2;
        internal const byte FIELDS_DELIMITER = 3;

        private ArrayList _dataList;

        public Guid ID { get; private set; }

        public KonakMessageType Type { get; protected set; }

        public Dictionary<string, string> Headers { get; private set; }

        public KonakMessageBase():this(Guid.NewGuid()) { }

        public KonakMessageBase(Guid id)
        {
            ID = id;
            Headers = new Dictionary<string, string>();
        }
    }
}
