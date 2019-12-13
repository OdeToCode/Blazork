using System;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class IndirectOperandResolver
    {
        public void AddOperands(OperandCollection operands, ReadOnlySpan<byte> bytes)
        {
            if (operands == null) throw new ArgumentNullException(nameof(operands));

            var operandType1 = OperandType.Variable;
            operands.Add(operandType1, bytes[0]);

            var operandType2 = OperandType.Small;
            operands.Add(operandType2, bytes[1]);
        }
    }
}
