using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace ZMacBlazor.Tests.Logging
{
    public class TestLogger : ILogger, IDisposable
    {
        private StreamWriter log;

        public TestLogger(string name)
        {
            log = new StreamWriter(File.OpenWrite(name));
            log.WriteLine();
            log.WriteLine($"**** {DateTime.Now.ToLongTimeString()} ****");
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public void Dispose()
        {
            log.Close();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            log.WriteLine(formatter(state, exception));
        }
    }
}