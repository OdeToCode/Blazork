using System;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public delegate void OperationDelegate(MemoryLocation memory);

    public class Operation
    {
        public Operation(string name, OperationDelegate method, bool hasBranch = false, bool hasStore = false)
        {
            Name = name;
            Method = method;
            HasBranch = hasBranch;
            HasStore = hasStore;

        }

        public void Deconstruct(out string name, out OperationDelegate method)
        {
            name = Name;
            method = Method;
        }

        public void Execute(MemoryLocation location)
        {
            Method(location);
        }

        public bool HasBranch { get; set; }
        public bool HasStore { get; set; }
        public string Name { get; set; }
        public OperationDelegate Method { get; set; }
    }
}