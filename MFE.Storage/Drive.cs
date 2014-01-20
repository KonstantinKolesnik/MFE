using GHI.Premium.IO;
using GHI.Premium.System;
using Microsoft.SPOT.IO;
using System;

namespace MFE.Storage
{
    /// <summary>
    /// SD card or USB Hard Drive
    /// </summary>
    [Serializable]
    public struct Drive
    {
        public VolumeInfo VolumeInfo;

        public PersistentStorage Storage;
        public USBH_Device Device;
    }
}
