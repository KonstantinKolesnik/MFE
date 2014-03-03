using Microsoft.SPOT;
using System;
using System.Threading;
using GT = Gadgeteer;
using GTI = Gadgeteer.Interfaces;
using GTM = Gadgeteer.Modules;

namespace Gadgeteer.Modules.KKS
{
    /// <summary>
    /// A NRF24 module for Microsoft .NET Gadgeteer
    /// </summary>
    public class NRF24 : GTM.Module
    {
        #region Pins mapping
        // Serial peripheral interface (SPI).
        // Pin 7 is MOSI line, pin 8 is MISO line, pin 9 is SCK line.
        // In addition, pins 3, 4 and 5 are general-purpose input/outputs, with pin 3 supporting interrupt capabilities.
        
        // NRF24L01Plus interface:      S-socket interface:
        // 1 - GND                      10 !
        // 2 - Vcc                      1 !
        // 3 - CE                       6
        // 4 - CSN                      5
        // 5 - SCK                      9 !
        // 6 - MOSI                     7 !
        // 7 - MISO                     8 !
        // 8 - IRQ                      3 !
        #endregion

        #region Commands
        /// <summary>
        ///   Commands for NRF24L01Plus
        /// </summary>
        public static class Commands
        {
            /// <summary>
            ///   Read command and status registers. 
            ///   AAAAA = 5 bit Register Map Address
            ///   Data bytes: 1 to 5 (LSByte first)
            /// </summary>
            public const byte R_REGISTER = 0x00;

            /// <summary>
            ///   Write command and status registers. 
            ///   AAAAA = 5 bit Register Map Address Executable in power down or standby modes only.
            ///   Data bytes: 1 to 5 (LSByte first)
            /// </summary>
            public const byte W_REGISTER = 0x20;

            /// <summary>
            ///   Read RX-payload: 1 – 32 bytes. 
            ///   A read operation always starts at byte 0. 
            ///   Payload is deleted from FIFO after it is read. Used in RX mode.
            ///   Data bytes: 1 to 32 (LSByte first)
            /// </summary>
            public const byte R_RX_PAYLOAD = 0x61;

            /// <summary>
            ///   Write TX-payload: 1 – 32 bytes. 
            ///   A write operation always starts at byte 0 used in TX payload.
            ///   Data bytes: 1 to 32 (LSByte first)
            /// </summary>
            public const byte W_TX_PAYLOAD = 0xA0;

            /// <summary>
            ///   Flush TX FIFO, used in TX mode
            ///   Data bytes: 0
            /// </summary>
            public const byte FLUSH_TX = 0xE1;

            /// <summary>
            ///   Flush RX FIFO, used in RX mode Should not be executed during transmission of acknowledge. 
            ///   Acknowledge package will not be completed.
            ///   Data bytes: 0
            /// </summary>
            public const byte FLUSH_RX = 0xE2;

            /// <summary>
            ///   Used for a PTX device Reuse last transmitted payload. 
            ///   TX payload reuse is active until W_TX_PAYLOAD or FLUSH TX is executed. 
            ///   TX payload reuse must not be activated or deactivated during package transmission.
            ///   Data bytes: 0
            /// </summary>
            public const byte REUSE_TX_PL = 0xE3;

            /// <summary>
            ///   Read RX payload width for the top R_RX_PAYLOAD in the RX FIFO.
            ///   Flush RX FIFO if the read value is larger than 32 bytes.
            ///   Data bytes: 1
            /// </summary>
            public const byte R_RX_PL_WID = 0x60;

            /// <summary>
            ///   Used in RX mode.
            ///   Write Payload to be transmitted together with ACK packet on PIPE PPP.
            ///   PPP valid in the range from 000 to 101.
            ///   Maximum three ACK packet payloads can be pending. 
            ///   Payloads with same PPP are handled using first in - first out principle. 
            ///   Write payload: 1– 32 bytes. 
            ///   A write operation always starts at byte 0.
            ///   Data bytes: 1 to 32 (LSByte first)
            /// </summary>
            public const byte W_ACK_PAYLOAD = 0xA8;

            /// <summary>
            ///   Used in TX mode. Disables AUTOACK on this specific packet.
            ///   Data bytes: 1 to 32 (LSByte first)
            /// </summary>
            public const byte W_TX_PAYLOAD_NO_ACK = 0xB0;

            /// <summary>
            ///   Lock/unlock exclusive features.
            ///   Data bytes: 1 (0x73)
            /// </summary>
            public const byte LOCK_UNLOCK = 0x50;
			
            /// <summary>
            ///   No Operation. Might be used to read the STATUS register
            ///   Data bytes: 0
            /// </summary>
            public const byte NOP = 0xFF;
        }
        #endregion

        #region Registers
        /// <summary>
        ///   Registers for NRF24L01+
        ///   Can be read with Commands.R_REGISTER and written by Commands.W_REGISTER
        /// </summary>
        public static class Registers
        {
            /// <summary>
            ///   Configuration Register
            /// </summary>
            public const byte CONFIG = 0x00;

            /// <summary>
            ///   Enable 'Auto Acknowledgment' Function. Disable this functionality to be compatible with nRF2401.
            /// </summary>
            public const byte EN_AA = 0x01;

            /// <summary>
            ///   Enabled RX Addresses
            /// </summary>
            public const byte EN_RXADDR = 0x02;

            /// <summary>
            ///   Setup of Address Widths (common for all data pipes)
            /// </summary>
            public const byte SETUP_AW = 0x03;

            /// <summary>
            ///   Setup of Automatic Retransmission
            /// </summary>
            public const byte SETUP_RETR = 0x04;

            /// <summary>
            ///   RF Channel
            /// </summary>
            public const byte RF_CH = 0x05;

            /// <summary>
            ///   RF Setup Register
            /// </summary>
            public const byte RF_SETUP = 0x06;

            /// <summary>
            ///   Status Register (In parallel to the SPI command word applied on the MOSI pin, the STATUS register is shifted serially out on the MISO pin)
            /// </summary>
            public const byte STATUS = 0x07;

            /// <summary>
            ///   Transmit observe register
            /// </summary>
            public const byte OBSERVE_TX = 0x08;

            /// <summary>
            ///   Received Power Detector.
            /// </summary>
            public const byte RPD = 0x09;

            /// <summary>
            ///   Receive address data pipe 0. 5 Bytes maximum length. (LSByte is written first. Write the number of bytes defined by SETUP_AW)
            /// </summary>
            public const byte RX_ADDR_P0 = 0x0A;

