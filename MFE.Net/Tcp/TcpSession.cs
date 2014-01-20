using Microsoft.SPOT;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MFE.Net.Tcp
{
    public class TcpSession
    {
        #region Fields
        private Socket clientSocket;
        internal bool IsHandshaked = false;
        #endregion

        #region Properties
        public EndPoint EndPoint
        {
            get { return clientSocket != null ? clientSocket.RemoteEndPoint : null; }
        }
        public object Tag
        {
            get; set;
        }
        #endregion

        #region Events
        internal event TcpSessionEventHandler Disconnected;
        internal event TcpSessionDataReceivedEventHandler DataReceived;
        #endregion

        #region Constructor
        internal TcpSession(Socket clientSocket)
        {
            this.clientSocket = clientSocket;
        }
        #endregion

        #region Public methods
        public void Send(byte[] buffer)
        {
            if (clientSocket != null && buffer != null && buffer.Length != 0)
            {
                // for debug:
                //string s = new string(Encoding.UTF8.GetChars(buffer));

                int offset = 0;
                int sent = 0;
                int len = buffer.Length;
                while (len > 0)
                {
                    try
                    {
                        sent = clientSocket.Send(buffer, offset, len, SocketFlags.None);
                        len -= sent;
                        offset += sent;
                    }
                    catch (SocketException e)
                    {
                        if (e.ErrorCode != 10035)
                        {
                            Debug.Print("Error in NetworkClient::Send(byte[] buffer): " + e.Message);
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Print("Error in NetworkClient::Send(byte[] buffer): " + e.Message);
                        break;
                    }
                }
            }
        }
        #endregion

        #region Private methods
        internal void Start()
        {
            if (clientSocket != null)
                new Thread(delegate
                {
                    using (clientSocket)
                    {
                        while (true)
                        {
                            try
                            {
                                if (clientSocket.Poll(1000 * 10, SelectMode.SelectRead)) // wait for the client request
                                {
                                    if (clientSocket.Available == 0) // connection has been closed, reset, or terminated.
                                        break;

                                    byte[] buffer = new byte[clientSocket.Available];
                                    int bytesRead = clientSocket.Receive(buffer, clientSocket.Available, SocketFlags.None);
                                    if (bytesRead > 0)
                                    {
                                        byte[] buffer2 = new byte[bytesRead];
                                        buffer.CopyTo(buffer2, 0);

                                        try
                                        {
                                            bool disconnect = (DataReceived != null ? DataReceived(this, buffer2) : false);
                                            if (disconnect)
                                                break;
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                            }
                            catch (SocketException e)
                            {
                                Debug.Print("Error in TCPSession::Start(): " + e.Message + "; ErrorCode=" + e.ErrorCode);
                                if (e.ErrorCode != 10035)
                                    break;
                            }
                            catch (Exception e)
                            {
                                Debug.Print("Error in TCPSession::Start(): " + e.Message);
                                break;
                            }
                            Thread.Sleep(0);
                        }
                    }

                    if (Disconnected != null)
                        Disconnected(this);

                    clientSocket.Close();
                    clientSocket = null;
                }).Start();
        }
        #endregion
    }
}
