using Konak.Net.Sockets.TCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Net.Sockets
{
    public class Client<TCon> : IClient where TCon : IConnection
    {
        private TCon _server = default(TCon);
        private ConnectionConfig _connectionConfig;
        private object _serverSync = new object();

        #region Events
        public event ConnectionErrorDelegate ConnectionErrorEvent;
        public event ConnectionDataReadDelegate ConnectionDataReadEvent;
        public event ConnectionClosedDelegate ConnectionClosedEvent;
        public event ErrorDelegate ErrorEvent;
        #endregion

        #region Constructors
        public Client(ConnectionConfig config)
        {
            _connectionConfig = config;
        }

        public Client(IPEndPoint ipEndPoint) : this(new ConnectionConfig() { Certificate = null, Mode = ConnectionMode.client, SecurityType = ConnectionSecurityType.none, Network = new ConnectionConfig.ConnectionNetworkConfig { RemoteEndPoint = ipEndPoint, AddressFamily = ipEndPoint.AddressFamily, ProtocolType = System.Net.Sockets.ProtocolType.Tcp, SocketType = System.Net.Sockets.SocketType.Stream } })
        {

        }

        public Client(string ip, int port) : this(new ConnectionConfig() { Certificate = null, Mode = ConnectionMode.client, SecurityType = ConnectionSecurityType.none, Network = new ConnectionConfig.ConnectionNetworkConfig { RemoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port), AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork, ProtocolType = System.Net.Sockets.ProtocolType.Tcp, SocketType = System.Net.Sockets.SocketType.Stream } })
        {

        }
        #endregion

        private void RaiseErrorEvent(Exception exception)
        {
            ErrorDelegate eventSubscribers = ErrorEvent;

            if (eventSubscribers == null) return;

            foreach (Delegate d in eventSubscribers.GetInvocationList())
            {
                try
                {
                    d.DynamicInvoke(exception);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }
        }

        #region Client start / stop functionality
        public void Start()
        {
            lock (_serverSync)
            {
                StopPrivate();

                _server = (TCon)Activator.CreateInstance(typeof(TCon), new object[] { _connectionConfig });

                _server.ConnectionErrorEvent += ServerConnectionErrorEventHandler;
                _server.ConnectionDataReadEvent += ServerConnectionDataReadEventHandler;
                _server.ConnectionClosedEvent += ServerConnectionClosedEventHandler;

                _server.Start();
            }
        }

        private void StopPrivate()
        {
            if (_server == null) return;

            _server.ConnectionErrorEvent -= ServerConnectionErrorEventHandler;
            _server.ConnectionDataReadEvent -= ServerConnectionDataReadEventHandler;
            _server.ConnectionClosedEvent -= ServerConnectionClosedEventHandler;

            _server.Stop();

            _server = default(TCon);
        }

        public void Stop()
        {
            lock(_serverSync)
                StopPrivate();
        }
        #endregion

        #region Connection event handlers
        private void ServerConnectionClosedEventHandler(IConnection connection)
        {
            ConnectionClosedDelegate eventSubscribers = ConnectionClosedEvent;

            if (eventSubscribers == null) return;

            foreach (Delegate d in eventSubscribers.GetInvocationList())
            {
                try
                {
                    d.DynamicInvoke(connection);
                }
                catch (Exception ex)
                {
                    RaiseErrorEvent(ex);
                }
            }
        }

        private void ServerConnectionDataReadEventHandler(IConnection connection, byte[] data)
        {
            ConnectionDataReadDelegate eventSubscribers = ConnectionDataReadEvent;

            if (eventSubscribers == null) return;

            foreach (Delegate d in eventSubscribers.GetInvocationList())
            {
                try
                {
                    d.DynamicInvoke(connection, data);
                }
                catch (Exception ex)
                {
                    RaiseErrorEvent(ex);
                }
            }
        }

        private void ServerConnectionErrorEventHandler(IConnection connection, Exception exception)
        {
            ConnectionErrorDelegate eventSubscribers = ConnectionErrorEvent;

            if (eventSubscribers == null) return;

            foreach (Delegate d in eventSubscribers.GetInvocationList())
            {
                try
                {
                    d.DynamicInvoke(connection, exception);
                }
                catch (Exception ex)
                {
                    RaiseErrorEvent(ex);
                }
            }
        }
        #endregion

        public void Send(byte[] data)
        {
            TCPMessage message = new TCPMessage(FrameOperationCode.bin, data);

            lock (_serverSync)
                _server.Send(message.GetBytes());
        }

    }
}
