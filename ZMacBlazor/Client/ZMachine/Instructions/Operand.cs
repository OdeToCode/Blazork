namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class Operand
    {
        public Operand(byte type, ushort value)
        {
            Value = value;
            OperandType = type;
        }

        public ushort Value { get; protected set; }
        public byte OperandType { get; protected set; }
    }
}