using System;
using System.Collections.Generic;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class MethodDescriptor
    {
        public MethodDescriptor(ushort address, Machine machine)
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
                var values = new ushort[LocalsCount];
                for (var i = 0; i < LocalsCount; i++)
                {
                    var offset = 1 + (i * 2);
                    values[i] = Bits.MakeWord(bytes.Slice(offset, 2));
                }
                InitialValues = values;
                HeaderSize = (ushort)(1 + (LocalsCount * 2));
            }
            StartAddress = (ushort)(address + HeaderSize);
        }

        public byte LocalsCount { get; }
        public ushort StartAddress { get; }
        public ICollection<ushort> InitialValues { get; } = Empty;
        public ushort HeaderSize { get; set; } = 1;
        readonly static ICollection<ushort> Empty = new List<ushort>();
    }
}
