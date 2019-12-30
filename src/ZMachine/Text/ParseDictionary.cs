using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazork.ZMachine.Text
{
    public class ParseDictionary
    {
        private readonly char[] space = new char[] { ' ' };

        public ParseDictionary(Machine machine)
        {
            var dictionaryAddress = machine.Memory.DictionaryAddress;
            var dictionaryMemory = machine.Memory.MemoryAt(dictionaryAddress);
            var separatorLength = dictionaryMemory.Bytes.Span[0];
            var entryLength = dictionaryMemory.Bytes.Span[1 + separatorLength];
            var dictionaryLength = Bits.MakeWord(dictionaryMemory.Bytes.Span.Slice(2 + separatorLength));
            var entryOffset = separatorLength + 4;
            
            Separators = new List<char>();
            Words = new Dictionary<string, DictionaryEntry>();

            for (var i = 0; i < separatorLength; i++)
            {
                Separators.Add((char)dictionaryMemory.Bytes.Span[i + 1]);   
            }

            var decoder = new ZStringDecoder(machine);
            for (var i = 0; i < dictionaryLength * entryLength; i += entryLength)
            {
                var entry = new DictionaryEntry();
                entry.Address = i + dictionaryAddress + entryOffset;
                entry.Word = decoder.Decode(machine.Memory.SpanAt(entry.Address)).Text;
                Words.Add(entry.Word, entry);
            }
        }

        public List<char> Separators { get; }
        public Dictionary<string, DictionaryEntry> Words { get; }
    }
}
