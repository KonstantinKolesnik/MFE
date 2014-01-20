using Gadgeteer.Modules.GHIElectronics;
using GHI.Premium.Net;
using MFE.Core;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System;
//using System.Net.Sockets;
using System.Threading;

namespace MFE.Net.Managers
{
    public class GadgeteerWiFiManager : INetworkManager
    {
        #region Fields
        //private PWM portNetworkLED = null;
        private string ssid;
        private string password;
        private WiFi_RS21 wifiModule = null;
        private WiFiRS9110 wifi = null;
        #endregion

        #region Events
        public event EventHandler Started;
        public event EventHandler Stopped;
        #endregion

        #region Constructor
        public GadgeteerWiFiManager(WiFi_RS21 wifi_RS21, string ssid, string password)//, Cpu.PWMChannel pinNetworkStatusLED)
        {
            //portNetworkLED = new PWM(pinNetworkStatusLED, 1, 0.5, true); // blink LED with 1 Hz

            wifiModule = wifi_RS21;
            wifi = wifi_RS21.Interface;
            wifi.WirelessConnectivityChanged += wifi_WirelessConnectivityChanged;
            wifi.NetworkAddressChanged += wifi_NetworkAddressChanged;

            this.ssid = ssid;
            this.password = password;
        }
        #endregion

        #region Public methods
        public void Start()
        {
            //portNetworkLED.Start();

            try
            {
                if (!wifi.IsOpen)
                    wifi.Open();

                //if (!wifi.NetworkInterface.IsDynamicDnsEnabled)
                //    wifi.NetworkInterface.EnableDynamicDns();

                if (!wifi.NetworkInterface.IsDhcpEnabled)
                    wifi.NetworkInterface.EnableDhcp();
                else
                    wifi.NetworkInterface.RenewDhcpLease();

                NetworkInterfaceExtension.AssignNetworkingStackTo(wifi);

                #region scan for network with required SSID:
                WiFiNetworkInfo[] nis = wifi.Scan(ssid);
                while (nis == null)
                {
                    Thread.Sleep(500);
                    Debug.Print("Searching for WiFi access point with required SSID...\n");
                    nis = wifi.Scan(ssid);
                }
                Debug.Print(WiFiNetworkInfoToString(nis[0]));
                #endregion

                #region scan for all networks:
                //WiFiNetworkInfo myNi = null;
                //while (myNi == null)
                //{
                //    // scan for all networks:
                //    WiFiNetworkInfo[] nis = null;
                //    while (nis == null)
                //    {
                //        Thread.Sleep(500);
                //        Debug.Print("Searching for WiFi access points...");
                //        nis = wifi.Scan();
                //    }
                //    // output networks info:
                //    Debug.Print("Found " + nis.Length.ToString() + " network(s):");
                //    foreach (WiFiNetworkInfo ni in nis)
                //    {
                //        Debug.Print("-----------------------------------------------------");
                //        Debug.Print(WiFiNetworkInfoToString(ni));
                //    }
                //    Debug.Print("-----------------------------------------------------");
                //}
                #endregion

                try
                {
                    Debug.Print("Connecting to " + nis[0].SSID + "...");
                    wifi.Join(nis[0], password);
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
                }
                catch (Exception e)
                {
                    Debug.Print("Join failed: " + e.Message);
                }

                Debug.Print("WiFi start completed");
            }
            catch (Exception ex)//(NetworkInterfaceExtensionException ex)
            {
                Debug.Print("WiFi start failed: " + ex.Message);
            }
        }
        #endregion

        #region Event handlers
        private void wifi_WirelessConnectivityChanged(object sender, WiFiRS9110.WirelessConnectivityEventArgs e)
        {
            Debug.Print("WirelessConnectivityChanged: " + e.IsConnected.ToString() + " - " + e.NetworkInformation.SSID);
            if (e.IsConnected)
            {
                if (wifi.IsActivated) // make sure that the event is fired by WiFi interface, not other networking interface.
                    if (wifi.IsLinkConnected)
                    {
                        Debug.Print("WiFi connection was established! IPAddress = " + wifiModule.NetworkSettings.IPAddress);

                        //portNetworkLED.DutyCycle = 0;

                        if (Started != null)
                            Started(this, EventArgs.Empty);
                    }
            }
            else
            {
                if (wifi.IsActivated) // make sure that the event is fired by WiFi interface, not other networking interface.
                    if (!wifi.IsLinkConnected)
                    {
                        Debug.Print("WiFi connection was dropped or disconnected!");

                        //portNetworkLED.DutyCycle = 1;
                        //portNetworkLED.Stop();

                        if (Stopped != null)
                            Stopped(this, EventArgs.Empty);

                        Start();
                    }
            }
        }
        private void wifi_NetworkAddressChanged(object sender, EventArgs e)
        {
            Debug.Print("**** Network Address Changed ******");
            Debug.Print("IP: " + wifiModule.NetworkSettings.IPAddress);
            Debug.Print("GatewayAddress: " + wifiModule.NetworkSettings.GatewayAddress);
            for (int i = 0; i < wifiModule.NetworkSettings.DnsAddresses.Length; i++)
                Debug.Print("DnsAddress " + i.ToString() + ": " + wifiModule.NetworkSettings.DnsAddresses[i]);
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
