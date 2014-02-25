
namespace Gadgeteer.Modules.KKS.NRF24L01Plus
{
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
        public static byte CD = 0;
        #endregion

        #region FIFO_STATUS (0x17) bits
        /// <summary>
        ///   Used for a PTX device Pulse the rfce high for at least 10?s to Reuse last transmitted payload. 
        ///   TX payload reuse is active until W_TX_PAYLOAD or FLUSH TX is executed. 
        ///   TX_REUSE is set by the SPI command REUSE_TX_PL, and is reset by the SPI commands W_TX_PAYLOAD or FLUSH TX
        /// </summary>
        public static byte TX_REUSE = 6;

        /// <summary>
        ///   TX FIFO full flag. 1: TX FIFO full. 0: Available locations in TX FIFO.
        /// </summary>
        public static byte TX_FIFO_FULL = 5;

        /// <summary>
        ///   TX FIFO empty flag. 
        ///   1: TX FIFO empty.
        ///   0: Data in TX FIFO.
        /// </summary>
        public static byte TX_EMPTY = 4;

        /// <summary>
        ///   RX FIFO full flag. 
        ///   1: RX FIFO full.
        ///   0: Available locations in RX FIFO.
        /// </summary>
        public static byte RX_FULL = 1;

        /// <summary>
        ///   RX FIFO empty flag.
        ///   1: RX FIFO empty.
        ///   0: Data in RX FIFO.
        /// </summary>
        public static byte RX_EMPTY = 0;
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
        public static byte DPL_P0;
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
}
