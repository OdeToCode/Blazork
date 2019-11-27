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
            var file = File.OpenRead(@"Data\ZORK1.DAT");
            var memory = new MachineMemory();
            memory.Load(file);

            var decoder = new Decoder(testLogger);
            var instruction = decoder.Decode(memory.At(memory.StartingProgramCounter));
            
            Assert.Equal(InstructionForm.VarForm, instruction.Form);
            Assert.Equal(OperandCount.Var, instruction.OpCount);
            Assert.Equal(0, instruction.OpCode);
        }
    }
}
