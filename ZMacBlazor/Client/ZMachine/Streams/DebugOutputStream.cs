using Serilog;
using System;
using System.Diagnostics;

namespace ZMacBlazor.Client.ZMachine.Streams
{
    public class DebugOutputStream : IOutputStream
    {
        private readonly ILogger logger;

        public DebugOutputStream(ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            this.logger = logger.ForContext<DebugOutputStream>();
        }

        public void Write(string text)
        {
            logger.Information(text);
        }
    }
}
