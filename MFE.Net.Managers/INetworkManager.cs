using Microsoft.SPOT;

namespace MFE.Net.Managers
{
    public interface INetworkManager
    {
        event EventHandler Started;
        event EventHandler Stopped;

        void Start();
    }
}
