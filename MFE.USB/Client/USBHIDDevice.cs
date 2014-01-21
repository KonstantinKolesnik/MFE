using GHI.Premium.USBClient;
using System;
using MS = Microsoft.SPOT.Hardware.UsbClient;

namespace MFE.USB.Client
{
    public class USBHIDDevice
    {
        const int USBSTREAM_WRITE_ENDPOINT = 1;
        const int USBSTREAM_READ_ENDPOINT = 2;
        const int MAXPOWER_MILLIAMPS = 280; // default; can be up to 500mA

        const int HID_DESCRIPTOR_TYPE = 0x21;

        USBC_Device device;

        public USBHIDDevice()
        {
            //if (Configuration.DebugInterface.GetCurrent() == Configuration.DebugInterface.Port.USB1)
            //{
            //    Configuration.DebugInterface.Set(Configuration.DebugInterface.Port.Sockets1, Configuration.DebugInterface.Port.USB1);
            //    throw new InvalidOperationException("Current debug interface is USB. Please restart your board.");
            //}

            ushort myVID = 0x1234;
            ushort myPID = 0x0007;
            ushort myDeviceVersion = 0x100;
            ushort myDeviceMaxPower = 250; // in milli amps
            string companyName = "Konstantin Kolesnik";
            string productName = "Aqua Expert";
            string myDeviceSerialNumber = "0";

            // Create the device. Assume it just has one read and one write endpoints.
            device = new USBC_Device(myVID, myPID, myDeviceVersion, myDeviceMaxPower, companyName, productName, myDeviceSerialNumber);

            // Endpoints
            byte writeEPNumber = device.ReserveNewEndpoint();
            byte readEPNumber = device.ReserveNewEndpoint();
            MS.Configuration.Endpoint[] epDesc = {
                new MS.Configuration.Endpoint(writeEPNumber, MS.Configuration.Endpoint.ATTRIB_Write | MS.Configuration.Endpoint.ATTRIB_Interrupt),
                new MS.Configuration.Endpoint(readEPNumber, MS.Configuration.Endpoint.ATTRIB_Read | MS.Configuration.Endpoint.ATTRIB_Interrupt),
            };
            epDesc[0].wMaxPacketSize = 16;
            epDesc[0].bInterval = 10;
            epDesc[1].wMaxPacketSize = 16;
            epDesc[1].bInterval = 10;

            // HID report descriptor
            byte[] hidGenericReportDescriptorPayload = new byte[]
            {
                0x05, 0x0d,                         // USAGE_PAGE (Digitizers)          0
                0x09, 0x02,                         // USAGE (Pen)                      2
                0xa1, 0x01,                         // COLLECTION (Application)         4
                0x85, 0x01,                         //   REPORT_ID (Pen)                6
                0x09, 0x20,                         //   USAGE (Stylus)                 8
                0xa1, 0x00,                         //   COLLECTION (Physical)          10
                0x09, 0x42,                         //     USAGE (Tip Switch)           12
                0x09, 0x44,                         //     USAGE (Barrel Switch)        14
                0x09, 0x45,                         //     USAGE (Eraser Switch)        16
                0x09, 0x3c,                         //     USAGE (Invert)               18
                0x09, 0x32,                         //     USAGE (In Range)             20
                0x15, 0x00,                         //     LOGICAL_MINIMUM (0)          22
                0x25, 0x01,                         //     LOGICAL_MAXIMUM (1)          24
                0x75, 0x01,                         //     REPORT_SIZE (1)              26
                0x95, 0x05,                         //     REPORT_COUNT (5)             28
                0x81, 0x02,                         //     INPUT (Data,Var,Abs)         30
                0x95, 0x0b,                         //     REPORT_COUNT (11)            32
                0x81, 0x03,                         //     INPUT (Cnst,Var,Abs)         34
                0x05, 0x01,                         //     USAGE_PAGE (Generic Desktop) 36
                0x26, 0xff, 0x7f,                   //     LOGICAL_MAXIMUM (32767)      38
                0x75, 0x10,                         //     REPORT_SIZE (16)             41
                0x95, 0x01,                         //     REPORT_COUNT (1)             43
                0xa4,                               //     PUSH                         45
                0x55, 0x0d,                         //     UNIT_EXPONENT (-3)           46
                0x65, 0x33,                         //     UNIT (Inch,EngLinear)        48
                0x09, 0x30,                         //     USAGE (X)                    50
                0x35, 0x00,                         //     PHYSICAL_MINIMUM (0)         52
                0x46, 0x00, 0x00,                   //     PHYSICAL_MAXIMUM (0)         54
                0x81, 0x02,                         //     INPUT (Data,Var,Abs)         57
                0x09, 0x31,                         //     USAGE (Y)                    59
                0x46, 0x00, 0x00,                   //     PHYSICAL_MAXIMUM (0)         61
                0x81, 0x02,                         //     INPUT (Data,Var,Abs)         64
                0xb4,                               //     POP                          66
                0x05, 0x0d,                         //     USAGE_PAGE (Digitizers)      67
                0x09, 0x30,                         //     USAGE (Tip Pressure)         69
                0x81, 0x02,                         //     INPUT (Data,Var,Abs)         71
                0x09, 0x3d,                         //     USAGE (X Tilt)               73
                0x09, 0x3e,                         //     USAGE (Y Tilt)               75
                0x16, 0x01, 0x80,                   //     LOGICAL_MINIMUM (-32767)     77
                0x95, 0x02,                         //     REPORT_COUNT (2)             80
                0x81, 0x02,                         //     INPUT (Data,Var,Abs)         82/84
                0xc0,                               //   END_COLLECTION                 0/1
                0xc0,                               // END_COLLECTION                   0/1
            };

            // Class Descriptor
            byte[] hidClassDescriptorPayload = new byte[] 
            {
                0x01, 0x01,     // bcdHID (v1.01)
                0x00,           // bCountryCode
                0x01,           // bNumDescriptors
                0x22,           // bDescriptorType (report)
                (byte)hidGenericReportDescriptorPayload.Length, 0x00      // wDescriptorLength (report descriptor size in bytes)
            };

            // HID class descriptor
            MS.Configuration.ClassDescriptor hidClassDescriptor = new MS.Configuration.ClassDescriptor(HID_DESCRIPTOR_TYPE, hidClassDescriptorPayload);
            // Interface
            MS.Configuration.UsbInterface usbInterface = new MS.Configuration.UsbInterface(0, epDesc);
            usbInterface.classDescriptors = new MS.Configuration.ClassDescriptor[] { hidClassDescriptor };
            usbInterface.bInterfaceClass = 0x03; // HID
            usbInterface.bInterfaceSubClass = 0x00;
            usbInterface.bInterfaceProtocol = 0x00;
            // Attach interface
            byte interfaceIndex = device.AddInterface(usbInterface, "My interface name");

            MS.Configuration.GenericDescriptor hidGenericReportDescriptor = new MS.Configuration.GenericDescriptor(0x81, 0x2200, hidGenericReportDescriptorPayload);
            hidGenericReportDescriptor.bRequest = 0x06; // GET_DESCRIPTOR
            hidGenericReportDescriptor.wIndex = 0x00; // INTERFACE 0 (zero)
            // attach descriptor
            device.AddDescriptor(hidGenericReportDescriptor);

            // Strings
            MS.Configuration.StringDescriptor stringDescriptor1 = new MS.Configuration.StringDescriptor(1, "String 1");
            MS.Configuration.StringDescriptor stringDescriptor2 = new MS.Configuration.StringDescriptor(2, "String 2");
            device.AddDescriptor(stringDescriptor1);
            device.AddDescriptor(stringDescriptor2);

            // This is used for reading and writing
            USBC_Stream stream = device.CreateUSBStream(writeEPNumber, readEPNumber);

            // All done, you can start the device now
            try
            {
                USBClientController.Start(device);
            }
            catch (Exception) {}

            byte[] report = new byte[13];

            int x = 0;
            int y = 0;

            // Check if connected to PC
            //while (USBClientController.GetState() != USBClientController.State.Running)
            //{
            //    Debug.Print("Waiting to connect to PC...");
            //    Thread.Sleep(1000);
            //}

            //while (true)
            //{
            //    Thread.Sleep(20);

            //    if (!buttonUp.Read())
            //        x += 32;

            //    if (!buttonDown.Read())
            //        y += 32;

            //    report[0] = 0x01;

            //    bool pressed = !buttonSelect.Read();

            //    report[1] = (byte)(pressed ? 0x31 : 0x30);
            //    report[3] = (byte)x;
            //    report[4] = (byte)((x >> 8) & 0x7f);
            //    report[5] = (byte)y;
            //    report[6] = (byte)((y >> 8) & 0x7f);
            //    report[7] = (byte)(pressed ? 0xFF : 0x00);
            //    report[8] = 0x0;

            //    stream.Write(report, 0, report.Length);
            //}
        }



    }
}
