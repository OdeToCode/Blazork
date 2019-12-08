using System;
using System.Collections.Generic;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class MethodDescriptor
    {
        public MethodDescriptor(MemoryLocation memory, Machine machine)
        {
            if(machine == null) { throw new ArgumentNullException(nameof(machine)); }

            LocalsCount = memory.Bytes[0];

            if(LocalsCount < 0 || LocalsCount > 15)
            {
                throw new InvalidOperationException($"Invalid numer of locals {LocalsCount}");
            }
            
            if(machine.Version < 5)
            {
                var values = new int[LocalsCount];
                for (var i = 0; i < LocalsCount; i++)
                {
                    var offset = 1 + (i * 2);
                    values[i] = Bits.MakeWord(memory.Bytes.Slice(offset, 2));
                }
                InitialValues = values;
                HeaderSize = (1 + (LocalsCount * 2));
            }
            StartAddress = memory.Address + HeaderSize;
        }

        public int LocalsCount { get; }
        public int StartAddress { get; }
        public ICollection<int> InitialValues { get; } = Empty;
        public int HeaderSize { get; set; } = 1;
        readonly static ICollection<int> Empty = new List<int>();
    }
}
