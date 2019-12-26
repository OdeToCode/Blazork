using System;
using System.IO;
using System.Linq;
using Xunit;
using Blazork.ZMachine;
using Blazork.ZMachine.Instructions;
using Blazork.Tests.Logging;
using Blazork.Tests.Input;

namespace Blazork.Tests.ZMachine
{
    public class MethodDescriptorTests 
    {
        [Fact]
        public void Decodes_Method_At_5472()
        {
            using var file = File.OpenRead(@"Data\ZORK1.DAT");
            var logger = NullLoggerFactory.GetLogger();
            var machine = new Machine(logger, new SolveZorkInputStream());
            machine.Load(file);

            var memory = machine.Memory.SpanAt(0x5472);
            var descriptor = new MethodDescriptor(memory, machine);

            Assert.Equal(3, descriptor.LocalsCount);
            Assert.Equal(0, descriptor.InitialValues.First());
            Assert.Equal(0, descriptor.InitialValues.Last());
            Assert.Equal(0x5479, descriptor.StartAddress);
        }
    }
}
