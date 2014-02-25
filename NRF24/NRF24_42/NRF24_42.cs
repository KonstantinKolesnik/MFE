using System;
using Microsoft.SPOT;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using GTI = Gadgeteer.Interfaces;
using System.Threading;
using Gadgeteer.Modules.KKS.NRF24L01Plus;

namespace Gadgeteer.Modules.KKS
{
    // -- CHANGE FOR MICRO FRAMEWORK 4.2 --
    // If you want to use Serial, SPI, or DaisyLink (which includes GTI.SoftwareI2C), you must do a few more steps
    // since these have been moved to separate assemblies for NETMF 4.2 (to reduce the minimum memory footprint of Gadgeteer)
    // 1) add a reference to the assembly (named Gadgeteer.[interfacename])
    // 2) in GadgeteerHardware.xml, uncomment the lines under <Assemblies> so that end user apps using this module also add a reference.


    /// <summary>
    /// A NRF24 module for Microsoft .NET Gadgeteer
    /// </summary>
    public class NRF24 : GTM.Module
    {
        // NRF24L01Plus interface:      S-socket interface:
        // 1 - GND                      10
        // 2 - Vcc                      1
        // 3 - CE                       6
        // 4 - CSN                      5
        // 5 - SCK                      9
        // 6 - MOSI                     7
        // 7 - MISO                     8
        // 8 - IRQ                      3

        #region Delegates
        public delegate void OnDataRecievedHandler(byte[] data);
        public delegate void OnInterruptHandler(StatusInfo status);
        #endregion

        public enum CRCLength
        {
            CRC1Byte = 0,
            CRC2Bytes = 1
        }

        public class StatusInfo
        {
            private byte reg;

            public bool DataReady
            {
                get { return (reg & (1 << Bits.RX_DR)) > 0; }
            }
            public bool DataSent
            {
                get { return (reg & (1 << Bits.TX_DS)) > 0; }
            }
            public bool ResendLimitReached
            {
                get { return (reg & (1 << Bits.MAX_RT)) > 0; }
            }
            /// <summary>
            /// Data pipe number for the payload available for reading from RX_FIFO
            /// </summary>
            public byte DataPipe
            {
                get { return (byte)((reg >> 1) & 7); }
            }
            public bool DataPipeNotUsed
            {
                get { return DataPipe == 6; }
            }
            public bool RxEmpty
            {
                get { return DataPipe == 7; }
            }
            public bool TxFull
            {
                get { return (reg & (1 << Bits.TX_FULL)) > 0; }
            }

            internal StatusInfo(byte reg)
            {
                this.reg = reg;
            }

            internal void Update(byte reg)
            {
                this.reg = reg;
            }
            public override string ToString()
            {
                return "DataReady: " + DataReady +
                       "\nDataSent: " + DataSent +
                       "\nResendLimitReached: " + ResendLimitReached +
                       "\nTxFull: " + TxFull +
                       "\nRxEmpty: " + RxEmpty +
                       "\nDataPipe #: " + DataPipe +
                       "\nDataPipeNotUsed: " + DataPipeNotUsed;
            }
        }






        #region Fields
        private GT.Socket socket;
        private uint spiSpeed = 2000; // kHz; 10 Mbps max
        private GTI.SPI spi;

        private GTI.DigitalOutput pinCE; // chip enable, active high
        private Socket.Pin pinCSN = Socket.Pin.Five; // chip select, active low
        private GTI.InterruptInput pinIRQ; // active low

        private byte[] slot0Address;
        private byte payloadSize = 32; // fixed size of payloads

        private bool wide_band = true; /* 2Mbs data rate in use? */
        private bool p_variant = false; /* False for RF24L01 and true for RF24L01P */
        private bool ack_payload_available = false; /* Whether there is an ack payload waiting */
        private bool dynamic_payloads_enabled = false; /**< Whether dynamic payloads are enabled. */
        //uint8_t ack_payload_length; /**< Dynamic size of pending ack payload. */
        //uint64_t pipe0_reading_address = 0; /**< Last address set on pipe 0 for reading. */

        private StatusInfo status = new StatusInfo(0);
        #endregion

