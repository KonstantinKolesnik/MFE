using GHI.Premium.Net;
using Microsoft.SPOT;
using System;
using System.Threading;

namespace MFE.Net.Managers
{
    public class EthernetManager : INetworkManager
    {
        #region Fields
        //private ManualResetEvent blocker = new ManualResetEvent(false);
        private bool isBuiltIn = false;
        private NetworkInterfaceExtension ethernet;
        #endregion

        #region Events
        public event EventHandler Started;
        public event EventHandler Stopped;
        #endregion

        #region Constructor
        public EthernetManager(EthernetENC28J60 ethernetInterface = null)
        {
            isBuiltIn = ethernetInterface == null;

            if (!isBuiltIn)
            {
                ethernet = ethernetInterface;
                (ethernet as EthernetENC28J60).CableConnectivityChanged += (object sender, EthernetENC28J60.CableConnectivityEventArgs e) => { ethernet_CableConnectivityChanged(sender, e.IsConnected); };
            }
            else
            {
                ethernet = new EthernetBuiltIn();
                (ethernet as EthernetBuiltIn).CableConnectivityChanged += (object sender, EthernetBuiltIn.CableConnectivityEventArgs e) => { ethernet_CableConnectivityChanged(sender, e.IsConnected); };
            }

            ethernet.NetworkAddressChanged += new NetworkInterfaceExtension.NetworkAddressChangedEventHandler(ethernet_NetworkAddressChanged);
        }
        #endregion

        #region Public methods
        public void Start()
        {
            if (OpenEthernet() && EnableDHCP())
            {
                NetworkInterfaceExtension.AssignNetworkingStackTo(ethernet);

                // wait for cable:
                //if (isBuiltIn)
                //{
                //    if (!(ethernet as EthernetBuiltIn).IsCableConnected)
                //    {
                //        do
                //        {
                //            if (!(ethernet as EthernetBuiltIn).IsCableConnected)
                //                Debug.Print("Waiting for ethernet cable...");
                //            else
                //                break;
                //        }
                //        while (!blocker.WaitOne(500, false));
                //    }
                //}
                //else
                //{
                //    if (!(ethernet as EthernetENC28J60).IsCableConnected)
                //    {
                //        do
                //        {
                //            if (!(ethernet as EthernetENC28J60).IsCableConnected)
                //                Debug.Print("Waiting for ethernet cable...");
                //            else
                //                break;
                //        }
                //        while (!blocker.WaitOne(500, false));
                //    }
                //}

                //while (ethernet.NetworkInterface.IPAddress == "0.0.0.0")
                //{
                //    Debug.Print("Waiting for IPAddress");
                //    Thread.Sleep(250);
                //}
            }
        }
        #endregion

        #region Event handlers
        private void ethernet_CableConnectivityChanged(object sender, bool isConnected)
        {
            ////Debug.Print("Built-in Ethernet Cable is " + (isConnected ? "Connected!" : "Disconnected!"));

            //if (ethernet.IsActivated) // make sure that the event is fired by ethernet interface, not other networking interface.
            //{
            //    if (isConnected)
            //    {
            //        //Debug.Print("Ethernet connection was established! IPAddress = " + ethernet.NetworkInterface.IPAddress);

            //        blocker.Set();

            //        if (Started != null)
            //            Started(this, EventArgs.Empty);
            //    }
            //    else
            //    {
            //        //Debug.Print("Ethernet connection was dropped or disconnected!");

            //        blocker.Set();

            //        if (Stopped != null)
            //            Stopped(this, EventArgs.Empty);
            //    }
            //}
        }
        private void ethernet_NetworkAddressChanged(object sender, EventArgs e)
        {
            //Debug.Print("New address for The built-in Ethernet Network Interface ");
            //Debug.Print("Is DhCp enabled: " + ethernet.NetworkInterface.IsDhcpEnabled);
            //Debug.Print("Is DynamicDnsEnabled enabled: " + ethernet.NetworkInterface.IsDynamicDnsEnabled);
            //Debug.Print("NetworkInterfaceType " + ethernet.NetworkInterface.NetworkInterfaceType);
            //Debug.Print("Network settings:");
            //Debug.Print("IP Address: " + ethernet.NetworkInterface.IPAddress);
            //Debug.Print("Subnet Mask: " + ethernet.NetworkInterface.SubnetMask);
            //Debug.Print("Default Getway: " + ethernet.NetworkInterface.GatewayAddress);
            //Debug.Print("Number of DNS servers:" + ethernet.NetworkInterface.DnsAddresses.Length);
            //for (int i = 0; i < ethernet.NetworkInterface.DnsAddresses.Length; i++)
            //    Debug.Print("DNS Server " + i.ToString() + ":" + ethernet.NetworkInterface.DnsAddresses[i]);
            //Debug.Print("------------------------------------------------------");

            if (ethernet.NetworkInterface.IPAddress != "0.0.0.0")
            {
                if (Started != null)
                    Started(this, EventArgs.Empty);
            }
            else
            {
                if (Stopped != null)
                    Stopped(this, EventArgs.Empty);
            }
        }
        #endregion

