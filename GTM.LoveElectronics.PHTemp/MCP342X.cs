using LoveElectronics.Resources;
using Microsoft.SPOT.Hardware;
using System;
using System.Threading;

namespace Gadgeteer.Modules.LoveElectronics
{
    /// <summary>The resolution of the MCP324X. Resolution affects convertion time (more bits, slower conversion)</summary>
    public enum MCP324xResolution : byte
    {
        /// <summary>
        /// Twelve Bit resolution.
        /// </summary>
        TwelveBits,
        /// <summary>
        /// Fourteen Bit resolution.
        /// </summary>
        FourteenBits,
        /// <summary>
        /// Sixteen Bit resolution.
        /// </summary>
        SixteenBits,
        /// <summary>
        /// Eighteen Bit resolution.
        /// </summary>
        EighteenBits
    }

    public enum MCP342xChannel : byte
    {
        Channel1,
        Channel2,
        Channel3,
        Channel4
    }

    /// <summary>Use One-Shot if very low power consuption is required</summary>
    public enum MCP342xConversionMode
    {
        /// <summary>
        /// Requests a value, waits for conversion then returns value
        /// </summary
        OneShot,
        Continuous
    }

    /// <summary>Selects the gain factor for the input signal</summary>
    public enum MCP342xGain : byte
    {
        x1,
        x2,
        x4,
        x8
    }

    public enum MCP342xJumperState : byte
    {
        Low,
        Floating,
        High
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------

    // my
    public class MCP342
    {
        private const ushort ADDRESS_BASE = 0x68; // 104 decimal, 1101000
        private const int CLOCK_RATE = 400; // KHz
        private const int TIME_OUT = 1000; // 1 sec
        private const string TIME_OUT_ERROR = "Error attepting to read MCP342x data, zero bytes transferred";

        private MCP342xConversionMode conversionMode = MCP342xConversionMode.Continuous;
        private MCP324xResolution resolution = MCP324xResolution.TwelveBits;
        private MCP342xChannel channel = MCP342xChannel.Channel1;
        private MCP342xGain gain = MCP342xGain.x1;

        private I2CDevice device = null;
        private I2CDevice.I2CTransaction[] xConfigAction = null;
        private I2CDevice.I2CTransaction[] xReadAction = null;
        private byte[] configReg = new byte[1];
        private byte[] dataReg = new byte[5];

        // params:
        private double[] lsbValues = new double[4] { 0.001, 0.00025, 0.0000625, 0.000015625 };
        private double lsbVolts = 0.001;
        private int countsMask;
        private int maxValue;
        private double gainDivisor;

        public MCP342xConversionMode ConversionMode
        {
            get { return conversionMode; }
            set
            {
                if (conversionMode != value)
                {
                    conversionMode = value;
                    WriteConfiguration();
                }
            }
        }
        public MCP324xResolution Resolution
        {
            get { return resolution; }
            set
            {
                if (resolution != value)
                {
                    resolution = value;
                    WriteConfiguration();
                }
            }
        }
        public MCP342xChannel Channel
        {
            get { return channel; }
            set
            {
                if (channel != value)
                {
                    channel = value;
                    WriteConfiguration();
                }
            }
        }
        public MCP342xGain Gain
        {
            get { return gain; }
            set
            {
                if (gain != value)
                {
                    gain = value;
                    WriteConfiguration();
                }
            }
        }

        public MCP342() : this(MCP342xJumperState.Floating, MCP342xJumperState.Floating)
        {
        }
        public MCP342(MCP342xJumperState j1, MCP342xJumperState j2)
        {
            device = new I2CDevice(new I2CDevice.Configuration(CalculateAddress(j1, j2), CLOCK_RATE));
            xConfigAction = new I2CDevice.I2CTransaction[1] { I2CDevice.CreateWriteTransaction(configReg) };
            xReadAction = new I2CDevice.I2CTransaction[1] { I2CDevice.CreateReadTransaction(dataReg) };
            
            CalculateParams();
        }

