using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Net.Sockets
{
    public enum SocketMode
    {
        server = 1, client = 2, connected_client = 3
    }

    public class SocketConfig
    {
        public System.Net.Sockets.AddressFamily AddressFamily { get; set; }
        public System.Net.Sockets.ProtocolType ProtocolType { get; set; }
        public System.Net.Sockets.SocketType SocketType { get; set; }
        public System.Net.IPEndPoint EndPoint { get; set; }
    }

    public enum ConnectionMode
    {
        server = 1, client = 2, connected_client = 3
    }

    public enum ConnectionSecurityType
    {
        none = 0, ssl = 1
    }

    public struct ConnectionConfig
    {
        public struct ConnectionCertificateConfig
        {
            public X509Certificate2 Certificate { get; set; }
            public string CertificateFilePath { get; set; }
            public RemoteCertificateValidationCallback CertValidationCallback { get; set; }
        }

        public struct ConnectionNetworkConfig
        {
            public Socket Socket { get; set; }
            public AddressFamily AddressFamily { get; set; }
            public ProtocolType ProtocolType { get; set; }
            public SocketType SocketType { get; set; }
            public IPEndPoint LocalEndPoint { get; set; }
            public IPEndPoint RemoteEndPoint { get; set; }
        }

        public ConnectionMode Mode { get; set; }
        public ConnectionSecurityType SecurityType { get; set; }
        public ConnectionCertificateConfig Certificate { get; set; }
        public ConnectionNetworkConfig Network { get; set; }
    }

}
