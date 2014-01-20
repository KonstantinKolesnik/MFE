using MFE.Core;
using MFE.Net.Http;
using System.Collections;
using System.Text;

namespace MFE.Net.WebSocket
{
    public class WSClientHandshake
    {
        public string ResourcePath { get; set; }
        public string Host { get; set; } // socket = new WebSocket("ws://" + Host + ":" + port + '/' + resourcePath);
        public string Origin { get; set; }
        public string Key { get; set; }
        public string SubProtocol { get; set; }
        public string Version { get; set; }
        public HttpCookieCollection Cookies { get; set; }
        public Hashtable AdditionalFields { get; set; }

        public bool IsValid
        {
            get { return Host != null && Origin != null && Key != null /*&& ResourcePath != null*/; }
        }

        public override string ToString()
        {
            /*
            GET /mychat HTTP/1.1
            Upgrade: websocket
            Connection: Upgrade
            Host: server.example.com
            Origin: http://example.com
            Sec-WebSocket-Key: x3JJHMbDL1EzLkh9GBhXDw==
            Sec-WebSocket-Protocol: chat
            Sec-WebSocket-Version: 13
            */

            var stringShake = "GET " + ResourcePath + " HTTP/1.1\r\n" +
                              "Upgrade: WebSocket\r\n" +
                              "Connection: Upgrade\r\n" +
                              "Host: " + Host + "\r\n" +
                              "Origin: " + Origin + "\r\n" +
                              "Sec-Websocket-Key: " + Key + "\r\n" +
                              "Sec-WebSocket-Version: " + Version + "\r\n";

            if (Cookies != null)
                stringShake += "Cookie: " + Cookies.ToString() + "\r\n";
            if (SubProtocol != null)
                stringShake += "Sec-Websocket-Protocol: " + SubProtocol + "\r\n";
            if (AdditionalFields != null)
                foreach (var key in AdditionalFields.Keys)
                    stringShake += (string)Key + ": " + (string)AdditionalFields[key] + "\r\n";

            stringShake += "\r\n";

            return stringShake;
        }

