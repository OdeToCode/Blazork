using Microsoft.Extensions.Logging;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public abstract class Instruction
    {
        public Instruction(Machine machine)
        {
            Machine = machine;
            Operands = new OperandCollection(machine);
            Operation = EmptyOperation;
            Store = int.MinValue;
        }

        public void DumpToLog(MemoryLocation memory)
        {
            Machine.Logger.Log(LogLevel.Trace, $"{ToString()} Raw: {memory.ToString()}");
        }

        public override string ToString()
        {
            return $"Instruction: {GetType().Name} {Operation.Name} {Operands.ToString()}";
        }

        public abstract void Execute(MemoryLocation memory);

        public OperandCollection Operands { get; }
        public Machine Machine { get; }
        public Operation Operation { get; protected set; }
        public Branch Branch { get; set; }
        public byte OpCode { get; protected set; }
        public int Store { get; set; }
        
        public readonly static Operation EmptyOperation = new Operation("Invalid", l => { });
    }
}