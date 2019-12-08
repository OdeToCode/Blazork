using System;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public abstract class Instruction
    {
        public Instruction(Machine machine)
        {
            Machine = machine;
            Operation = a => throw new InvalidOperationException("Operation not set");
            Operands = OperandCollection.Empty;
        }

        public abstract void Execute(MemoryLocation memory);

        public byte OpCode { get; protected set; }
        public Machine Machine { get; }
        public Operation Operation { get; protected set; }
        public OperandCollection Operands { get; protected set; }
    }

    public class VarInstruction : Instruction
    {
        public VarInstruction(Machine machine) : base(machine)
        {
            varOperandResolver = new VarOperandResolver();
        }

        public override void Execute(MemoryLocation memory)
        {
            OpCode = Bits.BottomFive(memory.Bytes[0]);
            Operands = varOperandResolver.DecodeOperands(memory.Bytes.Slice(1));
            Operation = OpCode switch
            {
                0x00 => Call,
                0x01 => StoreW,
                _ => throw new InvalidOperationException($"Unknown VAR opcode {OpCode:X}")
            };
            
            Operation(memory);
        }

        public void Call(MemoryLocation memory)
        {
            var callAddress = Machine.Memory.Unpack(Operands[0].Value);
            //var method = new MethodDescriptor(callAddress, machine);
            
            //var storeLocation = 

            //erk!
            //var returnLocation = machine.PC +
            //                    (instruction.Operands.Count * 2) +
            //                    1 + // method header
            //                    1;  // store result

        }

        public void StoreW(MemoryLocation memory)
        {
            throw new NotImplementedException();
        }

        VarOperandResolver varOperandResolver;
    }

#pragma warning disable CA1062 // Validate arguments of public methods
    public static class VarOps
    {
        public static void Call(Machine machine, Instruction instruction)
        {
            var callAddress = machine.Memory.Unpack(instruction.Operands[0].Value);
            var memory = machine.Memory.LocationAt(callAddress);
            var method = new MethodDescriptor(memory, machine);
            //var storeLocation = 

            //erk!
            //var returnLocation = machine.PC +
            //                    (instruction.Operands.Count * 2) +
            //                    1 + // method header
            //                    1;  // store result

            //machine.SetPC(callAddress + method.HeaderSize);
        }

        public static void StoreW(Machine machine, Instruction instruction)
        {

        }
    }
#pragma warning restore CA1062 // Validate arguments of public methods

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

    public readonly ref struct MemoryLocation
    {
        public MemoryLocation(int address, ReadOnlySpan<byte> bytes)
        {
            Address = address;
            Bytes = bytes;
        }

        public int Address { get; }
        public ReadOnlySpan<byte> Bytes { get; }
    }

    public class InstructionDecoder
    {
        private readonly Machine machine;

        public InstructionDecoder(Machine machine)
        {
            this.machine = machine;
        }

        public Instruction Decode(MemoryLocation memory)
        {
            var instruction = memory.Bytes[0] switch
            {
                0xBE => DecodeExt(memory),
                var v when Bits.SixSevenSet(v) => DecodeVar(memory),
                var v when Bits.SevenSet(v) => DecodeShort(memory),
                _ => DecodeLong(memory)
            };

            return instruction;
        }

        private Instruction DecodeExt(MemoryLocation memory)
        {
            var opcode = memory.Bytes[1];
            return CreateExtInstruction(opcode);
        }

        private Instruction DecodeShort(MemoryLocation memory)
        {
            var opcode = Bits.BottomFour(memory.Bytes[0]);

            if (Bits.FourFiveSet(memory.Bytes[0]))
            {
                return CreateOp0Instruction(opcode);
            }
            else
            {
                return CreateOp1Instruction(opcode);
            }
        }

        private Instruction DecodeLong(MemoryLocation memory)
        {
            var opcode = Bits.BottomFive(memory.Bytes[0]);

            return CreateOp2Instruction(opcode);
        }

        private Instruction DecodeVar(MemoryLocation memory)
        {
            if (Bits.FiveSet(memory.Bytes[0]))
            {
                return new VarInstruction(machine);
            }
            else
            {
                var opcode = Bits.BottomFive(memory.Bytes[0]);
                return CreateOp2Instruction(opcode);
            }
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