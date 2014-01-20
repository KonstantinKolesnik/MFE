using System;
//using System.Net.Sockets;
using System.Threading;
using GHI.Premium.Net;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Gadgeteer.Modules.GHIElectronics;
using MFE.Core;
//using System.Net;

namespace MFE.Net.Managers
{
    public class WiFiManager : INetworkManager
    {
        #region Fields
        private ManualResetEvent blocker = null;
        //private PWM portNetworkLED = null;
        private string ssid;
        private string password;
        private WiFiRS9110 wifi = null;
        private WiFi_RS21 wifi_RS21 = null;

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
        public WiFiManager(WiFi_RS21 wifi_RS21, string ssid, string password)//, Cpu.PWMChannel pinNetworkStatusLED)
        {
            blocker = new ManualResetEvent(false);
            //portNetworkLED = new PWM(pinNetworkStatusLED, 1, 0.5, ledToVcc); // blink LED with 1 Hz

            wifi = wifi_RS21.Interface;
            wifi.WirelessConnectivityChanged += new WiFiRS9110.WirelessConnectivityChangedEventHandler(wifi_WirelessConnectivityChanged);
            //wifi.NetworkAddressChanged += new NetworkInterfaceExtension.NetworkAddressChangedEventHandler(wifi_NetworkAddressChanged);

            this.ssid = ssid;
            this.password = password;
        }
        #endregion

        #region Public methods
        public void Start()
        {
            //wifi_RS21.NetworkDown += new Gadgeteer.Modules.Module.NetworkModule.NetworkEventHandler(wifi_NetworkDown);
            //wifi_RS21.NetworkUp += new Gadgeteer.Modules.Module.NetworkModule.NetworkEventHandler(wifi_NetworkUp);

            //// use the router's DHCP server to set my network info
            //if (!wifi_RS21.Interface.NetworkInterface.IsDhcpEnabled)
            //    wifi_RS21.UseDHCP();

            //// look for avaiable networks
            ////GHI.Premium.Net.WiFiNetworkInfo[] scanResults = wifi_RS21.Interface.Scan();
            ////foreach (GHI.Premium.Net.WiFiNetworkInfo result in scanResults)
            ////{
            ////    Debug.Print("****" + result.SSID + "****");
            ////    Debug.Print("ChannelNumber = " + result.ChannelNumber);
            ////    Debug.Print("networkType = " + result.networkType);
            ////    Debug.Print("PhysicalAddress = " + GetMACAddress(result.PhysicalAddress));
            ////    Debug.Print("RSSI = " + result.RSSI);
            ////    Debug.Print("SecMode = " + result.SecMode);
            ////}

            //// locate a specific network
            //WiFiNetworkInfo[] nis = wifi_RS21.Interface.Scan(ssid);
            //while (nis == null)
            //{
            //    Thread.Sleep(500);
            //    Debug.Print("Searching for WiFi access point with required SSID...\n");
            //    nis = wifi_RS21.Interface.Scan(ssid);
            //}
            //Debug.Print(WiFiNetworkInfoToString(nis[0]));
            //wifi_RS21.Interface.Join(nis[0], password);

            //if (Started != null)
            //    Started(this, EventArgs.Empty);

            //return;





            //portNetworkLED.Start();

            //wifi.UpdateFirmware();
            //if (!wifi_RS21.Interface.NetworkInterface.IsDhcpEnabled)
            //    wifi_RS21.UseDHCP();


            if (EnableWiFi())
            {
                //WiFiNetworkInfo ni = Scan();
                WiFiNetworkInfo ni = Scan2();

                if (Connect(ni) && EnableDHCP())
                {
                    //portNetworkLED.DutyCycle = ledToVcc ? 0 : 1;
                    if (Started != null)
                        Started(this, EventArgs.Empty);

                    //Debug.Print("Test DNS");
                    //try
                    //{
                    //    IPHostEntry myIP = Dns.GetHostEntry("www.ghielectronics.com");
                    //    if (myIP != null)
                    //        Debug.Print(myIP.HostName + ": " + myIP.AddressList[0].ToString());
                    //}
                    //catch (SocketException e)
                    //{
                    //    Debug.Print("Faild to Get the host entry of the FQN from DNS server!");
                    //    if (e.ErrorCode == 11003)
                    //        Debug.Print("Re-Enable the module.");
                    //    throw;
                    //}

                    return;
                }
            }

            //portNetworkLED.DutyCycle = ledToVcc ? 1 : 0;
            //portNetworkLED.Stop();
        }

        public void OnBeforeMessage()
        {
            //portNetworkLED.DutyCycle = ledToVcc ? 1 : 0;
        }
        public void OnAfterMessage()
        {
            //portNetworkLED.DutyCycle = ledToVcc ? 0 : 1;
        }
        #endregion

        #region Event handlers
        //private void wifi_NetworkUp(Gadgeteer.Modules.Module.NetworkModule sender, Gadgeteer.Modules.Module.NetworkModule.NetworkState state)
        //{
        //    if (state == Gadgeteer.Modules.Module.NetworkModule.NetworkState.Up)
        //        Debug.Print("Network Up event; state = Up");
        //    else
        //        Debug.Print("Network Up event; state = Down");
        //}
        //private void wifi_NetworkDown(Gadgeteer.Modules.Module.NetworkModule sender, Gadgeteer.Modules.Module.NetworkModule.NetworkState state)
        //{
        //    if (state == Gadgeteer.Modules.Module.NetworkModule.NetworkState.Down)
        //    {
        //        Debug.Print("Network Up event; state = Down");
        //        //if (Stopped != null)
        //        //    Stopped(this, EventArgs.Empty);
        //        //Start();
        //    }
        //    else
        //        Debug.Print("Network Up event; state = Up");
        //}

