using Serilog;

namespace ZMacBlazor.Tests.Logging
{
    public class NullLoggerFactory 
    {
        public static ILogger GetLogger()
        {
            var logger = new LoggerConfiguration()
                                .CreateLogger();
            return logger;
        }
    }
}