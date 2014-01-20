
namespace MFE.Net.Tcp
{
    public delegate bool TcpSessionDataReceivedEventHandler(TcpSession session, byte[] data); // returns true if session is to be disconnected
}
