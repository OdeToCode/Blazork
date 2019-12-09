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

            Size = 1 + Operands.Size;
            OpCode = Bits.BottomFive(memory.Bytes[0]);
            Operation = OpCode switch
            {
                0x01 => new Operation(nameof(JE), JE, hasBranch: true),
                0x0F => new Operation(nameof(LoadW), LoadW, hasStore: true),
                0x14 => new Operation(nameof(Add), Add, hasStore: true),
                _ => throw new InvalidOperationException($"Unknown OP2 opcode {OpCode:X}")
            };
            if (Operation.HasBranch)
            {
                var branchData = Machine.Memory.LocationAt(memory.Address + Size);
                Branch = branchResolver.ResolveBranch(branchData);
                Size += Branch.Size;
            }
            if (Operation.HasStore)
            {
                Store = memory.Bytes[Size];
                Size += 1;
            }

            DumpToLog(memory);
            Operation.Execute(memory);
        }

        public void LoadW(MemoryLocation location)
        {
            var baseArray = Operands[0].Value;
            var index = Operands[1].Value;
            var arrayLocation = baseArray + 2 * index;
            var word = Machine.Memory.WordAt(arrayLocation);

            Machine.SetWordVariable(Store, word);
            Machine.SetPC(location.Address + Size);
        }

        public void Add(MemoryLocation memory)
        {
            var a = Operands[0].Value;
            var b = Operands[1].Value;
            var result = a + b;

            Machine.SetWordVariable(Store, result);
            Machine.SetPC(memory.Address + Size);
        }
        
        public void JE(MemoryLocation location)
        {
            var a = Operands[0].Value;
            var b = Operands[1].Value;
            var result = a == b;

            Branch.Go(result, Machine, Size, location);
        }
    }
}
