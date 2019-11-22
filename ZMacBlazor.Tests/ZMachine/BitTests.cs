using Xunit;
using ZMacBlazor.Client.ZMachine;

namespace ZMacBlazor.Tests.ZMachine
{
    public class BitTests
    {
        [Fact]
        public void BottomFiveTests()
        {
            Assert.Equal(0x0F, Bits.BottomFive(0b0000_1111));
            Assert.Equal(0x01, Bits.BottomFive(0b0001_0001));
            Assert.Equal(0x10, Bits.BottomFive(0b0001_0000));
            Assert.Equal(0x12, Bits.BottomFive(0b0001_0010));
        }

        [Fact]
        public void BottomFourTests()
        {
            Assert.Equal(0x0F, Bits.BottomFour(0b0000_1111));
            Assert.Equal(0x01, Bits.BottomFour(0b0001_0001));
            Assert.Equal(0x00, Bits.BottomFour(0b0100_0000));
            Assert.Equal(0x02, Bits.BottomFour(0b0010_0010));
        }

        [Fact]
        public void FiveTests()
        {
            Assert.False(Bits.FiveSet(0b0000_0000));
            Assert.False(Bits.FiveSet(0b0001_0000));
            Assert.False(Bits.FiveSet(0b0100_0000));
            Assert.True(Bits.FiveSet(0b0010_0000));
        }

        [Fact]
        public void TopBitTests()
        {
            Assert.False(Bits.SevenSet(0b0000_0000));
            Assert.False(Bits.SevenSet(0b0100_0000));
            Assert.False(Bits.SevenSet(0b0000_0001));
            Assert.True(Bits.SevenSet(0b1000_0000));
        }

        [Fact]
        public void TopTwoTests()
        {
            Assert.False(Bits.SixSevenSet(0b0000_0000));
            Assert.False(Bits.SixSevenSet(0b0100_0000));
            Assert.False(Bits.SixSevenSet(0b1000_0000));
            Assert.True(Bits.SixSevenSet(0b1100_0000));
        }

        [Fact]
        public void FourFiveTests()
        {
            Assert.False(Bits.FourFiveSet(0b0000_0000));
            Assert.False(Bits.FourFiveSet(0b0010_0000));
            Assert.False(Bits.FourFiveSet(0b0001_0000));
            Assert.True(Bits.FourFiveSet(0b0011_0000));
        }
    }
}
