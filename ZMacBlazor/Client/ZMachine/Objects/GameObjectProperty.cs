using System;

namespace ZMacBlazor.Client.ZMachine.Objects
{
    //    A property list consists of this object’s name – which is a Z-string preceded by an unsigned
    //byte containing its length in words – a number of property entries, and a zero byte that terminates
    //the list.

    // A property entry contains one (one or two) size byte(s), followed by 1 to 8 (63) bytes of
    //property data; 

    // the (first) size byte holds the number of data bytes minus 1 in the top 3 (2) bits, and
    //the property number in the bottom 5 (6) bits. 

    // (In V4+ there is one exception: if the top bit of the
    //first size byte is %1, i.e., the number of bytes would be either 3 or 4, there are two size bytes. 

    // The second size byte contains the number of data bytes as its bottom 6 bits, and its top bit is %1; the
    //number of data bytes may not be 0. The use of bit 6 of a second size byte is unknown; Inform
    //always sets it to %0.) The entries in a property list must be sorted in order of descending property
    //number; it is an error for a property number to occur more than once in a property list. It is also
    //an error for property number 0 to occur.

    public class GameObjectProperty
    {
        public GameObjectProperty(Machine machine, MemoryLocation memory)
        {
            if (machine == null) throw new ArgumentNullException(nameof(machine));

            this.machine = machine;
            if(machine.Version > 3 && Bits.SevenSet(memory.Bytes.Span[0]))
            {
                throw new NotImplementedException("Not sure how to process 2 size bytes yet");
            }
            else
            {
                var sizeByte = memory.Bytes.Span[0];
                Size = Bits.TopThree(sizeByte) + 1;
                Number = Bits.BottomFive(sizeByte);
                Value = machine.Memory.MemoryAt(memory.Address + 1, Size).Bytes;
            }
        }

        private readonly Machine machine;
        public int Number { get; }
        public Memory<byte> Value { get; set; }
        public int Size { get; set; }
    }
}
