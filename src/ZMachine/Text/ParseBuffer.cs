using System;

namespace Blazork.ZMachine.Text
{
    public class ParseBuffer
    {
        private MemoryLocation memory;

        public int MaxLength { get; }

        public ParseBuffer(MemoryLocation memory)
        {
            this.memory = memory;
            MaxLength = memory.Bytes.Span[0] - 1;

            if(MaxLength <= 6)
            {
                throw new InvalidOperationException("Parse buffer size indicates a bug");
            }
        }
    }
}
