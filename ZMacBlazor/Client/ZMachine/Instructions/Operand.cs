using System;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class Operand
    {
        public Operand(byte type, int value, Machine machine)
        {
            RawValue = value;
            Type = type;
            this.machine = machine;
        }

        public int Value
        {
            get 
            {
                return Type switch
                {
                    OperandType.Small => (ushort)RawValue,
                    OperandType.Large => (ushort)RawValue,
                    OperandType.Variable => (ushort)machine.ReadVariable(RawValue),
                    _ => throw new InvalidOperationException($"Illegal operand resolve type {Type:x}")
                };
            }
        }

        public int SignedValue
        {
            get
            {
                return Type switch
                {
                    OperandType.Small => (short)RawValue,
                    OperandType.Large => (short)RawValue,
                    OperandType.Variable => (short)machine.ReadVariable(RawValue),
                    _ => throw new InvalidOperationException($"Illegal operand resolve type {Type:x}")
                };
            }
        }

        public int RawValue { get; protected set; }
        
        public byte Type { get; protected set; }
        private readonly Machine machine;
    }
}