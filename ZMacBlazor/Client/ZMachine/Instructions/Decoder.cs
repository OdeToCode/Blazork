using System;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class Decoder
    {
        public Instruction Decode(ReadOnlySpan<byte> bytes)
        {
            var instruction = new Instruction(bytes);
            return instruction;
        }
    }
}
