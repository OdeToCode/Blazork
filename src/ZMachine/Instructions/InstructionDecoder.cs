﻿using System;

namespace Blazork.ZMachine.Instructions
{
    // How Instructions are Encoded

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
    // fftooooo ******** 11111111 11111111 22222222 22222222 33333333 33333333 44444444 44444444 
    // 11xxxxxx xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx
    // 
    // if(t == 0) 2OP else VAROP 
    //
    // Double VAR (call_vs2 call vn_2)
    // fftooooo ******** ******** 1 - 2 - 3 - 4 - 5 - 6 - 7 - 8
    // 11xxxxxx xxxxxxxx xxxxxxxx 
    // 
    // if(t == 0) 2OP else VAROP 

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
        private readonly Machine machine;

        public InstructionDecoder(Machine machine)
        {
            this.machine = machine;
        }

        public Instruction Decode(SpanLocation memory)
        {
            var instruction = memory.Bytes[0] switch
            {
                0xBE => DecodeExt(memory),
                var v when Bits.SevenSixSet(v) => DecodeVar(memory),
                var v when Bits.SevenSet(v) => DecodeShort(memory),
                _ => DecodeLong(memory)
            };

            return instruction;
        }

        private Instruction DecodeExt(SpanLocation memory)
        {
            var opcode = memory.Bytes[1];
            return CreateExtInstruction(opcode);
        }

        private Instruction DecodeShort(SpanLocation memory)
        {
            if (Bits.FiveFourSet(memory.Bytes[0]))
            {
                return new Op0Instruction(machine);
            }
            else
            {
                return new Op1Instruction(machine);
            }
        }

        private Instruction DecodeLong(SpanLocation _)
        {
            return new Op2Instruction(machine);
        }

        private Instruction DecodeVar(SpanLocation memory)
        {
            if (Bits.FiveSet(memory.Bytes[0]))
            {
                return new VarInstruction(machine);
            }
            else
            {
                return new Op2Instruction(machine);
            }
        }

        private Instruction CreateExtInstruction(byte opcode)
        {
            return opcode switch
            {
                _ => throw new InvalidOperationException($"Unknown EXT opcode {opcode:X}")
            };
        }
    }
}