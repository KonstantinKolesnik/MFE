using GHI.Premium.Net;
using MFE.Core;
using Microsoft.SPOT;
using System;
using System.Threading;

namespace MFE.Net.Managers
{
    public class WiFiManager : INetworkManager
    {
        #region Fields
        //private ManualResetEvent blocker = new ManualResetEvent(false);
        private WiFiRS9110 wifi = null;
        private string ssid;
        private string password;

        // ChipworkX Developement System V1.5 UEXT header with WiFi RS21 Module: P/N:GHI-WIFIEXP2-298
        //private SPI.SPI_module _spi = SPI.SPI_module.SPI2; /*SPI bus*/
        //private Cpu.Pin _cs = ChipworkX.Pin.PC9; /*ChipSelect*/
        //private Cpu.Pin _extInt = ChipworkX.Pin.PA19; /*External Interrupt*/
        //private Cpu.Pin _reset = ChipworkX.Pin.PC8; /*Reset*/
        /*-------------------------------------------------------------------*/
        // EMX Developement System V1.3 UEXT header with WiFi RS21 Module: P/N:GHI-WIFIEXP2-298
        //private SPI.SPI_module _spi = SPI.SPI_module.SPI1; /*SPI bus*/
        //private Cpu.Pin _cs = EMX.Pin.IO2; /*ChipSelect*/
        //private Cpu.Pin _extInt = EMX.Pin.IO26; /*External Interrupt*/
        //private Cpu.Pin _reset = EMX.Pin.IO3; /*Reset*/
        /*-------------------------------------------------------------------*/
        // FEZ Cobra OEM board V1.2 or V1.3 UEXT header with WiFi RS21 Module: P/N:GHI-WIFIEXP2-298
        //private SPI.SPI_module _spi = SPI.SPI_module.SPI2; /*SPI bus*/
        //private Cpu.Pin _cs = (Cpu.Pin)2; /*ChipSelect*/
        //private Cpu.Pin _extInt = (Cpu.Pin)26; /*External Interrupt*/
        //private Cpu.Pin _reset = (Cpu.Pin)3; /*Reset*/
        #endregion

        #region Events
        public event EventHandler Started;
        public event EventHandler Stopped;
        #endregion

        #region Constructor
        public WiFiManager(WiFiRS9110 wifi, string ssid, string password)
        {
            this.wifi = wifi;
            this.ssid = ssid;
            this.password = password;

            wifi.WirelessConnectivityChanged += new WiFiRS9110.WirelessConnectivityChangedEventHandler(wifi_WirelessConnectivityChanged);
            wifi.NetworkAddressChanged += new NetworkInterfaceExtension.NetworkAddressChangedEventHandler(wifi_NetworkAddressChanged);
        }
        #endregion

        #region Public methods
        public void Start()
        {
            if (OpenWiFi() && EnableDHCP())
            {
                NetworkInterfaceExtension.AssignNetworkingStackTo(wifi);

                WiFiNetworkInfo ni = SearchForNetwork();

                if (!Join(ni))
                    if (Stopped != null)
                        Stopped(this, EventArgs.Empty);
            }
        }
        #endregion

        #region Event handlers
        private void wifi_WirelessConnectivityChanged(object sender, WiFiRS9110.WirelessConnectivityEventArgs e)
        {
            Debug.Print("Network Availability Event Triggered");

            if (wifi.IsActivated) // make sure that the event is fired by WiFi interface, not other networking interface.
            {

                if (e.IsConnected)
                {
                    if (wifi.IsLinkConnected)
                    {
                        //blocker.Set();
                        Debug.Print("WiFi connection was established!");

                        if (Started != null)
                            Started(this, EventArgs.Empty);
                    }
                }
                else
                {
                    if (!wifi.IsLinkConnected)
                    {
                        //blocker.Set();
                        Debug.Print("WiFi connection was dropped or disconnected!");

                        if (Stopped != null)
                            Stopped(this, EventArgs.Empty);
                    }
                }
            }
        }
        private void wifi_NetworkAddressChanged(object sender, EventArgs e)
        {
            Debug.Print("**** Network Address Changed ******");
            Debug.Print("IP: " + wifi.NetworkInterface.IPAddress);
            Debug.Print("GatewayAddress: " + wifi.NetworkInterface.GatewayAddress);
            for (int i = 0; i < wifi.NetworkInterface.DnsAddresses.Length; i++)
                Debug.Print("DnsAddress " + i.ToString() + ": " + wifi.NetworkInterface.DnsAddresses[i]);
        }
        #endregion

