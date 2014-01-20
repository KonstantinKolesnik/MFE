using System;
using Microsoft.SPOT;
//using System.Net;
using System.Text;
using System.Threading;
using GHI.Premium.Net;
using Microsoft.SPOT.Hardware;
using Gadgeteer.Modules.GHIElectronics;
using Microsoft.SPOT.Net.NetworkInformation;

namespace MFE.Net.Managers
{
    public class GadgeteerEthernetManager : INetworkManager
    {
        #region Fields
        //private PWM portNetworkLED = null;
        private ManualResetEvent blocker = null;

        // if you were to use ENC28J60-based Ethernet connection
        //static EthernetENC28J60 Eth1 = new EthernetENC28J60(add code here to configure it);
        // wifi is same thing
        // static WiFiRS9110 wifi = new WiFiRS9110(......);

        private Ethernet_ENC28 ethernetModule = null;
        private EthernetENC28J60 ethernet = null;

        //static EthernetBuiltIn ethernet;
        #endregion

        #region Events
        public event EventHandler Started;
        public event EventHandler Stopped;
        #endregion

        #region Constructor
        public GadgeteerEthernetManager(Ethernet_ENC28 ethernet_ENC28)//Cpu.PWMChannel pinNetworkStatusLED)
        {
            blocker = new ManualResetEvent(false);
            //portNetworkLED = new PWM(pinNetworkStatusLED, 1, 50, false); // blink LED with 1 Hz

            ethernetModule = ethernet_ENC28;
            ethernet = ethernet_ENC28.Interface;

            //ethernet = new EthernetBuiltIn();

            ethernet.CableConnectivityChanged += ethernet_CableConnectivityChanged;
            ethernet.NetworkAddressChanged += ethernet_NetworkAddressChanged;
        }
        #endregion

        #region Public methods
        public void Start()
        {
            //portNetworkLED.Start();

            if (!ethernet.IsOpen)
                ethernet.Open();

            if (!ethernet.NetworkInterface.IsDhcpEnabled)
                ethernet.NetworkInterface.EnableDhcp();
            else
                ethernet.NetworkInterface.RenewDhcpLease();

            NetworkInterfaceExtension.AssignNetworkingStackTo(ethernet);

            // wait for cable:
            if (!ethernet.IsCableConnected)
            {
                do
                {
                    if (!ethernet.IsCableConnected)
                        Debug.Print("Waiting for ethernet cable...");
                    else
                        break;
                }
                while (!blocker.WaitOne(500, false));
            }

            Debug.Print("Ethernet start completed");

            if (ethernet.IsActivated)
            {
                if (Started != null)
                    Started(this, EventArgs.Empty);
            }

            //portNetworkLED.Frequency = 0;//.Set(false);
        }
        #endregion

        #region Event handlers
        private void ethernet_CableConnectivityChanged(object sender, EthernetENC28J60.CableConnectivityEventArgs e)
        {
            Debug.Print("CableConnectivityChanged " + (e.IsConnected ? "Connected!" : "Disconnected!"));

            if (ethernet.IsActivated) // make sure that the event is fired by ethernet interface, not other networking interface.
            {
                if (e.IsConnected)
                {
                    //Debug.Print("Ethernet connection was established! IPAddress = " + wifiModule.NetworkSettings.IPAddress);

                    blocker.Set();
                    //portNetworkLED.DutyCycle = 0;

                    if (Started != null)
                        Started(this, EventArgs.Empty);
                }
                else
                {
                    Debug.Print("Ethernet connection was dropped or disconnected!");

                    blocker.Set();
                    //portNetworkLED.DutyCycle = 1;
                    //portNetworkLED.Stop();

                    if (Stopped != null)
                        Stopped(this, EventArgs.Empty);

                    Start();
                }
            }


            //if (e.IsConnected)
            //    blocker.Set();
        }

        private void ethernet_NetworkAddressChanged(object sender, EventArgs e)
        {
            Debug.Print("New address for The built-in Ethernet Network Interface ");

            Debug.Print("Is DhCp enabled: " + ethernet.NetworkInterface.IsDhcpEnabled);
            Debug.Print("Is DynamicDnsEnabled enabled: " + ethernet.NetworkInterface.IsDynamicDnsEnabled);
            Debug.Print("NetworkInterfaceType " + ethernet.NetworkInterface.NetworkInterfaceType);
            Debug.Print("Network settings:");
            Debug.Print("IP Address: " + ethernet.NetworkInterface.IPAddress);
            Debug.Print("Subnet Mask: " + ethernet.NetworkInterface.SubnetMask);
            Debug.Print("Default Getway: " + ethernet.NetworkInterface.GatewayAddress);
            Debug.Print("Number of DNS servers:" + ethernet.NetworkInterface.DnsAddresses.Length);

            for (int i = 0; i < ethernet.NetworkInterface.DnsAddresses.Length; i++)
                Debug.Print("DNS Server " + i.ToString() + ":" + ethernet.NetworkInterface.DnsAddresses[i]);

            Debug.Print("------------------------------------------------------");
        }
        //private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        //{
        //    if (e.IsAvailable)
        //    {
        //        if (Ethernet.IsCableConnected)
        //            blocker.Set();
        //    }
        //    else
        //    {
        //        blocker.Set();
        //        if (Stopped != null)
        //            Stopped(this, EventArgs.Empty);

        //        Start();
        //    }
        //}
        #endregion
    }
}
