using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MFE.Net.Udp
{
    public class DiscoveryListener
    {
        #region Fields
        //private const int pollPeriodMsec = 200; // poll period in milliseconds
        private Socket socket;
        //private int port;
        private string name;
        private byte[] response;
        #endregion

        #region Public methods
        /// <summary>
        /// Start listen at port = [port] for request string = [name].
        /// If received, responses with [name + "OK"].
        /// </summary>
        public void Start(int port, string name)
        {
            this.name = name;
            //this.port = port;

            response = Encoding.UTF8.GetBytes(name + "OK");

            if (socket == null)
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
                socket.Bind(new IPEndPoint(IPAddress.Any, port));

                new Thread(Listen).Start();
            }
        }
        #endregion

        #region Private methods
        private void Listen()
        {
            using (socket)
            {
                while (true)
                {
                    try
                    {
                        //if (socket.Poll(pollPeriodMsec * 1000, SelectMode.SelectRead))
                        if (socket.Poll(-1, SelectMode.SelectRead))
                        {
                            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0); // any endpoint
                            byte[] buffer = new byte[socket.Available];

                            int bytesRead = socket.ReceiveFrom(buffer, ref remoteEP);
                            if (bytesRead > 0)
                            {
                                string request = new string(Encoding.UTF8.GetChars(buffer));
                                if (String.Compare(request, name) == 0)
                                    socket.SendTo(response, remoteEP);
                            }
                        }
                        else
                            Thread.Sleep(10);
                    }
                    catch
                    {
                        break;
                    }
                }
            }

            socket = null;
        }
        #endregion
    }
}
