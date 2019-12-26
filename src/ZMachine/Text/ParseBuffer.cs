using System;

namespace Blazork.ZMachine.Text
{
    public class ParseBuffer
    {
        private MemoryLocation memory;

        public int MaxLength { get; }

        public ParseBuffer(MemoryLocation memory)
        {
            this.memory = memory;
            MaxLength = memory.Bytes.Span[0] - 1;

            if (MaxLength <= 6)
            {
                throw new InvalidOperationException("Parse buffer size indicates a bug");
            }
        }

        public void Populate(TextBuffer textInput, ParseDictionary dictionary)
        {
            var count = 0;
            var index = 1;
            foreach(var token in textInput.Tokens)
            {
                var dictionaryAddress = 0;
                var searchTerm = token.Value;
                if(searchTerm.Length > 6)
                {
                    searchTerm = searchTerm.Remove(6);
                }
                if(dictionary.Words.ContainsKey(searchTerm))
                {
                    dictionaryAddress = dictionary.Words[searchTerm].Address;
                }

                var (msb, lsb) = Bits.BreakWord(dictionaryAddress);
                memory.Bytes.Span[index] = msb;
                memory.Bytes.Span[index + 1] = lsb;
                memory.Bytes.Span[index + 2] = (byte)searchTerm.Length;
                memory.Bytes.Span[index + 3] = (byte)token.Key;

                count += 1;
                index += 4; 
                if(index >= MaxLength)
                {
                    break;
                }
            }

            memory.Bytes.Span[0] = (byte)count;
        }
    }
}
