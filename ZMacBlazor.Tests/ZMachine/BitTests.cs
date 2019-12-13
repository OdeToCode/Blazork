using Xunit;
using ZMacBlazor.Client.ZMachine;

namespace ZMacBlazor.Tests.ZMachine
{
    public class BitTests
    {
        [Fact]
        public void MAkeSignedWord()
        {
            Assert.Equal(-1, Bits.MakeSignedWord(new byte[] { 0xFF, 0xFF }));
            Assert.Equal(-2, Bits.MakeSignedWord(new byte[] { 0xFF, 0xFE }));
            Assert.Equal(-41, Bits.MakeSignedWord(new byte[] { 0xFF, 0xD7 }));
            Assert.Equal(1, Bits.MakeSignedWord(new byte[] { 0x00, 0x01 }));
            Assert.Equal(127, Bits.MakeSignedWord(new byte[] { 0x00, 0x7F }));
            Assert.Equal(32767, Bits.MakeSignedWord(new byte[] { 0x7F, 0xFF }));
        }

        [Fact]
        public void BreakWordTests()
        {
            Assert.Equal((0xFF, 0x00), Bits.BreakWord(0xFF00));
            Assert.Equal((0x00, 0xFF), Bits.BreakWord(0x00FF));
            Assert.Equal((0x0F, 0xF0), Bits.BreakWord(0x0FF0));
            Assert.Equal((0x12, 0x34), Bits.BreakWord(0x1234));
        }

        [Fact]
        public void MakeWordTests()
        {
            Assert.Equal(65280, Bits.MakeWord(new byte[] { 0xFF, 0x00 }));
            Assert.Equal(4080, Bits.MakeWord(new byte[] { 0x0F, 0xF0 }));
            Assert.Equal(240, Bits.MakeWord(new byte[] { 0x00, 0xF0 }));
            Assert.Equal(15, Bits.MakeWord(new byte[] { 0x00, 0x0F }));
        }

        [Fact]
        public void BreakIntoTwoTests()
        {
            var result1 = Bits.BreakIntoTwos(0b1001_1111);
            Assert.Equal(2, result1[0]);
            Assert.Equal(1, result1[1]);
            Assert.Equal(3, result1[2]);
            Assert.Equal(3, result1[3]);
        }

        [Fact]
        public void BottomFiveTests()
        {
            Assert.Equal(0x0F, Bits.BottomFive(0b0000_1111));
            Assert.Equal(0x11, Bits.BottomFive(0b0001_0001));
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
            Assert.False(Bits.SevenSixSet(0b0000_0000));
            Assert.False(Bits.SevenSixSet(0b0100_0000));
            Assert.False(Bits.SevenSixSet(0b1000_0000));
            Assert.True(Bits.SevenSixSet(0b1100_0000));
        }

        [Fact]
        public void FourFiveTests()
        {
            Assert.False(Bits.FiveFourSet(0b0000_0000));
            Assert.False(Bits.FiveFourSet(0b0010_0000));
            Assert.False(Bits.FiveFourSet(0b0001_0000));
            Assert.True(Bits.FiveFourSet(0b0011_0000));
        }
    }
}
