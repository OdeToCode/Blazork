using Microsoft.Extensions.Logging;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using ZMacBlazor.Client.ZMachine;
using ZMacBlazor.Client.ZMachine.Instructions;
using ZMacBlazor.Tests.Logging;

namespace ZMacBlazor.Tests.ZMachine
{
    public class DecoderTests
    {
        private readonly ILogger testLogger;

        public DecoderTests(ITestOutputHelper testOutput)
        {
            testLogger = new LoggerAdapter(testOutput);
        }

        [Fact]
        public void DecodesOpeningZorkInstruction()
        {
            using var file = File.OpenRead(@"Data\ZORK1.DAT");
            var machine = new Machine();
            machine.Load(file);
            
            var decoder = new InstructionDecoder(machine);
            var memory = machine.Memory.LocationAt(machine.PC);
            var instruction = decoder.Decode(memory) as VarInstruction;
            instruction.Execute(memory);

            Assert.NotNull(instruction);
            Assert.Equal(0, instruction.OpCode);
            Assert.Equal(3, instruction.Operands.Count);            
            Assert.Equal(0x5479, machine.PC);
        }
    }
}
