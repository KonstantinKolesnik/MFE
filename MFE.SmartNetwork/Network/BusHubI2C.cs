using MFE.Hardware;
using Microsoft.SPOT.Hardware;
using System.Collections;

namespace SmartNetwork.Network
{
    public class BusHubI2C : BusHub
    {
        #region Fields
        private BusConfigurationI2C busConfig;
        #endregion

        #region Constructor
        public BusHubI2C(BusConfigurationI2C busConfig)
            : base(0)
        {
            this.busConfig = busConfig;
        }
        #endregion

        #region Private methods
        protected override void Scan()
        {
            ArrayList addressesAdded = new ArrayList();
            ArrayList addressesRemoved = new ArrayList();

            for (ushort address = 1; address <= 127; address++)
            {
                byte type = 255;

                I2CDevice.Configuration config = new I2CDevice.Configuration(address, BusConfigurationI2C.ClockRate);
                if (busConfig.Bus.TryGetRegister(config, BusConfigurationI2C.Timeout, BusModuleAPI.CmdGetType, out type))
                {
                    // address is online

                    BusModule busModule = this[address];

                    if (busModule == null) // no registered module with this address
                    {
                        busModule = new BusModule(this, address, type);

                        // query control lines count:
                        busModule.QueryControlLines();

                        addressesAdded.Add(address);
                        BusModules.Add(busModule);
                    }
                    else // module with this address is registered
                    {
                        // updated when added;
                    }
                }
                else
                {
                    // address is offline
                    
                    BusModule busModule = this[address];
                    if (busModule != null) // offline module
                    {
                        addressesRemoved.Add(address);
                        BusModules.Remove(busModule);
                    }
                }
            }

            NotifyBusModulesCollectionChanged(addressesAdded, addressesRemoved);
        }
        internal override bool BusModuleWriteRead(BusModule busModule, byte[] request, byte[] response)
        {
            I2CDevice.Configuration config = new I2CDevice.Configuration(busModule.Address, BusConfigurationI2C.ClockRate);
            return busConfig.Bus.TryGet(config, BusConfigurationI2C.Timeout, request, response);
        }
        //internal override bool BusModuleWrite(BusModule busModule, byte[] request)
        //{
        //    I2CDevice.Configuration config = new I2CDevice.Configuration(busModule.Address, BusConfiguration.ClockRate);
        //    return busConfig.Bus.TrySet(config, BusConfiguration.Timeout, request);
        //}
        #endregion
    }
}