        #region Properties
        /// <summary>
        ///   Gets or sets a value indicating whether module is enabled (RX or TX mode).
        /// </summary>
        public bool IsEnabled
        {
            get { return pinCE.Read(); }
            set { pinCE.Write(value); }
        }
        /// <summary>
        ///   Gets module basic status information
        /// </summary>
        public StatusInfo Status
        {
            get { return status; }
        }
        public CRCLength CRCType
        {
            get
            {
                var bit = ReadRegisterBit(Registers.CONFIG, Bits.CRCO);
                return bit ? CRCLength.CRC2Bytes : CRCLength.CRC1Byte;
            }
            set
            {
                WriteRegisterBit(Registers.CONFIG, Bits.CRCO, value == CRCLength.CRC2Bytes);
            }
        }
        public bool IsCRCEnabled
        {
            get { return ReadRegisterBit(Registers.CONFIG, Bits.EN_CRC); }
            set { WriteRegisterBit(Registers.CONFIG, Bits.EN_CRC, value); }
        }
        public bool IsDataReceivedInterruptEnabled
        {
            get { return !ReadRegisterBit(Registers.CONFIG, Bits.MASK_RX_DR); }
            set { WriteRegisterBit(Registers.CONFIG, Bits.MASK_RX_DR, !value); }
        }
        public bool IsDataSentInterruptEnabled
        {
            get { return !ReadRegisterBit(Registers.CONFIG, Bits.MASK_TX_DS); }
            set { WriteRegisterBit(Registers.CONFIG, Bits.MASK_TX_DS, !value); }
        }
        public bool IsResendLimitReachedInterruptEnabled
        {
            get { return !ReadRegisterBit(Registers.CONFIG, Bits.MASK_MAX_RT); }
            set { WriteRegisterBit(Registers.CONFIG, Bits.MASK_MAX_RT, !value); }
        }
        public bool IsPowerOn
        {
            get { return ReadRegisterBit(Registers.CONFIG, Bits.PWR_UP); }
            set { WriteRegisterBit(Registers.CONFIG, Bits.PWR_UP, value); }
        }
        /// <summary>
        /// Gets or sets RF communication channel
        /// </summary>
        /// <param name="channel">RF channel (0-127)</param>
        public byte Channel
        {
            get { return ReadRegister(Registers.RF_CH); }
            set { WriteRegister(Registers.RF_CH, (byte)(value & 0x7F)); } // channel is 7 bits
        }


        public byte PayloadSize
        {
            get { return payloadSize; }
            set { payloadSize = (byte)System.Math.Max(value, 32); }
        }
        #endregion

        #region Events
        /// <summary>
        ///   Called on every IRQ interrupt
        /// </summary>
        public event OnInterruptHandler OnInterrupt;

        /// <summary>
        ///   Occurs when data packet has been received
        /// </summary>
        public event OnDataRecievedHandler OnDataReceived;

        /// <summary>
        ///   Occurs when ack has been received for send packet
        /// </summary>
        public event EventHandler OnTransmitSuccess;

        /// <summary>
        ///   Occurs when no ack has been received for send packet
        /// </summary>
        public event EventHandler OnTransmitFailed;
        #endregion

