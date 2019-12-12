using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public abstract class Instruction
    {
        public Instruction(Machine machine)
        {
            this.machine = machine;
            Operands = new OperandCollection(machine);
            Operation = EmptyOperation;
            StoreResult = int.MinValue;
            Branch = Branch.NullBranch;
            Size = 0;
        }

        public void DumpToLog(SpanLocation memory)
        {
            var sb = new StringBuilder();
            sb.Append($"{ToString()}");
            sb.Append($"\t Raw: @{memory.Address:X} {memory.ToString()}");

            machine.Logger.Log(LogLevel.Trace, sb.ToString());
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{Operation.Name}");
            sb.AppendLine($"\tSize: {Size}");
            sb.AppendLine($"\t{Operands.ToString()}");
            if(StoreResult >= 0)
            {
                sb.AppendLine($"\tStore:{StoreResult}");
            }
            if(Branch != Branch.NullBranch)
            {
                sb.AppendLine($"\tBranch: { Branch.ToString()}");
            }
            return sb.ToString();
        }

        public abstract void Execute(SpanLocation memory);

        public OperandCollection Operands { get; }
        public Operation Operation { get; protected set; }
        public Branch Branch { get; set; }
        public byte OpCode { get; protected set; }
        public int StoreResult { get; set; }
        public int Size { get; set; }

        readonly protected Machine machine;
        public readonly static Operation EmptyOperation = new Operation("Invalid", l => { });
    }
}