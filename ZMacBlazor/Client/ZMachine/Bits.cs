using System;

namespace ZMacBlazor.Client.ZMachine
{
    public static class Bits
    {
        public static (byte, byte) BreakWord(int value)
        {
            var b1 = (byte)((value & 0b11111111_00000000) >> 8);
            var b2 = (byte)(value & 0b00000000_11111111);

            return (b1, b2);
        }

        public static byte FourAndFive(byte value)
        {
            var mask = (byte)0b0011_0000;
            return (byte)((value & mask) >> 4);
        }

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

        public static byte BottomSix(byte value)
        {
            return (byte)(value & 0b0011_1111);
        }

        public static int MakeWordFromBottomFourteen(ReadOnlySpan<byte> bytes)
        {
            var msb = Bits.BottomSix(bytes[0]);
            var lsb = bytes[1];

            return ((msb << 8) | (lsb));
        }

        public static int MakeWord(ReadOnlySpan<byte> bytes)
        {
            var msb = bytes[0];
            var lsb = bytes[1];

            return ((msb << 8) | lsb); 
        }

        public static int MakeSignedWord(ReadOnlySpan<byte> bytes)
        {
            var msb = bytes[0];
            var lsb = bytes[1];
            var value = (short)((msb << 8) | lsb);

            return value;
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

        public static bool SixSet(byte value)
        {
            var mask = (byte)0b0100_0000;
            return (value & mask) == mask;
        }
    }
}
