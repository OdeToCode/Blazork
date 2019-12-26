using Serilog;
using Serilog.Events;
using Blazork.ZMachine;
using Blazork.ZMachine.Instructions;
using Blazork.ZMachine.Streams;
using Blazork.ZMachine.Text;

namespace Blazork.Tests.Logging
{
    public class TestLoggerFactory
    {
        public static ILogger GetLogger()
        {
            var logger = new LoggerConfiguration()
                                .MinimumLevel.Warning()
                                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                               // .MinimumLevel.Override(typeof(FrameCollection).FullName, LogEventLevel.Verbose)
                                .MinimumLevel.Override(typeof(Machine).FullName, LogEventLevel.Debug)
                               // .MinimumLevel.Override(typeof(DebugOutputStream).FullName, LogEventLevel.Verbose)
                                .MinimumLevel.Override(typeof(Instruction).FullName, LogEventLevel.Debug)
                                //.MinimumLevel.Override(typeof(ZStringDecoder).FullName, LogEventLevel.Verbose)
                                .Enrich.FromLogContext()
                                .WriteTo.File(@"..\..\..\..\..\tests.log",
                                             // outputTemplate: "\n{SourceContext:lj}\n{Message:lj}{NewLine}{Exception}")
                                             outputTemplate: "\n{Message:lj}{NewLine}{Exception}")
                                .CreateLogger();
            return logger;
        }
    }
}