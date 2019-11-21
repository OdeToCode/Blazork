using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZMacBlazor.Client.ZMachine.Address
{
    public static class PackedAddress
    {
        public static ushort ToShort(byte[] bytes, int offset)
        {
            throw new NotImplementedException();
            
            // ***[1.0] A packed address specifies where a routine or string begins in high memory. Given a packed address P, the formula to obtain the corresponding byte address B is:

            //  2P Versions 1, 2 and 3
            //  4P Versions 4 and 5
            //  4P + 8R_O Versions 6 and 7, for routine calls

            //4P + 8S_O    Versions 6 and 7, for print_paddr

            //8P           Version 8
            //R_O and S_O are the routine and strings offsets(specified in the header as words at $28 and $2a, respectively).
          
        }
    }
}
