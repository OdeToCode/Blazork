using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using ZMacBlazor.Client.ZMachine;

namespace ZMacBlazor.Tests.ZMachine
{
    public class BitTests
    {
        [Fact]
        public void TopTwoWorks()
        {
            Assert.Equal(0, Bits.TopTwo(0b0000_0000));
            Assert.Equal(1, Bits.TopTwo(0b0100_0000));
            Assert.Equal(2, Bits.TopTwo(0b1000_0000));
            Assert.Equal(3, Bits.TopTwo(0b1100_0000));
        }
    }
}
