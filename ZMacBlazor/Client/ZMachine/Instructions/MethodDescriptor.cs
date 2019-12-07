using System;
using System.Collections.Generic;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class MethodDescriptor
    {
        public MethodDescriptor(int address, Machine machine)
        {
            if(machine == null) { throw new ArgumentNullException(nameof(machine)); }

            var bytes = machine.Memory.SpanAt(address);
            LocalsCount = bytes[0];

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
                    values[i] = Bits.MakeWord(bytes.Slice(offset, 2));
                }
                InitialValues = values;
                HeaderSize = (1 + (LocalsCount * 2));
            }
            StartAddress = address + HeaderSize;
        }

        public int LocalsCount { get; }
        public int StartAddress { get; }
        public ICollection<int> InitialValues { get; } = Empty;
        public int HeaderSize { get; set; } = 1;
        readonly static ICollection<int> Empty = new List<int>();
    }
}
