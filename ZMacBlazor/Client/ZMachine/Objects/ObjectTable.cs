using System;
using System.Collections.Generic;

namespace ZMacBlazor.Client.ZMachine.Objects
{

    // The table begins with a block known as the property defaults table.
    // This contains 31 words in Versions 1 to 3 and 63 in Versions 4 and later. 
    // When the game attempts to read the value of property n for an object which does not provide 
    // property n, the n-th entry in this table is the resulting value.
    public class ObjectTable
    {
        private int[] defaults = Array.Empty<int>();
        private Machine machine;

        public ObjectTable(Machine machine)
        {
            this.machine = machine;
            this.GameObjects = new List<GameObject>();
        }

        public void Initialize()
        {
            ReadDefaultValues();
            ReadObjects();
        }

        // The largest valid object number is not directly stored anywhere in the Z-machine. 
        // Utility programs like Infodump deduce this number by assuming that, initially, 
        // the object entries end where the first property table begins.
        private void ReadObjects()
        {
            var objectEntrySize = 9;
            if(machine.Version > 4)
            {
                objectEntrySize = 14;
            }

            var propertyTablesStart = int.MaxValue;
            var objectTableStart = machine.Memory.WordAt(Header.OBJECTTABLE) + (Defaults.Length * 2);

            for(var address = objectTableStart; address < propertyTablesStart; address += objectEntrySize)
            {
                var gameObject = new GameObject(address, machine);
                if(gameObject.PropertyPointer < propertyTablesStart)
                {
                    propertyTablesStart = gameObject.PropertyPointer;
                }
                GameObjects.Add(gameObject);
            }
        }

        private void ReadDefaultValues()
        {
            int size = 31;
            if (machine.Version > 3)
            {
                size = 63;
            }
            defaults = new int[size];

            var startLocation = machine.Memory.WordAt(Header.OBJECTTABLE);
            var defaultValues = machine.Memory.LocationAt(startLocation, size);
            for (var i = 0; i < size; i++)
            {
                defaults[i] = defaultValues.Bytes[i];
            }
        }

        public List<GameObject> GameObjects { get; protected set; }
        public ReadOnlySpan<int> Defaults => defaults.AsSpan();
    }
}
