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
                0x0F => new Operation(nameof(LoadW), LoadW),
                0x14 => new Operation(nameof(Add), Add),
                _ => throw new InvalidOperationException($"Unknown OP2 opcode {OpCode:X}")
            };

            Operation.Method(memory);
        }

        public void LoadW(MemoryLocation location)
        {
            var baseArray = Operands[0].Value;
            var index = Operands[1].Value;
            var arrayLocation = baseArray + 2 * index;
            var word = Machine.Memory.WordAt(arrayLocation);

            Store = location.Bytes[Operands.Size + 1];
            Machine.SetWordVariable(Store, word);

            DumpToLog(location);
            Machine.SetPC(location.Address + Operands.Size + 2);
        }

        public void Add(MemoryLocation memory)
        {
            var a = Operands[0].Value;
            var b = Operands[1].Value;
            var result = a + b;

            Store = memory.Bytes[Operands.Size + 1];
            Machine.SetWordVariable(Store, result);

            DumpToLog(memory);
            Machine.SetPC(memory.Address + Operands.Size + 2);
        }
        
        public void JE(MemoryLocation location)
        {
            var a = Operands[0].Value;
            var b = Operands[1].Value;
            var result = a == b;

            var branchData = Machine.Memory.LocationAt(location.Address + Operands.Size + 1);
            Branch = branchResolver.ResolveBranch(branchData);

            var size = 1 + Operands.Size + Branch.Size;
            if (Branch.Offset == 0 && Branch.BranchOnTrue == result)
            {
                throw new InvalidOperationException("Means to return false from current routine");
            }
            else if(Branch.Offset == 1 && Branch.BranchOnTrue == result)
            {
                throw new InvalidOperationException("measure to return true from current routine");
            }
            else if(Branch.BranchOnTrue == result)
            {    
                var newPC = location.Address + size + Branch.Offset - 2;
                Machine.SetPC(newPC);
            }
            else
            {
                Machine.SetPC(location.Address + size);
            }
            DumpToLog(location);
        }
    }
}
