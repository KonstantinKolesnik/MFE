using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System;
using System.Collections;
using System.Threading;

namespace MFE.Hardware
{
    public static class I2CExtension
    {
        public static ArrayList Scan(this I2CDevice device, int startAddress, int endAddress, int clockRate = 100)
        {
            ArrayList res = new ArrayList();

            for (int address = startAddress; address <= endAddress; address++)
            {
                var ConfigSav = device.Config;
                device.Config = new I2CDevice.Configuration((ushort)address, clockRate);

                var xActions = new I2CDevice.I2CTransaction[1];
                xActions[0] = I2CDevice.CreateReadTransaction(new byte[1]);

                int result = device.Execute(xActions, 1000);
                if (result != 0)
                    res.Add(address);

                device.Config = ConfigSav;

                Thread.Sleep(5); // Mandatory after each Write transaction !!!
            }

            return res;
        }

        public static int Execute(this I2CDevice device, I2CDevice.Configuration config, I2CDevice.I2CTransaction[] transactions, int timeout)
        {
            var ConfigSav = device.Config;
            device.Config = config;
            var RetVal = device.Execute(transactions, timeout);
            device.Config = ConfigSav;
            return RetVal;
        }

        public static void Broadcast(this I2CDevice device, I2CDevice.Configuration[] config, I2CDevice.I2CTransaction[] transactions, int timeout)
        {
            var ConfigSav = device.Config;
            foreach (I2CDevice.Configuration cnf in config)
            {
                device.Config = cnf;
                device.Execute(transactions, timeout);
            }
            device.Config = ConfigSav;
        }

        public static bool TrySetRegister(this I2CDevice device, I2CDevice.Configuration config, int timeout, byte register, params byte[] value)
        {
            byte[] data = new byte[value.Length + 1];

            data[0] = register;
            for (int i = 0; i < value.Length; i++)
            {
                data[i + 1] = value[i];
            }

            int result = device.Execute(config, new I2CDevice.I2CTransaction[] { I2CDevice.CreateWriteTransaction(data) }, timeout);
            return (result == data.Length);
        }
        public static void SetRegister(this I2CDevice device, I2CDevice.Configuration config, int timeout, byte register, params byte[] value)
        {
            if (!TrySetRegister(device, config, timeout, register, value))
                throw new Exception();
        }

        public static bool TryGetRegister(this I2CDevice device, I2CDevice.Configuration config, int timeout, byte register, out byte result)
        {
            byte[] output = new byte[1];
            result = 0;

            int bytesTransfered = device.Execute(config, new I2CDevice.I2CTransaction[] { I2CDevice.CreateWriteTransaction(new byte[] { register }) }, timeout);
            bytesTransfered += device.Execute(config, new I2CDevice.I2CTransaction[] { I2CDevice.CreateReadTransaction(output) }, timeout);

            if (bytesTransfered != 2)
                return false;

            result = output[0];
            return true;
        }
        public static byte GetRegister(this I2CDevice device, I2CDevice.Configuration config, int timeout, byte register)
        {
            byte result;
            if (TryGetRegister(device, config, timeout, register, out result))
                return result;
            else
                throw new Exception();
        }

        public static bool TryGetRegisters(this I2CDevice device, I2CDevice.Configuration config, int timeout, byte register, byte[] result)
        {
            int bytesTransfered = device.Execute(config, new I2CDevice.I2CTransaction[] { I2CDevice.CreateWriteTransaction(new byte[] { register }) }, timeout);
            bytesTransfered += device.Execute(config, new I2CDevice.I2CTransaction[] { I2CDevice.CreateReadTransaction(result) }, timeout);

            if (bytesTransfered != result.Length + 1)
                return false;

            return true;
        }
        public static byte[] GetRegisters(this I2CDevice device, I2CDevice.Configuration config, int timeout, byte register, int size)
        {
            byte[] result = new byte[size];

            int bytesTransfered = device.Execute(config, new I2CDevice.I2CTransaction[] { I2CDevice.CreateWriteTransaction(new byte[] { register }) }, timeout);
            bytesTransfered += device.Execute(config, new I2CDevice.I2CTransaction[] { I2CDevice.CreateReadTransaction(result) }, timeout);

            if (bytesTransfered != result.Length + 1)
            {
                throw new Exception();
            }
            return result;
        }

