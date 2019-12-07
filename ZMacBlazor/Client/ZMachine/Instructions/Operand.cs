namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class Operand
    {
        public Operand(byte type, int value)
        {
            Value = value;
            OperandType = type;
        }

        public int Value { get; protected set; }
        public byte OperandType { get; protected set; }
    }
}