        public byte ReadConfiguration()
        {
            if (device.Execute(xReadAction, TIME_OUT) == 0)
                throw new Exception(TIME_OUT_ERROR);

            return dataReg[Resolution == MCP324xResolution.EighteenBits ? 3 : 2];
        }
        public void WriteConfiguration()
        {
            configReg[0] = (byte)((ushort)Gain +
                                 ((ushort)Resolution << 2) +
                                 ((ushort)ConversionMode << 4) +
                                 ((ushort)Channel << 5));

            // request for conversion if One-Shot Mode;
            // no effect for Continious mode
            configReg[0] |= 0x80; // set bit 7

            if (device.Execute(xConfigAction, TIME_OUT) == 0)
                throw new Exception(TIME_OUT_ERROR);
            else
            {
                // Block here until enough time has elapsed for the current conversion to have completed
                int[] conversionTimes = new int[4] { 5, 17, 67, 267 }; // ms
                Thread.Sleep(conversionTimes[(byte)Resolution]);
            }

            CalculateParams();
        }

        public double ReadVolts()
        {
            int data = ReadData();

            if (data > maxValue)
                data -= maxValue * 2;

            return (data * lsbVolts) / gainDivisor;
        }
        public int ReadData()
        {
            if (ConversionMode == MCP342xConversionMode.OneShot) // to set bit 7 to request conversion if One-Shot Mode
                WriteConfiguration();

            //if (device.Execute(xReadAction, TIME_OUT) == 0)
            //    throw new Exception(TIME_OUT_ERROR);

            byte config;
            do
            {
                config = ReadConfiguration();
            }
            while (config >> 7 != 0); // conversion result is ready 


            int Counts = 0;
            if (Resolution == MCP324xResolution.EighteenBits) // 3 data bytes + config byte
                Counts = (((Int32)dataReg[0]) << 16) + (((Int32)dataReg[1]) << 8) + (Int32)dataReg[2];
            else // 2 data bytes + config byte
                Counts = (((Int32)dataReg[0]) << 8) + (Int32)dataReg[1];

            return Counts &= countsMask;
        }

        public static ushort CalculateAddress(MCP342xJumperState j1, MCP342xJumperState j2)
        {
            ushort addressOffset = 0;

            if (j1 == MCP342xJumperState.Floating && j2 == MCP342xJumperState.Floating)
                addressOffset = 0;
            else if (j1 == MCP342xJumperState.Low && j2 == MCP342xJumperState.Low)
                addressOffset = 0;
            else if (j1 == MCP342xJumperState.Low && j2 == MCP342xJumperState.Floating)
                addressOffset = 1;
            else if (j1 == MCP342xJumperState.Low && j2 == MCP342xJumperState.High)
                addressOffset = 2;
            else if (j1 == MCP342xJumperState.High && j2 == MCP342xJumperState.Low)
                addressOffset = 4;
            else if (j1 == MCP342xJumperState.High && j2 == MCP342xJumperState.Floating)
                addressOffset = 5;
            else if (j1 == MCP342xJumperState.High && j2 == MCP342xJumperState.High)
                addressOffset = 6;
            else if (j1 == MCP342xJumperState.Floating && j2 == MCP342xJumperState.Low)
                addressOffset = 3;
            else if (j1 == MCP342xJumperState.Floating && j2 == MCP342xJumperState.High)
                addressOffset = 7;

            return (ushort)(addressOffset | ADDRESS_BASE); // 1101xxx, xxx = addressOffset; (104...111)
        }

        private void CalculateParams()
        {
            lsbVolts = lsbValues[(ushort)Resolution];
            countsMask = (1 << (12 + (int)Resolution * 2)) - 1;
            maxValue = 1 << (11 + (int)Resolution * 2);
            gainDivisor = Math.Pow(2.0, (double)Gain);
        }
    }
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------

    // from Love Electronics
    class MCP342X : SharedI2CDevice
    {
        private const ushort AddressBase = 0x68; // 104 decimal, 1101000
        private const int Clock = 400; // KHz

