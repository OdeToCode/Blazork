using System;

namespace ZMacBlazor.Client.ZMachine.Address
{
    public static class ByteAddress
    {
        public static ushort ToShort(byte[] bytes, int offset)
        {
            if(bytes == null) { throw new ArgumentNullException(nameof(bytes)); }

            byte a = bytes[offset];
            byte b = bytes[offset + 1];
            ushort val = (ushort)((a << 8) + b);

            return val;
        }
    }
}
