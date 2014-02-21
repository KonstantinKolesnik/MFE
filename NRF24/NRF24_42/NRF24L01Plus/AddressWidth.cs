using System;

namespace Gadgeteer.Modules.KKS.NRF24L01Plus
{
    public static class AddressWidth
    {
        public const int Min = 3;
        public const int Max = 5;

        public static byte GetRegisterValue(byte[] address)
        {
            Check(address);
            return (byte)(address.Length - 2);
        }

        public static void Check(byte[] address)
        {
            Check(address.Length);
        }
        public static void Check(int addressWidth)
        {
            if (addressWidth < Min || addressWidth > Max)
                throw new ArgumentException("Address width needs to be 3-5 bytes");
        }
    }
}
