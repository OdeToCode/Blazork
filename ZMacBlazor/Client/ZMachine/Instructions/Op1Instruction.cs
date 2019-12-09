using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class Op1Instruction : Instruction
    {
        private readonly BranchResolver branchResolver;
        private readonly Op1OperandResolver operandResolver;

        public Op1Instruction(Machine machine) : base(machine)
        {
            operandResolver = new Op1OperandResolver();
            branchResolver = new BranchResolver();
        }

        public override void Execute(MemoryLocation memory)
        {
            operandResolver.AddOperands(Operands, memory.Bytes);

            Size = 1 + Operands.Size;
            OpCode = Bits.BottomFour(memory.Bytes[0]);
            Operation = OpCode switch
            {
                0x0C => new Operation(nameof(Jump), Jump),
                _ => throw new InvalidOperationException($"Unknown OP1 opcode {OpCode:X}")
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

        public void Jump(MemoryLocation location)
        {
            //  Jump(unconditionally) to the given label. (This is not a branch instruction and the 
            // operand is a 2 - byte signed offset to apply to the program counter.) 
            // It is legal for this to jump into a different routine(which should not change the 
            // routine call state), although it is considered bad practice to do so and the Txd 
            // disassembler is confused by it.
            var offset = Operands[0].SignedValue;
            Machine.SetPC(location.Address + Size + offset - 2);
        }
    }
}
