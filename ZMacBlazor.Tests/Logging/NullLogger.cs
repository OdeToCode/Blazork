using Microsoft.Extensions.Logging;
using System;

namespace ZMacBlazor.Tests.Logging
{
    public class NullLogger : ILogger, IDisposable
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            
        }
    }
}