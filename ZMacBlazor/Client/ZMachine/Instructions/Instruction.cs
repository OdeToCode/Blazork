using System;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public abstract class Instruction
    {
        public Instruction(Machine machine)
        {
            Machine = machine;
            Operation = a => throw new InvalidOperationException("Operation not set");
            Operands = OperandCollection.Empty;
        }

        public abstract void Execute(MemoryLocation memory);

        public byte OpCode { get; protected set; }
        public Machine Machine { get; }
        public Operation Operation { get; protected set; }
        public OperandCollection Operands { get; protected set; }
    }
}