            /// <summary>
            ///   Receive address data pipe 1. 5 Bytes maximum length. (LSByte is written first. Write the number of bytes defined by SETUP_AW)
            /// </summary>
            public const byte RX_ADDR_P1 = 0x0B;

            /// <summary>
            ///   Receive address data pipe 2. Only LSB. MSBytes are equal to RX_ADDR_P1
            /// </summary>
            public const byte RX_ADDR_P2 = 0x0C;

            /// <summary>
            ///   Receive address data pipe 3. Only LSB. MSBytes are equal to RX_ADDR_P1
            /// </summary>
            public const byte RX_ADDR_P3 = 0x0D;

            /// <summary>
            ///   Receive address data pipe 4. Only LSB. MSBytes are equal to RX_ADDR_P1
            /// </summary>
            public const byte RX_ADDR_P4 = 0x0E;

            /// <summary>
            ///   Receive address data pipe 5. Only LSB. MSBytes are equal to RX_ADDR_P1
            /// </summary>
            public const byte RX_ADDR_P5 = 0x0F;

            /// <summary>
            ///   Transmit address. Used for a PTX device only. (LSByte is written first) 
            ///   Set RX_ADDR_P0 equal to this address to handle automatic acknowledge if this is a PTX device with Enhanced ShockBurst™ enabled.
            /// </summary>
            public const byte TX_ADDR = 0x10;

            /// <summary>
            ///   Number of bytes in RX payload in data pipe 0
            /// </summary>
            public const byte RX_PW_P0 = 0x11;

            /// <summary>
            ///   Number of bytes in RX payload in data pipe 1
            /// </summary>
            public const byte RX_PW_P1 = 0x12;

            /// <summary>
            ///   Number of bytes in RX payload in data pipe 2
            /// </summary>
            public const byte RX_PW_P2 = 0x13;

            /// <summary>
            ///   Number of bytes in RX payload in data pipe 3
            /// </summary>
            public const byte RX_PW_P3 = 0x14;

            /// <summary>
            ///   Number of bytes in RX payload in data pipe 4
            /// </summary>
            public const byte RX_PW_P4 = 0x15;

            /// <summary>
            ///   Number of bytes in RX payload in data pipe 5
            /// </summary>
            public const byte RX_PW_P5 = 0x16;

            /// <summary>
            ///   FIFO Status Register
            /// </summary>
            public const byte FIFO_STATUS = 0x17;

            /// <summary>
            ///   Enable dynamic payload length
            /// </summary>
            public const byte DYNPD = 0x1C;

            /// <summary>
            ///   Feature Register
            /// </summary>
            public const byte FEATURE = 0x1D;
        }
        #endregion

        #region Bits
        /// <summary>
        ///   Mnemonics for the NRF24L01+ registers bits
        /// </summary>
        public static class Bits
        {
            #region CONFIG (0x00) bits
            #region Interrupt masks
            /// <summary>
            ///   Mask interrupt caused by RX_DR
            ///   1: Interrupt not reflected on the IRQ pin
            ///   0: Reflect RX_DR as active low interrupt on the IRQ pin
            /// </summary>
            public static byte MASK_RX_DR = 6;

            /// <summary>
            ///   Mask interrupt caused by TX_DS
            ///   1: Interrupt not reflected on the IRQ pin
            ///   0: Reflect TX_DS as active low interrupt on the IRQ pin
            /// </summary>
            public static byte MASK_TX_DS = 5;

            /// <summary>
            ///   Mask interrupt caused by MAX_RT
            ///   1: Interrupt not reflected on the IRQ pin
            ///   0: Reflect MAX_RT as active low interrupt on the IRQ pin
            /// </summary>
            public static byte MASK_MAX_RT = 4;
            #endregion

            /// <summary>
            ///   Enable CRC. Forced high if one of the bits in the EN_AA is high
            /// </summary>
            public static byte EN_CRC = 3;

            /// <summary>
            ///   CRC encoding scheme
            ///   '0' - 1 byte
            ///   '1' – 2 bytes
            /// </summary>
            public static byte CRCO = 2;

            /// <summary>
            ///   1: POWER UP, 0:POWER DOWN
            /// </summary>
            public static byte PWR_UP = 1;

            /// <summary>
            ///   RX/TX control
            ///   1: PRX, 0: PTX
            /// </summary>
            public static byte PRIM_RX;
            #endregion

            #region EN_AA (0x01) bits
            /// <summary>
            ///   Enable auto acknowledgement data pipe 5
            /// </summary>
            public static byte ENAA_P5 = 5;

            /// <summary>
            ///   Enable auto acknowledgement data pipe 4
            /// </summary>
            public static byte ENAA_P4 = 4;

            /// <summary>
            ///   Enable auto acknowledgement data pipe 3
            /// </summary>
            public static byte ENAA_P3 = 3;

            /// <summary>
            ///   Enable auto acknowledgement data pipe 2
            /// </summary>
            public static byte ENAA_P2 = 2;

            /// <summary>
            ///   Enable auto acknowledgement data pipe 1
            /// </summary>
            public static byte ENAA_P1 = 1;

            /// <summary>
            ///   Enable auto acknowledgement data pipe 0
            /// </summary>
            public static byte ENAA_P0 = 0;
            #endregion

            #region EN_RXADDR (0x02) bits
            /// <summary>
            ///   Enable data pipe 5
            /// </summary>
            public static byte ERX_P5 = 5;

            /// <summary>
            ///   Enable data pipe 4
            /// </summary>
            public static byte ERX_P4 = 4;

            /// <summary>
            ///   Enable data pipe 3
            /// </summary>
            public static byte ERX_P3 = 3;

            /// <summary>
            ///   Enable data pipe 2
            /// </summary>
            public static byte ERX_P2 = 2;

            /// <summary>
            ///   Enable data pipe 1
            /// </summary>
            public static byte ERX_P1 = 1;

            /// <summary>
            ///   Enable data pipe 0
            /// </summary>
            public static byte ERX_P0 = 0;
            #endregion

            #region SETUP_AW (0x03) bits
            /// <summary>
            ///   RX/TX Address field width
            ///   '00' - Illegal
            ///   '01' - 3 bytes
            ///   '10' - 4 bytes
            ///   '11' – 5 bytes
            ///   LSByte is used if address width is below 5 bytes
            /// </summary>
            public static byte AW; // 0...1
            #endregion

            #region SETUP_RETR (0x04) bits
            /// <summary>
            ///   Auto Retransmit Delay
            ///   '0000' – Wait 250uS
            ///   '0001' – Wait 500uS
            ///   '0010' – Wait 750uS
            ///   ...
            ///   '1111' – Wait 4000uS
            ///   (Delay defined from end of transmission to start of next transmission)
            /// </summary>
            public static byte ARD = 4; // 4...7

