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
                0x01 => new Operation(nameof(StoreW), StoreW),
                0x03 => new Operation(nameof(PutProp), PutProp),
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

        public void PutProp(MemoryLocation location)
        {
            // Writes the given value to the given property of the given object.If the property 
            // does not exist for that object, the interpreter should halt with a suitable error message.
            // If the property length is 1, then the interpreter should store only the least 
            // significant byte of the value. (For instance, storing - 1 into a 1 - byte property 
            // results in the property value 255.) As with get_prop the property length must not be more 
            // than 2: if it is, the behaviour of the opcode is undefined.

            //Set property prop on object obj to a. The property must be present on the object.If the
            // property length is 1, then a must be byte-valued.

            //   e3 57 9c 06 04          PUT_PROP        "magic boat",#06,#04
        }

        public void StoreW(MemoryLocation location)
        {
            var baseArray = Operands[0].Value;
            var index = Operands[1].Value;
            var arrayLocation = baseArray + (2 * index);
            
            var entry = Machine.Memory.LocationAt(arrayLocation, 2);
            var value = Operands[2].Value;
            

            DumpToLog(location);
            Machine.SetPC(location.Address + Size);
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