using System.IO;
using Xunit;
using ZMacBlazor.Client.ZMachine;
using ZMacBlazor.Tests.Logging;

namespace ZMacBlazor.Tests.ZMachine
{
    public class MachineMemoryTests
    {
        [Fact]
        public void SetsGlobalWord()
        {
            using var file = File.OpenRead(@"Data\ZORK1.DAT");
            using var logger = new NullLogger();
            var machine = new Machine(logger);
            machine.Load(file);

            machine.SetWordVariable(20, 0xBEEF);

            Assert.Equal(0xBEEF, machine.ReadVariable(20));
        }

        [Fact]
        public void ReadsHeader()
        {
            using var file = File.OpenRead(@"Data\ZORK1.DAT");
            using var logger = new NullLogger();
            var machine = new Machine(logger);
            machine.Load(file);

            var memory = machine.Memory;

            Assert.Equal(3, memory.Version);
            Assert.Equal(20023, memory.HighMemory);
            Assert.Equal(0x4F05, memory.StartingProgramCounter);
            Assert.Equal(688, memory.ObjectTable);
            Assert.Equal(15137, memory.Dictionary);
        }
    }
}