            /// <summary>
            ///   Auto Retransmit Count
            ///   '0000' –Re-Transmit disabled
            ///   '0001' – Up to 1 Re-Transmit on fail of AA
            ///   ...
            ///   '1111' – Up to 15 Re-Transmit on fail of AA
            /// </summary>
            public static byte ARC = 0; // 0...3
            #endregion

            #region RF_SETUP (0x06) bits
            /// <summary>
            ///   Enables continuous carrier transmit when high.
            /// </summary>
            public static byte CONT_WAVE = 7;

            /// <summary>
            ///   Set RF Data Rate to 250kbps. See RF_DR_HIGH for encoding.
            /// </summary>
            public static byte RF_DR_LOW = 5;

            /// <summary>
            ///   Force PLL lock signal. Only used in test
            /// </summary>
            public static byte PLL_LOCK = 4;

            /// <summary>
            ///   Select between the high speed data rates. This bit is don’t care if RF_DR_LOW is set. Encoding:
            ///   [RF_DR_LOW, RF_DR_HIGH]:
            ///   '00' – 1Mbps
            ///   '01' – 2Mbps
            ///   '10' – 250kbps
            ///   '11' – Reserved
            /// </summary>
            public static byte RF_DR_HIGH = 3;

            /// <summary>
            ///   Set RF output power in TX mode
            ///   '00' – -18dBm
            ///   '01' – -12dBm
            ///   '10' – -6dBm
            ///   '11' – 0dBm
            /// </summary>
            public static byte RF_PWR = 1; // 1...2

            // Obsolet for Plus version
            ///// <summary>
            /////   Setup LNA gain
            ///// </summary>
            //public static byte LNA_HCURR = 0;
            #endregion

            #region STATUS (0x07) bits
            /// <summary>
            ///   Data Ready RX FIFO interrupt. 
            ///   Asserted when new data arrives RX FIFOc. 
            ///   Write 1 to clear bit.
            /// </summary>
            public static byte RX_DR = 6;

            /// <summary>
            ///   Data Sent TX FIFO interrupt. Asserted when packet transmitted on TX. 
            ///   If AUTO_ACK is activated, this bit is set high only when ACK is received. 
            ///   Write 1 to clear bit.
            /// </summary>
            public static byte TX_DS = 5;

            /// <summary>
            ///   Maximum number of TX retransmits interrupt 
            ///   Write 1 to clear bit. 
            ///   If MAX_RT is asserted it must be cleared to enable further communication.
            /// </summary>
            public static byte MAX_RT = 4;

            /// <summary>
            ///   Data pipe number for the payload available for reading from RX_FIFO
            ///   000-101: Data Pipe Number
            ///   110: Not Used
            ///   111: RX FIFO Empty
            ///   Updated during the IRQ pin high to low transition
            /// </summary>
            public static byte RX_P_NO = 1; // 1...3

            /// <summary>
            ///   TX FIFO full flag.
            ///   1: TX FIFO full.
            ///   0: Available locations in TX FIFO.
            /// </summary>
            public static byte TX_FULL;
            #endregion

            #region OBSERVE_TX (0x08) bits
            /// <summary>
            ///   Count lost packets. 
            ///   The counter is overflow protected to 15, and discontinues at max until reset.
            ///   The counter is reset by writing to RF_CH.
            /// </summary>
            public static byte PLOS_CNT = 4; // readonly; 4...7

            /// <summary>
            ///   Count retransmitted packets. The counter is reset when transmission of a new packet starts.
            /// </summary>
            public static byte ARC_CNT; // readonly; 0...3
            #endregion

            #region RPD (0x09) bits
            /// <summary>
            ///   Carrier detect
            /// </summary>
            //public static byte CD = 0; // 24L01
            public static byte RPD = 0; // 24L01+
            #endregion

            #region FIFO_STATUS (0x17) bits
            /// <summary>
            ///   Used for a PTX device Pulse the rfce high for at least 10us to Reuse last transmitted payload. 
            ///   TX payload reuse is active until W_TX_PAYLOAD or FLUSH TX is executed. 
            ///   TX_REUSE is set by the SPI command REUSE_TX_PL, and is reset by the SPI commands W_TX_PAYLOAD or FLUSH TX
            /// </summary>
            public static byte TX_REUSE = 6; // readonly

            /// <summary>
            ///   TX FIFO full flag. 1: TX FIFO full. 0: Available locations in TX FIFO.
            /// </summary>
            public static byte TX_FIFO_FULL = 5; // readonly

            /// <summary>
            ///   TX FIFO empty flag. 
            ///   1: TX FIFO empty.
            ///   0: Data in TX FIFO.
            /// </summary>
            public static byte TX_EMPTY = 4; // readonly

            /// <summary>
            ///   RX FIFO full flag. 
            ///   1: RX FIFO full.
            ///   0: Available locations in RX FIFO.
            /// </summary>
            public static byte RX_FULL = 1; // readonly

            /// <summary>
            ///   RX FIFO empty flag.
            ///   1: RX FIFO empty.
            ///   0: Data in RX FIFO.
            /// </summary>
            public static byte RX_EMPTY = 0; // readonly
            #endregion

            #region DYNPD (0x1C) bits. (Requires EN_DPL and ENAA_Px)
            /// <summary>
            ///   Enable dynamic payload length data pipe 5. (Requires EN_DPL and ENAA_P5)
            /// </summary>
            public static byte DPL_P5 = 5;

            /// <summary>
            ///   Enable dynamic payload length data pipe 4. (Requires EN_DPL and ENAA_P4)
            /// </summary>
            public static byte DPL_P4 = 4;

            /// <summary>
            ///   Enable dynamic payload length data pipe 3. (Requires EN_DPL and ENAA_P3)
            /// </summary>
            public static byte DPL_P3 = 3;

            /// <summary>
            ///   Enable dynamic payload length data pipe 2. (Requires EN_DPL and ENAA_P2)
            /// </summary>
            public static byte DPL_P2 = 2;

            /// <summary>
            ///   Enable dynamic payload length data pipe 1. (Requires EN_DPL and ENAA_P1)
            /// </summary>
            public static byte DPL_P1 = 1;

            /// <summary>
            ///   Enable dynamic payload length data pipe 0. (Requires EN_DPL and ENAA_P0)
            /// </summary>
            public static byte DPL_P0 = 0;
            #endregion

            #region FEATURE (0x1D) bits
            /// <summary>
            ///   Enables Dynamic Payload Length
            /// </summary>
            public static byte EN_DPL = 2;

