using GHI.Premium.System;
using GHI.Premium.USBHost;
using MFE.Core;
using System;

namespace MFE.USB
{
    public static class USBDeviceManager
    {
        #region Properties
        public static USBH_Device[] Devices
        {
            get
            {
                return USBHostController.GetDevices();
            }
        }
        #endregion

        #region Events
        public static event USBH_DeviceConnectionEventHandler USBDeviceInserted;
        public static event USBH_DeviceConnectionEventHandler USBDeviceRemoved;
        public static event USBH_DeviceConnectionEventHandler USBDeviceBadConnection;
        #endregion

        #region Constructor
        static USBDeviceManager()
        {
            if (!Utils.IsEmulator)
            {
                try
                {
                    USBHostController.DeviceConnectedEvent += new USBH_DeviceConnectionEventHandler(USBDevice_Connected);
                    USBHostController.DeviceDisconnectedEvent += new USBH_DeviceConnectionEventHandler(USBDevice_Disconnected);
                    USBHostController.DeviceBadConnectionEvent += new USBH_DeviceConnectionEventHandler(USBDevice_BadConnection);
                }
                catch (Exception) { }
            }
        }
        #endregion

        #region Public methods
        public static int GetCount()
        {
            return USBHostController.GetDevices().Length;
        }
        public static int GetCount(USBH_DeviceType type)
        {
            int n = 0;
            foreach (USBH_Device device in USBHostController.GetDevices())
                if (device.TYPE == type)
                    n++;

            return n;
        }
        #endregion

        #region Event handlers
        private static void USBDevice_Connected(USBH_Device device)
        {
            if (USBDeviceInserted != null)
                USBDeviceInserted(device);
        }
        private static void USBDevice_Disconnected(USBH_Device device)
        {
            if (USBDeviceRemoved != null)
                USBDeviceRemoved(device);
        }
        private static void USBDevice_BadConnection(USBH_Device device)
        {
            if (USBDeviceBadConnection != null)
                USBDeviceBadConnection(device);
        }
        #endregion
    }
}
