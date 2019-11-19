using System;
using System.IO;

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



        byte[] contents;
    }
}