            /// <summary>
            ///   Enables Payload with ACK
            /// </summary>
            public static byte EN_ACK_PAY = 1;

            /// <summary>
            ///   Enables the W_TX_PAYLOAD_NOACK command
            /// </summary>
            public static byte EN_DYN_ACK;
            #endregion
        }
        #endregion

        #region Delegates
        public delegate void DataRecievedEventHandler(byte[] data);
        public delegate void InterruptEventHandler(StatusInfo status);
        #endregion

        #region Inner types definitions
        public enum CRCType
        {
            CRC1 = 0, // 1 byte
            CRC2 = 1 // 2 bytes
        }
        public enum AddressLengthType
        {
            Illegal  = 0,
            AddressLength3 = 1, // 3 bytes
            AddressLength4 = 2, // 4 bytes
            AddressLength5 = 3  // 5 bytes
        }
        public enum AutoRetransmitDelayType
        {
            Wait250us,
            Wait500us,
            Wait750us,
            Wait1000us,
            Wait1250us,
            Wait1500us,
            Wait1750us,
            Wait2000us,
            Wait2250us,
            Wait2500us,
            Wait2750us,
            Wait3000us,
            Wait3250us,
            Wait3500us,
            Wait3750us,
            Wait4000us
        }
        public enum DataRateType
        {
            Speed1Mbps,
            Speed2Mbps,
            Speed250kbps,
            Reserved
        }
        public enum TransmitPowerType
        {
            Power18dBm,
            Power12dBm,
            Power6dBm,
            Power0dBm
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
            internal void Update(byte reg)
            {
                this.reg = reg;
            }
        }
        #endregion

        #region Fields
        private GT.Socket socket;
        private uint spiSpeed = 2000; // kHz; 8...10 Mbps max
        private GTI.SPI spi;

        private GTI.DigitalOutput pinCE; // chip enable, active high
        private Socket.Pin pinCSN = Socket.Pin.Five; // chip select, active low
        private GTI.InterruptInput pinIRQ; // active low

        private StatusInfo status = new StatusInfo(0);
        private byte[] pipe0ReadingAddress = null; // Last address set on pipe 0 for reading
        private byte payloadSize = 32;
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
        ///   Gets module status information
        /// </summary>
        public StatusInfo Status
        {
            get { return status; }
        }

        public byte PayloadSize // for fixed payload width
        {
            get { return payloadSize; }
            set { payloadSize = (byte)System.Math.Min(value, 32); }
        }

        public bool IsDataReceivedInterruptEnabled // default true
        {
            get { return !ReadRegisterBit(Registers.CONFIG, Bits.MASK_RX_DR); }
            set { WriteRegisterBit(Registers.CONFIG, Bits.MASK_RX_DR, !value); }
        }
        public bool IsDataSentInterruptEnabled // default true
        {
            get { return !ReadRegisterBit(Registers.CONFIG, Bits.MASK_TX_DS); }
            set { WriteRegisterBit(Registers.CONFIG, Bits.MASK_TX_DS, !value); }
        }
        public bool IsResendLimitReachedInterruptEnabled // default true
        {
            get { return !ReadRegisterBit(Registers.CONFIG, Bits.MASK_MAX_RT); }
            set { WriteRegisterBit(Registers.CONFIG, Bits.MASK_MAX_RT, !value); }
        }
        public bool IsCRCEnabled // default true
        {
            get { return ReadRegisterBit(Registers.CONFIG, Bits.EN_CRC); }
            set { WriteRegisterBit(Registers.CONFIG, Bits.EN_CRC, value); }
        }
        public CRCType CRCLength // default 1 byte
        {
            get
            {
                var bit = ReadRegisterBit(Registers.CONFIG, Bits.CRCO);
                return bit ? CRCType.CRC2 : CRCType.CRC1;
            }
            set
            {
                WriteRegisterBit(Registers.CONFIG, Bits.CRCO, value == CRCType.CRC2);
            }
        }
        public bool IsPowerOn // default false
        {
            get { return ReadRegisterBit(Registers.CONFIG, Bits.PWR_UP); }
            set { WriteRegisterBit(Registers.CONFIG, Bits.PWR_UP, value); }
        }
        public bool IsReceiver // default false
        {
            get { return ReadRegisterBit(Registers.CONFIG, Bits.PRIM_RX); }
            set { WriteRegisterBit(Registers.CONFIG, Bits.PRIM_RX, value); }
        }

        public bool IsAutoAckEnabled0 // default true
        {
            get { return ReadRegisterBit(Registers.EN_AA, Bits.ENAA_P0); }
            set { WriteRegisterBit(Registers.EN_AA, Bits.ENAA_P0, value); }
        }
        public bool IsAutoAckEnabled1 // default true
        {
            get { return ReadRegisterBit(Registers.EN_AA, Bits.ENAA_P1); }
            set { WriteRegisterBit(Registers.EN_AA, Bits.ENAA_P1, value); }
        }
        public bool IsAutoAckEnabled2 // default true
        {
            get { return ReadRegisterBit(Registers.EN_AA, Bits.ENAA_P2); }
            set { WriteRegisterBit(Registers.EN_AA, Bits.ENAA_P2, value); }
        }
        public bool IsAutoAckEnabled3 // default true
        {
            get { return ReadRegisterBit(Registers.EN_AA, Bits.ENAA_P3); }
            set { WriteRegisterBit(Registers.EN_AA, Bits.ENAA_P3, value); }
        }
        public bool IsAutoAckEnabled4 // default true
        {
            get { return ReadRegisterBit(Registers.EN_AA, Bits.ENAA_P4); }
            set { WriteRegisterBit(Registers.EN_AA, Bits.ENAA_P4, value); }
        }
        public bool IsAutoAckEnabled5 // default true
        {
            get { return ReadRegisterBit(Registers.EN_AA, Bits.ENAA_P5); }
            set { WriteRegisterBit(Registers.EN_AA, Bits.ENAA_P5, value); }
        }