        private MCP342xConversionMode conversionMode = MCP342xConversionMode.Continuous;
        private MCP324xResolution resolution = MCP324xResolution.TwelveBits;
        private MCP342xChannel channel = MCP342xChannel.Channel1;
        private MCP342xGain gain = MCP342xGain.x1;

        private double lsbVolts = 0.001;
        private Int32 countsMask;
        private Int32 maxValue;
        private double gainDivisor;


        public MCP342xConversionMode ConversionMode
        {
            get { return conversionMode; }
            set
            {
                if (conversionMode != value)
                {
                    conversionMode = value;
                    WriteConfiguration();
                }
            }
        }
        public MCP324xResolution Resolution
        {
            get { return resolution; }
            set
            {
                if (resolution != value)
                {
                    resolution = value;
                    WriteConfiguration();
                }
            }
        }
        public MCP342xChannel Channel
        {
            get { return channel; }
            set
            {
                if (channel != value)
                {
                    channel = value;
                    WriteConfiguration();
                }
            }
        }
        public MCP342xGain Gain
        {
            get { return gain; }
            set
            {
                if (gain != value)
                {
                    gain = value;
                    WriteConfiguration();
                }
            }
        }

        public MCP342X()
            : base(AddressBase, Clock)
        {
        }
        public MCP342X(ushort address, int clockRateKhz)
            : base(address, clockRateKhz)
        {
        }

        public byte ReadConfiguration()
        {
            byte conf = ReadLen(1)[0];
            conf &= 127;
            return conf;

            return ReadLen(1)[0];
        }
        public void WriteConfiguration()
        {
            byte configurationByte = 0;
            configurationByte = (byte)(configurationByte | (byte)gain);
            configurationByte = (byte)(configurationByte | (byte)((byte)resolution << 2));
            configurationByte = (byte)(configurationByte | (byte)((byte)conversionMode << 4));
            configurationByte = (byte)(configurationByte | (byte)((byte)channel << 5));

            // request for conversion if One-Shot Mode;
            // no effect for Continious mode
            configurationByte |= 0x80;
            
            Write(configurationByte);

            double[] lsbValues = new double[4] { 0.001, 0.00025, 0.0000625, 0.000015625 };
            lsbVolts = lsbValues[(ushort)resolution];

            countsMask = (1 << (12 + (int)resolution * 2)) - 1;
            maxValue = 1 << (11 + (int)resolution * 2);
            gainDivisor = Math.Pow(2.0, (double)gain);
        }

        /// <summary>
        /// Reads a channel from the device.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns>Value in mV from a 2.048V reference.</returns>
        public double ReadChannel(MCP342xChannel channel)
        {
            Channel = channel;
            return ReadValue();
        }

        /// <summary>
        /// Reads the value in mV from the current channel.
        /// </summary>
        /// <returns></returns>
        protected double ReadValue()
        {
            // request for conversion if One-Shot Mode;
            if (conversionMode == MCP342xConversionMode.OneShot)
                WriteConfiguration();

            byte[] data;
            do
            {
                //data = Read(3, 1);
                //data = Read(3, 5);
                data = ReadLen(5);
            }
            while (data[2] >> 7 != 0); // conversion result is ready 

            Int32 Counts = 0;
            if (resolution == MCP324xResolution.EighteenBits)
                Counts = (((Int32)data[0]) << 16) + (((Int32)data[1]) << 8) + (Int32)data[2];
            else
                Counts = (((Int32)data[0]) << 8) + (Int32)data[1];

            Counts &= countsMask;
            return Counts;


            //int dataValue = 0;
            //double possible = 0;
            
            //if (resolution == MCP324XResolution.TwelveBits)
            //{
            //    dataValue = (data[0] & 7) << 8 | data[1];
            //    if ((data[0] & 8) != 0)
            //        dataValue = -dataValue;
            //    possible = 2048;
            //}
            //else if (resolution == MCP324XResolution.FourteenBits)
            //{
            //    dataValue = (data[0] & 63) << 8 | data[1];
            //    if ((data[0] & 32) != 0)
            //        dataValue = -dataValue;
            //    possible = -1;
            //}
            //else if (resolution == MCP324XResolution.SixteenBits)
            //{
            //    dataValue = data[0] << 8 | data[1];
            //    if ((data[0] & 128) != 0)
            //        dataValue = -dataValue;
            //    possible = 32767.5;
            //}

            //return 2048 / possible * (double)dataValue;
        }
    }
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------

