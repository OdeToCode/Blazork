using Microsoft.Extensions.Logging;
using System;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class Decoder
    {
        private readonly ILogger logger;

        public Decoder(ILogger logger)
        {
            this.logger = logger;
        }

        public Instruction Decode(ReadOnlySpan<byte> bytes)
        {
            var instruction = new Instruction(bytes);

            logger.LogInformation($"Instruction decoded: " +
                $"{instruction.Form} {instruction.OpCount} {instruction.OpCode}");
            
            return instruction;
        }
    }
}
