using System;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class Op0Instruction : Instruction
    {
        public Op0Instruction(Machine machine) : base(machine)
        {
        }

        public override void Execute(SpanLocation memory)
        {
            //operandResolver.AddOperands(Operands, memory.Bytes);

            //Size = 1 + Operands.Size;
            //OpCode = Bits.BottomFour(memory.Bytes[0]);
            Operation = OpCode switch
            {
                0x02 => new Operation(nameof(Print), Print),
                _ => throw new InvalidOperationException($"Unknown OP0 opcode {OpCode:X}")
            };
            //if (Operation.HasBranch)
            //{
            //    var branchData = machine.Memory.SpanAt(memory.Address + Size);
            //    Branch = branchResolver.ResolveBranch(branchData);
            //    Size += Branch.Size;
            //}
            //if (Operation.HasStore)
            //{
            //    StoreResult = memory.Bytes[Size];
            //    Size += 1;
            //}

            //DumpToLog(memory);
            //Operation.Execute(memory);
        }

        public void Print(SpanLocation memory)
        {

        }
    }
}
