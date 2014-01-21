using GHI.Premium.USBClient;
using MFE.Core;
using System.Text;

namespace MFE.USB.Client
{
    public class USBCDCDevice
    {
        private USBC_CDC cdc;

        public USBCDCDevice()
        {
            cdc = USBClientController.StandardDevices.StartCDC();
        }

        public void Send(string data)
        {
            if (!Utils.StringIsNullOrEmpty(data))
                Send(Encoding.UTF8.GetBytes(data));
        }
        public void Send(byte[] data)
        {
            if (USBClientController.GetState() == USBClientController.State.Running)
                if (data != null && data.Length != 0)
                    cdc.Write(data, 0, data.Length);
        }

        public void Read()
        {

        }
    }
}
