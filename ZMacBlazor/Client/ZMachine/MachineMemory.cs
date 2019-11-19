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

        public byte Version
        {
            get { return contents[0]; }
        }

        public short HighMemory
        {
            get { return ByteAddress.ToShort(contents, 4); }
        }

        public short StartingProgramCounter 
        {
            get
            {
                if(Version < 6)
                {
                    return ByteAddress.ToShort(contents, 6);
                }
                else
                {
                    return PackedAddress.ToShort(contents, 6);
                }
            }
        }


        byte[] contents;
    }
}
