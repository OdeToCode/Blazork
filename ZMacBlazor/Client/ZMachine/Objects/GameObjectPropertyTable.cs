using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZMacBlazor.Client.ZMachine.Objects
{
    public class GameObjectPropertyTable
    {
        private readonly Machine machine;

        public GameObjectPropertyTable(Machine machine, int startingAddress)
        {
            this.machine = machine;

            var location = machine.Memory.LocationAt(startingAddress);
            var textLength = location.Bytes[0] * 2;


        }

        public string Description { get; set; }
    }
}
