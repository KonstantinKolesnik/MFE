using System;
using System.Text;

namespace MFE.Net.WebSocket
{
    /*
      0                   1                   2                   3
      0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
     +-+-+-+-+-------+-+-------------+-------------------------------+
     |F|R|R|R| opcode|M| Payload len |    Extended payload length    |
     |I|S|S|S|  (4)  |A|     (7)     |             (16/64)           |
     |N|V|V|V|       |S|             |   (if payload len==126/127)   |
     | |1|2|3|       |K|             |                               |
     +-+-+-+-+-------+-+-------------+ - - - - - - - - - - - - - - - +
     |     Extended payload length continued, if payload len == 127  |
     + - - - - - - - - - - - - - - - +-------------------------------+
     |                               |Masking-key, if MASK set to 1  |
     +-------------------------------+-------------------------------+
     | Masking-key (continued)       |          Payload Data         |
     +-------------------------------- - - - - - - - - - - - - - - - +
     :                     Payload Data continued ...                :
     + - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - +
     |                     Payload Data continued ...                |
     +---------------------------------------------------------------+
    
    */

    public class WSDataFrame
    {
        #region Fields
        private byte[] m_InnerData;
        private long m_ActualPayloadLength = -1;
        private byte[] m_maskKey = null;
        #endregion

        #region Properties
        public byte[] InnerData
        {
            get { return m_InnerData; }
        }
        public int Length
        {
            get { return m_InnerData.Length; }
        }

        public bool FIN
        {
            get { return ((m_InnerData[0] & 0x80) == 0x80); }
        }
        public bool RSV1
        {
            get { return ((m_InnerData[0] & 0x40) == 0x40); }
        }
        public bool RSV2
        {
            get { return ((m_InnerData[0] & 0x20) == 0x20); }
        }
        public bool RSV3
        {
            get { return ((m_InnerData[0] & 0x10) == 0x10); }
        }
        public sbyte OpCode
        {
            get { return (sbyte)(m_InnerData[0] & 0x0f); }
        }
        public bool HasMask
        {
            get { return ((m_InnerData[1] & 0x80) == 0x80); }
        }
        
        public sbyte PayloadLength
        {
            get { return (sbyte)(m_InnerData[1] & 0x7f); }
        }
        public long ActualPayloadLength // ExtensionData + ApplicationData
        {
            get
            {
                if (m_ActualPayloadLength >= 0)
                    return m_ActualPayloadLength;

                var payloadLength = PayloadLength;

                if (payloadLength < 126)
                    m_ActualPayloadLength = payloadLength;
                else if (payloadLength == 126)
                {
                    m_ActualPayloadLength = (int)m_InnerData[2] * 256 + (int)m_InnerData[3];
                }
                else
                {
                    long len = 0;
                    int n = 1;

                    for (int i = 7; i >= 0; i--)
                    {
                        len += (int)m_InnerData[i + 2] * n;
                        n *= 256;
                    }

                    m_ActualPayloadLength = len;
                }

                return m_ActualPayloadLength;
            }
        }
        
        public byte[] MaskKey // 4 bytes
        {
            get
            {
                if (m_maskKey != null)
                    return m_maskKey;
                
                int length = 4;
                m_maskKey = new byte[length];
                if (HasMask)
                {
                    int offset = MaskKeyOffset;
                    Array.Copy(m_InnerData, offset, m_maskKey, 0, length);
                }

                return m_maskKey;
            }
        }
        private int MaskKeyOffset // byte index
        {
            get
            {
                sbyte payloadLength = PayloadLength;

                if (payloadLength < 126)
                    return 2;
                else if (payloadLength == 126)
                    return 4;
                else
                    return 10;
            }
        }

        public byte[] ExtensionData { get; set; }
        public byte[] ApplicationData { get; set; }
        #endregion

        #region Constructor
        public WSDataFrame(byte[] data) 
        {
            m_InnerData = data;
        }
        #endregion

        #region Public methods
        public void Clear()
        {
            Array.Clear(m_InnerData, 0, m_InnerData.Length);
            ExtensionData = new byte[0];
            ApplicationData = new byte[0];
            m_ActualPayloadLength = -1;
        }
        public bool IsValid()
        {
            //Check RSV bits are 0
            return (m_InnerData[0] & 0x70) == 0x00;
        }
        
        public byte[] GetPayload() // ExtensionData + ApplicationData
        {
            int length = (int)ActualPayloadLength;
            int offset = Length - length;

            if (length > 0)
            {
                byte[] res = new byte[length];
                Array.Copy(InnerData, offset, res, 0, length);

                if (HasMask)
                    DecodeMask(res);

                return res;
            }

            return null;
        }
        private void DecodeMask(byte[] data)
        {
            /*
               To convert masked data into unmasked data, or vice versa, the following
               algorithm is applied. The same algorithm applies regardless of the
               direction of the translation, e.g., the same steps are applied to
               mask the data as to unmask the data.

               Octet i of the transformed data ("transformed-octet-i") is the XOR of
               octet i of the original data ("original-octet-i") with octet at index
               i modulo 4 of the masking key ("masking-key-octet-j"):

                 j                   = i MOD 4
                 transformed-octet-i = original-octet-i XOR masking-key-octet-j
            */

            for (int i = 0; i < data.Length; i++)
            {
                int j = i % 4;
                data[i] = (byte)(data[i] ^ MaskKey[j]);
            }
        }
        #endregion

