using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using ZMacBlazor.Client.ZMachine;
using ZMacBlazor.Client.ZMachine.Instructions;
using ZMacBlazor.Tests.Logging;

namespace ZMacBlazor.Tests.ZMachine
{
    public class MethodDescriptorTests
    {
        private LogAdapter logger;

        public MethodDescriptorTests(ITestOutputHelper testOutput)
        {
            logger = new LogAdapter(testOutput);
        }

        [Fact]
        public void Decodes_Method_At_5472()
        {
            using var file = File.OpenRead(@"Data\ZORK1.DAT");
            var machine = new Machine(logger);
            machine.Load(file);

            var memory = machine.Memory.LocationAt(0x5472);
            var descriptor = new MethodDescriptor(memory, machine);

            Assert.Equal(3, descriptor.LocalsCount);
            Assert.Equal(0, descriptor.InitialValues.First());
            Assert.Equal(0, descriptor.InitialValues.Last());
            Assert.Equal(0x5479, descriptor.StartAddress);
        }
    }
}
