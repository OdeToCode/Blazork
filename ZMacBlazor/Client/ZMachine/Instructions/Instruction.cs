using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ZMacBlazor.Client.ZMachine.Instructions
{

    public interface IInstruction
    {
        void Execute(Machine machine) { }
    }

    public class VCall : IInstruction
    {

    }

    public class StoreW : IInstruction
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

    // Per the specs:
    // $00 -- $1f  long      2OP small constant, small constant
    // $20 -- $3f  long      2OP small constant, variable
    // $40 -- $5f  long      2OP variable, small constant
    // $60 -- $7f  long      2OP variable, variable
    // $80 -- $8f  short     1OP large constant
    // $90 -- $9f  short     1OP small constant
    // $a0 -- $af  short     1OP variable
    // $b0 -- $bf  short     0OP
    // except $be  extended  opcode given in next byte
    // $c0 -- $df  variable  2OP(operand types in next byte)
    // $e0 -- $ff  variable  VAR(operand types in next byte(s))

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
                0xBE => DecodeExt(bytes),
                var v when Bits.SixSevenSet(v) => DecodeVar(bytes),
                var v when Bits.SevenSet(v) => DecodeShort(bytes),
                _ => DecodeLong(bytes)
            };

            logger.LogInformation($"Decoded {instruction.GetType()}");
            return instruction;
        }

        private IInstruction DecodeExt(ReadOnlySpan<byte> bytes)
        {
            var opcode = bytes[1];
            return CreateExtInstruction(opcode);
        }

        private IInstruction DecodeShort(ReadOnlySpan<byte> bytes)
        {
            var opcode = Bits.BottomFour(bytes[0]);

            if (Bits.FourFiveSet(bytes[0]))
            {
                return CreateOp0Instruction(opcode);
            }
            else
            {
                return CreateOp1Instruction(opcode);
            }
        }

        private IInstruction DecodeLong(ReadOnlySpan<byte> bytes)
        {
            var opcode = Bits.BottomFive(bytes[0]);

            return CreateOp2Instruction(opcode);
        }

        private IInstruction DecodeVar(ReadOnlySpan<byte> bytes)
        {
            var opcode = Bits.BottomFive(bytes[0]);

            if (Bits.FiveSet(bytes[0]))
            {
                return CreateVarInstruction(opcode);
            }
            else
            {
                return CreateOp2Instruction(opcode);
            }
        }

        private IInstruction CreateVarInstruction(byte opcode)
        {
            return opcode switch
            {
                0x00 => new VCall(),
                0x01 => new StoreW(),
                _ => throw new InvalidOperationException($"Unknown VAR opcode {opcode:X}")
            };
        }

        private IInstruction CreateOp1Instruction(byte opcode)
        {
            return opcode switch
            {
                _ => throw new InvalidOperationException($"Unknown OP1 opcode {opcode:X}")
            };
        }

        private IInstruction CreateOp0Instruction(byte opcode)
        {
            return opcode switch
            {
                _ => throw new InvalidOperationException($"Unknown OP0 opcode {opcode:X}")
            };
        }

        private IInstruction CreateExtInstruction(byte opcode)
        {
            return opcode switch
            {
                _ => throw new InvalidOperationException($"Unknown EXT opcode {opcode:X}")
            };
        }

        private IInstruction CreateOp2Instruction(byte opcode)
        {
            return opcode switch
            {
                _ => throw new InvalidOperationException($"Unknown OP2 opcode {opcode:X}")
            };
        }
    }
}
