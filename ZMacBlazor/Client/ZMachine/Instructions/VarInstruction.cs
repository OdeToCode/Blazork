using System;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class VarInstruction : Instruction
    {
        public VarInstruction(Machine machine) : base(machine)
        {
            operandResolver = new VarOperandResolver();
        }

        public override void Execute(MemoryLocation memory)
        {
            operandResolver.AddOperands(Operands, memory.Bytes.Slice(1));

            Size = 2 + Operands.Size;
            OpCode = Bits.BottomFive(memory.Bytes[0]);
            Operation = OpCode switch
            {
                0x00 => new Operation(nameof(Call), Call, hasStore: true),
                _ => throw new InvalidOperationException($"Unknown VAR opcode {OpCode:X}")
            };
            if (Operation.HasBranch)
            {
                throw new NotImplementedException("Do this");
                Size += Branch.Size;
            }
            if(Operation.HasStore)
            {
                Store = memory.Bytes[Size];
                Size += 1;
            }
            
            DumpToLog(memory);
            Operation.Execute(memory);
        }

        public void Call(MemoryLocation memory)
        {
            var callAddress = Machine.Memory.Unpack(Operands[0].Value);
            var methodMemory = Machine.Memory.LocationAt(callAddress);
            var method = new MethodDescriptor(methodMemory, Machine);

            
            var newFrame = new StackFrame(memory.Address + Size,
                                          method.LocalsCount, Store);
            Machine.StackFrames.PushFrame(newFrame);
            
            for(var i = 1; i < Operands.Count; i++)
            {
                Machine.SetWordVariable(i, Operands[i].Value);
            }

            Machine.SetPC(callAddress + method.HeaderSize);
        }

        VarOperandResolver operandResolver;
    }
}