    // from https://www.ghielectronics.com/community/codeshare/entry/307
    /// <summary>
    /// A class to manage the MicroChip MCP342x delta-sigma ADC series</summary>
    /// <remarks>
    /// Can be used with MCP3422/3/4, uses I2C interface.
    /// </remarks>
    public class Mcp342x
    {
        private const ushort AddressBase = 0x68; // 104 decimal, 11010000
        private const int EmxI2cClock = 400; // KHz

        private ushort baseAddressOffset = 0;
        private MCP342xGain gain = MCP342xGain.x1;
        private MCP342xChannel channel = MCP342xChannel.Channel1;
        private MCP324xResolution resolution = MCP324xResolution.TwelveBits;
        private MCP342xConversionMode conversionMode = MCP342xConversionMode.Continuous;

        private bool configDirty = true;
        private int timeOut = 100; // ms
        private double lsbVolts = 0.001;
        private Int32 countsMask;
        private Int32 maxValue;
        private double gainDivisor;
        private I2CDevice device = null;
        private I2CDevice.I2CTransaction[] xConfigAction = null;
        private I2CDevice.I2CTransaction[] xReadAction = null;
        private byte[] configReg = new byte[1];
        private byte[] dataReg = new byte[5];

        private void ConfigDevice()
        {
            if (device == null)
            {
                device = new I2CDevice(new I2CDevice.Configuration((ushort)(AddressBase + baseAddressOffset), EmxI2cClock));

                xConfigAction = new I2CDevice.I2CTransaction[1];
                xConfigAction[0] = I2CDevice.CreateWriteTransaction(configReg);
                xReadAction = new I2CDevice.I2CTransaction[1];
                xReadAction[0] = I2CDevice.CreateReadTransaction(dataReg);
            }

            double[] lsbValues = new double[4] { 0.001, 0.00025, 0.0000625, 0.000015625 };
            lsbVolts = lsbValues[(ushort)resolution];

            countsMask = (1 << (12 + (int)resolution * 2)) - 1;
            maxValue = 1 << (11 + (int)resolution * 2);
            gainDivisor = Math.Pow(2.0, (double)gain);


            configReg[0] = (byte)((ushort)gain +
                                 ((ushort)resolution << 2) +
                                 ((ushort)conversionMode << 4) +
                                 ((ushort)channel << 5));
            WriteConfigReg();

            configDirty = false;
        }
        private void WriteConfigReg()
        {
            configReg[0] |= 0x80; // Queue a conversion (for One-Shot Mode)

            if (device.Execute(xConfigAction, timeOut) == 0)
                throw new Exception("Error attepting to configure the MCP342x device, zero bytes transferred");
            else
            {
                int[] conversionTimes = new int[4] { 5, 17, 67, 267 }; // ms
                // Block here until enough time has elapsed for the current conversion to have completed
                Thread.Sleep(conversionTimes[(byte)resolution]);
            }
        }

