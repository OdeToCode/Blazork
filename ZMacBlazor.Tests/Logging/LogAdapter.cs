using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace ZMacBlazor.Tests.Logging
{
    public class LogAdapter : ILogger
    {
        private readonly ITestOutputHelper testOutput;

        public LogAdapter(ITestOutputHelper testOutput)
        {
            this.testOutput = testOutput;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            testOutput.WriteLine(formatter(state, exception));
        }
    }
}
