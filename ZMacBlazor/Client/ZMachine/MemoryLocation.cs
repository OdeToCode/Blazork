using System;

namespace ZMacBlazor.Client.ZMachine
{
    public readonly ref struct MemoryLocation
    {
        public MemoryLocation(int address, ReadOnlySpan<byte> bytes)
        {
            Address = address;
            Bytes = bytes;
        }

        public int Address { get; }
        public ReadOnlySpan<byte> Bytes { get; }
    }
}