        public bool IsReceiverAddressEnabled0 // default true
        {
            get { return ReadRegisterBit(Registers.EN_RXADDR, Bits.ERX_P0); }
            set { WriteRegisterBit(Registers.EN_RXADDR, Bits.ERX_P0, value); }
        }
        public bool IsReceiverAddressEnabled1 // default true
        {
            get { return ReadRegisterBit(Registers.EN_RXADDR, Bits.ERX_P1); }
            set { WriteRegisterBit(Registers.EN_RXADDR, Bits.ERX_P1, value); }
        }
        public bool IsReceiverAddressEnabled2 // default false
        {
            get { return ReadRegisterBit(Registers.EN_RXADDR, Bits.ERX_P2); }
            set { WriteRegisterBit(Registers.EN_RXADDR, Bits.ERX_P2, value); }
        }
        public bool IsReceiverAddressEnabled3 // default false
        {
            get { return ReadRegisterBit(Registers.EN_RXADDR, Bits.ERX_P3); }
            set { WriteRegisterBit(Registers.EN_RXADDR, Bits.ERX_P3, value); }
        }
        public bool IsReceiverAddressEnabled4 // default false
        {
            get { return ReadRegisterBit(Registers.EN_RXADDR, Bits.ERX_P4); }
            set { WriteRegisterBit(Registers.EN_RXADDR, Bits.ERX_P4, value); }
        }
        public bool IsReceiverAddressEnabled5 // default false
        {
            get { return ReadRegisterBit(Registers.EN_RXADDR, Bits.ERX_P5); }
            set { WriteRegisterBit(Registers.EN_RXADDR, Bits.ERX_P5, value); }
        }

        public AddressLengthType AddressType // default 5 bytes
        {
            get { return (AddressLengthType)ReadRegister(Registers.SETUP_AW); }
            set { WriteRegister(Registers.SETUP_AW, (byte)value); }
        }

        public byte AutoRetransmitCount // default 3
        {
            get { return (byte)((ReadRegister(Registers.SETUP_RETR) >> Bits.ARC) & 0x0F); }
            set
            {
                byte reg = ReadRegister(Registers.SETUP_RETR);
                byte ard = (byte)((reg >> Bits.ARD) & 0x0F);
                byte newReg = (byte)((ard & 0x0F) << Bits.ARD | (value & 0x0F) << Bits.ARC);
                WriteRegister(Registers.SETUP_RETR, newReg);
            }
        }
        public AutoRetransmitDelayType AutoRetransmitDelay // default Wait250us
        {
            get { return (AutoRetransmitDelayType)(byte)((ReadRegister(Registers.SETUP_RETR) >> Bits.ARD) & 0x0F); }
            set
            {
                byte reg = ReadRegister(Registers.SETUP_RETR);
                byte arc = (byte)((reg >> Bits.ARC) & 0x0F);
                byte newReg = (byte)(((byte)value & 0x0F) << Bits.ARD | (arc & 0x0F) << Bits.ARC);
                WriteRegister(Registers.SETUP_RETR, newReg);
            }
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

        public bool IsContinuousCarrierTransmitEnabled // default false
        {
            get { return ReadRegisterBit(Registers.RF_SETUP, Bits.CONT_WAVE); }
            set { WriteRegisterBit(Registers.RF_SETUP, Bits.CONT_WAVE, value); }
        }
        public bool IsPllLockEnabled // default false
        {
            get { return ReadRegisterBit(Registers.RF_SETUP, Bits.PLL_LOCK); }
            set { WriteRegisterBit(Registers.RF_SETUP, Bits.PLL_LOCK, value); }
        }
        public DataRateType DataRate // default 2 Mbps
        {
            get
            {
                var reg = ReadRegister(Registers.RF_SETUP);
                var val = ((reg >> Bits.RF_DR_HIGH) & 0x01) + ((reg >> Bits.RF_DR_LOW - 1) & 0x02);
                return (DataRateType)val;
            }
            set
            {
                bool bitHigh = ((byte)value & (1 << 0)) > 0;
                bool bitLow = ((byte)value & (1 << 1)) > 0;
                
                var reg = ReadRegister(Registers.RF_SETUP);
                reg = (bitHigh ? (byte)(reg | (1 << Bits.RF_DR_HIGH)) : (byte)(reg & ~(1 << Bits.RF_DR_HIGH)));
                reg = (bitLow ? (byte)(reg | (1 << Bits.RF_DR_LOW)) : (byte)(reg & ~(1 << Bits.RF_DR_LOW)));
                WriteRegister(Registers.RF_SETUP, reg);
            }
        }
        public TransmitPowerType TransmitPower // default 0 dBm
        {
            get
            {
                var reg = ReadRegister(Registers.RF_SETUP);
                var val = (reg >> Bits.RF_PWR) & 0x03;
                return (TransmitPowerType)val;
            }
            set
            {
                //bool bitHigh = ((byte)value & (1 << 0)) > 0;
                //bool bitLow = ((byte)value & (1 << 1)) > 0;

                //var reg = ReadRegister(Registers.RF_SETUP);
                //reg = (bitHigh ? (byte)(reg | (1 << Bits.RF_DR_HIGH)) : (byte)(reg & ~(1 << Bits.RF_DR_HIGH)));
                //reg = (bitLow ? (byte)(reg | (1 << Bits.RF_DR_LOW)) : (byte)(reg & ~(1 << Bits.RF_DR_LOW)));
                //WriteRegister(Registers.RF_SETUP, reg);
            }
        }

        public byte RetransmittedPacketsCount
        {
            get { return (byte)((ReadRegister(Registers.OBSERVE_TX) >> Bits.ARC_CNT) & 0x0F); }
        }
        public byte LostPacketsCount
        {
            get { return (byte)((ReadRegister(Registers.OBSERVE_TX) >> Bits.PLOS_CNT) & 0x0F); }
        }

        public bool RPD
        {
            get { return ReadRegisterBit(Registers.RPD, Bits.RPD); }
        }

        public byte[] TransmitterAddress // 3...5 bytes
        {
            get { return ReadRegister(Registers.TX_ADDR, 5); }
            set { WriteRegister(Registers.TX_ADDR, value); }
        }
        public byte[] ReceiverAddress0
        {
            get { return ReadRegister(Registers.RX_ADDR_P0, 5); }
            set { WriteRegister(Registers.RX_ADDR_P0, value); }
        }
        public byte[] ReceiverAddress1
        {
            get { return ReadRegister(Registers.RX_ADDR_P1, 5); }
            set { WriteRegister(Registers.RX_ADDR_P1, value); }
        }
        public byte ReceiverAddress2
        {
            get { return ReadRegister(Registers.RX_ADDR_P2); }
            set { WriteRegister(Registers.RX_ADDR_P2, value); }
        }
        public byte ReceiverAddress3
        {
            get { return ReadRegister(Registers.RX_ADDR_P3); }
            set { WriteRegister(Registers.RX_ADDR_P3, value); }
        }
        public byte ReceiverAddress4
        {
            get { return ReadRegister(Registers.RX_ADDR_P4); }
            set { WriteRegister(Registers.RX_ADDR_P4, value); }
        }
        public byte ReceiverAddress5
        {
            get { return ReadRegister(Registers.RX_ADDR_P5); }
            set { WriteRegister(Registers.RX_ADDR_P5, value); }
        }

        public byte ReceiverPayloadWidth0 // default 0
        {
            get { return ReadRegister(Registers.RX_PW_P0); }
            set { WriteRegister(Registers.RX_PW_P0, (byte)(value & 0x3F)); }
        }
        public byte ReceiverPayloadWidth1 // default 0
        {
            get { return ReadRegister(Registers.RX_PW_P1); }
            set { WriteRegister(Registers.RX_PW_P1, (byte)(value & 0x3F)); }
        }
        public byte ReceiverPayloadWidth2 // default 0
        {
            get { return ReadRegister(Registers.RX_PW_P2); }
            set { WriteRegister(Registers.RX_PW_P2, (byte)(value & 0x3F)); }
        }
        public byte ReceiverPayloadWidth3 // default 0
        {
            get { return ReadRegister(Registers.RX_PW_P3); }
            set { WriteRegister(Registers.RX_PW_P3, (byte)(value & 0x3F)); }
        }
        public byte ReceiverPayloadWidth4 // default 0
        {
            get { return ReadRegister(Registers.RX_PW_P4); }
            set { WriteRegister(Registers.RX_PW_P4, (byte)(value & 0x3F)); }
        }
        public byte ReceiverPayloadWidth5 // default 0
        {
            get { return ReadRegister(Registers.RX_PW_P5); }
            set { WriteRegister(Registers.RX_PW_P5, (byte)(value & 0x3F)); }
        }

        public bool IsReceiverFifoEmpty
        {
            get { return ReadRegisterBit(Registers.FIFO_STATUS, Bits.RX_EMPTY); }
        }
        public bool IsReceiverFifoFull
        {
            get { return ReadRegisterBit(Registers.FIFO_STATUS, Bits.RX_FULL); }
        }
        public bool IsTransmitterFifoEmpty
        {
            get { return ReadRegisterBit(Registers.FIFO_STATUS, Bits.TX_EMPTY); }
        }
        public bool IsTransmitterFifoFull
        {
            get { return ReadRegisterBit(Registers.FIFO_STATUS, Bits.TX_FIFO_FULL); }
        }
        public bool IsTransmitterFifoReuse
        {
            get { return ReadRegisterBit(Registers.FIFO_STATUS, Bits.TX_REUSE); }
        }

        public bool IsDynamicPayloadEnabled0 // default true
        {
            get { return ReadRegisterBit(Registers.DYNPD, Bits.DPL_P0); }
            set { WriteRegisterBit(Registers.DYNPD, Bits.DPL_P0, value); }
        }
        public bool IsDynamicPayloadEnabled1 // default true
        {
            get { return ReadRegisterBit(Registers.DYNPD, Bits.DPL_P1); }
            set { WriteRegisterBit(Registers.DYNPD, Bits.DPL_P1, value); }
        }
        public bool IsDynamicPayloadEnabled2 // default false
        {
            get { return ReadRegisterBit(Registers.DYNPD, Bits.DPL_P2); }
            set { WriteRegisterBit(Registers.DYNPD, Bits.DPL_P2, value); }
        }
        public bool IsDynamicPayloadEnabled3 // default false
        {
            get { return ReadRegisterBit(Registers.DYNPD, Bits.DPL_P3); }
            set { WriteRegisterBit(Registers.DYNPD, Bits.DPL_P3, value); }
        }
        public bool IsDynamicPayloadEnabled4 // default false
        {
            get { return ReadRegisterBit(Registers.DYNPD, Bits.DPL_P4); }
            set { WriteRegisterBit(Registers.DYNPD, Bits.DPL_P4, value); }
        }
        public bool IsDynamicPayloadEnabled5 // default false
        {
            get { return ReadRegisterBit(Registers.DYNPD, Bits.DPL_P5); }
            set { WriteRegisterBit(Registers.DYNPD, Bits.DPL_P5, value); }
        }

        public bool IsDynamicPayloadFeatureEnabled // default true
        {
            get { return ReadRegisterBit(Registers.FEATURE, Bits.EN_DPL); }
            set { WriteRegisterBit(Registers.FEATURE, Bits.EN_DPL, value); }
        }
        public bool IsAckPayloadEnabled // default false
        {
            get { return ReadRegisterBit(Registers.FEATURE, Bits.EN_ACK_PAY); }
            set { WriteRegisterBit(Registers.FEATURE, Bits.EN_ACK_PAY, value); }
        }
        public bool IsDynamicAckEnabled // default false
        {
            get { return ReadRegisterBit(Registers.FEATURE, Bits.EN_DYN_ACK); }
            set { WriteRegisterBit(Registers.FEATURE, Bits.EN_DYN_ACK, value); }
        }

        public object Tag
        {
            get;
            set;
        }
        #endregion

        #region Events
        /// <summary>
        ///   Called on every IRQ interrupt
        /// </summary>
        public event InterruptEventHandler Interrupt;

        /// <summary>
        ///   Occurs when data packet has been received
        /// </summary>
        public event DataRecievedEventHandler DataReceived;

        /// <summary>
        ///   Occurs when ack has been received for send packet
        /// </summary>
        public event EventHandler TransmitSuccess;

        /// <summary>
        ///   Occurs when no ack has been received for send packet
        /// </summary>
        public event EventHandler TransmitFailed;
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

            GTI.SPI.Configuration spiConfig = new GTI.SPI.Configuration(false, 0, 0, false, true, spiSpeed);
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

            Initialize();
        }
        #endregion

