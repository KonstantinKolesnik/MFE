
using System.Collections;
namespace MFE.SmartNetwork.Network
{
    public class BusHubRF : BusHubBase
    {
        #region Constructor
        public BusHubRF(uint address)
            : base(address)
        {
        }
        #endregion

        #region Private methods
        protected override void ScanModules(out ArrayList modulesAdded, out ArrayList modulesRemoved)
        {
            modulesAdded = new ArrayList();
            modulesRemoved = new ArrayList();

        }
        internal override bool BusModuleWriteRead(BusModule busModule, byte[] request, byte[] response)
        {
            return false;
        }
        //internal override bool BusModuleWrite(BusModule busModule, byte[] request)
        //{
        //    return false;
        //}
        #endregion
    }
}