        private void wifi_WirelessConnectivityChanged(object sender, WiFiRS9110.WirelessConnectivityEventArgs e)
        {
            Debug.Print("Network Availability Event Triggered");

            if (e.IsConnected)
            {
                if (wifi.IsActivated) // make sure that the event is fired by WiFi interface, not other networking interface.
                    if (wifi.IsLinkConnected)
                    {
                        blocker.Set();
                        Debug.Print("WiFi connection was established!");
                    }
            }
            else
            {
                if (wifi.IsActivated) // make sure that the event is fired by WiFi interface, not other networking interface.
                    if (!wifi.IsLinkConnected)
                    {
                        blocker.Set();
                        Debug.Print("WiFi connection was dropped or disconnected!");
                        if (Stopped != null)
                            Stopped(this, EventArgs.Empty);

                        Start();
                    }
            }




            //if (!e.IsConnected && wifi.IsActivated)// We need to try to reconnect because the TCPIP stack is assigned to wifi
            //{
            //    Debug.Print(e.IsConnected ? " WiFi link is connected" : "WiFi connection was lost");
            //    //if (!ConnectWiFiThread.IsAlive)
            //    {
            //        ConnectWiFi();
            //        //NetworkInterfaceExtension.AssignNetworkingStackTo(wifi);
            //    }
            //}
        }
        private void wifi_NetworkAddressChanged(object sender, EventArgs e)
        {
            //Debug.Print("AddressChanged");
        }
        #endregion

        #region Private methods
        private bool EnableWiFi()
        {
            try
            {
                Thread.Sleep(2000);
                wifi.Open();
                NetworkInterfaceExtension.AssignNetworkingStackTo(wifi);
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
        //private WiFiNetworkInfo Scan()
        //{
        //    WiFiNetworkInfo myNi = null;
        //    while (myNi == null)
        //    {
        //        // scan for all networks:
        //        WiFiNetworkInfo[] nis = null;
        //        while (nis == null)
        //        {
        //            Thread.Sleep(500);
        //            Debug.Print("Searching for WiFi access points...");
        //            nis = wifi.Scan();
        //        }

        //        // output networks info:
        //        Debug.Print("Found " + nis.Length.ToString() + " network(s):");
        //        foreach (WiFiNetworkInfo ni in nis)
        //        {
        //            Debug.Print("-----------------------------------------------------");
        //            Debug.Print(WiFiNetworkInfoToString(ni));
        //        }
        //        Debug.Print("-----------------------------------------------------");

        //        // check for required SSID:
        //        Debug.Print("Check for required SSID...");
        //        foreach (WiFiNetworkInfo ni in nis)
        //            if (string.Compare(ni.SSID, ssid) == 0)
        //            {
        //                myNi = ni;
        //                break;
        //            }
        //    }

        //    return myNi;
        //}
        private WiFiNetworkInfo Scan2()
        {
            // scan for network with required SSID:
            WiFiNetworkInfo[] nis = wifi.Scan(ssid);
            while (nis == null)
            {
                Thread.Sleep(500);
                Debug.Print("Searching for WiFi access point with required SSID...\n");
                nis = wifi.Scan(ssid);
            }
            Debug.Print(WiFiNetworkInfoToString(nis[0]));
            return nis[0];
        }
        private bool Connect(WiFiNetworkInfo ni)
        {
            if (wifi.IsLinkConnected)
                return true;

            try
            {
                Debug.Print("Connecting to " + ni.SSID + "...");
                blocker.Reset();
                wifi.Join(ni, password);
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
            blocker.WaitOne();
            Debug.Print("We got NetworkAvailable event. WiFi link is ready!\n");
            return true;
        }
        private bool EnableDHCP()
        {
            Debug.Print("Enable DHCP...\n");

            try
            {
                #region DHCP Code (dynamic IP)
                if (!wifi.NetworkInterface.IsDhcpEnabled)
                    wifi.NetworkInterface.EnableDhcp(); // This function is blocking
                else
                    wifi.NetworkInterface.RenewDhcpLease(); // This function is blocking
                #endregion

                #region Static IP code
                // Uncomment the following line if you want to use a static IP address, and comment out the DHCP code region above
                //wifi.NetworkInterface.EnableStaticIP("192.168.0.110", "255.255.255.0", "192.168.0.1");
                //wifi.NetworkInterface.EnableStaticDns(new string[] { "10.1.10.1" });
                #endregion

                Debug.Print("Network settings:");
                Debug.Print("IP Address: " + wifi.NetworkInterface.IPAddress);
                Debug.Print("Subnet Mask: " + wifi.NetworkInterface.SubnetMask);
                Debug.Print("Default Getway: " + wifi.NetworkInterface.GatewayAddress);
                Debug.Print("DNS Server: " + wifi.NetworkInterface.DnsAddresses[0]);

                return true;
            }
            catch (Exception e)//SocketException e
            {
                Debug.Print("DHCP faild");
                //if (e.ErrorCode == 11003)
                //    Debug.Print("Re-Enable the module.");
            }

            return false;
        }
        private void Disconnect()
        {
            if (wifi.IsLinkConnected)
                wifi.Disconnect();
        }
        #endregion

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
    }
}