        #region Public methods
        public void SetAutoAckEnabled(bool value)
        {
            IsAutoAckEnabled0 = value;
            IsAutoAckEnabled1 = value;
            IsAutoAckEnabled2 = value;
            IsAutoAckEnabled3 = value;
            IsAutoAckEnabled4 = value;
            IsAutoAckEnabled5 = value;
        }
        public void EnableAckPayload()
        {
            IsAckPayloadEnabled = true;
            
            //IsDynamicPayloadFeatureEnabled = true;
            //IsDynamicPayloadEnabled0 = true;
            //IsDynamicPayloadEnabled1 = true;
        }

        public void OpenReadingPipe(byte idx, byte[] address)
        {
            if (idx <= 6)
            {
                // For pipes 2-5, only write the LSB
                switch (idx)
                {
                    case 0:
                        // For pipe 0, cache the address. It's needed because OpenWritingPipe() will overwrite the pipe 0 address, so StartListening() will have to restore it.
                        pipe0ReadingAddress = address;
                        ReceiverAddress0 = address;
                        ReceiverPayloadWidth0 = (byte)System.Math.Min(payloadSize, 32);
                        IsReceiverAddressEnabled0 = true;
                        break;
                    case 1:
                        ReceiverAddress1 = address;
                        ReceiverPayloadWidth1 = (byte)System.Math.Min(payloadSize, 32);
                        IsReceiverAddressEnabled1 = true;
                        break;
                    case 2:
                        ReceiverAddress2 = address[0];
                        ReceiverPayloadWidth2 = (byte)System.Math.Min(payloadSize, 32);
                        IsReceiverAddressEnabled2 = true;
                        break;
                    case 3:
                        ReceiverAddress3 = address[0];
                        ReceiverPayloadWidth3 = (byte)System.Math.Min(payloadSize, 32);
                        IsReceiverAddressEnabled3 = true;
                        break;
                    case 4:
                        ReceiverAddress4 = address[0];
                        ReceiverPayloadWidth4 = (byte)System.Math.Min(payloadSize, 32);
                        IsReceiverAddressEnabled4 = true;
                        break;
                    case 5:
                        ReceiverAddress5 = address[0];
                        ReceiverPayloadWidth5 = (byte)System.Math.Min(payloadSize, 32);
                        IsReceiverAddressEnabled5 = true;
                        break;
                }
            }
        }
        public void StartListening()
        {
            SetReceiveMode();
            ResetStatus();

            // Restore the pipe0 adddress, if exists
            if (pipe0ReadingAddress != null)
                ReceiverAddress0 = pipe0ReadingAddress;

            FlushTX();
            FlushRX();

            // Go!
            IsEnabled = true;

            // wait for the radio to come up (130us actually only needed)
            Thread.Sleep(1);
        }
        public void StopListening()
        {
            IsEnabled = false;
            FlushTX();
            FlushRX();
        }

