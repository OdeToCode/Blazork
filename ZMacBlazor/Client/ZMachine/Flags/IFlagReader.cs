using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZMacBlazor.Client.ZMachine.Flags
{
    public class IFlagReader
    {
        public StatusLine StatusLine { get; }
        public bool StorySplit { get; }
    }
}
