using System;
using System.Diagnostics;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class Op0Instruction : Instruction
    {
        private readonly TextResolver textResolver;
        private readonly BranchResolver branchResolver;

        public Op0Instruction(Machine machine) : base(machine)
        {
            textResolver = new TextResolver(machine);
            branchResolver = new BranchResolver();
        }

        public override void Execute(SpanLocation memory)
        {
            Size = 1;
            OpCode = Bits.BottomFour(memory.Bytes[0]);
            Operation = OpCode switch
            {
                0x00 => new Operation(nameof(RetTrue), RetTrue),
                0x02 => new Operation(nameof(Print), Print, hasText: true),
                0x0B => new Operation(nameof(NewLine), NewLine),
                _ => throw new InvalidOperationException($"Unknown OP0 opcode {OpCode:X}")
            };

            if(Operation.HasText)
            {
                var decoded = textResolver.Decode(memory.Bytes.Slice(1));
                Size += decoded.BytesConsumed;
                Text = decoded.Text;
            }
            if (Operation.HasBranch)
            {
                var branchData = machine.Memory.SpanAt(memory.Address + Size);
                Branch = branchResolver.ResolveBranch(branchData);
                Size += Branch.Size;
            }
            if (Operation.HasStore)
            {
                StoreResult = memory.Bytes[Size];
                Size += 1;
            }

            DumpToLog(memory);
            Operation.Execute(memory);
        }

        public void RetTrue(SpanLocation memory)
        {
            var returnValue = 1;
            var frame = machine.StackFrames.PopFrame();

            machine.SetVariable(frame.StoreVariable, returnValue);
            machine.SetPC(frame.ReturnPC);
        }

        public void NewLine(SpanLocation memory)
        {
            machine.Output.Write(Environment.NewLine);
            machine.SetPC(memory.Address + Size);
        }

        public void Print(SpanLocation memory)
        {
            machine.Output.Write(Text);
            machine.SetPC(memory.Address + Size);
        }
    }
}
