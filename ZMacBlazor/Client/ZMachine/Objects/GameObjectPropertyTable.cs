using System;
using System.Collections.Generic;
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

            var location = machine.Memory.SpanAt(startingAddress);
            var textLength = location.Bytes[0] * 2;
            
            var decoder = new ZStringDecoder(machine);
            Description = decoder.Decode(location.Bytes.Slice(1));

            Properties = new Dictionary<int, GameObjectProperty>();
            var propertyEntry = machine.Memory.MemoryAt(startingAddress + textLength + 1);
            while(propertyEntry.Bytes.Span[0] != 0)
            {
                var property = new GameObjectProperty(machine, propertyEntry);
                var nextEntryAddress = propertyEntry.Address + property.Size;
                Properties.Add(property.Number, property);

                propertyEntry = machine.Memory.MemoryAt(nextEntryAddress);                
            }
        }
        
        public string Description { get; } = "";
        public Dictionary<int, GameObjectProperty> Properties { get; }
    }
}