        #region Wrappers
        public static byte[] WrapString(string message)
        {
            byte[] playloadData = Encoding.UTF8.GetBytes(message);
            return CreateDataFrame(WSOpCode.Text, playloadData, 0, playloadData.Length);
        }
        public static byte[] WrapBinary(byte[] data, int offset, int length)
        {
            return CreateDataFrame(WSOpCode.Binary, data, offset, length);
        }
        //public static byte[] WrapCloseHandshake(int statusCode, string closeReason)
        //{
        //    byte[] playloadData = new byte[(Utils.IsStringNullOrEmpty(closeReason) ? 0 : Encoding.UTF8.GetMaxByteCount(closeReason.Length)) + 2];

        //    int highByte = statusCode / 256;
        //    int lowByte = statusCode % 256;

        //    playloadData[0] = (byte)highByte;
        //    playloadData[1] = (byte)lowByte;

        //    var playloadLength = playloadData.Length;

        //    if (!Utils.IsStringNullOrEmpty(closeReason))
        //    {
        //        int bytesCount = Encoding.UTF8.GetBytes(closeReason, 0, closeReason.Length, playloadData, 2);
        //        playloadLength = bytesCount + 2;
        //    }

        //    return CreateDataFrame(WebSocketOpCode.Close, playloadData, 0, playloadLength);
        //}
        public static byte[] WrapPong(byte[] pong)
        {
            return CreateDataFrame(WSOpCode.Pong, pong, 0, pong.Length);
        }
        public static byte[] WrapPing(byte[] ping)
        {
            return CreateDataFrame(WSOpCode.Ping, ping, 0, ping.Length);
        }
        private static byte[] CreateDataFrame(WSOpCode opCode, byte[] data, int offset, int length)
        {
            byte[] fragment;

            if (length < 126)
            {
                fragment = new byte[2 + length];
                fragment[1] = (byte)length;
            }
            else if (length < 65536)
            {
                fragment = new byte[4 + length];
                fragment[1] = (byte)126;
                fragment[2] = (byte)(length / 256);
                fragment[3] = (byte)(length % 256);
            }
            else
            {
                fragment = new byte[10 + length];
                fragment[1] = (byte)127;

                int left = length;
                int unit = 256;
                for (int i = 9; i > 1; i--)
                {
                    fragment[i] = (byte)(left % unit);
                    left = left / unit;

                    if (left == 0)
                        break;
                }
            }

            fragment[0] = (byte)((int)opCode | 0x80);

            if (length > 0)
                Array.Copy(data, offset, fragment, fragment.Length - length, length);

            return fragment;
        }
        #endregion

        //public string GetWebSocketText()
        //{
        //    int offset = InnerData.Length - (int)ActualPayloadLength;
        //    int length = (int)ActualPayloadLength;

        //    //if (HasMask && length > 0)
        //    //{
        //    //    InnerData.DecodeMask(MaskKey, offset, length);
        //    //}

        //    string text;

        //    if (length > 0)
        //    {
        //        text = InnerData.Decode(Utf8Encoding, offset, length);
        //    }
        //    else
        //    {
        //        text = string.Empty;
        //    }

        //    return text;
        //}







        
        //public void Append(byte[] data)
        //{
        //    int start = 0, end = data.Length - 1;

        //    var bufferList = data.ToList();

        //    bool endIsInThisBuffer = data.Contains(WebSocketConstants.EndByte); // 255 = end
        //    if (endIsInThisBuffer)
        //    {
        //        end = bufferList.IndexOf(WebSocketConstants.EndByte);
        //        end--; // we dont want to include this byte
        //    }

        //    bool startIsInThisBuffer = data.Contains(WebSocketConstants.StartByte); // 0 = start
        //    if (startIsInThisBuffer)
        //    {
        //        var zeroPos = bufferList.IndexOf(WebSocketConstants.StartByte);
        //        if (zeroPos < end) // we might be looking at one of the bytes in the end of the array that hasn't been set
        //        {
        //            start = zeroPos;
        //            start++; // we dont want to include this byte
        //        }
        //    }

        //    builder.Append(Encoding.UTF8.GetString(data, start, (end - start) + 1));

        //    IsComplete = endIsInThisBuffer;
        //}

        //public override string ToString()
        //{
        //    if (builder != null)
        //        return builder.ToString();
        //    else
        //        return "";
        //}
    }
}

/*
        public void Receive(WebSocketDataFrame frame = null)
        {
            if (frame == null)
                frame = new WebSocketDataFrame();

            var buffer = new byte[BufferSize];

            if(Socket == null || !Socket.Connected)
                WebSocket.Disconnected();

            Socket.AsyncReceive(buffer, frame, (sizeOfReceivedData, df) =>
            {
                var dataframe = (DataFrame)df;

                if (sizeOfReceivedData > 0)
                {
                    dataframe.Append(buffer);

                    if (dataframe.IsComplete)
                    {
                        var data = dataframe.ToString();

                        var model = CreateModel(data);
                        var isValid = ModelIsValid(model);

                        // if the model was created it must be valid,
                        if (isValid && Factory != null || model == null && Factory == null)
                        {
                            if (model == null && Factory == null) // if the factory is null, use the raw string
                                model = (object)data;

                            WebSocket.Incoming(model);
                        }

                        Receive();

                    }
                    else // end is not is this buffer
                    {
                        Receive(dataframe); // continue to read
                    }
                }
                else // no data - the socket must be closed
                {
                    WebSocket.Disconnected();
                    Socket.Close();
                }
            });
        }
*/