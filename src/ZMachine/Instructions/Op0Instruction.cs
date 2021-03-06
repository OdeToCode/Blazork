﻿using System;

namespace Blazork.ZMachine.Instructions
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

        public override void Prepare(SpanLocation memory)
        {
            Size = 1;
            OpCode = Bits.BottomFour(memory.Bytes[0]);
            Operation = OpCode switch
            {
                0x00 => new Operation(nameof(RetTrue), RetTrue),
                0x01 => new Operation(nameof(RetFalse), RetFalse),
                0x02 => new Operation(nameof(Print), Print, hasText: true),
                0x08 => new Operation(nameof(RetPopped), RetPopped),
                0x0B => new Operation(nameof(NewLine), NewLine),
                _ => throw new InvalidOperationException($"Unknown OP0 opcode {OpCode:X}")
            };

            if(Operation.HasText)
            {
                var decoded = textResolver.Decode(memory.Forward(1));
                Size += decoded.BytesConsumed;
                Text = decoded.Text;
            }
            if (Operation.HasStore)
            {
                StoreResult = memory.Bytes[Size];
                Size += 1;
            }
            if (Operation.HasBranch)
            {
                var branchData = machine.Memory.SpanAt(memory.Address + Size);
                Branch = branchResolver.ResolveBranch(branchData);
                Size += Branch.Size;
            }

            DumpToLog(memory, Size);
        }

        public void RetPopped(SpanLocation location)
        {
            var returnValue = machine.StackFrames.RoutineStack.Pop();
            var frame = machine.StackFrames.PopFrame();

            log.Debug($"RetPopped {returnValue} to {frame.ReturnPC:X}");

            machine.SetVariable(frame.StoreVariable, returnValue);
            machine.SetPC(frame.ReturnPC);
        }

        public void RetFalse(SpanLocation location)
        {
            var returnValue = 0;
            var frame = machine.StackFrames.PopFrame();

            log.Debug($"RetFalse to {frame.ReturnPC:X}");

            machine.SetVariable(frame.StoreVariable, returnValue);
            machine.SetPC(frame.ReturnPC);
        }

        public void RetTrue(SpanLocation memory)
        {
            var returnValue = 1;
            var frame = machine.StackFrames.PopFrame();

            log.Debug($"RetTrue to {frame.ReturnPC:X}");

            machine.SetVariable(frame.StoreVariable, returnValue);
            machine.SetPC(frame.ReturnPC);
        }

        public void NewLine(SpanLocation memory)
        {
            log.Debug("NewLine");

            machine.Output.Write(Environment.NewLine);
            machine.SetPC(memory.Address + Size);
        }

        public void Print(SpanLocation memory)
        {
            log.Debug($"Print {Text}");
            machine.Output.Write(Text);
            machine.SetPC(memory.Address + Size);
        }
    }
}
