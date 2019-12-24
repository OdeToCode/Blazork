using System;

namespace Blazork.ZMachine.Instructions
{
    public delegate void OperationDelegate(SpanLocation memory);

    public class Operation
    {
        public Operation(string name, OperationDelegate method, 
                         bool hasBranch = false, bool hasStore = false, bool hasText = false)
        {
            Name = name;
            Method = method;
            HasBranch = hasBranch;
            HasStore = hasStore;
            HasText = hasText;
        }

        public void Execute(SpanLocation location) => Method(location);

        public bool HasBranch { get; }
        public bool HasStore { get; }
        public bool HasText { get; }
        public string Name { get; }
        public OperationDelegate Method { get; set; }
    }
}