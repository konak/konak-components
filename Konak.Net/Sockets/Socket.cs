using Konak.Net.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.IO;

namespace Konak.Net.Sockets
{
    public delegate void StreamClosedDelegate(ConnectionBak socket);
    public delegate void StreamDataReadDelegate(ConnectionBak socket, byte[] data);
    public delegate void StreamClientConnectedDelegate(ConnectionBak socket);
    public delegate void StreamErrorDelegate(ConnectionBak socket, Exception exception);


    public partial class ConnectionBak
    {
        #region events

        public event StreamClosedDelegate ConnectionClosedEvent;
        public event StreamClientConnectedDelegate ClientConnectedEvent;
        public event StreamErrorDelegate SocketErrorEvent;
        public event StreamDataReadDelegate SocketDataReadEvent;

        #endregion

        private const int BUF_SIZE = 2048000;


        private System.Net.Sockets.Socket _socketBase;
        private System.Net.EndPoint _hostEndPoint;
        private System.IO.Stream _stream;
        //private SocketConfig _socketConfig;
        private ConnectionSecurityType _connectionSecurityType;
        private X509Certificate2 _certificate;

        private RemoteCertificateValidationCallback certValidationCallback;
        //private SecureConnectionResultsCallback connectionCallback;

        private object _connectionSynch;
        private object _readSynch;
        private object _writeSynch;

        private byte[] _buffer;
        private int _read;

        private long _bytesRead;
        private long _bytesWrite;
        private long _bytesReadWrite;

        public Guid RID { get; private set; }
        public long BytesRead { get { return _bytesRead; } }
        public long BytesWrite { get { return _bytesWrite; } }
        public long BytesReadWrite { get { return _bytesReadWrite; } }

        #region Constructors
        private ConnectionBak(ConnectionSecurityType securityType, string certificatePath = null)
        {
            RID = Guid.NewGuid();
            _connectionSynch = new object();
            _connectionSecurityType = securityType;
            _readSynch = new object();
            _writeSynch = new object();
            _buffer = new byte[BUF_SIZE];
            _read = 0;

            _bytesRead = 0;
            _bytesWrite = 0;
            _bytesReadWrite = 0;

            _certificate = null;

            if (!string.IsNullOrEmpty(certificatePath))
            {
                _certificate = new X509Certificate2(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), certificatePath));
            }