        public void OpenWritingPipe(byte[] address)
        {
            TransmitterAddress = address;
            ReceiverAddress0 = address;
            ReceiverPayloadWidth0 = (byte)System.Math.Min(payloadSize, 32);
        }
        public void StartWrite(byte[] data)
        {
            SetTransmitMode();
            Thread.Sleep(1); //delayMicroseconds(150);

            WritePayload(data);

            IsEnabled = true;
            Thread.Sleep(1); //delayMicroseconds(15);
            IsEnabled = false;
        }

        public void WriteAckPayload(byte idx, byte[] data)
        {
            WriteCommand((byte)(Commands.W_ACK_PAYLOAD | (idx & 0x07)), data);

            //SPI.transfer(W_ACK_PAYLOAD | (pipe & B111));
            //uint8_t data_len = min(len, 32);
            //while (data_len--)
            //    SPI.transfer(*current++);
        }

        /// <summary>
        ///   Send <param name = "bytes">bytes</param> to given <param name = "address">address</param>
        /// </summary>
        public void SendTo(byte[] address, byte[] bytes)
        {
            IsEnabled = false; // Chip enable low

            SetTransmitMode();
            Thread.Sleep(1); //delayMicroseconds(150);

            TransmitterAddress = address; // Write transmit address to TX_ADDR register. 
            ReceiverAddress0 = address; // Write transmit address to RX_ADDRESS_P0 (Pipe0) (For Auto ACK)
            
            WritePayload(bytes);

            IsEnabled = true; // Pulse for CE -> starts the transmission.
        }
        public void Configure(byte[] address)
        {
            Initialize();

            // Set module address
            pipe0ReadingAddress = address;
            ReceiverAddress0 = address;

            // Setup, CRC enabled, Power Up, PRX
            SetReceiveMode();

            IsEnabled = true;
        }


        /// <summary>
        ///   Scanning all 128 channels in the 2.4GHz band
        /// </summary>
        public byte[] ScanChannels()
        {
            int numberOfChannels = 128; // 0 to 127
            byte curConfig = ReadRegister(Registers.CONFIG);
            bool isEnabled = IsEnabled;
            byte curChannel = Channel;

            byte[] result = new byte[numberOfChannels];

            SetAutoAckEnabled(false);
            StartListening();
            StopListening();

            IsEnabled = false;
            for (int j = 0; j < 40; j++)
                for (byte i = 0; i < numberOfChannels; i++)
                {
                    Channel = i;
                    StartListening();
                    Thread.Sleep(1);
                    StopListening();

                    // received power > -64dBm
                    if (RPD)
                        result[i]++;
                }

            //restore values
            WriteRegister(Registers.CONFIG, curConfig);
            Channel = curChannel;
            IsEnabled = isEnabled;

            return result;
        }
        #endregion

        #region Private methods
        private byte[] ReadRegister(byte register, int size)
        {
            return Execute(Commands.R_REGISTER, register, new byte[size]);
        }
        private byte ReadRegister(byte register)
        {
            return ReadRegister(register, 1)[0];
        }
        private bool ReadRegisterBit(byte register, byte bitNumber)
        {
            return (ReadRegister(register) & (1 << bitNumber)) > 0;
        }

        private void WriteRegister(byte register, byte[] value)
        {
            Execute(Commands.W_REGISTER, register, value);
        }
        private void WriteRegister(byte register, byte value)
        {
            WriteRegister(register, new byte[] { value });
        }
        private void WriteRegisterBit(byte register, byte bitNumber, bool value)
        {
            var reg = ReadRegister(register);
            byte newReg = (value == true ? (byte)(reg | (1 << bitNumber)) : (byte)(reg & ~(1 << bitNumber)));
            WriteRegister(register, newReg);
        }

        private byte[] WriteCommand(byte command)
        {
            return WriteCommand(command, new byte[0]);
        }
        private byte[] WriteCommand(byte command, byte[] data)
        {
            return Execute(command, 0x00, data);
        }