        #region Constructor
        // Note: A constructor summary is auto-generated by the doc builder.
        /// <summary></summary>
        /// <param name="socketNumber">The socket that this module is plugged in to.</param>
        public NRF24(int socketNumber)
        {
            // This finds the Socket instance from the user-specified socket number.  
            // This will generate user-friendly error messages if the socket is invalid.
            // If there is more than one socket on this module, then instead of "null" for the last parameter, 
            // put text that identifies the socket to the user (e.g. "S" if there is a socket type S)
            socket = Socket.GetSocket(socketNumber, true, this, null);
            socket.EnsureTypeIsSupported('S', this);

            socket.ReservePin(Socket.Pin.Three, this); // IRQ
            socket.ReservePin(Socket.Pin.Five, this); // CSN
            socket.ReservePin(Socket.Pin.Six, this); // CE
            socket.ReservePin(Socket.Pin.Seven, this); // MOSI
            socket.ReservePin(Socket.Pin.Eight, this); // MISO
            socket.ReservePin(Socket.Pin.Nine, this); // SCK

            // Serial peripheral interface (SPI).
            // Pin 7 is MOSI line, pin 8 is MISO line, pin 9 is SCK line.
            // In addition, pins 3, 4 and 5 are general-purpose input/outputs, with pin 3 supporting interrupt capabilities.

            GTI.SPI.Configuration spiConfig = new GTI.SPI.Configuration(false, 0, 0, false, false, spiSpeed);
            spi = new GTI.SPI(socket, spiConfig, GTI.SPI.Sharing.Shared, socket, pinCSN, this);

            pinCE = new GTI.DigitalOutput(socket, Socket.Pin.Six, false, this); // pin 6
            pinIRQ = new GTI.InterruptInput(socket, GT.Socket.Pin.Three, GTI.GlitchFilterMode.Off, GTI.ResistorMode.PullUp, GTI.InterruptMode.FallingEdge, this);
            pinIRQ.Interrupt += new GTI.InterruptInput.InterruptEventHandler(pinIRQ_Interrupt);

            // Must allow the radio time to settle else configuration bits will not necessarily stick.
            // This is actually only required following power up but some settling time also appears to
            // be required after resets too. For full coverage, we'll always assume the worst.
            // Enabling 16b CRC is by far the most obvious case if the wrong timing is used - or skipped.
            // Technically we require 4.5ms + 14us as a worst case. We'll just call it 5ms for good measure.
            // WARNING: Delay is based on P-variant whereby non-P *may* require different timing.
            //Thread.Sleep(5);
            Thread.Sleep(100);

            IsEnabled = false;

            // Set 1500uS (minimum for 32B payload in ESB@250KBPS) timeouts, to make testing a little easier
            // WARNING: If this is ever lowered, either 250KBS mode with AA is broken or maximum packet
            // sizes must never be used. See documentation for a more complete explanation.
            //write_register(SETUP_RETR, (B0100 << ARD) | (B1111 << ARC));


            // Restore our default PA level
            //setPALevel(RF24_PA_MAX);


            // Determine if this is a p or non-p RF24 module and then
            // reset our data rate back to default value. This works
            // because a non-P variant won't allow the data rate to
            // be set to 250Kbps.
            //if (setDataRate(RF24_250KBPS))
            //    p_variant = true;

            // Then set the data rate to the slowest (and most reliable) speed supported by all
            // hardware.
            //setDataRate(RF24_1MBPS);


            // Initialize CRC and request 2-byte (16bit) CRC
            //setCRCLength(RF24_CRC_16);

            // Disable dynamic payloads, to match dynamic_payloads_enabled setting
            //write_register(DYNPD, 0);


            // Reset current status
            // Notice reset and flush is the last thing we do
            //write_register(STATUS, _BV(RX_DR) | _BV(TX_DS) | _BV(MAX_RT));


            // Set up default configuration. Callers can always change it later.
            // This channel should be universally safe and not bleed over into adjacent spectrum.
            Channel = 76;

            // Flush buffers
            FlushRX();
            FlushTX();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Configure the module basic settings. Module needs to be initiaized.
        /// </summary>
        /// <param name="address">RF address (3-5 bytes). The width of this address determins the width of all addresses used for sending/receiving.</param>
        /// <param name="channel">RF channel (0-127)</param>
        public void Configure(byte[] address, byte channel)
        {
            AddressWidth.Check(address);

            // Set radio channel
            Channel = channel;

            // Enable dynamic payload length
            Execute(Commands.W_REGISTER, Registers.FEATURE,
                    new[]
                        {
                            (byte) (1 << Bits.EN_DPL)
                        });

            // Set auto-ack
            Execute(Commands.W_REGISTER, Registers.EN_AA,
                    new[]
                        {
                            (byte) (1 << Bits.ENAA_P0 |
                                    1 << Bits.ENAA_P1)
                        });

            // Set dynamic payload length for pipes
            Execute(Commands.W_REGISTER, Registers.DYNPD,
                    new[]
                        {
                            (byte) (1 << Bits.DPL_P0 |
                                    1 << Bits.DPL_P1)
                        });

            FlushRX();
            FlushTX();

            // Clear IRQ Masks
            Execute(Commands.W_REGISTER, Registers.STATUS,
                    new[]
                        {
                            (byte) (1 << Bits.MASK_RX_DR |
                                    1 << Bits.MASK_TX_DS |
                                    1 << Bits.MAX_RT)
                        });

            // Set default address
            Execute(Commands.W_REGISTER, Registers.SETUP_AW,
                    new[]
                        {
                            AddressWidth.GetCorrespondingRegisterValue(address)
                        });

            // Set module address
            slot0Address = address;
            Execute(Commands.W_REGISTER, (byte)RXAddressSlot.Zero, address);

            // Setup, CRC enabled, Power Up, PRX
            SetReceiveMode();
        }

        /// <summary>
        /// Set one of 6 available module addresses
        /// </summary>
        public void SetRXAddress(RXAddressSlot slot, byte[] address)
        {
            AddressWidth.Check(address);
            WriteRegister((byte)slot, address);

            if (slot == RXAddressSlot.Zero)
                slot0Address = address;
        }

        /// <summary>
        /// Read one of 6 available module addresses
        /// </summary>
        public byte[] GetRXAddress(RXAddressSlot slot, int width)
        {
            AddressWidth.Check(width);
            return ReadRegister((byte)slot, width);
        }

        /// <summary>
        ///   Send <param name = "bytes">bytes</param> to given <param name = "address">address</param>
        /// </summary>
        public void SendTo(byte[] address, byte[] bytes)
        {
            IsEnabled = false; // Chip enable low

            SetTransmitMode(); // Setup PTX (Primary TX)
            WriteRegister(Registers.TX_ADDR, address); // Write transmit address to TX_ADDR register. 
            WriteRegister(Registers.RX_ADDR_P0, address); // Write transmit address to RX_ADDRESS_P0 (Pipe0) (For Auto ACK)
            WriteCommand(Commands.W_TX_PAYLOAD, bytes); // Send payload

            IsEnabled = true; // Pulse for CE -> starts the transmission.
        }

        // scanning all channels in the 2.4GHz band
        // Data is packed into RAWDATA reading
        public void ScanChannels()
        {
            int numberOfChannels = 126; // 0 to 125
            byte curConfig = ReadRegister(Registers.CONFIG);
            bool isEnabled = IsEnabled;
            byte curChannel = Channel;

            IsEnabled = false;
            
            //Set RX mode
            //nrf24l01_set_config(nrf24l01_get_config() | 0x01);

            //clear RAWDATA
            //for (i = 0; i < RAWDATASIZE; i++){
            //    RAWDATA[i] = 0;
            //}

            //RAWDATA[0] = (BYTE) numberOfChannels;

            IsEnabled = false;
            for (int j = 0; j < 40; j++)
                for (byte i = 0; i < numberOfChannels; i++)
                {
                    Channel = i; //will do every second one;
                    IsEnabled = false;
                    //recomended at least 258uS but I found 350uS is need for reliability
                    //Delay10uS(26);

                    // received power > -64dBm
                    //if (nrf24l01_cd_active())
                    //{
                    //    RAWDATA[i + 1]++;
                    //}

                    ////this seems to be needed after the timeout not before
                    IsEnabled = false;
                    //Delay10uS(4); //may not be needed
                }

            //restore values
            WriteRegister(Registers.CONFIG, curConfig);
            Channel = curChannel;
            IsEnabled = isEnabled;
            //Delay10uS(20); //may not be needed
        }
        #endregion

        #region Private methods
        private byte ReadRegister(byte register)
        {
            return ReadRegister(register, 1)[0];
        }
        private byte[] ReadRegister(byte register, int byteCount)
        {
            var read = Execute(Commands.R_REGISTER, register, new byte[byteCount]);
            return ParseResponseData(read);
        }
        private bool ReadRegisterBit(byte register, byte bitNumber)
        {
            var read = Execute(Commands.R_REGISTER, register, new byte[1] { 0xff });
            var registerValue = ParseResponseData(read)[0];
            return (registerValue & (1 << Bits.CRCO)) > 0;
        }

        private byte[] WriteRegister(byte register, byte value)
        {
            return Execute(Commands.W_REGISTER, register, new byte[] { value });
        }
        private void WriteRegister(byte register, byte[] value)
        {
            Execute(Commands.W_REGISTER, register, value);
        }
        private void WriteRegisterBit(byte register, byte bitNumber, bool value)
        {
            var read = Execute(Commands.R_REGISTER, register, new byte[1] { 0xff });
            var oldRegisterValue = ParseResponseData(read)[0];
            byte newValue = (value == true ? (byte)(oldRegisterValue | (1 << bitNumber)) : (byte)(oldRegisterValue & ~(1 << bitNumber)));
            Execute(Commands.W_REGISTER, register, new[] { newValue });
        }

        private void WriteCommand(byte command)
        {
            Execute(command, 0x00, new byte[0]);
        }
        private void WriteCommand(byte command, byte[] data)
        {
            Execute(command, 0x00, data);
        }

        /// <summary>
        ///   Executes a command in NRF24L01+ (for details see module datasheet)
        /// </summary>
        /// <param name = "command">Command</param>
        /// <param name = "register">Register to write to</param>
        /// <param name = "data">Data to write</param>
        /// <returns>Response byte array. First byte is the status register</returns>
        private byte[] Execute(byte command, byte register, byte[] data)
        {
            // This command requires module to be in power down or standby mode
            var wasEnabled = IsEnabled;
            if (command == Commands.W_REGISTER)
                IsEnabled = false;

            // Create SPI Buffers with Size of Data + 1 (For Command)
            var writeBuffer = new byte[data.Length + 1];
            writeBuffer[0] = (byte)(command | register); // Add command and register to SPI buffer
            Array.Copy(data, 0, writeBuffer, 1, data.Length); // Add data to SPI buffer

            var readBuffer = new byte[data.Length + 1];
            spi.WriteRead(writeBuffer, readBuffer); // Do SPI Read/Write

            // Enable module back if it was disabled
            if (command == Commands.W_REGISTER && wasEnabled)
                IsEnabled = true;

            // Return ReadBuffer
            return readBuffer;
        }

        private void pinIRQ_Interrupt(GTI.InterruptInput sender, bool value)
        {
            // Disable RX/TX
            IsEnabled = false;

            // Set PRX
            SetReceiveMode();

            GetStatus();
            
            var payloads = new byte[3][];
            byte payloadCount = 0;
            var payloadCorrupted = false;

            if (OnInterrupt != null)
                OnInterrupt(status);

            if (status.DataReady)
            {
                while (!status.RxEmpty)
                {
                    // Read payload size
                    var payloadLength = Execute(Commands.R_RX_PL_WID, 0x00, new byte[1]);

                    // this indicates corrupted data
                    if (payloadLength[1] > 32)
                    {
                        payloadCorrupted = true;
                        FlushRX(); // Flush anything that remains in buffer
                    }
                    else
                    {
                        // Read payload data
                        payloads[payloadCount] = Execute(Commands.R_RX_PAYLOAD, 0x00, new byte[payloadLength[1]]);
                        payloadCount++;
                    }

                    // Clear RX_DR bit 
                    var result = WriteRegister(Registers.STATUS, (byte)(1 << Bits.RX_DR));
                    status.Update(result[0]);
                }
            }

            if (status.ResendLimitReached)
            {
                FlushTX();
                Execute(Commands.W_REGISTER, Registers.STATUS, new[] { (byte)(1 << Bits.MAX_RT) });// Clear MAX_RT bit in status register
            }

            if (status.TxFull)
                FlushTX();

            if (status.DataSent)
                Execute(Commands.W_REGISTER, Registers.STATUS, new[] { (byte)(1 << Bits.TX_DS) });// Clear TX_DS bit in status register


            // Enable RX
            IsEnabled = true;

            if (payloadCorrupted)
                Debug.Print("Corrupted data received");
            else if (payloadCount > 0)
            {
                for (var i = 0; i < payloadCount; i++)
                {
                    var payload = payloads[i];
                    var payloadWithoutCommand = new byte[payload.Length - 1];
                    Array.Copy(payload, 1, payloadWithoutCommand, 0, payload.Length - 1);
                    OnDataReceived(payloadWithoutCommand);
                }
            }
            else if (status.DataSent)
            {
                if (OnTransmitSuccess != null)
                    OnTransmitSuccess(this, EventArgs.Empty);
            }
            else
            {
                if (OnTransmitFailed != null)
                    OnTransmitFailed(this, EventArgs.Empty);
            }
        }

        private void GetStatus()
        {
            var readBuffer = new byte[1];
            spi.WriteRead(new[] { Commands.NOP }, readBuffer);
            status.Update(readBuffer[0]);
        }
        private void FlushRX()
        {
            WriteCommand(Commands.FLUSH_RX);
        }
        private void FlushTX()
        {
            WriteCommand(Commands.FLUSH_TX);
        }
        private void SetTransmitMode()
        {
            Execute(Commands.W_REGISTER, Registers.CONFIG, new[] { (byte)(1 << Bits.PWR_UP | 1 << Bits.EN_CRC | 0 << Bits.PRIM_RX) });
        }
        private void SetReceiveMode()
        {
            WriteRegister(Registers.RX_ADDR_P0, slot0Address);
            Execute(Commands.W_REGISTER, Registers.CONFIG, new[] { (byte)(1 << Bits.PWR_UP | 1 << Bits.EN_CRC | 1 << Bits.PRIM_RX) });
        }

        private byte[] ParseResponseData(byte[] response)
        {
            status.Update(response[0]);

            var result = new byte[response.Length - 1];
            Array.Copy(response, 1, result, 0, result.Length);
            return result;
        }
        #endregion
    }
}
