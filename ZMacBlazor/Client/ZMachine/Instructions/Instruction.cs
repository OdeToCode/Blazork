using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    // f - form
    // o - opcode
    // t - opcode type
    // * - operand type
    // 1,2,3,4 - operands
    //
    // Long 2OP
    // f**ooooo 11111111 22222222
    // 0xxxxxxx xxxxxxxx xxxxxxxx
    //
    // * - 0 small constant 1 is variable
    //
    // Short 0OP / 1OP
    // ffttoooo 11111111
    // 10xxxxxx xxxxxxxx
    //
    // if(tt == 11) 0OP else 1OP
    //
    // Extended EXT
    // 0xBE     oooooooo ******** 11111111 22222222 33333333 44444444
    // 10111110 xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx
    //
    // Variable 2OP / VAR
    // fftooooo ******** 11111111 22222222 33333333 44444444
    // 11xxxxxx xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx
    // 
    // if(t == 0) 2OP else VAROP 
    //
    // Double VAR (call_vs2 call vn_2)
    // fftooooo ******** ******** 11111111 22222222 33333333 44444444 55555555 66666666 77777777 88888888
    // 11xxxxxx xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx 
    // 
    // if(t == 0) 2OP else VAROP 


    public class Instruction
    {
        public Instruction(ReadOnlySpan<byte> bytes)
        {
            Form = DecodeForm(bytes[0]);
            OpCount = DecodeOpCount(bytes[0]);
            OpCode = DecodeOpCode(bytes);
            OperandTypes = DecodeOperandTypes(bytes);
        }

        private ICollection<OperandType> DecodeOperandTypes(ReadOnlySpan<byte> bytes)
        {
            
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
 
