using System;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class VarOperandResolver 
    {
        public OperandCollection DecodeOperands(ReadOnlySpan<byte> bytes)
        {
            var byteOffset = 1;
            var operands = new OperandCollection();
            var operandTypes = Bits.BreakIntoTwos(bytes[0]);

            foreach (var type in operandTypes)
            {
                switch (type)
                {
                    case OperandType.Small:
                        operands.Add(new Operand(type, bytes[byteOffset]));
                        byteOffset += 1;
                        break;

                    case OperandType.Variable:
                        operands.Add(new Operand(type, bytes[byteOffset]));
                        byteOffset += 1;
                        break;

                    case OperandType.Large:
                        operands.Add(new Operand(type, Bits.MakeWord(bytes.Slice(byteOffset, 2))));
                        byteOffset += 2;
                        break;

                    case OperandType.Ommitted:
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown operand type {type:X}");
                }
            }

            return operands;
        }
    }
}