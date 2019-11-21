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
            var file = File.OpenRead(@"Data\ZORK1.DAT");
            var memory = new MachineMemory();
            memory.Load(file);

            Assert.Equal(3, memory.Version);
            Assert.Equal(20023, memory.HighMemory);
            Assert.Equal(20229, memory.StartingProgramCounter);
            Assert.Equal(688, memory.ObjectTable);
            Assert.Equal(15137, memory.Dictionary);
        }
    }
}
