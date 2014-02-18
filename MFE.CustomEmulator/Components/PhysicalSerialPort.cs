using Microsoft.SPOT.Emulator.Com;
using System;
using System.IO.Ports;
using System.Threading;

namespace MFE.CustomEmulator.Components
{
    /// <summary>
    /// PhysicalSerialPort is an serial port emulator component that maps an emulator
    /// serial port to a physical serial port on the host PC
    /// </summary>
    public class PhysicalSerialPort : ComPortToStream, ISerialPortToStream
    {
        #region Fields
        private SerialPort port;
        private string name;
        private int baudRate;
        #endregion

        #region Properties
        /// <summary>
        /// The port name of the physical serial port, such as "COM1" or "COM2"
        /// </summary>
        public string PhysicalPortName
        {
            get { return name; }
            set
            {
                ThrowIfNotConfigurable();
                name = value;
            }
        }

        /// <summary>
        /// The baud rate for the physical serial port, defaults to 38400.
        /// </summary>
        public int BaudRate
        {
            get { return baudRate; }
            set
            {
                ThrowIfNotConfigurable();
                baudRate = value;
            }
        }
        #endregion

        #region Constructor
        public PhysicalSerialPort()
        {
            name = null;
            baudRate = 38400;
        }
        #endregion

        public override void SetupComponent()
        {
            base.SetupComponent();
        }

        bool ISerialPortToStream.Initialize(int BaudRate, int Parity, int DataBits, int StopBits, int FlowValue)
        {
            // From MSDN:  The best practice for any application is to wait for some amount of time after calling the Close 
            // method  before attempting to call the Open method, as the port may not be closed instantly.
            for (int retries = 5; retries != 0; retries--)
            {
                try
                {
                    if (port == null)
                    {
                        port = new SerialPort(name, BaudRate, (Parity)Parity, DataBits);
                        switch (FlowValue)
                        {
                            case 0x18: port.Handshake = Handshake.XOnXOff; break;
                            case 0x06: port.Handshake = Handshake.RequestToSend; break;
                            default: port.Handshake = Handshake.None; break;
                        }
                    }
                    port.Open();
                    Stream = port.BaseStream;
                    return true;
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                }
            }
            return false;
        }
        bool ISerialPortToStream.Uninitialize()
        {
            if (port != null)
            {
                Stream = null;
                port.Close();
                port.Dispose();
                port = null;
            }
            return true;
        }
        bool ISerialPortToStream.SetDataEventHandler(OnEmuSerialPortEvtHandler handler)
        {
            return true;
        }
    }
}
