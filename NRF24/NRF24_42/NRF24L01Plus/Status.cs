
namespace Gadgeteer.Modules.KKS.NRF24L01Plus
{
    public class Status
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

        public Status(byte reg)
        {
            this.reg = reg;
        }

        public void Update(byte reg)
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
}
