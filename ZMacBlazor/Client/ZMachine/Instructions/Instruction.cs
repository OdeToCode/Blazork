using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ZMacBlazor.Client.ZMachine.Instructions
{

    public interface IInstruction
    {
        void Execute(Machine machine) { }
    }

    public class VarCall : IInstruction
    {

    }

    public class VarStoreW : IInstruction
    {
        
    }

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

    //private OperandCount DecodeOpCount(byte value)
    //{
    //    OperandCount forShort(byte v)
    //    {
    //        if (Bits.FourFiveSet(v)) return OperandCount.Zero;
    //        return OperandCount.One;
    //    }

    //    OperandCount forVar(byte v)
    //    {
    //        if (Bits.FiveSet(v)) return OperandCount.Var;
    //        return OperandCount.Two;
    //    };

    //    return Form switch
    //    {
    //        InstructionForm.ShortForm => forShort(value),
    //        InstructionForm.LongForm => OperandCount.Two,
    //        InstructionForm.VarForm => forVar(value),
    //        InstructionForm.ExtForm => OperandCount.Var,
    //        _ => throw new InvalidOperationException("Unknown instruction form")
    //    };

    public class InstructionDecoder
    {
        private readonly ILogger logger;

        public InstructionDecoder(ILogger logger)
        {
            this.logger = logger;
        }
        
        public IInstruction Decode(ReadOnlySpan<byte> bytes)
        {
            var instruction = bytes[0] switch
            {
                0xBE => CreateExtInstruction(bytes),
                var v when Bits.SixSevenSet(v) => CreateVarInstruction(bytes),
                var v when Bits.SevenSet(v) => CreateShortInstruction(bytes),
                _ => CreateLongInstruction(bytes)
            };

            logger.LogInformation($"Decoded {instruction.GetType()}");
            return instruction;
        }

        private IInstruction CreateExtInstruction(ReadOnlySpan<byte> bytes)
        {
            return bytes[1] switch
            {
                _ => throw new InvalidOperationException("Unknown OpCode")
            };
        }

        private IInstruction CreateShortInstruction(ReadOnlySpan<byte> bytes)
        {
            return Bits.BottomFour(bytes[0]) switch
            {
                _ => throw new InvalidOperationException("Unknown OpCode")
            };
        }

        private IInstruction CreateLongInstruction(ReadOnlySpan<byte> bytes)
        {
            return Bits.BottomFive(bytes[0]) switch
            {
                _ => throw new InvalidOperationException("Unknown OpCode")
            };
        }

        private IInstruction CreateVarInstruction(ReadOnlySpan<byte> bytes)
        {
            return Bits.BottomFive(bytes[0]) switch
            {
                0x00 => new VarCall(),
                0x01 => new VarStoreW(),
                _ => throw new InvalidOperationException("Unknown OpCode")
            };
        }
    }
}
