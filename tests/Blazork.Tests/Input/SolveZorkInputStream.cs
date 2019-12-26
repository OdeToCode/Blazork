using Blazork.ZMachine.Streams;

namespace Blazork.Tests.Input
{
    class SolveZorkInputStream : IInputStream
    {
        int index = 0;
        string[] commands =
        {
            "open mailbox",
            "quit"
        };

        public string Read()
        {
            return commands[index++];
        }
    }
}
