namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public static class OperandType
    {
        public const byte Ommitted = 0b11;
        public const byte Variable = 0b10;
        public const byte Small = 0b01;
        public const byte Large = 0b00;
    }
}
