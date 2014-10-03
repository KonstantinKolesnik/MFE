using Microsoft.SPOT.Hardware;

namespace MFE.SmartNetwork.Network
{
    public class BusConfigurationI2C
    {
        public const int ClockRate = 400; // 100 kHz
        public const int Timeout = 1000; // 1 sec
        
        public I2CDevice Bus;

        public BusConfigurationI2C(I2CDevice bus)
        {
            Bus = bus;
        }
    }
}
