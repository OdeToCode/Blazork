using System;

namespace Blazork.ZMachine
{
    public readonly struct MemoryLocation
    {
        public MemoryLocation(int address, Memory<byte> bytes)
        {
            Address = address;
            Bytes = bytes;
        }

        public int Address { get; }
        public Memory<byte> Bytes { get; }
    }
}
