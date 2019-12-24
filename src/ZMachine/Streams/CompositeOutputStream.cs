using System.Collections.Generic;

namespace Blazork.ZMachine.Streams
{
    public class CompositeOutputStream : IOutputStream
    {
        public CompositeOutputStream(params IOutputStream[] streams)
        {
            Streams = new List<IOutputStream>(streams);
        }

        public void Write(string text)
        {
            foreach(var stream in Streams)
            {
                stream.Write(text);
            }
        }

        public List<IOutputStream> Streams { get; }
    }
}
