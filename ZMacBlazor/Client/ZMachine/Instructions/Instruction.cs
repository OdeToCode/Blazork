﻿using Microsoft.Extensions.Logging;
using System;
using System.Text;

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
            Branch = Branch.NullBranch;
        }

        public void DumpToLog(MemoryLocation memory)
        {
            var sb = new StringBuilder();
            sb.Append($"{ToString()}");
            sb.Append($"\t Raw: @{memory.Address:X} {memory.ToString()}");

            Machine.Logger.Log(LogLevel.Trace, sb.ToString());
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"OP: {Operation.Name}");
            sb.AppendLine($"\t{Operands.ToString()}");
            if(Store >= 0)
            {
                sb.AppendLine($"\tStore:{Store}");
            }
            if(Branch != Branch.NullBranch)
            {
                sb.AppendLine($"\tBranch: { Branch.ToString()}");
            }
            return sb.ToString();
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