        /// <summary>Sets the low order 3 bits of the I2C address of the device</summary>
        /// <param name='offset'>See datasheet, only required for MCP3423/4 or MCP3422 where Address-Option is NOT A0</param>
        public void SetAddressOffset(ushort offset)
        {
            if (offset != (offset & 0x07))
                throw new Exception("Argument error calling \"Mcp342x::SetAddressOffset\", argument \"offset\" must be between 0 And 7");

            baseAddressOffset = offset;
            configDirty = true;
        }
        /// <summary>Sets the gain of the internal programmable amplifier</summary>
        /// <param name='Gain'>Gain multiplier of the pre-ADC amplifier</param>
        public void SetGain(MCP342xGain Gain)
        {
            gain = Gain;
            configDirty = true;
        }
        /// <summary>Selects the input channel to be read</summary>
        /// <param name='Ch'>Channel Number</param>
        public void SetInputChannel(MCP342xChannel Ch)
        {
            channel = Ch;
            configDirty = true;
        }
        /// <summary>Selects the operating mode that defines conversion timing</summary>
        /// <param name='cm'>Conversion Mode</param>
        public void SetConversionMode(MCP342xConversionMode cm)
        {
            conversionMode = cm;
            configDirty = true;
        }
        /// <summary>Specifies the number of bits of resolution provided - (more bits, slower conversion rate)</summary>
        /// <param name='res'>Bits of resolution required</param>
        public void SetResolution(MCP324xResolution res)
        {
            resolution = res;
            configDirty = true;
        }
        /// <summary>Specifies the timeout period while waiting for a response for all read/write operations</summary>
        /// <param name='toMilliSeconds'>TimeOut milliseconds (default = 100)</param>
        public void SetReadWriteTimeout(int toMilliSeconds)
        {
            timeOut = toMilliSeconds;
        }

        #region Constructors
        /// <summary>Parameterless Constructor - set up options later</summary>
        public Mcp342x()
        {
        }
        /// <summary>Standard Constructor</summary>
        /// <param name='Ch'>Channel Number</param>
        /// <param name='res'>Bits of resolution required</param>
        /// <param name='cm'>Conversion Mode</param>
        /// <param name='Gain'>Gain multiplier of the pre-ADC amplifier</param>
        public Mcp342x(MCP342xChannel ch, MCP324xResolution res, MCP342xConversionMode cm, MCP342xGain g)
        {
            channel = ch;
            resolution = res;
            conversionMode = cm;
            gain = g;
        }
        /// <summary>Extended Constructor</summary>
        /// <param name='Ch'>Channel Number</param>
        /// <param name='res'>Bits of resolution required</param>
        /// <param name='cm'>Conversion Mode</param>
        /// <param name='Gain'>Gain multiplier of the pre-ADC amplifier</param>
        /// <param name='Offset'>See datasheet, only required for MCP3423/4 or MCP3422 where Address-Option is NOT A0</param>
        public Mcp342x(MCP342xChannel ch, MCP324xResolution res, MCP342xConversionMode cm, MCP342xGain g, ushort addrOffset)
            : this (ch, res, cm, g)
        {
            baseAddressOffset = addrOffset;
        }
        #endregion

        /// <summary>Reads the selected input channel and converts to voltage units</summary>
        /// <returns>Voltage representated as a double-precision real</returns>
        public double ReadVolts()
        {
            Int32 Counts = ReadRawCounts();

            if (Counts > maxValue)
                Counts -= maxValue * 2;

            return (Counts * lsbVolts) / gainDivisor;
        }

        /// <summary>Reads the raw counts from the ADC</summary>
        /// <returns>A 2's complement counts value from the ADC</returns>
        public Int32 ReadRawCounts()
        {
            if (configDirty)
                ConfigDevice();
            if (conversionMode == MCP342xConversionMode.OneShot)
                WriteConfigReg();

            Int32 Counts = 0;
            if (device.Execute(xReadAction, timeOut) == 0)
                throw new Exception("Error attepting to read MCP342x data, zero bytes transferred");
            else if (resolution == MCP324xResolution.EighteenBits)
                Counts = (((Int32)dataReg[0]) << 16) + (((Int32)dataReg[1]) << 8) + (Int32)dataReg[2];
            else
                Counts = (((Int32)dataReg[0]) << 8) + (Int32)dataReg[1];

            return Counts &= countsMask;
        }
    }

    // usage:

    //// Create an Analogue Input object based on the Microchip MCP3422
    // Mcp342x adc = new Mcp342x(MCP342XChannel.Channel1, MCP324XResolution.EighteenBits, MCP342XConversionMode.Continuous, MCP342XGain.x1);
    // while (true)
    // {
    //     Debug.Print("Input voltage= " + adc.ReadVolts().ToString());
    //     System.Threading.Thread.Sleep(100);
    // }
}
