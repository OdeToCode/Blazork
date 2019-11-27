using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class Instruction
    {
        public Instruction(ReadOnlySpan<byte> bytes)
        {
            Form = DecodeForm(bytes[0]);
            OpCount = DecodeOpCount(bytes[0]);
            OpCode = DecodeOpCode(bytes);
        }

        private byte DecodeOpCode(ReadOnlySpan<byte> bytes)
        {
            return Form switch
            {
                InstructionForm.ShortForm => Bits.BottomFour(bytes[0]),
                InstructionForm.LongForm => Bits.BottomFive(bytes[0]),
                InstructionForm.VarForm => Bits.BottomFive(bytes[0]),
                InstructionForm.ExtForm => bytes[1],
                _ => throw new InvalidOperationException("Invalid instruction form")
            };
        }

        private OperandCount DecodeOpCount(byte value)
        {
            OperandCount forShort(byte v)
            {
                if (Bits.FourFiveSet(v)) return OperandCount.Zero;
                return OperandCount.One;
            }

            OperandCount forVar(byte v)
            {
                if (Bits.FiveSet(v)) return OperandCount.Var;
                return OperandCount.Two;
            };

            return Form switch
            {
                InstructionForm.ShortForm => forShort(value),
                InstructionForm.LongForm => OperandCount.Two,
                InstructionForm.VarForm => forVar(value),
                InstructionForm.ExtForm => OperandCount.Var,
                _ => throw new InvalidOperationException("Unknown instruction form")
            };
        }

        private InstructionForm DecodeForm(byte value)
        {
            return value switch
            {
                0xBE => InstructionForm.ExtForm,
                var v when Bits.SixSevenSet(v) => InstructionForm.VarForm,
                var v when Bits.SevenSet(v) => InstructionForm.ShortForm,
                _ => InstructionForm.LongForm
            };
        }

        public InstructionForm Form { get; }
        public OperandCount OpCount { get; }
        public byte OpCode { get; }
        public ICollection<OperandType> OperandTypes { get; }
        public ICollection<ushort> Operands { get; }
    }
}
 