        public static WSClientHandshake FromBytes(byte[] buffer)
        {
            WSClientHandshake handshake = new WSClientHandshake();

            string hs = new string(Encoding.UTF8.GetChars(buffer));
            string[] lines = hs.Split('\r', '\n');
            foreach (string line in lines)
            {
                if (!Utils.StringIsNullOrEmpty(line))
                {
                    int idx = line.IndexOf(": ");
                    if (idx == -1) // first line
                        handshake.ResourcePath = line.Split(' ')[1];
                    else
                    {
                        string name = line.Substring(0, idx);
                        string value = line.Substring(idx + 2);

                        switch (name.ToLower())
                        {
                            case "host": handshake.Host = value; break;
                            case "origin": handshake.Origin = value; break;
                            case "sec-websocket-key": handshake.Key = value; break;
                            case "sec-websocket-version": handshake.Version = value; break;
                            case "sec-websocket-protocol": handshake.SubProtocol = value; break;
                            case "cookie":
                                handshake.Cookies = new HttpCookieCollection();
                                string[] cookies = value.Split(';');
                                foreach (string item in cookies)
                                {
                                    int i = item.IndexOf("=");
                                    string c_name = item.Substring(0, i);
                                    string c_value = item.Substring(i + 1);
                                    handshake.Cookies.Add(new HttpCookie(c_name.TrimStart(), c_value));
                                }
                                break;
                            default:
                                if (handshake.AdditionalFields == null)
                                    handshake.AdditionalFields = new Hashtable();
                                handshake.AdditionalFields[name] = value;
                                break;
                        }
                    }
                }
            }

            return handshake;



            //string pattern = @"^(?<connect>[^\s]+)\s(?<path>[^\s]+)\sHTTP\/1\.1\r\n" +  // request line
            //                 @"((?<field_name>[^:\r\n]+):\s(?<field_value>[^\r\n]+)\r\n)+"; // unordered set of fields (name-chars colon space any-chars cr lf)


            //string paramNamePattern = @"[^:\r\n]+";
            //string paramValuePattern = @"[^\r\n]+";

            //string pattern = @"^[\S]+\s([\S]+)\sHTTP\/1\.1\r\n" +
            //    "((" + paramNamePattern + @"):\s(" + paramValuePattern + @")\r\n)+";

            //string pattern2 = @"([^:\r\n]+):\s([^\r\n]+)\r\n";


            // subtract the challenge bytes from the handshake
            //handshake.ChallengeBytes = new byte[8];
            //Array.Copy(buffer, buffer.Length - 8, handshake.ChallengeBytes, 0, 8);

            // get the rest of the handshake
            //byte[] buffer2 = new byte[buffer.Length - 8];
            //Array.Copy(buffer, 0, buffer2, 0, buffer.Length - 8);
            //string utf8_handshake = new string(Encoding.UTF8.GetChars(buffer2));

            //string utf8_handshake = new string(Encoding.UTF8.GetChars(buffer));


            //Regex regex;
            //try
            //{
            //    regex = new Regex(pattern, RegexOptions.IgnoreCase);
            //}
            //catch (Exception e)
            //{
            //    Debug.Print("Exception thrown from method ParseClientHandshake:\n" + e.Message);
            //    return null;
            //}

            //var match = regex.Match(utf8_handshake);
            //var groups = match.Groups;

            //// save the request path
            //handshake.ResourcePath = groups["path"].Value;

            //// run through every match and save them in the handshake object
            //for (int i = 0; i < groups["field_name"].Captures.Count; i++)
            //{
            //    var name = groups["field_name"].Captures[i].ToString();
            //    var value = groups["field_value"].Captures[i].ToString();

            //    switch (name.ToLower())
            //    {
            //        case "sec-websocket-key":
            //            handshake.Key = value;
            //            break;
            //        //case "sec-websocket-key1":
            //        //    handshake.Key1 = value;
            //        //    break;
            //        //case "sec-websocket-key2":
            //        //    handshake.Key2 = value;
            //        //    break;
            //        case "sec-websocket-protocol":
            //            handshake.SubProtocol = value;
            //            break;
            //        case "origin":
            //            handshake.Origin = value;
            //            break;
            //        case "host":
            //            handshake.Host = value;
            //            break;
            //        case "cookie":
            //            //            // create and fill a cookie collection from the data in the handshake
            //            //            handshake.Cookies = new HttpCookieCollection();
            //            //            var cookies = value.Split(';');
            //            //            foreach (var item in cookies)
            //            //            {
            //            //                // the name if before the '=' char
            //            //                var c_name = item.Remove(item.IndexOf('='));
            //            //                // the value is after
            //            //                var c_value = item.Substring(item.IndexOf('=') + 1);
            //            //                // put the cookie in the collection (this also parses the sub-values and such)
            //            //                handshake.Cookies.Add(new HttpCookie(c_name.TrimStart(), c_value));
            //            //            }
            //            break;
            //        default:
            //            //            // some field that we don't know about
            //            //            if (handshake.AdditionalFields == null)
            //            //                handshake.AdditionalFields = new Dictionary<string, string>();
            //            //            handshake.AdditionalFields[name] = value;
            //            break;
            //    }
            //}
            //return handshake;
        }
    }

    public class WSServerHandshake
    {
        private const string magic = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

        public string Accept { get; private set; }
        //public string SubProtocol { get { return "chat"; } }
        public string SubProtocol { get { return null; } }

        public WSServerHandshake(string clientKey)
        {
            byte[] sha1 = SHA.computeSHA1(Encoding.UTF8.GetBytes(clientKey + magic));
            Accept = Utils.ToBase64String(sha1);
        }

        public override string ToString()
        {
            /*
            HTTP/1.1 101 Switching Protocols
            Upgrade: websocket
            Connection: Upgrade
            Sec-WebSocket-Accept: HSmrc0sMlYUkAGmm5OPpG2HaGWk=
            Sec-WebSocket-Protocol: chat
            */

            var stringShake = "HTTP/1.1 101 Switching Protocols\r\n" +
                              "Upgrade: WebSocket\r\n" +
                              "Connection: Upgrade\r\n" +
                              "Sec-Websocket-Accept: " + Accept + "\r\n";

            if (SubProtocol != null)
                stringShake += "Sec-Websocket-Protocol: " + SubProtocol + "\r\n";

            stringShake += "\r\n";

            return stringShake;
        }
    }
}
