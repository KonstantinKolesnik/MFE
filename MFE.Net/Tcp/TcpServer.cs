using Microsoft.SPOT;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MFE.Net.Tcp
{
    public class TcpServer
    {
        #region Fields
        private int port;
        private Socket serverSocket;
        private bool isStopped = true;
        private ArrayList sessions = new ArrayList();
        #endregion

        #region Properties
        public bool IsActive
        {
            get { return serverSocket != null && !isStopped; }
        }
        public ArrayList Sessions
        {
            get { return sessions; }
        }
        #endregion

        #region Events
        public event TcpSessionEventHandler SessionConnected;
        public event TcpSessionDataReceivedEventHandler SessionDataReceived;
        public event TcpSessionEventHandler SessionDisconnected;
        #endregion

        #region Constructor
        public TcpServer(int port)
        {
            this.port = port;
        }
        #endregion

        #region Public methods
        public void Start()
        {
            if (serverSocket == null)
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint localEP = new IPEndPoint(IPAddress.Any, port);
                serverSocket.Bind(localEP);
                serverSocket.Listen(int.MaxValue);

                isStopped = false;

                new Thread(delegate
                {
                    using (serverSocket)
                    {
                        while (!isStopped)
                        {
                            try
                            {
                                if (serverSocket.Poll(1000 * 10, SelectMode.SelectRead))
                                {
                                    TcpSession session = new TcpSession(serverSocket.Accept());
                                    session.DataReceived += new TcpSessionDataReceivedEventHandler(NetworkClient_DataReceived);
                                    session.Disconnected += new TcpSessionEventHandler(NetworkClient_Disconnected);
                                    sessions.Add(session);

                                    if (SessionConnected != null)
                                        SessionConnected(session);

                                    session.Start();
                                }
                            }
                            catch (SocketException e)
                            {
                                if (e.ErrorCode != 10035)
                                    break;
                            }
                            catch (Exception e)
                            {
                                Debug.Print(e.Message);
                                break;
                            }
                            Thread.Sleep(0);
                        }
                    }

                    serverSocket.Close();
                    serverSocket = null;
                }).Start();
            }
        }
        public void Stop()
        {
            isStopped = true;
        }
        public void SendToAll(byte[] buffer)
        {
            foreach (TcpSession session in sessions)
                session.Send(buffer);
        }
        #endregion

        #region Event handlers
        private bool NetworkClient_DataReceived(TcpSession session, byte[] data)
        {
            return SessionDataReceived != null ? SessionDataReceived(session, data) : false;
        }
        private void NetworkClient_Disconnected(TcpSession session)
        {
            if (SessionDisconnected != null)
                SessionDisconnected(session);

            if (sessions.Contains(session))
                sessions.Remove(session);
        }
        #endregion
    }
}