        #region Private methods
        private bool OpenWiFi()
        {
            try
            {
                if (!wifi.IsOpen)
                    wifi.Open();
            }
            catch (NetworkInterfaceExtensionException e)
            {
                switch (e.errorCode)
                {
                    case NetworkInterfaceExtensionException.ErrorCode.AlreadyActivated: break;
                    case NetworkInterfaceExtensionException.ErrorCode.HardwareFirmwareVersionMismatch:
                        wifi.UpdateFirmware();
                        wifi.Open();
                        break;
                    case NetworkInterfaceExtensionException.ErrorCode.HardwareCommunicationFailure:
                    case NetworkInterfaceExtensionException.ErrorCode.HardwareNotEnabled:
                    case NetworkInterfaceExtensionException.ErrorCode.HardwareCommunicationTimeout:
                        Debug.Print("Error Message: " + e.ErrorMsg);
                        Debug.Print("Check WiFi module hardware connections and SPI/signals configurations.");
                        wifi.Open();
                        break;
                    default:
                        Debug.Print("Error Message: " + e.ErrorMsg);
                        return false;
                }
            }
            catch (Exception e)
            {
                Debug.Print("EnableWiFi Error: " + e.Message);
                return false;
            }

            //NetworkInterfaceExtension.AssignNetworkingStackTo(wifi);
            Debug.Print("\nEnabled successfully!\nAt this point, the on-board LED on RS9110_N_11_21_1_Compatible module is ON.\n");
            return true;
        }
        private bool EnableDHCP()
        {
            Debug.Print("Enable DHCP...\n");

            try
            {
                #region Dynamic IP
                if (!wifi.NetworkInterface.IsDynamicDnsEnabled)
                    wifi.NetworkInterface.EnableDynamicDns();

                if (!wifi.NetworkInterface.IsDhcpEnabled)
                    wifi.NetworkInterface.EnableDhcp(); // This function is blocking
                else
                    wifi.NetworkInterface.RenewDhcpLease(); // This function is blocking
                #endregion

                #region Static IP
                // Uncomment the following line if you want to use a static IP address, and comment out the DHCP code region above
                //wifi.NetworkInterface.EnableStaticIP("192.168.1.110", "255.255.255.0", "192.168.1.1");
                //wifi.NetworkInterface.EnableStaticDns(new string[] { "10.1.10.1" });
                #endregion

                Debug.Print("Network settings:");
                Debug.Print("IP Address: " + wifi.NetworkInterface.IPAddress);
                Debug.Print("Subnet Mask: " + wifi.NetworkInterface.SubnetMask);
                Debug.Print("Default Getway: " + wifi.NetworkInterface.GatewayAddress);
                Debug.Print("DNS Server: " + wifi.NetworkInterface.DnsAddresses[0]);
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
        private WiFiNetworkInfo SearchForNetwork()
        {
            WiFiNetworkInfo[] nis = null;

            while (nis == null)
            {
                Thread.Sleep(250);
                Debug.Print("Searching for WiFi access point with required SSID...\n");
                nis = wifi.Scan(ssid);
            }

            Debug.Print(WiFiNetworkInfoToString(nis[0]));
            return nis[0];
        }
        private bool Join(WiFiNetworkInfo ni)
        {
            if (wifi.IsLinkConnected)
                return true;

            try
            {
                Debug.Print("Connecting to " + ni.SSID + "...");
                //blocker.Reset();
                wifi.Join(ni, password);

                while (wifi.NetworkInterface.IPAddress == "0.0.0.0")
                {
                    Debug.Print("Waiting for IPAddress");
                    Thread.Sleep(250);
                }
            }
            catch (NetworkInterfaceExtensionException e)
            {
                switch (e.errorCode)
                {
                    case NetworkInterfaceExtensionException.ErrorCode.AuthenticationFailed: Debug.Print("AuthenticationFailed"); break;
                    //case NetworkInterfaceExtensionException.ErrorCode.AlreadyActivated: break;
                    default: Debug.Print(e.errorCode.ToString()); break;
                }
                //Debug.Print("Error Message: " + e.ErrorMsg);
                //if (e.errorCode != NetworkInterfaceExtensionException.ErrorCode.AlreadyActivated)
                return false;
            }
            catch (Exception e)
            {
                Debug.Print("Error Message: " + e.Message);
                return false;
            }

            Debug.Print("Done connecting...\n");
            //blocker.WaitOne();
            Debug.Print("We got NetworkAvailable event. WiFi link is ready!\n");

            return true;
        }
        private void Disconnect()
        {
            if (wifi.IsLinkConnected)
                wifi.Disconnect();
        }
        private static string WiFiNetworkInfoToString(WiFiNetworkInfo info)
        {
            string str = "SSID: " + info.SSID + "\n";
            str += "Channel Number: " + info.ChannelNumber + "\n";
            str += "RSSI: -" + info.RSSI + "dB" + "\n";
            str += "Security Mode: ";
            switch (info.SecMode)
            {
                case SecurityMode.Open: str += "Open"; break;
                case SecurityMode.WEP: str += "WEP"; break;
                case SecurityMode.WPA: str += "WPA"; break;
                case SecurityMode.WPA2: str += "WPA2"; break;
            };
            str += "\n";
            str += "Network Type: ";
            switch (info.networkType)
            {
                case NetworkType.AccessPoint: str += "Access Point"; break;
                case NetworkType.AdHoc: str += "AdHoc"; break;
            }
            str += "\n";
            str += "BS MAC: " +
                Utils.ByteToHex(info.PhysicalAddress[0]) + "-" +
                Utils.ByteToHex(info.PhysicalAddress[1]) + "-" +
                Utils.ByteToHex(info.PhysicalAddress[2]) + "-" +
                Utils.ByteToHex(info.PhysicalAddress[3]) + "-" +
                Utils.ByteToHex(info.PhysicalAddress[4]) + "-" +
                Utils.ByteToHex(info.PhysicalAddress[5]);
            str += "\n";

            return str;
        }
        #endregion
    }
}

/*
    private static WiFiRS9110 netif;

    public static void Main()
    {
        NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
        NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;

        netif = new WiFiRS9110(SPI.SPI_module.SPI1, Cpu.Pin.GPIO_Pin1, Cpu.Pin.GPIO_Pin2, Cpu.Pin.GPIO_Pin3);
        netif.Open();
        netif.EnableDhcp();
        netif.EnableDynamicDns();
        netif.Join("SSID", "Password");

        while (netif.IPAddress == "0.0.0.0")
        {
            Debug.Print("Waiting for DHCP");
            Thread.Sleep(250);
        }

        //The network is now ready to use.
    }

    private static void NetworkChange_NetworkAddressChanged(object sender, Microsoft.SPOT.EventArgs e)
    {
        Debug.Print("Network address changed");
    }
    private static void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
    {
        Debug.Print("Network availability: " + e.IsAvailable.ToString());
    }
*/