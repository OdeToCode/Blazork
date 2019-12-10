using System;

namespace ZMacBlazor.Client.ZMachine.Objects
{
    /*
     * 
     * In V 1 to 3
     * 
     * 4 bytes of attributes
     * 1 byte parent
     * 1 byte sibling
     * 1 byte child
     * 2 bytes properties pointer
     * -- 9 bytes total
     * 
     * In v4+
     * 
     * 6 bytes of attributes
     * 2 byte parent
     * 2 byte sibling
     * 2 byte child
     * 2 byte prop pointer
     * -- 14 bytes total
     */

    public class GameObject
    {
        public GameObject(int startAddress, Machine machine)
        {
            if (machine == null) throw new ArgumentNullException(nameof(machine));

            this.machine = machine;
            this.startAddress = startAddress;
            this.entrySize = 9;

            if(this.machine.Version > 3)
            {
                this.entrySize = 14; 
            }

            var entry = machine.Memory.SpanAt(startAddress, entrySize);
            if(this.machine.Version > 3)
            {
                Parent = Bits.MakeWord(entry.Slice(6, 2));
                Sibling = Bits.MakeWord(entry.Slice(8, 2));
                Child = Bits.MakeWord(entry.Slice(10, 2));
                PropertyPointer = Bits.MakeWord(entry.Slice(12, 2));
            }
            else
            {
                Parent = entry[4];
                Sibling = entry[5];
                Child = entry[6];
                PropertyPointer = Bits.MakeWord(entry.Slice(7,2));
            }
            this.PropertyTable = new GameObjectPropertyTable(this.machine, PropertyPointer);
        }

        public void SetAttribute(int number, bool value)
        {
            var byteToSet = Attributes[number / 8];
            var mask = 0x80 >> (number % 8);
            var result = (byte)(byteToSet | mask);
            
            Attributes[number / 8] = result;
        }

        public bool ReadAttribute(int number)
        {
            var byteToTest = Attributes[number / 8];
            var mask = 0x80 >> (number % 8);

            return (byteToTest & mask) > 0;
        }

        public Span<byte> Attributes
        {
            get
            {
                var attributeSize = machine.Version > 3 ? 6 : 4;
                return machine.Memory.SpanAt(startAddress, attributeSize);
            }
        }

        public int Parent { get; }
        public int Sibling { get; }
        public int Child { get; }
        public int PropertyPointer { get; }
        public GameObjectPropertyTable PropertyTable { get; }

        private readonly Machine machine;
        private readonly int startAddress;
        private readonly int entrySize;
    }
}
