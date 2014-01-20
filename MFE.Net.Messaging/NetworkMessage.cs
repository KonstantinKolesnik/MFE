using MFE.Core;
using System;
using System.Collections;
using System.Ext.Xml;
using System.IO;
using System.Text;
using System.Xml;

namespace MFE.Net.Messaging
{
    public class NetworkMessage
    {
        #region Fields
        private string id;
        private Hashtable parameters = new Hashtable();
        #endregion

        #region Properties
        public string ID
        {
            get { return id; }
        }
        public string this[string parameterName]
        {
            get
            {
                return parameters.Contains(parameterName) ? (string)parameters[parameterName] : null;
            }
            set
            {
                if (parameters.Contains(parameterName))
                    parameters[parameterName] = value;
                else
                    parameters.Add(parameterName, value);
            }
        }
        public int ParametersCount
        {
            get { return parameters.Count; }
        }
        #endregion

        #region Constructor
        public NetworkMessage(string id)
        {
            if (Utils.StringIsNullOrEmpty(id))
                throw new ArgumentNullException("NetworkMessage ID not specified");

            this.id = id;
        }
        #endregion

        #region Public Methods
        public static NetworkMessage FromString(string str, NetworkMessageFormat format)
        {
            return (format == NetworkMessageFormat.XML ? FromXml(str) : FromText(str));
        }
        public byte[] Pack(NetworkMessageFormat format)
        {
            return Encoding.UTF8.GetBytes(PackToString(format));
        }
        public string PackToString(NetworkMessageFormat format)
        {
            return NetworkMessageDelimiters.BOM +
                (format == NetworkMessageFormat.XML ? ToXml() : ToText()) +
                NetworkMessageDelimiters.EOM;
        }
        #endregion

        #region Private methods
        private string ToXml()
        {
            MemoryStream ms = new MemoryStream();
            using (XmlWriter writer = XmlWriter.Create(ms))
            {
                //writer.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"utf-8\"");

                writer.WriteStartElement("MSG");
                writer.WriteAttributeString("ID", id);

                foreach (string key in parameters.Keys)
                {
                    writer.WriteStartElement("P");
                    writer.WriteAttributeString("Name", key);
                    writer.WriteString((string)parameters[key]);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.Flush();
                writer.Close();
            }

            return new string(Encoding.UTF8.GetChars(ms.ToArray()));
        }
        private string ToText()
        {
            string res = "ID=" + id + ";";
            foreach (string key in parameters.Keys)
                res += key + "=" + (string)parameters[key] + ";";

            return res;
        }

        private static NetworkMessage FromXml(string xml)
        {
            NetworkMessage msg = null;

            byte[] bb = Encoding.UTF8.GetBytes(xml);
            if (bb != null && bb.Length != 0)
            {
                try
                {
                    MemoryStream xmlStream = new MemoryStream(bb);

                    XmlReaderSettings ss = new XmlReaderSettings();
                    ss.IgnoreWhitespace = true;
                    ss.IgnoreComments = true;
                    using (XmlReader reader = XmlReader.Create(xmlStream, ss))
                    {
                        while (!reader.EOF)
                        {
                            reader.Read();
                            switch (reader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    switch (reader.Name)
                                    {
                                        case "MSG":
                                            msg = new NetworkMessage(reader.GetAttribute("ID"));
                                            break;
                                        case "P":
                                            if (msg != null)
                                                msg[reader.GetAttribute("Name")] = reader.ReadString();
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    msg = null;
                }
            }

            return msg;
        }
        private static NetworkMessage FromText(string txt)
        {
            NetworkMessage msg = null;

            Hashtable parameters = new Hashtable();

            try
            {
                string[] pairs = txt.Split(new Char[] { ';' });
                foreach (string pair in pairs)
                {
                    if (!Utils.StringIsNullOrEmpty(pair))
                    {
                        string[] s = pair.Split(new Char[] { '=' });
                        parameters.Add(s[0], s[1]);
                    }
                }

                if (parameters.Contains("ID"))
                {
                    msg = new NetworkMessage((string)parameters["ID"]);
                    foreach (string key in parameters.Keys)
                        if (key != "ID")
                            msg[key] = (string)parameters[key];
                }
            }
            catch { }

            return msg;
        }
        #endregion
    }
}
