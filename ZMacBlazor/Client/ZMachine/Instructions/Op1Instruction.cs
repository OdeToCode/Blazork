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

        public override void Execute(SpanLocation memory)
        {
            operandResolver.AddOperands(Operands, memory.Bytes);

            Size = 1 + Operands.Size;
            OpCode = Bits.BottomFour(memory.Bytes[0]);
            Operation = OpCode switch
            {
                0x00 => new Operation(nameof(JZ), JZ, hasBranch: true),
                0x01 => new Operation(nameof(GetSibling), GetSibling, hasBranch: true, hasStore: true),
                0x02 => new Operation(nameof(GetChild), GetChild, hasStore: true, hasBranch: true),
                0x03 => new Operation(nameof(GetParent), GetParent, hasStore: true),
                0x06 => new Operation(nameof(Dec), Dec),
                0x0A => new Operation(nameof(PrintObj), PrintObj),
                0x0B => new Operation(nameof(Ret), Ret),
                0x0C => new Operation(nameof(Jump), Jump),
                _ => throw new InvalidOperationException($"Unknown OP1 opcode {OpCode:X}")
            };
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
            Operation.Execute(memory);
        }

        public void Dec(SpanLocation location)
        {
            var variable = Operands[0].RawValue;
            var value = machine.ReadVariable(variable);

            value += 1;

            machine.SetVariable(variable, value);

            machine.SetPC(location.Address + Size);
        }

        public void GetSibling(SpanLocation location)
        {
            var objectNumber = Operands[0].Value;
            var gameObject = machine.ObjectTable.GetObject(objectNumber);
            var siblingNumber = gameObject.Sibling;
            var hasSibling = siblingNumber != 0;

            machine.SetVariable(StoreResult, hasSibling ? 1 : 0);
            Branch.Go(hasSibling, machine, Size, location);
        }

        public void GetChild(SpanLocation location)
        {
            var objectNumber = Operands[0].Value;
            var gameObject = machine.ObjectTable.GetObject(objectNumber);
            var childNumber = gameObject.Child;
            var hasChild = childNumber != 0;

            machine.SetVariable(StoreResult, hasChild ? 1 : 0);
            Branch.Go(hasChild, machine, Size, location);
        }

        public void GetParent(SpanLocation location)
        {
            var objectNumber = Operands[0].Value;
            var gameObject = machine.ObjectTable.GetObject(objectNumber);
            var parentNumber = gameObject.Parent;

            machine.SetVariable(StoreResult, parentNumber);
            machine.SetPC(location.Address + Size);
        }

        public void PrintObj(SpanLocation location)
        {
            var number = Operands[0].Value;
            var gameObject = machine.ObjectTable.GetObject(number);
            machine.Output.Write(gameObject.Description);
            machine.SetPC(location.Address + Size);
        }

        public void JZ(SpanLocation location)
        {
            var value = Operands[0].Value;
            var result = value == 0;
            Branch.Go(result, machine, Size, location);
        }

        public void Ret(SpanLocation location)
        {
            var returnValue = Operands[0].Value;
            var frame = machine.StackFrames.PopFrame();

            machine.SetVariable(frame.StoreVariable, returnValue);
            machine.SetPC(frame.ReturnPC);
        }

        public void Jump(SpanLocation location)
        {
            //  Jump(unconditionally) to the given label. (This is not a branch instruction and the 
            // operand is a 2 - byte signed offset to apply to the program counter.) 
            // It is legal for this to jump into a different routine(which should not change the 
            // routine call state), although it is considered bad practice to do so and the Txd 
            // disassembler is confused by it.
            var offset = Operands[0].SignedValue;
            machine.SetPC(location.Address + offset);
        }
    }
}
