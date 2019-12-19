using System.IO;
using Xunit;
using ZMacBlazor.Client.ZMachine;
using ZMacBlazor.Tests.Logging;

namespace ZMacBlazor.Tests.ZMachine
{
    public class MachineMemoryTests
    {
        [Fact]
        public void SetsGlobalWordUnSigned()
        {
            using var file = File.OpenRead(@"Data\ZORK1.DAT");
            var logger = NullLoggerFactory.GetLogger();
            var machine = new Machine(logger);
            machine.Load(file);

            machine.SetVariable(20, 1);
            Assert.Equal(1, machine.ReadVariable(20));

            machine.SetVariable(20, 0xFFFF);
            Assert.Equal(0xFFFF, machine.ReadVariable(20));

            machine.SetVariable(20, 0xFFFF);
            Assert.Equal(-1, (short)machine.ReadVariable(20));
        }

        [Fact]
        public void ReadsHeader()
        {
            using var file = File.OpenRead(@"Data\ZORK1.DAT");
            var logger = NullLoggerFactory.GetLogger();
            var machine = new Machine(logger);
            machine.Load(file);

            var memory = machine.Memory;

            Assert.Equal(3, memory.Version);
            Assert.Equal(20023, memory.HighMemory);
            Assert.Equal(0x4F05, memory.StartingProgramCounter);
            Assert.Equal(0x02B0, memory.ObjectTable);
            Assert.Equal(15137, memory.Dictionary);
        }
    }
}
