using System;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public delegate void OperationDelegate(MemoryLocation memory);

    public class Operation
    {
        public Operation(string name, OperationDelegate method)
        {
            Name = name;
            Method = method;
        }

        public void Deconstruct(out string name, out OperationDelegate method)
        {
            name = Name;
            method = Method;
        }
        
        public string Name { get; set; }
        public OperationDelegate Method { get; set; }
    }
}