using Blazork.ZMachine.Text;

namespace Blazork.ZMachine.Instructions
{
    public class TextResolver
    {
        private Machine machine;

        public TextResolver(Machine machine)
        {
            this.machine = machine;
        }

        public DecodedString Decode(SpanLocation location)
        {
            var decoder = new ZStringDecoder(machine);
            var result = decoder.Decode(location);
            return result;
        }
    }
}
