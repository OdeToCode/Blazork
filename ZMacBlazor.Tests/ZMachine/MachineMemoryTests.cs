using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
            Assert.Equal(14158, memory.HighMemory);
        }
    }
}
