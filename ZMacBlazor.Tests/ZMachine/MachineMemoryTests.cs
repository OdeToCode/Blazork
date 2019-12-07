using System.IO;
using Xunit;
using ZMacBlazor.Client.ZMachine;

namespace ZMacBlazor.Tests.ZMachine
{
    public class MachineMemoryTests
    {
        [Fact]
        public void ReadsHeader()
        {
            using var file = File.OpenRead(@"Data\ZORK1.DAT");
            
            var machine = new Machine();
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
