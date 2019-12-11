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

        public override string ToString()
        {
            var sb = new StringBuilder();
            for(var i = 0; i < 12; i++)
            {
                sb.Append($"{Bytes[i]:X} ");
            }
            return sb.ToString();
        }

        public int Address { get; }
        public Span<byte> Bytes { get; }
    }
}
