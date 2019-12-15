using System;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class BranchDescriptor
    {
        public BranchDescriptor(bool branchOnTrue, int offset, int size)
        {
            Offset = offset;
            Size = size;
            BranchOnTrue = branchOnTrue;
        }

        public void Go(bool result, Machine machine, int instructionSize, SpanLocation currentLocation)
        {
            if (machine == null) throw new ArgumentNullException(nameof(machine));

            if (Offset == 0 && BranchOnTrue == result)
            {
                var frame = machine.StackFrames.PopFrame();
                machine.SetVariable(frame.StoreVariable, 0);
                machine.SetPC(frame.ReturnPC);
            }
            else if (Offset == 1 && BranchOnTrue == result)
            {
                var frame = machine.StackFrames.PopFrame();
                machine.SetVariable(frame.StoreVariable, 1);
                machine.SetPC(frame.ReturnPC);
            }
            else if (BranchOnTrue == result)
            {
                var newPC = currentLocation.Address + instructionSize + Offset - 2;
                machine.SetPC(newPC);
            }
            else
            {
                machine.SetPC(currentLocation.Address + instructionSize);
            }
        }

        public override string ToString()
        {
            return $"Offset:{Offset:X} when {BranchOnTrue}";
        }

        public int Offset { get; }
        public bool BranchOnTrue { get; }
        public int Size { get; }

        public static readonly BranchDescriptor NullBranch = new BranchDescriptor(false, 0, 0);
    }
}
