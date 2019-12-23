using System.IO;
using Xunit;
using ZMacBlazor.Client.ZMachine;
using ZMacBlazor.Client.ZMachine.Text;
using ZMacBlazor.Tests.Logging;

namespace ZMacBlazor.Tests.ZMachine
{
    public class ZStringDecoderTests
    {
        [Fact]
        public void CanDecodeTenBit()
        {
            using var file = File.OpenRead(@"Data\ZORK1.DAT");
            var logger = NullLoggerFactory.GetLogger();
            var machine = new Machine(logger);
            machine.Load(file);

            var decoder = new ZStringDecoder(machine);
            var result = decoder.Decode(machine.Memory.SpanAt(0x5908)).Text;

            Assert.Equal(">", result);
        }

        [Fact]
        public void CanDecodePairOfHands()
        {
            using var file = File.OpenRead(@"Data\ZORK1.DAT");
            var logger = NullLoggerFactory.GetLogger();
            var machine = new Machine(logger);
            machine.Load(file);

            var decoder = new ZStringDecoder(machine);

            Assert.Equal("pair of hands", decoder.Decode(machine.Memory.SpanAt(0xBB9)).Text);
        }
    }
}
