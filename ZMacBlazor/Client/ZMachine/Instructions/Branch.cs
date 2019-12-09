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
