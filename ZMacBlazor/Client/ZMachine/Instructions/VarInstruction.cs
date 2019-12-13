using System;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class VarInstruction : Instruction
    {
        public VarInstruction(Machine machine) : base(machine)
        {
            operandResolver = new VarOperandResolver();
        }

        public override void Execute(SpanLocation memory)
        {
            operandResolver.AddOperands(Operands, memory.Bytes.Slice(1));

            Size = 2 + Operands.Size;
            OpCode = Bits.BottomFive(memory.Bytes[0]);
            Operation = OpCode switch
            {
                0x00 => new Operation(nameof(Call), Call, hasStore: true),
                0x01 => new Operation(nameof(StoreW), StoreW),
                0x05 => new Operation(nameof(PrintChar), PrintChar),
                0x06 => new Operation(nameof(PrintNum), PrintNum),
                0x03 => new Operation(nameof(PutProp), PutProp),
                _ => throw new InvalidOperationException($"Unknown VAR opcode {OpCode:X}")
            };
            if (Operation.HasBranch)
            {
                Size += Branch.Size;
                throw new NotImplementedException("Do this");
            }
            if(Operation.HasStore)
            {
                StoreResult = memory.Bytes[Size];
                Size += 1;
            }

            DumpToLog(memory);
            Operation.Execute(memory);
        }

        public void PrintChar(SpanLocation location)
        {
            var character = (char)Operands[0].Value;
            machine.Output.Write(character.ToString());

            machine.SetPC(location.Address + Size);
        }

        public void PrintNum(SpanLocation location)
        {
            var number = Operands[0].Value;
            machine.Output.Write(number.ToString());

            machine.SetPC(location.Address + Size);
        }

        public void PutProp(SpanLocation location)
        {
            // Writes the given value to the given property of the given object.If the property 
            // does not exist for that object, the interpreter should halt with a suitable error message.
            // If the property length is 1, then the interpreter should store only the least 
            // significant byte of the value. (For instance, storing - 1 into a 1 - byte property 
            // results in the property value 255.) As with get_prop the property length must not be more 
            // than 2: if it is, the behaviour of the opcode is undefined.

            var objectNumber = Operands[0].Value;
            var gameObject = machine.ObjectTable.GetObject(objectNumber);

            var propertyNumber = Operands[1].Value;
            var gameProperty = gameObject.Properties[propertyNumber];

            if(gameProperty.Value.Length > 2)
            {
                throw new InvalidOperationException("Illegal to PutProp on a property larger than 2 bytes");
            }

            gameProperty.SetValue(Operands[2].Value);

            machine.SetPC(location.Address + Size);
        }

        public void StoreW(SpanLocation location)
        {
            var baseArray = Operands[0].Value;
            var index = Operands[1].Value;
            var arrayLocation = baseArray + (2 * index);
            
            var entry = machine.Memory.SpanAt(arrayLocation, 2);
            var value = Operands[2].Value;
            
            machine.SetPC(location.Address + Size);
        }

        public void Call(SpanLocation memory)
        {
            var callAddress = machine.Memory.Unpack(Operands[0].Value);
            var methodMemory = machine.Memory.SpanAt(callAddress);
            var method = new MethodDescriptor(methodMemory, machine);

            
            var newFrame = new StackFrame(memory.Address + Size,
                                          method.LocalsCount, StoreResult);
            machine.StackFrames.PushFrame(newFrame);
            
            for(var i = 1; i < Operands.Count; i++)
            {
                machine.SetVariable(i, Operands[i].Value);
            }

            machine.SetPC(callAddress + method.HeaderSize);
        }

        VarOperandResolver operandResolver;
    }
}