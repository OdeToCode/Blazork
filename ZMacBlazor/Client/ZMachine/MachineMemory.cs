using System;
using System.IO;
using ZMacBlazor.Client.ZMachine.Address;

namespace ZMacBlazor.Client.ZMachine
{
    public class MachineMemory
    {
        public void Load(Stream bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));

            using (var reader = new BinaryReader(bytes))
            {
                contents = new byte[bytes.Length];
                bytes.Read(contents, 0, (int)bytes.Length);
            }
        }

        public ReadOnlySpan<byte> At(ushort address)
        {
            return contents.AsSpan(address);
        }

        public byte Version => contents[Header.VERSION];
    
        public ushort HighMemory => ByteAddress.ToShort(contents, Header.HIGHMEMORY);

        public ushort StartingProgramCounter 
        {
            get
            {
                if(Version < 6)
                {
                    return ByteAddress.ToShort(contents, Header.PC);
                }
                else
                {
                    return PackedAddress.ToShort(contents, Header.PC);
                }
            }
        }

        public ushort Dictionary => ByteAddress.ToShort(contents, Header.DICTIONARY);

        public ushort ObjectTable => ByteAddress.ToShort(contents, Header.OBJECTTABLE);

        public int FileLength
        {
            get 
            {
                var value = ByteAddress.ToShort(contents, Header.FILELENGTH);
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
