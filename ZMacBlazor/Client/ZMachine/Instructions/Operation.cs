using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public delegate void Operation(Machine machine, Instruction instruction);
}
