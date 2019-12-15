using Serilog;
using System.Diagnostics;

namespace ZMacBlazor.Client.ZMachine.Streams
{
    public class DebugOutputStream : IOutputStream
    {
        private readonly ILogger logger;

        public DebugOutputStream(ILogger logger)
        {
            this.logger = logger;
        }

        public void Write(string text)
        {
            logger.Information(text);
            Debug.Write(text);
        }
    }
}
