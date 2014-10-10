
using System.Collections;
namespace MFE.SmartNetwork.Network
{
    public class BusMasterRF : BusMasterBase
    {
        #region Constructor
        public BusMasterRF(uint address)
            : base(address)
        {
        }
        #endregion

        #region Private methods
        protected override ArrayList GetOnlineModules()
        {
            ArrayList result = new ArrayList();

            return result;
        }
        protected override void ScanModules(out ArrayList modulesAdded, out ArrayList modulesRemoved)
        {
            modulesAdded = new ArrayList();
            modulesRemoved = new ArrayList();

        }
        internal override bool BusModuleWriteRead(BusModule busModule, byte[] request, byte[] response)
        {
            return false;
        }
        internal override bool BusModuleWrite(BusModule busModule, byte[] request)
        {
            return false;
        }
        #endregion
    }
}
