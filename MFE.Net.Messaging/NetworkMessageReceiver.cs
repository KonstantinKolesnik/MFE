using System.Collections;
using System.Text;

namespace MFE.Net.Messaging
{
    public class NetworkMessageReceiver
    {
        #region Fields
        private string buffer = "";
        private NetworkMessageFormat msgFormat;
        #endregion

        #region Properties
        public NetworkMessageFormat MessageFormat
        {
            get { return msgFormat; }
        }
        #endregion

        #region Constructor
        public NetworkMessageReceiver(NetworkMessageFormat msgFormat)
        {
            this.msgFormat = msgFormat;
        }
        #endregion

        #region Public methods
        public NetworkMessage[] Process(byte[] rawData)
        {
            string s;
            try
            {
                s = new string(Encoding.UTF8.GetChars(rawData));
            }
            catch
            {
                return null;
            }

            return Process(s);
        }
        public NetworkMessage[] Process(string rawData)
        {
            ArrayList msgs = new ArrayList();

            buffer += rawData;

            string payload = null;
            do
            {
                payload = FindPayload();
                if (payload != null)
                {
                    NetworkMessage msg = NetworkMessage.FromString(payload, msgFormat);
                    if (msg != null)
                        msgs.Add(msg);
                }
            }
            while (payload != null);

            return (NetworkMessage[])msgs.ToArray(typeof(NetworkMessage));
        }
        #endregion

        #region Private methods
        private string FindPayload()
        {
            int a = buffer.IndexOf(NetworkMessageDelimiters.BOM);
            int b = buffer.IndexOf(NetworkMessageDelimiters.EOM);

            if (a != -1 && b != -1) // there's a msg inside of s
            {
                // ccccc<BOM>ccccccccc<EOM>ccccc
                //      a             b

                string data = buffer.Substring(a + NetworkMessageDelimiters.BOM.Length, b - a - NetworkMessageDelimiters.BOM.Length);
                buffer = buffer.Substring(b + NetworkMessageDelimiters.EOM.Length);

                return data;
            }

            return null;
        }
        #endregion
    }
}
