﻿using System;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class Branch
    {
        public Branch(bool branchOnTrue, int offset, int size)
        {
            Offset = offset;
            Size = size;
            BranchOnTrue = branchOnTrue;
        }

        public void Go(bool result, Machine machine, int instructionSize, MemoryLocation location)
        {
            if (Offset == 0 && BranchOnTrue == result)
            {
                throw new InvalidOperationException("Means to return false from current routine");
            }
            else if (Offset == 1 && BranchOnTrue == result)
            {
                throw new InvalidOperationException("measure to return true from current routine");
            }
            else if (BranchOnTrue == result)
            {
                var newPC = location.Address + instructionSize + Offset - 2;
                machine.SetPC(newPC);
            }
            else
            {
                machine.SetPC(location.Address + instructionSize);
            }

        }

        public override string ToString()
        {
            return $"Offset:{Offset:X} when {BranchOnTrue}";
        }

        public int Offset { get; }
        public bool BranchOnTrue { get; }
        public int Size { get; }

        public static readonly Branch NullBranch = new Branch(false, 0, 0);
    }
}
