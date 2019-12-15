using Serilog;
using Serilog.Events;

namespace ZMacBlazor.Tests.Logging
{
    public class TestLoggerFactory
    {
        public static ILogger GetLogger()
        {
            var logger = new LoggerConfiguration()
                                .MinimumLevel.Debug()
                                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                                .Enrich.FromLogContext()
                                .WriteTo.Console()
                                .WriteTo.File(@"..\..\..\..\zmacblazor.tests.log")
                                .CreateLogger();
            return logger;
        }
    }
}