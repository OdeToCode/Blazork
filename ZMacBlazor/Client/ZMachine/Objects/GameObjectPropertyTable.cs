using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZMacBlazor.Client.ZMachine.Text;

namespace ZMacBlazor.Client.ZMachine.Objects
{
    public class GameObjectPropertyTable
    {
        private readonly Machine machine;

        public GameObjectPropertyTable(Machine machine, int startingAddress)
        {
            if (machine == null) throw new ArgumentNullException(nameof(machine));

            this.machine = machine;

            var location = machine.Memory.LocationAt(startingAddress);
            var textLength = location.Bytes[0] * 2;
            var decoder = new ZStringDecoder(machine);
            Description = decoder.Decode(location.Bytes);
        }
        
        public string Description { get; set; } = "";
    }
}