        private void Initialize()
        {
            // Set 1500uS (minimum for 32B payload in ESB@250KBPS) timeouts, to make testing a little easier
            // WARNING: If this is ever lowered, either 250KBS mode with AA is broken or maximum packet
            // sizes must never be used. See documentation for a more complete explanation.
            AutoRetransmitCount = 15;
            AutoRetransmitDelay = AutoRetransmitDelayType.Wait1500us;

            // Restore our default PA level
            TransmitPower = TransmitPowerType.Power0dBm;

            // Determine if this is a p or non-p RF24 module and then
            // reset our data rate back to default value. This works
            // because a non-P variant won't allow the data rate to
            // be set to 250Kbps.
            //if (setDataRate(RF24_250KBPS))
            //    p_variant = true;
            // Then set the data rate to the slowest (and most reliable) speed supported by all
            // hardware.
            //setDataRate(RF24_1MBPS);
            DataRate = DataRateType.Speed2Mbps;

            // Initialize CRC and request 2-byte (16bit) CRC
            IsCRCEnabled = true;
            CRCLength = CRCType.CRC2;

            IsReceiverAddressEnabled0 = true;
            IsReceiverAddressEnabled1 = true;
            IsReceiverAddressEnabled2 = true;
            IsReceiverAddressEnabled3 = true;
            IsReceiverAddressEnabled4 = true;
            IsReceiverAddressEnabled5 = true;

            IsAutoAckEnabled0 = true;
            IsAutoAckEnabled1 = true;
            IsAutoAckEnabled2 = true;
            IsAutoAckEnabled3 = true;
            IsAutoAckEnabled4 = true;
            IsAutoAckEnabled5 = true;
            //SetAutoAck(true);

            IsDynamicPayloadFeatureEnabled = true;
            IsDynamicPayloadEnabled0 = true;
            IsDynamicPayloadEnabled1 = true;
            IsDynamicPayloadEnabled2 = true;
            IsDynamicPayloadEnabled3 = true;
            IsDynamicPayloadEnabled4 = true;
            IsDynamicPayloadEnabled5 = true;

            // Set up default configuration. Callers can always change it later.
            // This channel should be universally safe and not bleed over into adjacent spectrum.
            Channel = 76;

            AddressType = AddressLengthType.AddressLength5;

            // Notice reset and flush is the last thing we do
            ResetStatus();
            FlushRX();
            FlushTX();
        }
        private void GetStatus()
        {
            WriteCommand(Commands.NOP);
        }
        private void ResetStatus()
        {
            WriteRegisterBit(Registers.STATUS, Bits.RX_DR, true);
            WriteRegisterBit(Registers.STATUS, Bits.TX_DS, true);
            WriteRegisterBit(Registers.STATUS, Bits.MAX_RT, true);
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
            //WriteRegisterBit(Registers.CONFIG, Bits.PRIM_RX, false);
            //WriteRegisterBit(Registers.CONFIG, Bits.PWR_UP, true);

            Execute(Commands.W_REGISTER, Registers.CONFIG,
        new[]
                        {
                            (byte) (1 << Bits.PWR_UP |
                                    1 << Bits.CRCO)
                        });

        }
        private void SetReceiveMode()
        {
            ReceiverAddress0 = pipe0ReadingAddress;

            //WriteRegisterBit(Registers.CONFIG, Bits.PRIM_RX, true);
            //WriteRegisterBit(Registers.CONFIG, Bits.PWR_UP, true);

            Execute(Commands.W_REGISTER, Registers.CONFIG,
                    new[]
                        {
                            (byte) (1 << Bits.PWR_UP |
                                    1 << Bits.CRCO |
                                    1 << Bits.PRIM_RX)
                        });

        }

        private void WritePayload(byte[] data)
        {
            int dataSize = System.Math.Min(data.Length, payloadSize);
            byte[] buffer = new byte[IsDynamicPayloadFeatureEnabled ? dataSize : payloadSize];
            Array.Copy(data, buffer, dataSize);
  
            WriteCommand(Commands.W_TX_PAYLOAD, buffer);
        }
        private byte[] ReadPayload(byte size)
        {
            int dataSize = System.Math.Min(size, payloadSize);
            return WriteCommand(Commands.R_RX_PAYLOAD, new byte[IsDynamicPayloadFeatureEnabled ? dataSize : payloadSize]);
        }

        private byte GetDynamicPayloadSize()
        {
            return WriteCommand(Commands.R_RX_PL_WID, new byte[1])[0];
        }

        private byte[] Execute(byte command, byte register, byte[] data)
        {
            var wasEnabled = IsEnabled;
            if (command == Commands.W_REGISTER) // This command requires module to be in power down or standby mode
                IsEnabled = false;

            var writeBuffer = new byte[1 + data.Length]; // Create SPI Buffers with Size of Data + 1 (For Command)
            writeBuffer[0] = (byte)(command | register); // Add command and register to SPI buffer
            Array.Copy(data, 0, writeBuffer, 1, data.Length); // Add data to SPI buffer

            var readBuffer = new byte[1 + data.Length]; // STATUS + data
            
            spi.WriteRead(writeBuffer, readBuffer); // Do SPI Write/Read

            if (command == Commands.W_REGISTER && wasEnabled) // Enable module back if it was disabled
                IsEnabled = true;

            status.Update(readBuffer[0]); // update status with value from byte #0

            if (readBuffer.Length > 1)
            {
                var result = new byte[readBuffer.Length - 1];
                Array.Copy(readBuffer, 1, result, 0, result.Length);
                return result;
            }

            return null;
        }

        private void pinIRQ_Interrupt(GTI.InterruptInput sender, bool value)
        {
            IsEnabled = false;


            GetStatus();

            //bool isDataReady = status.DataReady;
            //bool isResendLimitReached = status.ResendLimitReached;
            //bool isDataSent = status.DataSent;

            SetReceiveMode();
            //ResetStatus();

            if (Interrupt != null)
                Interrupt(status);

            var payloads = new byte[3][];
            byte payloadIdx = 0;
            var payloadCorrupted = false;

            if (status.DataReady)
            {
                while (!status.RxEmpty)
                {
                    // Read payload size
                    var payloadLength = GetDynamicPayloadSize();
                    if (payloadLength > 32) // this indicates corrupted data
                    {
                        payloadCorrupted = true;
                        FlushRX(); // Flush anything that remains in buffer
                    }
                    else
                    {
                        // Read payload data
                        payloads[payloadIdx] = ReadPayload(payloadLength);
                        payloadIdx++;
                    }
                    GetStatus();
                WriteRegisterBit(Registers.STATUS, Bits.RX_DR, true); // Clear RX_DR bit in status register
                }

            }
            if (status.ResendLimitReached)
            {
                FlushTX();
                WriteRegisterBit(Registers.STATUS, Bits.MAX_RT, true); // Clear MAX_RT bit in status register
            }
            if (status.TxFull)
                FlushTX();
            if (status.DataSent)
                WriteRegisterBit(Registers.STATUS, Bits.TX_DS, true); // Clear TX_DS bit in status register

            IsEnabled = true;

            if (payloadCorrupted)
                Debug.Print("Corrupted data received");
            else if (payloadIdx > 0)
            {
                for (var i = 0; i < payloadIdx; i++)
                    DataReceived(payloads[i]);
            }
            else if (status.DataSent)
            {
                if (TransmitSuccess != null)
                    TransmitSuccess(this, EventArgs.Empty);
            }
            else
            {
                if (TransmitFailed != null)
                    TransmitFailed(this, EventArgs.Empty);
            }
        }
        #endregion
    }
}
