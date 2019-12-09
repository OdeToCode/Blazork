using System;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class Op2Instruction : Instruction
    {
        private readonly Op2OperandResolver operandResolver;
        private readonly BranchResolver branchResolver;

        public Op2Instruction(Machine machine) : base(machine)
        {
            operandResolver = new Op2OperandResolver();
            branchResolver = new BranchResolver();
        }

        public override void Execute(MemoryLocation memory)
        {
            operandResolver.AddOperands(Operands, memory.Bytes);

            OpCode = Bits.BottomFive(memory.Bytes[0]);
            Operation = OpCode switch
            {
                0x01 => new Operation(nameof(JE), JE),
                0x14 => new Operation(nameof(Add), Add),
                _ => throw new InvalidOperationException($"Unknown OP2 opcode {OpCode:X}")
            };

            DumpToLog(memory);
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

        public void JE(MemoryLocation location)
        {
            var a = Operands[0].Value;
            var b = Operands[1].Value;
            var result = a == b;

            var branchData = Machine.Memory.LocationAt(location.Address + Operands.Size + 1);
            Branch = branchResolver.ResolveBranch(branchData);

            if(Branch.Offset == 0)
            {
                throw new InvalidOperationException("Means to return false from current routine");
            }
            else if(Branch.Offset == 1)
            {
                throw new InvalidOperationException("measure to return true from current routine");
            }
            else
            {
                var size = 1 + Operands.Size + Branch.Size;
                var newPC = location.Address + size + Branch.Offset - 2;
                Machine.SetPC(newPC);
            }
        }
    }
}
