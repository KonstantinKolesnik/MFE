using System;
using Microsoft.SPOT;
//using System.Net;
using System.Text;
using System.Threading;
using GHI.Premium.Net;
using Microsoft.SPOT.Hardware;

namespace MFE.Net.Managers
{
    public class EthernetManager : INetworkManager
    {
        #region Fields
        // if you were to use ENC28J60-based Ethernet connection
        //static EthernetENC28J60 Eth1 = new EthernetENC28J60(add code here to configure it);
        // wifi is same thing
        // static WiFiRS9110 wifi = new WiFiRS9110(......);
        static EthernetBuiltIn ethernet;// = new EthernetBuiltIn();
        private ManualResetEvent blocker = null;
        //private PWM portNetworkLED = null;
        #endregion

        #region Events
        public event EventHandler Started;
        public event EventHandler Stopped;
        #endregion

        #region Constructor
        public EthernetManager()//Cpu.PWMChannel pinNetworkStatusLED)
        {
            blocker = new ManualResetEvent(false);
            //portNetworkLED = new PWM(pinNetworkStatusLED, 1, 50, false); // blink LED with 1 Hz

            ethernet = new EthernetBuiltIn();

            ethernet.CableConnectivityChanged += new EthernetBuiltIn.CableConnectivityChangedEventHandler(Eth1_CableConnectivityChanged);
            ethernet.NetworkAddressChanged += new NetworkInterfaceExtension.NetworkAddressChangedEventHandler(Eth1_NetworkAddressChanged);
            //NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
        }
        #endregion

        #region Public methods
        public void Start()
        {
            if (!ethernet.IsOpen)
                ethernet.Open();

            if (!ethernet.NetworkInterface.IsDhcpEnabled)
                ethernet.NetworkInterface.EnableDhcp();
            //ethernet.NetworkInterface.EnableStaticIP("192.168.0.110", "255.255.255.0", "192.168.0.1");

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

            // wait for ip-address set:
            //while (IPAddress.GetDefaultLocalAddress() == IPAddress.Any)
            //{
            //    Debug.Print("IP address is not set yet.");
            //}

            Debug.Print("IP address is set");

            if (Started != null)
                Started(this, EventArgs.Empty);













            //portNetworkLED.Start();

            //if (!Ethernet.IsEnabled)
            //    Ethernet.Enable();

            //if (!Ethernet.IsCableConnected)
            //{
            //    blocker.Reset();
            //    while (!blocker.WaitOne(500, false))
            //    {
            //        if (Ethernet.IsCableConnected)
            //            break;
            //    }
            //}

            //NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
            //foreach (NetworkInterface ni in nis)
            //{
            //    if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            //    {
            //        #region DHCP Code (dynamic IP)
            //        if (!ni.IsDhcpEnabled)
            //            ni.EnableDhcp();
            //        else
            //            ni.RenewDhcpLease();
            //        #endregion

            //        #region Static IP code
            //        // Uncomment the following line if you want to use a static IP address, and comment out the DHCP code region above
            //        //string ip = "192.168.0.150";
            //        //string subnet = "255.255.255.0";
            //        //string gateway = "192.168.0.1";
            //        ////byte[] mac = ni.PhysicalAddress;// { 0x00, 0x26, 0x1C, 0x7B, 0x29, 0xE8 };

            //        //ni.EnableStaticIP(ip, subnet, gateway);
            //        //ni.EnableStaticDns(gateway);
            //        #endregion

            //        portNetworkLED.Frequency = 1000;//.Set(true);
            //        if (Started != null)
            //            Started(this, EventArgs.Empty);

            //        return;
            //    }
            //}

            //portNetworkLED.Frequency = 0;//.Set(false);
        }
        #endregion

        #region Event handlers
        void Eth1_CableConnectivityChanged(object sender, EthernetBuiltIn.CableConnectivityEventArgs e)
        {
            Debug.Print("Built-in Ethernet Cable is " + (e.IsConnected ? "Connected!" : "Disconnected!"));

            if (e.IsConnected)
                blocker.Set();
        }

        static void Eth1_NetworkAddressChanged(object sender, EventArgs e)
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
