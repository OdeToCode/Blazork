using System;
using System.Collections.Generic;
using System.Text;

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
        }
    }
}
