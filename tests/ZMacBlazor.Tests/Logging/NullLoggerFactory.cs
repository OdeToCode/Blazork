using Serilog;

namespace Blazork.Tests.Logging
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