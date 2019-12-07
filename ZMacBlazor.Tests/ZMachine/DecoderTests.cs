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
            
            var decoder = new InstructionDecoder(testLogger);
            var instruction = decoder.Decode(machine.Memory.SpanAt(machine.PC));

            Assert.Equal(VarOps.Call, instruction.Operation);
            Assert.Equal(3, instruction.Operands.Count);

            instruction.Execute(machine);

            Assert.Equal(0x5472, machine.PC);
        }
    }
}
