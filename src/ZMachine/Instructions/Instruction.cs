using Serilog;
using System;
using System.Text;

namespace Blazork.ZMachine.Instructions
{
    public abstract class Instruction
    {
        public Instruction(Machine machine)
        {
            if (machine == null) throw new ArgumentNullException(nameof(machine));

            this.machine = machine;
            log = machine.Logger.ForContext<Instruction>();
            Operands = new OperandCollection(machine);
            Operation = EmptyOperation;
            StoreResult = int.MinValue;
            Branch = BranchDescriptor.NullBranch;
            Text = "";
            Size = 0;
        }

        public void DumpToLog(SpanLocation memory, int size)
        {
            var sb = new StringBuilder();
            sb.Append($"\t{ToString()}");
            sb.Append($"\t{memory.ToString(size)}");

            log.Verbose(sb.ToString());
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
            if(Branch != BranchDescriptor.NullBranch)
            {
                sb.AppendLine($"\tBranch: { Branch.ToString()}");
            }
            return sb.ToString();
        }

        public abstract void Prepare(SpanLocation memory);

        public virtual void Execute(SpanLocation memory)
        {
            Operation.Execute(memory);
        }

        public OperandCollection Operands { get; }
        public Operation Operation { get; protected set; }
        public BranchDescriptor Branch { get; set; }
        public string Text { get; set; }
        public byte OpCode { get; protected set; }
        public int StoreResult { get; set; }
        public int Size { get; set; }
        public readonly static Operation EmptyOperation = new Operation("Invalid", l => { });

        protected Machine machine;
        protected readonly ILogger log; 
    }
}