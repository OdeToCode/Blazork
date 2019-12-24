namespace Blazork.ZMachine.Text
{
    public class TextBuffer
    {
        private readonly MemoryLocation memory;
        
        public int MaxLength { get; }

        public TextBuffer(MemoryLocation memory)
        {
            this.memory = memory;
            MaxLength = memory.Bytes.Span[0] - 1;
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
        }
    }
}
