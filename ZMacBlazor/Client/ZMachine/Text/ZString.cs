using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMacBlazor.Client.ZMachine.Text
{
    public class ZString
    {
        private readonly StringBuilder builder;

        public ZString()
        {
            builder = new StringBuilder();
        }

        public void Append(char letter)
        {
            builder.Append(letter);
        }

        public void Append(string abbreviation)
        {
            builder.Append(abbreviation);
        }

        public override string ToString()
        {
            return builder.ToString();
        }
    }
}
