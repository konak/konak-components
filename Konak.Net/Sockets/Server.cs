using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Konak.Net.Sockets
{
    public class Server<TCon> : IServer where TCon:IConnection
    {
        private TCon _listener = default(TCon);
        private ConnectionConfig _listenerConfig;
        private ConcurrentDictionary<Guid, IConnection> _clients = new ConcurrentDictionary<Guid, IConnection>();

        private int _clientCount = 0;
        private object _syncListener = new object();

        public int ClientCount { get { return _clientCount; } }

        public event ConnectionErrorDelegate ClientConnectionErrorEvent;
        public event ConnectionDataReadDelegate ClientConnectionDataReadEvent;

        #region Constructors
        public Server(ConnectionConfig config)
        {
            _listenerConfig = config;
        }

        public Server(IPEndPoint ipEndPoint) : this (new ConnectionConfig() { Certificate = null, Mode = ConnectionMode.server, SecurityType = ConnectionSecurityType.none, Network = new ConnectionConfig.ConnectionNetworkConfig { LocalEndPoint = ipEndPoint, AddressFamily = ipEndPoint.AddressFamily, ProtocolType = System.Net.Sockets.ProtocolType.Tcp, SocketType = System.Net.Sockets.SocketType.Stream } })
        {
            
        }

        public Server(string ip, int port) : this(new ConnectionConfig() { Certificate = null, Mode = ConnectionMode.server, SecurityType = ConnectionSecurityType.none, Network = new ConnectionConfig.ConnectionNetworkConfig { LocalEndPoint = new IPEndPoint(IPAddress.Parse(ip), port), AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork, ProtocolType = System.Net.Sockets.ProtocolType.Tcp, SocketType = System.Net.Sockets.SocketType.Stream } })
        {

        }

        public Server(int port) : this(new ConnectionConfig() { Certificate = null, Mode = ConnectionMode.server, SecurityType = ConnectionSecurityType.none, Network = new ConnectionConfig.ConnectionNetworkConfig { LocalEndPoint = new IPEndPoint(IPAddress.Any, port), AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork, ProtocolType = System.Net.Sockets.ProtocolType.Tcp, SocketType = System.Net.Sockets.SocketType.Stream } })
        {

        }
        #endregion

        public void Start()
        {
            lock(_syncListener)
            {
                _listener?.Stop();
                _listener = (TCon)Activator.CreateInstance(typeof(TCon), new object[] { _listenerConfig });
                _listener.ClientConnectedEvent += ListenerClientConnectedEventHandler;
                _listener.Start();
            }
        }

        public void Stop()
        {
            lock(_syncListener)
            {
                _listener.ClientConnectedEvent -= ListenerClientConnectedEventHandler;
                _listener.Stop();
                _listener = default(TCon);
            }
        }

        private void ListenerClientConnectedEventHandler(IConnection connection)
        {
            connection.ConnectionClosedEvent += ClientConnectionClosedEventHandler;
            connection.ConnectionErrorEvent += ClientConnectionErrorEventHandler;
            connection.ConnectionDataReadEvent += ClientConnectionDataReadEventHandler;

            if (_clients.TryAdd(connection.RID, connection))
                Interlocked.Increment(ref _clientCount);

            connection.Start();
        }

        #region Client connection event handlers
        private void ClientConnectionDataReadEventHandler(IConnection connection, byte[] data)
        {
            ConnectionDataReadDelegate eventSubscribers = ClientConnectionDataReadEvent;

            if (eventSubscribers == null) return;

            foreach (Delegate d in eventSubscribers.GetInvocationList())
            {
                try
                {
                    d.DynamicInvoke(connection, data);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }
        }

        private void ClientConnectionErrorEventHandler(IConnection connection, Exception exception)
        {
            ConnectionErrorDelegate eventSubscribers = ClientConnectionErrorEvent;

            if (eventSubscribers == null) return;

            foreach(Delegate d in eventSubscribers.GetInvocationList())
            {
                try
                {
                    d.DynamicInvoke(connection, exception);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }
        }

        private void ClientConnectionClosedEventHandler(IConnection connection)
        {
            IConnection tmpCon;

            if (_clients.TryRemove(connection.RID, out tmpCon))
                Interlocked.Decrement(ref _clientCount);

            connection.ConnectionClosedEvent -= ClientConnectionClosedEventHandler;
            connection.ConnectionErrorEvent -= ClientConnectionErrorEventHandler;
            connection.ConnectionDataReadEvent -= ClientConnectionDataReadEventHandler;
        }
        #endregion

    }
}
