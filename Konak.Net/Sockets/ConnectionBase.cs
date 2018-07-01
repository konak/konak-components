using Konak.Net.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Konak.Net.Sockets
{
    public abstract class ConnectionBase<TCon> : IConnection where TCon : IConnection
    {
        #region public events

        /// <summary>
        /// Event that fires on connection close
        /// </summary>
        public event ConnectionClosedDelegate ConnectionClosedEvent;

        /// <summary>
        /// Event fired when client is connected to local socket
        /// </summary>
        public event ClientConnectedDelegate ClientConnectedEvent;

        /// <summary>
        /// Event fired on any error on connection
        /// </summary>
        public event ConnectionErrorDelegate ConnectionErrorEvent;

        /// <summary>
        /// Event fired on data received from connection
        /// </summary>
        public event ConnectionDataReadDelegate ConnectionDataReadEvent;

        #endregion

        #region constants

        protected readonly int BUF_SIZE = TCP.TCPFrame.MAX_SIZE * 4;

        #endregion

        #region private properties

        private ConnectionConfig _config;

        private Socket _socket = null;
        private Stream _stream = null;

        private X509Certificate2 _certificate = null;
        private RemoteCertificateValidationCallback _certificateValidationCallback;

        private object _connectionSync = new object();
        private object _readSync = new object();
        private object _writeSync = new object();

        private bool _isWriting = false;

        //private ManualResetEventSlim _writeSyncEvent;

        protected ConcurrentQueue<byte[]> _writeDataQueue = new ConcurrentQueue<byte[]>();
        protected ConcurrentQueue<byte[]> _receivedDataQueue = new ConcurrentQueue<byte[]>();

        private byte[] _buffer;

        protected long _bytesRead = 0;
        protected long _bytesWrite = 0;
        protected long _bytesReadWrite = 0;

        protected bool _raisingDataReadEvent = false;
        protected object _raisingDataReadSync = new object();

        #endregion

        #region public properties

        /// <summary>
        /// ID of connection
        /// </summary>
        public Guid RID { get; private set; }

        /// <summary>
        /// Number of bytes read by connection
        /// </summary>
        public long BytesRead { get { return _bytesRead; } }

        /// <summary>
        /// Number of bytes sent by connection
        /// </summary>
        public long BytesWrite { get { return _bytesWrite; } }

        /// <summary>
        /// Number of bytes sent and received by connection
        /// </summary>
        public long BytesReadWrite { get { return _bytesReadWrite; } }


        #endregion

        #region constructors

        protected ConnectionBase()
        {
            _buffer = new byte[BUF_SIZE];
        }

        public ConnectionBase(ConnectionConfig config) : this()
        {
            _config = config;

            InitCertificate(config.Certificate);
            InitSocket(config.Network);
        }

        private void InitCertificate(ConnectionConfig.ConnectionCertificateConfig config)
        {
            if (_config.SecurityType != ConnectionSecurityType.ssl) return;

            if (config.Certificate == null)
            {
                if (string.IsNullOrEmpty(config.CertificateFilePath))
                {
                    ArgumentException exception = new ArgumentException("Can not init SSL connection while certificate is not provided.", "config.Certificate.CertificateFilePath");

                    Root.RaiseErrorEvent(this, exception);

                    throw exception;
                }

                string filePath = config.CertificateFilePath.Contains(@"\") ? config.CertificateFilePath : Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), config.CertificateFilePath);
                _certificate = new X509Certificate2(filePath);
            }
            else
            {
                _certificate = config.Certificate;
            }

            _certificateValidationCallback = config.CertValidationCallback == null ? new RemoteCertificateValidationCallback(OnCertValidationCallbac) : config.CertValidationCallback;
        }

        private void InitSocket(ConnectionConfig.ConnectionNetworkConfig config)
        {
            if (config.Socket == null)
            {
                _socket = new Socket(config.AddressFamily, config.SocketType, config.ProtocolType);

                if (config.LocalEndPoint != null)
                    _socket.Bind(config.LocalEndPoint);
            }
            else
            {
                _socket = config.Socket;
            }
        }
        #endregion

        #region OnCertValidationCallbac
        private bool OnCertValidationCallbac(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        #endregion

        #region Raise event methods

        #region RaiseConnectionClosedEvent
        private void RaiseConnectionClosedEvent()
        {
            ConnectionClosedDelegate eventSubscribers = ConnectionClosedEvent;

            if (eventSubscribers == null) return;

            foreach (Delegate d in eventSubscribers.GetInvocationList())
            {
                try
                {
                    d.DynamicInvoke(this);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }
        }
        #endregion

        #region RaiseClientConnectedEvent
        private void RaiseClientConnectedEvent(IConnection connection)
        {
            ClientConnectedDelegate eventSubscribers = ClientConnectedEvent;

            if (eventSubscribers == null) return;

            foreach (Delegate d in eventSubscribers.GetInvocationList())
            {
                try
                {
                    d.DynamicInvoke(connection);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }
        }
        #endregion

        #region RaiseConnectionErrorEvent
        private void RaiseConnectionErrorEvent(Exception exception)
        {
            ConnectionErrorDelegate eventSubscribers = ConnectionErrorEvent;

            if (eventSubscribers == null) return;

            foreach (Delegate d in eventSubscribers.GetInvocationList())
            {
                try
                {
                    d.DynamicInvoke(this, exception);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }

            Root.RaiseErrorEvent(this, exception);
        }
        #endregion

        #region RaiseConnectionDataReadEvent
        protected void RaiseConnectionDataReadEvent(byte[] data)
        {
            ConnectionDataReadDelegate eventSubscribers = ConnectionDataReadEvent;

            if (eventSubscribers == null) return;

            foreach (Delegate d in eventSubscribers.GetInvocationList())
            {
                try
                {
                    d.DynamicInvoke(this, data);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }
        }
        #endregion

        #endregion

        public void Start()
        {
            switch (_config.Mode)
            {
                case ConnectionMode.server:
                    InitServer();
                    BeginAcceptConnection();
                    break;

                case ConnectionMode.connected_client:
                    InitStream();
                    switch (_config.SecurityType)
                    {
                        case ConnectionSecurityType.none:
                            BeginRead();
                            break;

                        case ConnectionSecurityType.ssl:
                            BeginAuthenticateAsServer();
                            break;
                    }
                    break;

                case ConnectionMode.client:
                    BeginConnect();
                    break;
            }
        }

        public void Stop()
        {
            CloseConnection();
        }

        #region InitServer
        private void InitServer()
        {
            Exception exception = null;

            try
            {
                lock (_connectionSync)
                {
                    _socket.Listen(1024);
                }
            }
            catch (ArgumentNullException ex)
            {
                exception = new SocketGenericException("localEP is null.", ex);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                exception = new SocketGenericException("An error occurred when attempting to access the socket. ErrorCode: " + ex.ErrorCode + " NativeErrorCode: " + ex.NativeErrorCode + " SocketErrorCode: " + ex.SocketErrorCode, ex);
            }
            catch (ObjectDisposedException ex)
            {
                exception = new SocketGenericException("The Socket has been closed.", ex);
            }
            catch (System.Security.SecurityException ex)
            {
                exception = new SocketGenericException("A caller higher in the call stack does not have permission for the requested operation.", ex);
            }
            catch (Exception ex)
            {
                exception = new SocketGenericException(ex);
            }

            if (exception != null)
            {
                RaiseConnectionErrorEvent(exception);

                CloseConnection();
            }
        }
        #endregion

        #region Accept connection
        private void BeginAcceptConnection()
        {
            Exception exx = null;

            try
            {
                lock (_connectionSync)
                {
                    _socket.BeginAccept(new AsyncCallback(BeginAcceptCompleted), null);
                }
            }
            catch (ObjectDisposedException ex)
            {
                exx = new SocketGenericException("The Socket object has been closed", ex);
            }
            catch (InvalidOperationException ex)
            {
                exx = new SocketGenericException("The accepting socket is not listening for connections. You must call Bind and Listen before calling BeginAccept.  -- OR -- The accepted socket is bound.", ex);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                exx = new SocketGenericException("receiveSize is less than 0.", ex);
            }
            catch (SocketException ex)
            {
                exx = new SocketGenericException("An error occurred when attempting to access the socket. See the Remarks section for more information.", ex);
            }
            catch (Exception ex)
            {
                exx = new SocketGenericException(ex);
            }
            finally
            {
                if (exx != null)
                {
                    RaiseConnectionErrorEvent(exx);

                    CloseConnection();
                }
            }
        }

        private void BeginAcceptCompleted(IAsyncResult result)
        {
            Socket clientSocket = null;
            Exception exx = null;

            try
            {
                lock (_connectionSync)
                {
                    clientSocket = _socket.EndAccept(result);
                }
            }
            catch (ArgumentNullException ex)
            {
                exx = new SocketGenericException("asyncResult is null.", ex);
            }
            catch (ArgumentException ex)
            {
                exx = new SocketGenericException("asyncResult was not created by a call to BeginAccept.", ex);
            }
            catch (SocketException ex)
            {
                exx = new SocketGenericException("An error occurred when attempting to access the socket. See the Remarks section for more information.", ex);
            }
            catch (ObjectDisposedException ex)
            {
                exx = new SocketGenericException("The Socket has been closed.", ex);
            }
            catch (InvalidOperationException ex)
            {
                exx = new SocketGenericException("EndAccept method was previously called.", ex);
            }
            catch (Exception ex)
            {
                exx = new SocketGenericException(ex);
            }
            finally
            {
                if (exx != null)
                {
                    RaiseConnectionErrorEvent(exx);

                    CloseConnection();
                }
                else
                {
                    BeginAcceptConnection();

                    ConnectionConfig cfg = new ConnectionConfig()
                    {
                        Certificate = new ConnectionConfig.ConnectionCertificateConfig()
                        {
                            Certificate = _certificate,
                            CertValidationCallback = _certificateValidationCallback,
                            CertificateFilePath = null
                        },
                        Mode = ConnectionMode.connected_client,
                        SecurityType = _config.SecurityType,
                        Network = new ConnectionConfig.ConnectionNetworkConfig()
                        {
                            AddressFamily = _config.Network.AddressFamily,
                            Socket = clientSocket,
                            LocalEndPoint = null,
                            RemoteEndPoint = null,
                            ProtocolType = ProtocolType.Tcp,
                            SocketType = SocketType.Stream
                        }
                    };

                    RaiseClientConnectedEvent((TCon)Activator.CreateInstance(typeof(TCon), new object[] { cfg }));
                }
            }
        }
        #endregion

        #region CloseConnection
        private void CloseConnection()
        {
            try
            {
                lock (_connectionSync)
                {
                    if (_stream != null) _stream.Close();
                    if (_socket != null) _socket.Close();
                }
            }
            catch (Exception ex)
            {
                RaiseConnectionErrorEvent(ex);
            }

            RaiseConnectionClosedEvent();
        }
        #endregion

        #region InitStream
        private void InitStream()
        {
            Exception exx = null;

            try
            {
                _stream = GetStream();
            }
            catch (ArgumentNullException ex)
            {
                exx = new SocketGenericException("The socket or innerstream parameter is null.", ex);
            }
            catch (ArgumentException ex)
            {
                exx = new SocketGenericException("innerStream is not readable OR innerStream is not writable OR encryptionPolicy is not valid", ex);
            }
            catch (IOException ex)
            {
                exx = new SocketGenericException("The socket parameter is not connected. OR the value of the SocketType property of the socket parameter is not SocketType.Stream OR the socket parameter is in a nonblocking state.", ex);
            }
            catch (Exception ex)
            {
                exx = new SocketGenericException("Unknown exception", ex);
            }
            finally
            {
                if (exx != null)
                {
                    RaiseConnectionErrorEvent(exx);

                    CloseConnection();
                }
            }

            //if(_config.SecurityType == ConnectionSecurityType.ssl)
            //{
            //    BeginAuthenticateAsServer();
            //}
        }
        #endregion

        #region AuthenticateAsServer
        private void BeginAuthenticateAsServer()
        {
            Exception exx = null;
            try
            {
                ((SslStream)_stream).BeginAuthenticateAsServer(_certificate, false, SslProtocols.Tls12, false, new AsyncCallback(BeginAuthenticateAsServerComplete), null);
            }
            catch (ArgumentNullException ex)
            {
                exx = new SocketGenericException("serverCertificate is null", ex);
            }
            catch (ArgumentException ex)
            {
                exx = new SocketGenericException("enabledSslProtocols is not a valid SslProtocols value.", ex);
            }
            catch (AuthenticationException ex)
            {
                exx = new SocketGenericException("The authentication failed and left this object in an unusable state.", ex);
            }
            catch (ObjectDisposedException ex)
            {
                exx = new SocketGenericException("This object has been closed", ex);
            }
            catch (InvalidOperationException ex)
            {
                exx = new SocketGenericException("Authentication has already occurred OR Server authentication using this SslStream was tried previously OR Authentication is already in progress", ex);
            }
            catch (Exception ex)
            {
                exx = new SocketGenericException("Unknown exception", ex);
            }
            finally
            {
                if (exx != null)
                {
                    RaiseConnectionErrorEvent(exx);

                    CloseConnection();
                }
            }
        }

        private void BeginAuthenticateAsServerComplete(IAsyncResult result)
        {
            Exception exx = null;

            try
            {
                ((SslStream)_stream).EndAuthenticateAsServer(result);
            }
            catch (ArgumentNullException ex)
            {
                exx = new SocketGenericException("asyncResult is null.", ex);
            }
            catch (ArgumentException ex)
            {
                exx = new SocketGenericException("asyncResult was not created by a call to BeginAuthenticateAsClient.", ex);
            }
            catch (AuthenticationException ex)
            {
                exx = new SocketGenericException("The authentication failed and left this object in an unusable state.", ex);
            }
            catch (InvalidOperationException ex)
            {
                exx = new SocketGenericException("There is no pending client authentication to complete.", ex);
            }
            catch (Exception ex)
            {
                exx = new SocketGenericException("Unknown exception", ex);
            }
            finally
            {
                if (exx != null)
                {
                    RaiseConnectionErrorEvent(exx);

                    CloseConnection();
                }
                else
                {
                    BeginRead();
                }
            }

        }
        #endregion

        #region Read
        private void BeginRead()
        {
            Exception exx = null;

            try
            {
                _stream.BeginRead(_buffer, 0, BUF_SIZE, new AsyncCallback(BeginReadComplete), null);
            }
            catch (IOException ex)
            {
                exx = new SocketGenericException("Attempted an asynchronous read past the end of the stream, or a disk error occurs.", ex);
            }
            catch (ArgumentException ex)
            {
                exx = new SocketGenericException("One or more of the arguments is invalid.", ex);
            }
            catch (ObjectDisposedException ex)
            {
                exx = new SocketGenericException("Methods were called after the stream was closed.", ex);
            }
            catch (Exception ex)
            {
                exx = new SocketGenericException("Unknown exception.", ex);
            }
            finally
            {
                if (exx != null)
                {
                    RaiseConnectionErrorEvent(exx);

                    CloseConnection();
                }
            }
        }

        private void BeginReadComplete(IAsyncResult result)
        {
            Exception exx = null;
            int read = 0;

            try
            {
                read = _stream.EndRead(result);
            }
            catch (ArgumentNullException ex)
            {
                exx = new SocketGenericException("asyncResult is null.", ex);
            }
            catch (ArgumentException ex)
            {
                exx = new SocketGenericException("A handle to the pending read operation is not available OR The pending operation does not support reading.", ex);
            }
            catch (InvalidOperationException ex)
            {
                exx = new SocketGenericException("asyncResult did not originate from a BeginRead method on the current stream.", ex);
            }
            catch (IOException ex)
            {
                exx = new SocketGenericException("The stream is closed or an internal error has occurred.", ex);
            }
            catch (Exception ex)
            {
                exx = new SocketGenericException("Unknown exception.", ex);
            }

            if (exx != null)
            {
                RaiseConnectionErrorEvent(exx);

                CloseConnection();

                return;
            }

            byte[] data = new byte[read];

            Array.Copy(_buffer, data, read);

            _receivedDataQueue.Enqueue(data);

            BeginRead();

            lock (_raisingDataReadSync)
            {
                if (_raisingDataReadEvent)
                    return;

                _raisingDataReadEvent = true;

                ThreadPool.QueueUserWorkItem(new WaitCallback(StartRaisingConnectionDataReadEvents));
            }
        }

        protected abstract void StartRaisingConnectionDataReadEvents(object state);

        #endregion

        #region Connect
        private void BeginConnect()
        {
            Exception exx = null;

            try
            {
                lock (_connectionSync)
                {
                    _socket.BeginConnect(_config.Network.RemoteEndPoint, new AsyncCallback(BeginConnectComplete), null);
                }
            }
            catch (ArgumentNullException ex)
            {
                exx = new SocketGenericException("remoteEP is null.", ex);
            }
            catch (SocketException ex)
            {
                exx = new SocketGenericException("An error occurred when attempting to access the socket. Socket error code: " + ex.SocketErrorCode + " Native error code: " + ex.NativeErrorCode, ex);
            }
            catch (ObjectDisposedException ex)
            {
                exx = new SocketGenericException("The Socket has been closed.", ex);
            }
            catch (SecurityException ex)
            {
                exx = new SocketGenericException("A caller higher in the call stack does not have permission for the requested operation.", ex);
            }
            catch (InvalidOperationException ex)
            {
                exx = new SocketGenericException("The Socket is Listening.", ex);
            }
            catch (Exception ex)
            {
                exx = new SocketGenericException("Unknown exception.", ex);
            }
            finally
            {
                if (exx != null)
                {
                    RaiseConnectionErrorEvent(exx);

                    CloseConnection();
                }
            }

        }

        private void BeginConnectComplete(IAsyncResult result)
        {
            Exception exx = null;

            try
            {
                lock (_connectionSync)
                {
                    _socket.EndConnect(result);
                }
            }
            catch (ObjectDisposedException ex)
            {
                exx = new SocketGenericException("The Socket has been closed", ex);
            }
            catch (ArgumentNullException ex)
            {
                exx = new SocketGenericException("asyncResult is null.", ex);
            }
            catch (ArgumentException ex)
            {
                exx = new SocketGenericException("asyncResult was not returned by a call to the BeginConnect method.", ex);
            }
            catch (InvalidOperationException ex)
            {
                exx = new SocketGenericException("EndConnect was previously called for the asynchronous connection.", ex);
            }
            catch (SocketException ex)
            {
                exx = new SocketGenericException("An error occurred when attempting to access the socket. Socket error code: " + ex.SocketErrorCode + " Native error code: " + ex.NativeErrorCode, ex);
            }
            catch (Exception ex)
            {
                exx = new SocketGenericException("Unknown exception", ex);
            }
            finally
            {
                if (exx != null)
                {
                    RaiseConnectionErrorEvent(exx);

                    CloseConnection();
                }
            }

            switch (_config.SecurityType)
            {
                case ConnectionSecurityType.ssl:
                    InitStream();
                    BeginAuthenticateAsClient();
                    break;

                case ConnectionSecurityType.none:
                    InitStream();
                    BeginRead();
                    break;
            }
        }

        #endregion

        #region AuthenticateAsClient
        private void BeginAuthenticateAsClient()
        {
            Exception exx = null;

            try
            {
                ((SslStream)_stream).BeginAuthenticateAsClient("", new X509CertificateCollection(new X509Certificate[] { _certificate }), SslProtocols.Tls12, false, new AsyncCallback(BeginAuthenticateAsClientComplete), null);
            }
            catch (ObjectDisposedException ex)
            {
                exx = new SocketGenericException("This object has been closed.", ex);
            }
            catch (ArgumentNullException ex)
            {
                exx = new SocketGenericException("targetHost is null.", ex);
            }
            catch (ArgumentException ex)
            {
                exx = new SocketGenericException("enabledSslProtocols is not a valid SslProtocols value.", ex);
            }
            catch (AuthenticationException ex)
            {
                exx = new SocketGenericException("The authentication failed and left this object in an unusable state.", ex);
            }
            catch (InvalidOperationException ex)
            {
                exx = new SocketGenericException("Authentication has already occurred OR Server authentication using this SslStream was tried previously OR Authentication is already in progress", ex);
            }
            catch (Exception ex)
            {
                exx = new SocketGenericException("Unknown exception.", ex);
            }
            finally
            {
                if (exx != null)
                {
                    RaiseConnectionErrorEvent(exx);

                    CloseConnection();
                }
            }
        }

        private void BeginAuthenticateAsClientComplete(IAsyncResult result)
        {
            Exception exx = null;

            try
            {
                ((SslStream)_stream).EndAuthenticateAsClient(result);
            }
            catch (ArgumentNullException ex)
            {
                exx = new SocketGenericException("asyncResult is null.", ex);
            }
            catch (ArgumentException ex)
            {
                exx = new SocketGenericException("asyncResult was not created by a call to BeginAuthenticateAsServer.", ex);
            }
            catch (AuthenticationException ex)
            {
                exx = new SocketGenericException("The authentication failed and left this object in an unusable state.", ex);
            }
            catch (InvalidOperationException ex)
            {
                exx = new SocketGenericException("There is no pending server authentication to complete.", ex);
            }
            catch (Exception ex)
            {
                exx = new SocketGenericException("Unknown exception.", ex);
            }
            finally
            {
                if (exx == null)
                {
                    BeginRead();
                }
                else
                {
                    RaiseConnectionErrorEvent(exx);

                    CloseConnection();
                }
            }
        }

        #endregion

        #region GetStream
        private Stream GetStream()
        {
            switch (_config.SecurityType)
            {
                case ConnectionSecurityType.none:
                    return new NetworkStream(_socket, true);

                case ConnectionSecurityType.ssl:
                    return new SslStream(new NetworkStream(_socket, true), false, _certificateValidationCallback, null, EncryptionPolicy.RequireEncryption);

                default:
                    throw new NotImplementedException(_config.SecurityType.ToString() + " streaming type is not implemented.");
            }
        }
        #endregion

        #region Write

        private async Task WriteAsync(byte[] data, int offset, int count)
        {
            Exception exx = null;

            try
            {
                await _stream.WriteAsync(data, offset, count); //, new AsyncCallback(BeginWriteComplete), data);

                Interlocked.Add(ref _bytesWrite, data.Length);
                Interlocked.Add(ref _bytesReadWrite, data.Length);
            }
            catch (ArgumentNullException ex)
            {
                exx = new SocketGenericException("buffer is null.", ex);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                exx = new SocketGenericException("offset or count is negative.", ex);
            }
            catch (ArgumentException ex)
            {
                exx = new SocketGenericException("The sum of offset and count is larger than the buffer length.", ex);
            }
            catch (NotSupportedException ex)
            {
                exx = new SocketGenericException("The stream does not support writing.", ex);
            }
            catch (ObjectDisposedException ex)
            {
                exx = new SocketGenericException("The stream has been disposed.", ex);
            }
            catch (InvalidOperationException ex)
            {
                exx = new SocketGenericException("The stream is currently in use by a previous write operation.", ex);
            }
            catch (Exception ex)
            {
                exx = new SocketGenericException("Unknown exception.", ex);
            }
            finally
            {
                if (exx != null)
                {
                    RaiseConnectionErrorEvent(exx);

                    CloseConnection();
                }
            }
        }

        private void StartWriteThread(object state)
        {
            byte[] data;

            try
            {
                while (_writeDataQueue.TryDequeue(out data))
                    BeginWrite(data, 0, data.Length);
            }
            finally
            {
                _isWriting = false;
            }
        }

        private void BeginWrite(byte[] data, int offset, int count)
        {
            Exception exx = null;

            try
            {
                _stream.BeginWrite(data, offset, count, new AsyncCallback(BeginWriteComplete), data);
            }
            catch (IOException ex)
            {
                exx = new SocketGenericException("Attempted an asynchronous write past the end of the stream, or a disk error occurs.", ex);
            }
            catch (ArgumentException ex)
            {
                exx = new SocketGenericException("One or more of the arguments is invalid.", ex);
            }
            catch (ObjectDisposedException ex)
            {
                exx = new SocketGenericException("Methods were called after the stream was closed.", ex);
            }
            catch (Exception ex)
            {
                exx = new SocketGenericException("Unknown exception.", ex);
            }
            finally
            {
                if (exx != null)
                {
                    RaiseConnectionErrorEvent(exx);

                    CloseConnection();
                }
            }
        }

        private void BeginWriteComplete(IAsyncResult result)
        {
            Exception exx = null;
            byte[] data = null;

            try
            {
                data = result.AsyncState as byte[];

                _stream.EndWrite(result);

                Interlocked.Add(ref _bytesWrite, data.Length);
                Interlocked.Add(ref _bytesReadWrite, data.Length);
            }
            catch (IOException ex)
            {
                exx = new SocketGenericException("The stream is closed or an internal error has occurred.", ex);
            }
            catch (ArgumentNullException ex)
            {
                exx = new SocketGenericException("asyncResult is null.", ex);
            }
            catch (ArgumentException ex)
            {
                exx = new SocketGenericException("A handle to the pending write operation is not available OR The pending operation does not support writing.", ex);
            }
            catch (InvalidOperationException ex)
            {
                exx = new SocketGenericException("asyncResult did not originate from a BeginWrite method on the current stream.", ex);
            }
            catch (Exception ex)
            {
                exx = new SocketGenericException("Unknown exception.", ex);
            }
            finally
            {
                if (exx != null)
                {
                    RaiseConnectionErrorEvent(exx);

                    CloseConnection();
                }
            }
        }
        #endregion


        #region Send

        public void Send(byte[] data)
        {
            lock (_writeSync)
            {
                _writeDataQueue.Enqueue(data);

                if (_isWriting) return;

                _isWriting = true;

                ThreadPool.QueueUserWorkItem(new WaitCallback(StartWriteThread));
            }
        }

        public void Send(IEnumerable<byte[]> data)
        {
            lock (_writeSync)
            {
                foreach (byte[] d in data)
                    _writeDataQueue.Enqueue(d);

                if (_isWriting) return;

                _isWriting = true;

                ThreadPool.QueueUserWorkItem(new WaitCallback(StartWriteThread));
            }
        }

        //private async Task SendAsyncThread()
        //{
        //    lock (_writeSync)
        //    {
        //        if (_isWriting) return;
        //        _isWriting = true;
        //    }

        //    byte[] data;

        //    int c, d;

        //    try
        //    {
        //        while (_dataQueue.TryDequeue(out data)) // will exit on InvalidOperationException exception on Dequeue method
        //        {
        //            c = data.Length / BUF_SIZE;
        //            d = data.Length % BUF_SIZE;

        //            for (int i = 0; i < c; i++)
        //                await WriteAsync(data, i * BUF_SIZE, BUF_SIZE);

        //            if (d > 0)
        //                await WriteAsync(data, c * BUF_SIZE, d);
        //        }
        //    }
        //    catch (InvalidOperationException)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Data queue is empty.");
        //    }
        //    catch (Exception ex)
        //    {
        //        RaiseConnectionErrorEvent(ex);
        //        CloseConnection();
        //    }
        //    finally
        //    {
        //        lock (_writeSync)
        //        _isWriting = false;
        //    }
        //}


        #endregion
    }
}
