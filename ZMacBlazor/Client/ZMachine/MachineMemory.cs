using System;
using System.IO;
using ZMacBlazor.Client.ZMachine.Instructions;

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

        public int WordAt(int address)
        {
            return Bits.MakeWord(SpanAt(address).Bytes);
        }

        public int WordAddressAt(int address)
        {
            return Bits.MakeWord(SpanAt(address).Bytes) * 2;
        }

        public int Unpack(int address, bool print = false)
        {
            switch (Version)
            {
                case 0x01:
                case 0x02:
                case 0x03:
                    return (address * 2);

                case 0x04:
                case 0x05:
                    return (address * 4);

                case 0x06:
                case 0x07:
                    if (print)
                    {
                        return ((address * 4) + (8 * StringOffset));
                    }
                    else
                    {
                        return ((address * 4) + (8 * RoutineOffset));
                    }
                case 0x08:
                    return (address * 8);
                default:
                    throw new InvalidOperationException($"Bad version number {Version:X}");
            }
        }
        
        public void StoreByteAt(int address, byte value)
        {
            contents[address] = value;
        }

        public void StoreWordAt(int address, int value)
        {
            var (msb, lsb) = Bits.BreakWord(value);
            contents[address] = msb;
            contents[address + 1] = lsb;
        }

        public MemoryLocation MemoryAt(int address, int length = 0)
        {
            if(length != 0)
            {
                return new MemoryLocation(address, contents.AsMemory(address, length));
            }
            return new MemoryLocation(address, contents.AsMemory(address));
        }

        public SpanLocation SpanAt(int address, int length = 0)
        {
            if (length != 0)
            {
                return new SpanLocation(address, contents.AsSpan(address, length));
            }
            return new SpanLocation(address, contents.AsSpan(address));
        }

        public byte Version => contents[Header.VERSION];
    
        public int HighMemory => Bits.MakeWord(SpanAt(Header.HIGHMEMORY).Bytes);

        public int RoutineOffset => Bits.MakeWord(SpanAt(Header.ROUTINESOFFSET).Bytes);

        public int StringOffset => Bits.MakeWord(SpanAt(Header.STATICSTRINGSOFFSET).Bytes);

        public int StartingProgramCounter 
        {
            get
            {
                if(Version < 6)
                {
                    return Bits.MakeWord(SpanAt(Header.PC).Bytes);
                }
                else
                {
                    return Unpack((ushort)Header.PC);
                }
            }
        }

        public Span<byte> Globals => SpanAt(Header.GLOBALS).Bytes;

        public int Dictionary => Bits.MakeWord(SpanAt(Header.DICTIONARY).Bytes);

        public int ObjectTable => Bits.MakeWord(SpanAt(Header.OBJECTTABLE).Bytes);

        public int FileLength
        {
            get 
            {
                var value = Bits.MakeWord(SpanAt(Header.FILELENGTH).Bytes);
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
