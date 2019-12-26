using System.Collections.Generic;
using System.Linq;

namespace Blazork.ZMachine.Text
{
    public class TextBuffer
    {
        private readonly MemoryLocation memory;
        
        public int MaxLength { get; }
        public string Text { get; protected set; }
        public Dictionary<int, string> Tokens { get; set; }

        public TextBuffer(MemoryLocation memory)
        {
            this.memory = memory;
            MaxLength = memory.Bytes.Span[0] - 1;
            Text = "";
            Tokens = new Dictionary<int, string>();
        }

        public void Write(string input)
        {
            input = input.TrimEnd('\n');
            input = input.Substring(0, MaxLength - 1);
            input = input.ToLower();

            var i = 0;
            while(i < input.Length)
            {
                memory.Bytes.Span[i + 1] = (byte)input[i];
                i += 1;
            }
            memory.Bytes.Span[i + 1] = 0;
            Text = input;
        }

        public void Tokenize(ParseDictionary dictionary)
        {
            var space = new[] { ' ' };
            var allSeparators = dictionary.Separators.Union(space).ToArray();

            var startToken = 0;
            for (var i = 0; i < Text.Length; i++)
            {
                if (allSeparators.Contains(Text[i]))
                {
                    Tokens.Add(startToken, Text.Substring(startToken, i - startToken));    

                    if (Text[i] != ' ')
                    {
                        Tokens.Add(i, Text.Substring(i, 1));
                    }
                }
                startToken = i;
            }
            if (startToken < Text.Length)
            {
                Tokens.Add(startToken, Text.Substring(startToken));
            }
        }
    }
}
