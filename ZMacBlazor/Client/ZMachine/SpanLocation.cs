using System;
using System.Text;

namespace ZMacBlazor.Client.ZMachine
{

    public readonly ref struct SpanLocation
    {
        public SpanLocation(int address, Span<byte> bytes)
        {
            Address = address;
            Bytes = bytes;
        }

        public SpanLocation Forward(int byteCount)
        {
            return new SpanLocation(Address + byteCount, Bytes.Slice(byteCount));
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for(var i = 0; i < 10; i++)
            {
                sb.Append($"@{Address:X} {Bytes[i]:X} ");
            }
            return sb.ToString();
        }

        public int Address { get; }
        public Span<byte> Bytes { get; }
    }
}
