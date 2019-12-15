using Serilog;
using Serilog.Events;

namespace ZMacBlazor.Tests.Logging
{
    public class TestLoggerFactory
    {
        public static ILogger GetLogger()
        {
            var logger = new LoggerConfiguration()
                                .MinimumLevel.Verbose()
                                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                                .MinimumLevel.Override("ZMacBlazor.Client.ZMachine.FrameCollection", LogEventLevel.Warning)
                                .MinimumLevel.Override("ZMacBlazor.Client.ZMachine.Machine", LogEventLevel.Warning)
                                .MinimumLevel.Override("ZMacBlazor.Client.ZMachine.Streams.DebugOutputStream", LogEventLevel.Warning)
                                .Enrich.FromLogContext()
                                .WriteTo.File(@"..\..\..\..\tests.log",
                                              outputTemplate: "{SourceContext:lj}\n{Message:lj}{NewLine}{Exception}")
                                .CreateLogger();
            return logger;
        }
    }
}