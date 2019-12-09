using Microsoft.Extensions.Logging;
using System;
using System.Linq;

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
            
            OpCode = Bits.BottomFive(memory.Bytes[0]);
            Operation = OpCode switch
            {
                0x00 => new Operation(nameof(Call), Call),
                0x01 => new Operation(nameof(StoreW), StoreW),
                _ => throw new InvalidOperationException($"Unknown VAR opcode {OpCode:X}")
            };
            
            Operation.Method(memory);
        }

        public void Call(MemoryLocation memory)
        {
            var callAddress = Machine.Memory.Unpack(Operands[0].Value);
            var methodMemory = Machine.Memory.LocationAt(callAddress);
            var method = new MethodDescriptor(methodMemory, Machine);
            var callInstructionSize = 4 + (Operands.Size);
            
            Store = memory.Bytes[callInstructionSize - 1];
            var newFrame = new StackFrame(memory.Address + callInstructionSize,
                                          method.LocalsCount, Store);
            Machine.StackFrames.PushFrame(newFrame);
            
            for(var i = 1; i < Operands.Count; i++)
            {
                Machine.SetWordVariable(i, Operands[i].Value);
            }
            
            Machine.SetPC(callAddress + method.HeaderSize);
        }

        public void StoreW(MemoryLocation memory)
        {
            throw new NotImplementedException();
        }

        VarOperandResolver operandResolver;
    }
}