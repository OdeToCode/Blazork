using System;
using System.IO;
using ZMacBlazor.Client.ZMachine.Address;

namespace ZMacBlazor.Client.ZMachine
{
    public class MachineMemory
    {
        public MachineMemory(Stream bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));

            using (var reader = new BinaryReader(bytes))
            {
                contents = new byte[bytes.Length];
                bytes.Read(contents, 0, (int)bytes.Length);
            }
        }

        public byte ByteAt(int address)
        {
            return contents[address];
        }

        public ushort WordAt(int address)
        {
            return Bits.MakeWord(SpanAt(address));
        }

        public ReadOnlySpan<byte> SpanAt(int address, int length = 0)
        {
            if (length != 0)
            {
                return contents.AsSpan(address, length);
            }
            return contents.AsSpan(address);
        }

        internal ushort Unpack(ushort address, bool print = false)
        {
            switch(Version)
            {
                case 0x01:
                case 0x02:
                case 0x03:
                    return (ushort)(address * 2);

                case 0x04:
                case 0x05:
                    return (ushort)(address * 4);

                case 0x06:
                case 0x07:
                    if(print)
                    {
                        return (ushort)((address * 4) + (8 * StringOffset));
                    }
                    else
                    {
                        return (ushort)((address * 4) + (8 * RoutineOffset));
                    }
                case 0x08:
                    return (ushort)(address * 8);
                default:
                    throw new InvalidOperationException($"Bad version number {Version:X}");
            }
        }

        public byte Version => contents[Header.VERSION];
    
        public ushort HighMemory => ByteAddress.ToWord(contents, Header.HIGHMEMORY);

        public ushort RoutineOffset => ByteAddress.ToWord(contents, Header.ROUTINESOFFSET);

        public ushort StringOffset => ByteAddress.ToWord(contents, Header.STATICSTRINGSOFFSET);

        public ushort StartingProgramCounter 
        {
            get
            {
                if(Version < 6)
                {
                    return ByteAddress.ToWord(contents, Header.PC);
                }
                else
                {
                    return PackedAddress.ToShort(contents, Header.PC);
                }
            }
        }

        public ushort Dictionary => ByteAddress.ToWord(contents, Header.DICTIONARY);

        public ushort ObjectTable => ByteAddress.ToWord(contents, Header.OBJECTTABLE);

        public int FileLength
        {
            get 
            {
                var value = ByteAddress.ToWord(contents, Header.FILELENGTH);
                if(Version <= 3)
                {
                    return value * 2;
                }
                else if(Version <= 5)
                {
                    return value * 4;
                }
                else
                {
                    return value * 8;
                }
            }
        }

        byte[] contents;
    }
}