        #region Private methods
        private bool OpenEthernet()
        {
            try
            {
                if (!ethernet.IsOpen)
                    ethernet.Open();
            }
            catch (NetworkInterfaceExtensionException e)
            {
                switch (e.errorCode)
                {
                    case NetworkInterfaceExtensionException.ErrorCode.AlreadyActivated: break;
                    case NetworkInterfaceExtensionException.ErrorCode.HardwareFirmwareVersionMismatch:
                        ethernet.Open();
                        break;
                    case NetworkInterfaceExtensionException.ErrorCode.HardwareCommunicationFailure:
                    case NetworkInterfaceExtensionException.ErrorCode.HardwareNotEnabled:
                    case NetworkInterfaceExtensionException.ErrorCode.HardwareCommunicationTimeout:
                        Debug.Print("Error Message: " + e.ErrorMsg);
                        ethernet.Open();
                        break;
                    default:
                        Debug.Print("Error Message: " + e.ErrorMsg);
                        return false;
                }
            }
            catch (Exception e)
            {
                Debug.Print("Open Error: " + e.Message);
                return false;
            }

            Debug.Print("\nOpened successfully!");

            return true;
        }
        private bool EnableDHCP()
        {
            Debug.Print("Enable DHCP...\n");

            try
            {
                #region Dynamic IP
                if (!ethernet.NetworkInterface.IsDynamicDnsEnabled)
                    ethernet.NetworkInterface.EnableDynamicDns();

                if (!ethernet.NetworkInterface.IsDhcpEnabled)
                    ethernet.NetworkInterface.EnableDhcp(); // This function is blocking
                else
                    ethernet.NetworkInterface.RenewDhcpLease(); // This function is blocking
                #endregion

                #region Static IP
                // Uncomment the following line if you want to use a static IP address, and comment out the DHCP code region above
                //ethernet.NetworkInterface.EnableStaticIP("192.168.1.110", "255.255.255.0", "192.168.1.1");
                //ethernet.NetworkInterface.EnableStaticDns(new string[] { "10.1.10.1" });
                #endregion

                //Debug.Print("Network settings:");
                //Debug.Print("IP Address: " + ethernet.NetworkInterface.IPAddress);
                //Debug.Print("Subnet Mask: " + ethernet.NetworkInterface.SubnetMask);
                //Debug.Print("Default Getway: " + ethernet.NetworkInterface.GatewayAddress);
                //Debug.Print("DNS Server: " + ethernet.NetworkInterface.DnsAddresses[0]);
            }
            catch (Exception e)//SocketException e
            {
                Debug.Print("DHCP faild");
                //if (e.ErrorCode == 11003)
                //    Debug.Print("Re-Enable the module.");

                return false;
            }

            return true;
        }
        #endregion
    }
}
