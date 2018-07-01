using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Net.Messages
{
    

    public enum KonakMessageType
    {
        request,
        response
    }

    public enum KonakMessageResult
    {
        unknown,
        ok,
        ok_async,
        ok_message,
        ok_async_message,
        error,
        error_async
    }

    public enum KonakMessageDataType
    {
        text,
        image
    }

    public enum KonakMessageHeaderType
    {
        command = 1,
        content_size = 2
    }

    public enum KonakMessageRequestType
    {
        send_and_forget,
        broadcast,
        async,
        sync
    }
}
