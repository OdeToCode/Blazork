using System;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class VarOperandResolver 
    {
        public void AddOperands(OperandCollection operands, ReadOnlySpan<byte> bytes)
        {
            var byteOffset = 1;
            var operandTypes = Bits.BreakIntoTwos(bytes[0]);

            foreach (var type in operandTypes)
            {
                switch (type)
                {
                    case OperandType.Small:
                        operands.Add(type, bytes[byteOffset]);
                        byteOffset += 1;
                        break;

                    case OperandType.Variable:
                        operands.Add(type, bytes[byteOffset]);
                        byteOffset += 1;
                        break;

                    case OperandType.Large:
                        operands.Add(type, Bits.MakeWord(bytes.Slice(byteOffset, 2)));
                        byteOffset += 2;
                        break;

                    case OperandType.Ommitted:
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown operand type {type:X}");
                }
            }
        }
    }
}