using System;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class VarInstruction : Instruction
    {
        public VarInstruction(Machine machine) : base(machine)
        {
            varOperandResolver = new VarOperandResolver();
        }

        public override void Execute(MemoryLocation memory)
        {
            OpCode = Bits.BottomFive(memory.Bytes[0]);
            Operands = varOperandResolver.DecodeOperands(memory.Bytes.Slice(1));
            Operation = OpCode switch
            {
                0x00 => Call,
                0x01 => StoreW,
                _ => throw new InvalidOperationException($"Unknown VAR opcode {OpCode:X}")
            };
            
            Operation(memory);
        }

        public void Call(MemoryLocation memory)
        {
            var callAddress = Machine.Memory.Unpack(Operands[0].Value);
            //var method = new MethodDescriptor(callAddress, machine);
            
            //var storeLocation = 

            //erk!
            //var returnLocation = machine.PC +
            //                    (instruction.Operands.Count * 2) +
            //                    1 + // method header
            //                    1;  // store result

        }

        public void StoreW(MemoryLocation memory)
        {
            throw new NotImplementedException();
        }

        VarOperandResolver varOperandResolver;
    }
}