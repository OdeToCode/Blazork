using Microsoft.Extensions.Logging;
using System;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class Instruction
    {
        public Instruction(Operation operation,
                           OperandCollection operands)
        {
            Operands = operands;
            Operation = operation;
        }

        public OperandCollection Operands { get; }
        public Operation Operation { get; }

        public virtual void Execute(Machine machine)
        {
            Operation(machine, Operands);
        }
    }

    public static class VarOps
    {
        internal static void Call(Machine machine, OperandCollection operands)
        {
            var callAddress = machine.Memory.Unpack(operands[0].Value);
            
            
            machine.SetPC(callAddress);
        }

        internal static void StoreW(Machine machine, OperandCollection operands)
        {

        }
    }

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
        private readonly ILogger logger;
        private VarOperandResolver varOperandResolver = new VarOperandResolver();

        public InstructionDecoder(ILogger logger)
        {
            this.logger = logger;
        }

        public Instruction Decode(ReadOnlySpan<byte> bytes)
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

        private Instruction DecodeExt(ReadOnlySpan<byte> bytes)
        {
            var opcode = bytes[1];
            return CreateExtInstruction(opcode);
        }

        private Instruction DecodeShort(ReadOnlySpan<byte> bytes)
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

        private Instruction DecodeLong(ReadOnlySpan<byte> bytes)
        {
            var opcode = Bits.BottomFive(bytes[0]);

            return CreateOp2Instruction(opcode);
        }

        private Instruction DecodeVar(ReadOnlySpan<byte> bytes)
        {
            if (Bits.FiveSet(bytes[0]))
            {
                return CreateVarInstruction(bytes);
            }
            else
            {
                var opcode = Bits.BottomFive(bytes[0]);
                return CreateOp2Instruction(opcode);
            }
        }

        private Instruction CreateVarInstruction(ReadOnlySpan<byte> bytes)
        {
            var opcode = Bits.BottomFive(bytes[0]);
            var operands = varOperandResolver.DecodeOperands(bytes.Slice(1));

            Operation operation =  opcode switch
            {
                0x00 => VarOps.Call,
                0x01 => VarOps.StoreW,
                _ => throw new InvalidOperationException($"Unknown VAR opcode {opcode:X}")
            };

            var instruction = new Instruction(operation, operands);
            return instruction;
        }

        private Instruction CreateOp1Instruction(byte opcode)
        {
            return opcode switch
            {
                _ => throw new InvalidOperationException($"Unknown OP1 opcode {opcode:X}")
            };
        }

        private Instruction CreateOp0Instruction(byte opcode)
        {
            return opcode switch
            {
                _ => throw new InvalidOperationException($"Unknown OP0 opcode {opcode:X}")
            };
        }

        private Instruction CreateExtInstruction(byte opcode)
        {
            return opcode switch
            {
                _ => throw new InvalidOperationException($"Unknown EXT opcode {opcode:X}")
            };
        }

        private Instruction CreateOp2Instruction(byte opcode)
        {
            return opcode switch
            {
                _ => throw new InvalidOperationException($"Unknown OP2 opcode {opcode:X}")
            };
        }
    }
}