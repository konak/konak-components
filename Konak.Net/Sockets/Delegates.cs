using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Net.Sockets
{
    public delegate void ConnectionClosedDelegate(IConnection connection);
    public delegate void ConnectionErrorDelegate(IConnection connection, Exception exception);
    public delegate void ConnectionDataReadDelegate(IConnection connection, byte[] data);

    public delegate void ClientConnectedDelegate(IConnection connection);

    //public delegate void ClientDisconnectedDelegate(IConnection client);
    //public delegate void ClientConnectionErrorDelegate(IConnection connection, Exception exception);

    //public delegate void ServerDisconnectedDelegate(IConnection server);
    //public delegate void ServerDataReadDelegate(IConnection server, byte[] data);
    //public delegate void ServerConnectionErrorDelegate(IConnection connection, Exception exception);

    //public delegate void ConnectionErrorDelegate(Connection connection, Exception exception);
    //public delegate void ErrorDelegate(Exception exception);

}
