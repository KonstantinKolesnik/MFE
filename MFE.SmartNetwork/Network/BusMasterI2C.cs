using MFE.Hardware;
using Microsoft.SPOT.Hardware;
using System.Collections;

namespace MFE.SmartNetwork.Network
{
    public class BusMasterI2C : BusMasterBase
    {
        #region Fields
        private BusConfigurationI2C busConfig;
        #endregion

        #region Constructor
        public BusMasterI2C(BusConfigurationI2C busConfig)
            : base(0)
        {
            this.busConfig = busConfig;
        }
        #endregion

        #region Protected methods
        protected override ArrayList GetOnlineModules()
        {
            ArrayList result = new ArrayList();

            for (byte address = 1; address <= 127; address++)
            {
                byte type = (byte)BusModuleType.Unknown;

                var config = new I2CDevice.Configuration(address, BusConfigurationI2C.ClockRate); // config for I2C-module with "address"
                if (busConfig.Bus.TryGetRegister(config, BusConfigurationI2C.Timeout, BusModuleAPI.CmdGetType, out type)) // query module
                    result.Add(new BusModule(this, new byte[] { address }, (BusModuleType)type));
            }

            return result;
        }
        protected override void ScanModules(out ArrayList modulesAdded, out ArrayList modulesRemoved)
        {
            modulesAdded = new ArrayList();
            modulesRemoved = new ArrayList();

            for (byte address = 1; address <= 127; address++)
            {
                byte type = (byte)BusModuleType.Unknown;

                var config = new I2CDevice.Configuration(address, BusConfigurationI2C.ClockRate); // config for I2C-module with "address"
                if (busConfig.Bus.TryGetRegister(config, BusConfigurationI2C.Timeout, BusModuleAPI.CmdGetType, out type)) // query module
                {
                    // module with this address is online;
                    // check if it's already registered in BusModules:

                    BusModule busModule = this[new byte[] { address }];

                    if (busModule == null) // module with this address isn't registered
                    {
                        busModule = new BusModule(this, new byte[] { address }, (BusModuleType)type);

                        // query module control lines count with updating lines states:
                        //busModule.QueryControlLines(true);

                        // register this module in BusModules:
                        modulesAdded.Add(busModule);
                        BusModules.Add(busModule);
                    }
                    else // module with this address is already registered
                    {
                        // updated when added;
                    }
                }
                else
                {
                    // module with this address is offline;
                    // check if it's already registered in BusModules:

                    BusModule busModule = this[new byte[] { address }];

                    if (busModule != null) // offline module
                    {
                        modulesRemoved.Add(busModule);
                        BusModules.Remove(busModule);
                    }
                }
            }
        }
        
        internal override bool BusModuleWriteRead(BusModule busModule, byte[] request, byte[] response)
        {
            I2CDevice.Configuration config = new I2CDevice.Configuration(busModule.Address[0], BusConfigurationI2C.ClockRate);
            return busConfig.Bus.TryGet(config, BusConfigurationI2C.Timeout, request, response);
        }
        internal override bool BusModuleWrite(BusModule busModule, byte[] request)
        {
            I2CDevice.Configuration config = new I2CDevice.Configuration(busModule.Address[0], BusConfigurationI2C.ClockRate);
            return busConfig.Bus.TrySet(config, BusConfigurationI2C.Timeout, request);
        }
        #endregion
    }
}
