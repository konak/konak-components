using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.HttpBrowser.Helpers
{
    internal static class StreamHelper
    {
        internal static Stream GetContentEncodingReadStream(string contentEncoding, Stream sourceStream)
        {
            if (contentEncoding.Contains("gzip")) return new GZipStream(sourceStream, CompressionMode.Decompress);

            if (contentEncoding.Contains("deflate")) return new DeflateStream(sourceStream, CompressionMode.Decompress);

            return sourceStream;
        }

    }
}
