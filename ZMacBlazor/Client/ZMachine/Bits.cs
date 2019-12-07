using System;

namespace ZMacBlazor.Client.ZMachine
{
    public static class Bits
    {
        public static byte[] BreakIntoTwos(byte value)
        {
            var result = new byte[4];

            result[0] = (byte)((value & 0b1100_0000) >> 6);
            result[1] = (byte)((value & 0b0011_0000) >> 4);
            result[2] = (byte)((value & 0b0000_1100) >> 2);
            result[3] = (byte)(value & 0b0000_0011);

            return result;
        }

        public static byte BottomFour(byte value)
        {
            return (byte)(value & 0b0000_1111);
        }

        public static byte BottomFive(byte value)
        {
            return (byte)(value & 0b0001_1111);
        }

        public static int MakeWord(ReadOnlySpan<byte> bytes)
        {
            var byte1 = bytes[0];
            var byte2 = bytes[1];

            return ((byte1 << 8) | (byte2)); 
        }

        public static bool SixSevenSet(byte value)
        {
            var mask = (byte)0b1100_0000;
            return (value & mask) == mask;
        }

        public static bool SevenSet(byte value)
        {
            var mask = (byte)0b1000_0000;
            return (value & mask) == mask;
        }

        public static bool FourFiveSet(byte value)
        {
            var mask = (byte)0b0011_0000;
            return (value & mask) == mask;
        }

        public static bool FiveSet(byte value)
        {
            var mask = (byte)0b0010_0000;
            return (value & mask) == mask;
        }
    }
}
