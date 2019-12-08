using System.Collections.Generic;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class OperandCollection : List<Operand>
    {
        public static readonly OperandCollection Empty = new OperandCollection();
    }
}