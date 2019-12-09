using System.IO;
using Xunit;
using ZMacBlazor.Client.ZMachine;
using ZMacBlazor.Client.ZMachine.Instructions;
using ZMacBlazor.Tests.Logging;

namespace ZMacBlazor.Tests.ZMachine
{
    public class DecoderTests
    {
        [Fact]
        public void RunZork()
        {
            using var file = File.OpenRead(@"Data\ZORK1.DAT");
            using var logger = new TestLogger($"{nameof(DecoderTests)}-{nameof(RunZork)}.log");
            var machine = new Machine(logger);
            machine.Load(file);
            machine.Execute();
        }

        [Fact]
        public void DecodesOpeningZorkInstruction()
        {
            using var file = File.OpenRead(@"Data\ZORK1.DAT");
            using var logger = new NullLogger();
            var machine = new Machine(logger);
            machine.Load(file);
            
            var decoder = new InstructionDecoder(machine);
            var memory = machine.Memory.LocationAt(machine.PC);
            var instruction = decoder.Decode(memory);
            instruction.Execute(memory);

            Assert.NotNull(instruction as VarInstruction);
            Assert.Equal("Call", instruction.Operation.Name);
            Assert.Equal(0, instruction.OpCode);
            Assert.Equal(0, instruction.Store);
            Assert.Equal(3, instruction.Operands.Count);
            Assert.Equal(0x5479, machine.PC);

            memory = machine.Memory.LocationAt(machine.PC);
            instruction = decoder.Decode(memory);
            instruction.Execute(memory);

            Assert.NotNull(instruction as VarInstruction);
            Assert.Equal("Call", instruction.Operation.Name);
            Assert.Equal(2, instruction.Operands.Count);
            Assert.Equal(3, instruction.Store);
            Assert.Equal(OperandType.Variable, instruction.Operands[1].Type);
            Assert.Equal(1, instruction.Operands[1].RawValue);

            memory = machine.Memory.LocationAt(machine.PC);
            instruction = decoder.Decode(memory);
            instruction.Execute(memory);

            Assert.NotNull(instruction as Op2Instruction);
            Assert.Equal("Add", instruction.Operation.Name);
            Assert.Equal(4, instruction.Size);
            Assert.Equal(2, instruction.Operands.Count);
            Assert.Equal(OperandType.Variable, instruction.Operands[0].Type);
            Assert.Equal(0x94, instruction.Operands[0].RawValue);
            Assert.Equal(OperandType.Small, instruction.Operands[1].Type);
            Assert.Equal(0xB4, instruction.Operands[1].RawValue);
            Assert.Equal(0x03, instruction.Store);
        }
    }
}
