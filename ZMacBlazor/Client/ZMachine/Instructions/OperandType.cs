using System;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public static class OperandType
    {
        public const byte Ommitted = 0b11;
        public const byte Variable = 0b10;
        public const byte Small = 0b01;
        public const byte Large = 0b00;

        public static string ToString(byte value)
        {
            return value switch
            {
                Large => nameof(Large),
                Variable => nameof(Variable),
                Small => nameof(Small),
                Ommitted => nameof(Ommitted),
                _ => throw new InvalidOperationException("Unknown OperandType {value:X}")
            };
        }
    }
}
