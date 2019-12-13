using System;
using System.Collections.Generic;

namespace ZMacBlazor.Client.ZMachine.Objects
{

    // The table begins with a block known as the property defaults table.
    // This contains 31 words in Versions 1 to 3 and 63 in Versions 4 and later. 
    // When the game attempts to read the value of property n for an object which does not provide 
    // property n, the n-th entry in this table is the resulting value.
    public class GameObjectTable
    {
        private int[] defaults = Array.Empty<int>();
        private Machine machine;

        public GameObjectTable(Machine machine)
        {
            this.machine = machine;
            this.GameObjects = new List<GameObject>();
        }

        public void Initialize()
        {
            ReadDefaultValues();
            ReadObjects();
        }

        public int GetDefault(int index)
        {
            return Defaults[index];
        }

        public void AddObject(GameObject gameObject)
        {
            GameObjects.Add(gameObject);
        }

        public GameObject GetObject(int number)
        {
            return GameObjects[number - 1];
        }

        public void InsertObject(int target, int destination)
        {
            RemoveFromParent(target);
            AddToParent(target, destination);
        }

        public void AddToParent(int childNumber, int parentNumber)
        {
            var child = GameObjects[childNumber - 1];
            var parent = GameObjects[parentNumber - 1];

            var existingChild = parent.Child;
            parent.Child = childNumber;
            child.Sibling = existingChild;
            child.Parent = parentNumber;
        }

        public void RemoveFromParent(int targetNumber)
        {
            if (targetNumber < 1 || targetNumber > GameObjects.Count - 1)
            {
                throw new ArgumentException($"Target is {targetNumber} but must be in the range 1 to {GameObjects.Count}", 
                                            nameof(targetNumber));
            }
            
            var target = GameObjects[targetNumber - 1];
            if (target.Parent != 0)
            {
                var parent = GameObjects[target.Parent - 1];
                target.Parent = 0;
                
                if (parent.Child == targetNumber)
                {
                    parent.Child = target.Sibling;
                }
                else
                {
                    var index = parent.Child;
                    while (index != 0)
                    {
                        var node = GameObjects[index - 1];
                        if (node.Sibling == targetNumber)
                        {
                            node.Sibling = target.Sibling;
                            break;
                        }
                    }
                }
            }
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
            var defaultValues = machine.Memory.SpanAt(startLocation, size);
            for (var i = 0; i < size; i++)
            {
                defaults[i] = defaultValues.Bytes[i];
            }
        }

        protected List<GameObject> GameObjects { get; set; }
        protected ReadOnlySpan<int> Defaults => defaults.AsSpan();
    }
}
