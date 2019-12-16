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

        public string ToString(int size = 10)
        {
            var sb = new StringBuilder();
            sb.Append($"@{Address:X} ");
            for(var i = 0; i < size; i++)
            {
                sb.Append($"{Bytes[i]:X} ");
            }
            return sb.ToString();
        }

        public int Address { get; }
        public Span<byte> Bytes { get; }
    }
}
