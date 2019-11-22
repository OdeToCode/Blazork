namespace ZMacBlazor.Client.ZMachine
{
    public static class Bits
    {
        public static byte BottomFour(byte value)
        {
            return (byte)(value & 0b0000_1111);
        }

        public static byte BottomFive(byte value)
        {
            return (byte)(value & 0b0001_1111);
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
