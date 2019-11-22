namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public enum OperandType
    {
        Ommitted = 0b11,
        Variable = 0b10,
        Small = 0b01,
        Large = 0b00
    }
}
