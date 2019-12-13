using System;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class IndirectOperandResolver
    {
        public void AddOperands(OperandCollection operands, ReadOnlySpan<byte> bytes)
        {
            if (operands == null) throw new ArgumentNullException(nameof(operands));

            

            var operandType1 = Bits.SixSet(bytes[0]) ?
                                OperandType.Variable : OperandType.Small;
            operands.Add(operandType1, bytes[1]);

            var operandType2 = Bits.FiveSet(bytes[0]) ?
                                OperandType.Variable : OperandType.Small;
            operands.Add(operandType2, bytes[2]);
        }
    }
}