        public static bool TrySetBit(this I2CDevice device, I2CDevice.Configuration config, int timeout, byte register, byte bitNum, bool value)
        {
            byte oldValue;
            if (!device.TryGetRegister(config, timeout, register, out oldValue))
                return false;

            byte newValue = (value == true ? (byte)(oldValue | (1 << bitNum)) : (byte)(oldValue & ~(1 << bitNum)));

            return device.TrySetRegister(config, timeout, register, newValue);
        }
        public static void SetBit(this I2CDevice device, I2CDevice.Configuration config, int timeout, byte register, byte bitNum, bool value)
        {
            if (!device.TrySetBit(config, timeout, register, bitNum, value))
                throw new Exception();
        }

        public static bool TrySetBits(this I2CDevice device, I2CDevice.Configuration config, int timeout, byte register, byte startBit, byte length, byte value)
        {
            byte oldValue;
            if (!device.TryGetRegister(config, timeout, register, out oldValue))
                return false;
            byte mask = (byte)(((1 << length) - 1) << (startBit - length + 1));
            value <<= (startBit - length + 1);
            value &= mask;
            oldValue &= (byte)~mask;
            oldValue |= value;
            return device.TrySetRegister(config, timeout, register, oldValue);
        }
        public static void SetBits(this I2CDevice device, I2CDevice.Configuration config, int timeout, byte register, byte bitstart, byte length, byte value)
        {
            if (!device.TrySetBits(config, timeout, register, bitstart, length, value))
                throw new Exception();
        }

        public static bool TryGetBit(this I2CDevice device, I2CDevice.Configuration config, int timeout, byte register, byte bitNum, out bool result)
        {
            byte value;
            if (!device.TryGetRegister(config, timeout, register, out value))
            {
                result = false;
                return false;
            }

            result = (value & (1 << bitNum)) > 0;

            return true;
        }
        public static bool GetBit(this I2CDevice device, I2CDevice.Configuration config, int timeout, byte register, byte bitNum)
        {
            bool result;
            if (device.TryGetBit(config, timeout, register, bitNum, out result))
                return result;
            else
                throw new Exception();
        }

        public static bool TryGetBits(this I2CDevice device, I2CDevice.Configuration config, int timeout, byte register, byte bitStart, byte length, out byte data)
        {
            byte value;
            if (!device.TryGetRegister(config, timeout, register, out value))
            {
                data = 0;
                return false;
            }
            byte mask = (byte)(((1 << length) - 1) << (bitStart - length + 1));
            value &= mask;
            value >>= (bitStart - length + 1);

            data = value;
            return true;
        }
        public static byte GetBits(this I2CDevice device, I2CDevice.Configuration config, int timeout, byte register, byte bitStart, byte length)
        {
            byte result;
            if (device.TryGetBits(config, timeout, register, bitStart, length, out result))
                return result;
            else
                throw new Exception();
        }

        public static ArrayList Scan(int startAddress, int endAddress, int clockRate = 100)
        {
            ArrayList res = new ArrayList();

            I2CDevice.Configuration device = null;
            I2CDevice bus = new I2CDevice(device);

            for (int address = startAddress; address <= endAddress; address++)
            {
                device = new I2CDevice.Configuration((ushort)address, clockRate);

                var xActions = new I2CDevice.I2CTransaction[1];
                xActions[0] = I2CDevice.CreateReadTransaction(new byte[1]);

                int result = bus.Execute(device, xActions, 1000);
                if (result != 0)
                    res.Add(address);

                Thread.Sleep(5); // Mandatory after each Write transaction !!!
            }

            return res;
        }
    }
}

//Bits in a byte are numbered as
//Register: XXXXXXXX
//          76543210

//Example 1:

//You want to set bit 5 in a register to 1 (=true)
//Register data: OOXOOOOO
//X marks the bit to set

//myDevice.SetBit(myConfig, myTimeout, myRegister, 5, true);

//Example 2

//You want to read bit 4 in a register
//Register data: OOOXOOOO
//X marks the bit to read

//bool bit = myDevice.GetBit(myConfig, myTimeout, myRegister, 4);

//Example 3

//You want to set bit 3-5 in a register to 101 (=5)
//Register data: OOXXXOOO
//X marks the bits to set

//myDevice.SetBits(myConfig, myTimeout, myRegister, 3 /*bitstart*/, 3 /*length*/, 5 /*value*/ );

//Example 4

//You want to read bit 2-5 in a register
//Register data: OOXXXXOO
//X marks the bits to read

//myDevice.GetBits(myConfig, myTimeout, myRegister, 2 /*bitstart*/, 4 /*length*/);
