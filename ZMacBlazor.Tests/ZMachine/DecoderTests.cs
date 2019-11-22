using System.IO;
using Xunit;
using ZMacBlazor.Client.ZMachine;
using ZMacBlazor.Client.ZMachine.Instructions;

namespace ZMacBlazor.Tests.ZMachine
{
    public class DecoderTests
    {
        [Fact]
        public void DecodesOpeningZorkInstruction()
        {
            var file = File.OpenRead(@"Data\ZORK1.DAT");
            var memory = new MachineMemory();
            memory.Load(file);

            var decoder = new Decoder();
            var instruction = decoder.Decode(memory.At(memory.StartingProgramCounter));
            
            Assert.Equal(InstructionForm.VarForm, instruction.Form);
            Assert.Equal(OperandCount.Two, instruction.OpCount);
            Assert.Equal(0, instruction.OpCode);
        }
    }
}
