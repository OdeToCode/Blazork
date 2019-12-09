using System;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class Op2Instruction : Instruction
    {
        private readonly Op2OperandResolver operandResolver;

        public Op2Instruction(Machine machine) : base(machine)
        {
            operandResolver = new Op2OperandResolver();
        }

        public override void Execute(MemoryLocation memory)
        {
            operandResolver.AddOperands(Operands, memory.Bytes);

            OpCode = Bits.BottomFive(memory.Bytes[0]);
            Operation = OpCode switch
            {
                0x14 => new Operation(nameof(Add), Add),
                _ => throw new InvalidOperationException($"Unknown OP2 opcode {OpCode:X}")
            };

            Operation.Method(memory);
        }

        public void Add(MemoryLocation memory)
        {
            var a = Operands[0].Value;
            var b = Operands[1].Value;
            var result = a + b;

            Store = memory.Bytes[3];
            Machine.SetWordVariable(Store, result);
            Machine.SetPC(memory.Address + 4);
        }
    }
}
