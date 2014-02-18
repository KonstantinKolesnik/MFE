using Microsoft.SPOT.Emulator.Com;

namespace MFE.CustomEmulator.Components
{
    /// <summary>
    /// A dummy wrapper class to ComPortToMemoryStream for the sample emulator, so it can
    /// be identified and hook up to the drop down menu (send text dialog box).
    /// </summary>
    public class VirtualSerialPort : ComPortToMemoryStream, ISerialPortToStream
    {
        bool ISerialPortToStream.Initialize(int BaudRate, int Parity, int DataBits, int StopBits, int FlowValue)
        {
            return true;
        }

        bool ISerialPortToStream.Uninitialize()
        {
            return true;
        }

        bool ISerialPortToStream.SetDataEventHandler(OnEmuSerialPortEvtHandler handler)
        {
            return true;
        }
    }

}
