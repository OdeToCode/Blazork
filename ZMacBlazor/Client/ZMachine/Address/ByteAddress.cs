using System;

namespace ZMacBlazor.Client.ZMachine.Address
{
    public static class ByteAddress
    {
        public static short ToShort(byte[] bytes, int offset)
        {
            return BitConverter.ToInt16(bytes, offset);
        }
    }
}
