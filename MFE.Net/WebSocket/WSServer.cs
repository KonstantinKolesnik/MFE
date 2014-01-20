using MFE.Net.Tcp;
using System.Text;

namespace MFE.Net.WebSocket
{
    public class WSServer
    {
        #region Fields
        private TcpServer tcpServer;
        //private string origin;
        //private string location;
        #endregion

        #region Properties
        public bool IsActive
        {
            get { return tcpServer.IsActive; }
        }
        #endregion

        #region Events
        public event TcpSessionEventHandler SessionConnected;
        public event TcpSessionDataReceivedEventHandler SessionDataReceived;
        public event TcpSessionEventHandler SessionDisconnected;
        #endregion

        #region Constructor
        /// <summary>
        /// Instantiate a new web socket server
        /// </summary>
        /// <param name="port">the port to run on/listen to</param>
        /// <param name="origin">the url where connections are allowed to come from (e.g. http://localhost)</param>
        /// <param name="location">the url of this web socket server (e.g. ws://localhost:8181)</param>
        public WSServer(int port)//, string origin, string location)
        {
            //this.origin = origin;
            //this.location = location;

            tcpServer = new TcpServer(port);
            tcpServer.SessionConnected += new TcpSessionEventHandler(tcpServer_SessionConnected);
            tcpServer.SessionDataReceived += new TcpSessionDataReceivedEventHandler(tcpServer_SessionReceived);
            tcpServer.SessionDisconnected += new TcpSessionEventHandler(tcpServer_SessionDisconnected);
        }
        #endregion

        #region Public methods
        public void Start()
        {
            tcpServer.Start();
        }
        public void Stop()
        {
            tcpServer.Stop();
        }
        public void SendToAll(string message)
        {
            tcpServer.SendToAll(WSDataFrame.WrapString(message));
        }
        public void SendToAll(byte[] data, int offset, int length)
        {
            tcpServer.SendToAll(WSDataFrame.WrapBinary(data, offset, length));
        }
        #endregion

        #region Event handlers
        private void tcpServer_SessionConnected(TcpSession session)
        {
            if (SessionConnected != null)
                SessionConnected(session);
        }
        private bool tcpServer_SessionReceived(TcpSession session, byte[] data)
        {
            if (!session.IsHandshaked)
            {
                // from tablet:
                /*
                GET /Typhoon HTTP/1.1
                Upgrade: WebSocket
                Connection: Upgrade
                Host: 192.168.0.102:2013
                Origin: http://192.168.0.102:81
                Sec-Websocket-Key: +d6dlnAp7rrQ3otS7Zvi7g==
                Sec-WebSocket-Version: 13
                +d6dlnAp7rrQ3otS7Zvi7g==: websocket
                +d6dlnAp7rrQ3otS7Zvi7g==: Upgrade 
                */

                // from local:
                /*
                GET /Typhoon HTTP/1.1
                Upgrade: WebSocket
                Connection: Upgrade
                Host: localhost:2013
                Origin: http://localhost:81
                Sec-Websocket-Key: K92AZtSFpS+9OgiwcPMheg==
                Sec-WebSocket-Version: 13
                K92AZtSFpS+9OgiwcPMheg==: websocket
                K92AZtSFpS+9OgiwcPMheg==: x-webkit-deflate-frame
                K92AZtSFpS+9OgiwcPMheg==: Upgrade
                */

                // location: ws://localhost:2013
                // origin: "http://localhost:81"

                WSClientHandshake chs = WSClientHandshake.FromBytes(data);
                //if (chs.IsValid && "ws://" + chs.Host == location && chs.Origin == origin)
                //if (chs.IsValid && "ws://" + chs.Host == location)
                if (chs.IsValid)
                {
                    WSServerHandshake shs = new WSServerHandshake(chs.Key);
                    string stringShake = shs.ToString();
                    byte[] byteResponse = Encoding.UTF8.GetBytes(stringShake);
                    session.Send(byteResponse);
                    session.IsHandshaked = true;

                    // for debug:
                    //client.Send(WebSocketDataFrame.WrapString("Hello from server!!!"));

                    return false;
                }
                else
                    return true; // disconnect client
            }
            else
            {
                WSDataFrame frame = new WSDataFrame(data);
                byte[] payload = null;
                if (frame.IsValid() && frame.FIN)
                    payload = frame.GetPayload();

                // for debug:
                //string s = new string(Encoding.UTF8.GetChars(payload));
                //string b = "";
                //b += s;

                return SessionDataReceived != null ? SessionDataReceived(session, payload) : false;
            }
        }
        private void tcpServer_SessionDisconnected(TcpSession session)
        {
            if (SessionDisconnected != null)
                SessionDisconnected(session);
        }
        #endregion
    }
}