            certValidationCallback = new RemoteCertificateValidationCallback(OnCertValidationCallbac);
        }

        private bool OnCertValidationCallbac(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public ConnectionBak(ConnectionSecurityType securityType, SocketConfig config, string certificatePath = null)
            : this(securityType, certificatePath)
        {
            //_socketConfig = config;
            _socketBase = new System.Net.Sockets.Socket(config.AddressFamily, config.SocketType, config.ProtocolType);
            _hostEndPoint = config.EndPoint;
        }

        public ConnectionBak(ConnectionSecurityType securityType, System.Net.Sockets.Socket socket, string certificatePath = null)
            : this(securityType, certificatePath)
        {
            _socketBase = socket;

            _stream = GetStream(securityType, socket);

            if (securityType == ConnectionSecurityType.ssl)
            {
                ((System.Net.Security.SslStream)_stream).AuthenticateAsServer(_certificate, false, System.Security.Authentication.SslProtocols.Default, false);
            }
        }

        #endregion


        #region event raising methods
        private System.IO.Stream GetStream(ConnectionSecurityType securityType, System.Net.Sockets.Socket socket)
        {
            switch (securityType)
            {
                case ConnectionSecurityType.none:
                    return new System.Net.Sockets.NetworkStream(socket, true);

                case ConnectionSecurityType.ssl:
                    return new System.Net.Security.SslStream(new System.Net.Sockets.NetworkStream(socket, true),false, certValidationCallback, null, EncryptionPolicy.NoEncryption);

                default:
                    throw new NotImplementedException(securityType.ToString() + " streaming type is not implemented.");
            }
        }

        #region private methods

        #region RaiseSocketDataReadEvent
        private void RaiseSocketDataReadEvent(byte[] data)
        {
            StreamDataReadDelegate eventSubscribers = SocketDataReadEvent;

            if (eventSubscribers == null) return;

            foreach (Delegate d in eventSubscribers.GetInvocationList())
            {
                try
                {
                    d.DynamicInvoke(this, data);
                }
                catch (Exception) { }
            }
        }
        #endregion

        #region RaiseConnectionClosedEvent
        private void RaiseConnectionClosedEvent()
        {
            StreamClosedDelegate eventSubscribers = ConnectionClosedEvent;

            if (eventSubscribers == null) return;

            foreach(Delegate d in eventSubscribers.GetInvocationList())
            {
                try
                {
                    d.DynamicInvoke(this);
                }
                catch (Exception) { }
            }
        }
        #endregion

        #region RaiseClientConnectedEvent
        private void RaiseClientConnectedEvent(ConnectionBak clientSocket)
        {
            StreamClientConnectedDelegate eventSubscribers = ClientConnectedEvent;

            if (eventSubscribers == null) return;

            foreach (Delegate d in eventSubscribers.GetInvocationList())
            {
                try
                {
                    d.DynamicInvoke(clientSocket);
                }
                catch (Exception) { }
            }
        }
        #endregion

        #region RaiseSocketErrorEvent
        private void RaiseSocketErrorEvent(Exception ex)
        {
            StreamErrorDelegate eventSubscribers = SocketErrorEvent;

            if (eventSubscribers == null) return;

            foreach (Delegate d in eventSubscribers.GetInvocationList())
            {
                try
                {
                    d.DynamicInvoke(this, ex);
                }
                catch (Exception) { }
            }

            Root.RaiseErrorEvent(this, ex);
        }
        #endregion
        
        #endregion

        #region Init
        public void Init(SocketMode mode)
        {
            switch (mode)
            {
                case SocketMode.client:
                    BeginConnect();
                    break;

                case SocketMode.connected_client:
                    BeginReceive();
                    break;

                case SocketMode.server:
                    InitServerSocket();
                    break;
            }
        }
        #endregion

        #region InitServerSocket
        private void InitServerSocket()
        {
            Exception exx = null;

            try
            {
                lock (_connectionSynch)
                {
                    if (_socketBase == null) return;

                    _socketBase.Bind(_hostEndPoint);

                    _socketBase.Listen(1024);
                }
            }
            catch (ArgumentNullException ex)
            {
                exx = new SocketGenericException("localEP is null.", ex);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                exx = new SocketGenericException("An error occurred when attempting to access the socket. ErrorCode: " + ex.ErrorCode + " NativeErrorCode: " + ex.NativeErrorCode + " SocketErrorCode: " + ex.SocketErrorCode, ex);
            }
            catch (ObjectDisposedException ex)
            {
                exx = new SocketGenericException("The Socket has been closed.", ex);
            }
            catch (System.Security.SecurityException ex)
            {
                exx = new SocketGenericException("A caller higher in the call stack does not have permission for the requested operation.", ex);
            }
            catch (Exception ex)
            {
                exx = new SocketGenericException(ex);
            }

            if (exx != null)
            {
                RaiseSocketErrorEvent(exx);

                CloseSocket();

                return;
            }

            BeginAcceptConnection();
        }
        #endregion

        #region AcceptConnection methods

        #region BeginAcceptConnection
        private void BeginAcceptConnection()
        {
            Exception exx = null;

            try
            {
                lock (_connectionSynch)
                {
                    if (_socketBase == null) return;

                    _socketBase.BeginAccept(new AsyncCallback(BeginAcceptCompleted), null);
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
            catch (System.Net.Sockets.SocketException ex)
            {
                exx = new SocketGenericException("An error occurred when attempting to access the socket. See the Remarks section for more information.", ex);
            }
            catch (Exception ex)
            {
                exx = new SocketGenericException(ex);
            }

            if(exx != null)
            {
                RaiseSocketErrorEvent(exx);

                CloseSocket();
            }
        }
        #endregion

        #region BeginAcceptCompleted
        private void BeginAcceptCompleted(IAsyncResult result)
        {
            System.Net.Sockets.Socket clientSocket = null;
            Exception exx = null;

            try
            {
                lock (_connectionSynch)
                {
                    if (_socketBase == null) return;
                    
                    clientSocket = _socketBase.EndAccept(result);
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
            catch (System.Net.Sockets.SocketException ex)
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
                    RaiseSocketErrorEvent(exx);

                    CloseSocket();
                }
                else
                {
                    RaiseClientConnectedEvent(new ConnectionBak(_connectionSecurityType, clientSocket, "Server.pfx"));

                    BeginAcceptConnection();
                }
            }
        }
        #endregion
        #endregion

        #region Connect methods
        #region BeginConnect
        private void BeginConnect()
        {
            Exception exx = null;

            try
            {
                lock (_connectionSynch)
                {
                    if (_socketBase == null) return;

                    _socketBase.BeginConnect(_hostEndPoint, new AsyncCallback(BeginConnectCompleted), null);
                }
            }
            catch (ArgumentNullException ex)
            {
                exx = new SocketGenericException("address is null.", ex);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                exx = new SocketGenericException("An error occurred when attempting to access the socket.", ex);
            }
            catch (ObjectDisposedException ex)
            {
                exx = new SocketGenericException("The Socket has been closed.", ex);
            }
            catch (NotSupportedException ex)
            {
                exx = new SocketGenericException("The Socket is not in the socket family.", ex);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                exx = new SocketGenericException("The port number is not valid.", ex);
            }
            catch (ArgumentException ex)
            {
                exx = new SocketGenericException("The length of address is zero.", ex);
            }
            catch (InvalidOperationException ex)
            {
                exx = new SocketGenericException("The Socket is Listening.", ex);
            }
            catch (Exception ex)
            {
                exx = new SocketGenericException(ex);
            }

            if(exx != null)
            {
                RaiseSocketErrorEvent(exx);

                CloseSocket();
            }
        }
        #endregion

        #region BeginConnectCompleted
        private void BeginConnectCompleted(IAsyncResult result)
        {
            Exception exx = null;

            try
            {
                lock (_connectionSynch)
                {
                    if (_socketBase == null) return;

                    _socketBase.EndConnect(result);
                }

                _stream = GetStream(_connectionSecurityType, _socketBase);

            }
            catch (ArgumentNullException ex)
            {
                exx = new SocketGenericException("asyncResult is null.", ex);
            }
            catch (ArgumentException ex)
            {
                exx = new SocketGenericException("asyncResult was not returned by a call to the BeginConnect method.", ex);
            }
            catch (ObjectDisposedException ex)
            {
                exx = new SocketGenericException("The Socket has been closed.", ex);
            }
            catch (InvalidOperationException ex)
            {
                exx = new SocketGenericException("EndConnect was previously called for the asynchronous connection.", ex);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                exx = new SocketGenericException("An error occurred when attempting to access the socket. ErrorCode: " + ex.ErrorCode.ToString() + " Native Error Code: " + ex.NativeErrorCode.ToString() + " SocketErrorCode: " + ex.SocketErrorCode.ToString(), ex);
            }
            catch (Exception ex)
            {
                exx = new SocketGenericException(ex);
            }

            if(exx != null)
            {
                RaiseSocketErrorEvent(exx);

                CloseSocket();

                return;
            }

            if(this._connectionSecurityType == ConnectionSecurityType.ssl)
            {
                ((SslStream)_stream).BeginAuthenticateAsClient("", new AsyncCallback(BeginAuthenticateAsClientComplete), null);
            }

            //if(_connectionSecurityType == ConnectionSecurityType.ssl)
            //{
            //    ((System.Net.Security.SslStream)_stream).AuthenticateAsClient("H1");
            //}
        }
        #endregion
        #endregion

        private void BeginAuthenticateAsClientComplete(IAsyncResult res)
        {
            SslStream stream = _stream as SslStream;

            try
            {
                stream.EndAuthenticateAsClient(res);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #region Receive data methods

        #region BeginReceive
        private void BeginReceive()
        {
            //System.Net.Sockets.SocketError errorCode;
            Exception exx = null;

            try
            {
                _stream.BeginRead(_buffer, 0, BUF_SIZE, new AsyncCallback(BeginReceiveComplete), null);
                //_stream.BeginReceive(_buffer, 0, BUF_SIZE, System.Net.Sockets.SocketFlags.None, out errorCode, new AsyncCallback(BeginReceiveComplete), null);
                //_socketBase.BeginReceive(_buffer, _read, BUF_SIZE - _read, System.Net.Sockets.SocketFlags.None, out errorCode, new AsyncCallback(BeginReceiveComplete), null);

                //if (errorCode != System.Net.Sockets.SocketError.Success)
                //    exx = new SocketGenericException("Socket error: " + errorCode);
            }
            catch (ArgumentNullException ex)
            {
                exx = new SocketGenericException("buffer is null.", ex);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                exx = new SocketGenericException("An error occurred when attempting to access the socket. ErrorCode: " + ex.ErrorCode + " NativeErrorCode: " + ex.NativeErrorCode + " SocketErrorCode: " + ex.SocketErrorCode, ex);
            }
            catch (ObjectDisposedException ex)
            {
                exx = new SocketGenericException("Socket has been closed.", ex);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                exx = new SocketGenericException("offset is less than 0 or is greater than the length of buffer. Or size is less than 0 or is greater than the length of buffer minus the value of the offset parameter.", ex);
            }
            catch (Exception ex)
            {
                exx = new SocketGenericException(ex);
            }
            finally
            {
                if(exx != null)
                {
                    RaiseSocketErrorEvent(exx);

                    CloseSocket();
                }
            }
        }
        #endregion

        #region BeginReceiveComplete
        private void BeginReceiveComplete(IAsyncResult result)
        {
            //System.Net.Sockets.SocketError errorCode;
            Exception exx = null;

            try
            {
                _read = _stream.EndRead(result);
                //_read = _stream.EndReceive(result, out errorCode);

                //if (errorCode != System.Net.Sockets.SocketError.Success)
                //    exx = new SocketGenericException("Socket error: " + errorCode);

                Interlocked.Add(ref _bytesRead, _read);
                Interlocked.Add(ref _bytesReadWrite, _read);
            }
            catch (ArgumentNullException ex)
            {
                exx = new SocketGenericException("The asyncResult is null.", ex);
            }
            catch (ArgumentException ex)
            {
                exx = new SocketGenericException("asyncResult was not returned by a call to the BeginReceive method.", ex);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                exx = new SocketGenericException("An error occurred when attempting to access the Socket. See the Inner Exception for more information.", ex);
            }
            catch (ObjectDisposedException ex)
            {
                exx = new SocketGenericException("The Socket has been closed.", ex);
            }
            catch (InvalidOperationException ex)
            {
                exx = new SocketGenericException("The EndReceive method was previously called for the asynchronous read.", ex);
            }
            catch (Exception ex)
            {
                exx = new SocketGenericException(ex);
            }

            if(exx != null)
            {
                RaiseSocketErrorEvent(exx);

                CloseSocket();

                return;
            }

            // implement received data processing

            if(_read > 0)
            {
                byte[] data = new byte[_read];

                Array.Copy(_buffer, data, _read);

                RaiseSocketDataReadEvent(data);
            }

            BeginReceive();
        }
        #endregion

        #endregion

        #region Send data methods

        #region Send
        public void Send(byte[] data)
        {
            BeginSend(data);
        }
        #endregion

        #region BeginSend
        private void BeginSend(byte[] data)
        {
            Exception exx = null;
            //System.Net.Sockets.SocketError errorCode;

            try
            {
                //lock (_writeSynch)
                    _stream.BeginWrite(data, 0, data.Length, new AsyncCallback(BeginSendComplete), data.Length);
                    //_stream.BeginSend(data, 0, data.Length, System.Net.Sockets.SocketFlags.None, out errorCode, new AsyncCallback(BeginSendComplete), null);

                //if (errorCode != System.Net.Sockets.SocketError.Success)
                //    exx = new SocketGenericException("Socket error: " + errorCode);
            }
            catch (ArgumentNullException ex)
            {
                exx = new SocketGenericException("buffer is null.", ex);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                exx = new SocketGenericException("An error occurred when attempting to access the socket. ErrorCode: " + ex.ErrorCode + " NativeErrorCode: " + ex.NativeErrorCode + " SocketErrorCode: " + ex.SocketErrorCode, ex);
            }
            catch (ObjectDisposedException ex)
            {
                exx = new SocketGenericException("The Socket has been closed.", ex);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                exx = new SocketGenericException("offset is less than 0. -OR- offset is less than the length of buffer. -OR- size is less than 0. -OR- size is greater than the length of buffer minus the value of the offset parameter.", ex);
            }
            catch (Exception ex)
            {
                exx = new SocketGenericException(ex);
            }

            if(exx != null)
            {
                RaiseSocketErrorEvent(exx);

                CloseSocket();
            }
        }
        #endregion

        #region BeginSendComplete
        private void BeginSendComplete(IAsyncResult res)
        {
            int sent = 0;
            //System.Net.Sockets.SocketError errorCode;
            Exception exx = null;

            try
            {
                _stream.EndWrite(res);
                //sent = _stream.EndSend(res, out errorCode);

                //if (errorCode != System.Net.Sockets.SocketError.Success)
                //    exx = new SocketGenericException("Socket error: " + errorCode);

                Interlocked.Increment(ref _bytesWrite);
                Interlocked.Increment(ref _bytesReadWrite);
            }
            catch (ArgumentNullException ex)
            {
                exx = new SocketGenericException("The asyncResult parameter is null.", ex);
            }
            catch (ArgumentException ex)
            {
                exx = new SocketGenericException("The asyncResult parameter was not returned by a call to a BeginSend method.", ex);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                exx = new SocketGenericException("An error occurred when attempting to access the socket. ErrorCode: " + ex.ErrorCode + " NativeErrorCode: " + ex.NativeErrorCode + " SocketErrorCode: " + ex.SocketErrorCode, ex);
            }
            catch (ObjectDisposedException ex)
            {
                exx = new SocketGenericException("The Socket has been closed.", ex);
            }
            catch (InvalidOperationException ex)
            {
                exx = new SocketGenericException("The EndSend method was previously called for the asynchronous send.", ex);
            }
            catch (Exception ex)
            {
                exx = new SocketGenericException(ex);
            }

            if (exx != null)
            {
                RaiseSocketErrorEvent(exx);

                CloseSocket();
            }

        }
        #endregion

        #endregion

        #region CloseSocket
        private void CloseSocket()
        {
            try
            {
                lock(_connectionSynch)
                    if (_stream != null)
                        _stream.Close();
            }
            catch (Exception ex)
            {
                RaiseSocketErrorEvent(ex);
            }

            RaiseConnectionClosedEvent();
        }
        #endregion

        #endregion

    }
}
