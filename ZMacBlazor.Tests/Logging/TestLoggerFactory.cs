using Serilog;
using Serilog.Events;
using ZMacBlazor.Client.ZMachine;
using ZMacBlazor.Client.ZMachine.Instructions;
using ZMacBlazor.Client.ZMachine.Streams;
using ZMacBlazor.Client.ZMachine.Text;

namespace ZMacBlazor.Tests.Logging
{
    public class TestLoggerFactory
    {
        public static ILogger GetLogger()
        {
            var logger = new LoggerConfiguration()
                                .MinimumLevel.Warning()
                                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                               // .MinimumLevel.Override(typeof(FrameCollection).FullName, LogEventLevel.Verbose)
                                //.MinimumLevel.Override(typeof(Machine).FullName, LogEventLevel.Debug)
                               // .MinimumLevel.Override(typeof(DebugOutputStream).FullName, LogEventLevel.Verbose)
                                .MinimumLevel.Override(typeof(Instruction).FullName, LogEventLevel.Debug)
                                //.MinimumLevel.Override(typeof(ZStringDecoder).FullName, LogEventLevel.Verbose)
                                .Enrich.FromLogContext()
                                .WriteTo.File(@"..\..\..\..\tests.log",
                                              outputTemplate: "\n{SourceContext:lj}\n{Message:lj}{NewLine}{Exception}")
                                .CreateLogger();
            return logger;
        }
    }
}