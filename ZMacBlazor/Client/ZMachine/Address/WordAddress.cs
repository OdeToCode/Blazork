using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZMacBlazor.Client.ZMachine.Address
{
    public static class WordAddress
    {
        public static ushort ToShort(byte[] bytes, int offset)
        {
            // A word address specifies an even address in the bottom 128K of memory(by giving the address divided by 2). (Word addresses are used only in the abbreviations table.)
            throw new NotImplementedException();
        }
    }
}
