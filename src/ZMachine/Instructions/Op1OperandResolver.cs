using System;

namespace Blazork.ZMachine.Instructions
{
    public class Op1OperandResolver
    {
        public void AddOperands(OperandCollection operands, ReadOnlySpan<byte> bytes)
        {
            if (operands == null) throw new ArgumentNullException(nameof(operands));

            var type = Bits.FiveAndFour(bytes[0]);
            if(type == OperandType.Small || type == OperandType.Variable)
            {
                operands.Add(type, bytes[1]);
            }
            else if(type == OperandType.Large)
            {
                operands.Add(type, Bits.MakeWord(bytes.Slice(1, 2)));
            }
        }
    }
}