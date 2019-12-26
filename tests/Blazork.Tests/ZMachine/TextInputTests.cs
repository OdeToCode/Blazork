using Blazork.Tests.Logging;
using Blazork.ZMachine;
using Blazork.ZMachine.Text;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Blazork.Tests.ZMachine
{
    public class TextInputTests
    {
        [Fact]
        public void CanTokensize()
        {
            using var file = File.OpenRead(@"Data\ZORK1.DAT");
            var logger = NullLoggerFactory.GetLogger();
            var machine = new Machine(logger);
            machine.Load(file);

            var textBytes = new byte[37];
            textBytes[0] = (byte)textBytes.Length;

            var textMemory = new MemoryLocation(1, textBytes.AsMemory());
            var textBuffer = new TextBuffer(textMemory);
            textBuffer.Write("fred,go fishing");

            var dictionary = new ParseDictionary(machine);
            textBuffer.Tokenize(dictionary);

            Assert.Equal(4, textBuffer.Tokens.Count);
            Assert.Equal("fred", textBuffer.Tokens[0]);
            Assert.Equal(",", textBuffer.Tokens[4]);
            Assert.Equal("go", textBuffer.Tokens[5]);
            Assert.Equal("fishing", textBuffer.Tokens[8]);
        }

        [Fact]
        public void CanDecodeDictionary()
        {
            using var file = File.OpenRead(@"Data\ZORK1.DAT");
            var logger = NullLoggerFactory.GetLogger();
            var machine = new Machine(logger);
            machine.Load(file);

            var dictionary = new ParseDictionary(machine);

            Assert.Equal(0x2B9, dictionary.Words.Count);
            Assert.True(dictionary.Words.ContainsKey("altar"));
            Assert.False(dictionary.Words.ContainsKey("ackack"));
        }

        [Fact]
        public void CanWriteToTextBuffer()
        {
            var buffer = new byte[37];
            buffer[0] = (byte)buffer.Length;

            var memory = new MemoryLocation(1, buffer.AsMemory());
            var textBuffer = new TextBuffer(memory);

            textBuffer.Write("This is a long string with extra words.");

            Assert.Equal(buffer.Length, buffer[0]);
            Assert.Equal(36, textBuffer.MaxLength);
            Assert.Equal((byte)'t', buffer[1]);
            Assert.Equal((byte)'o', buffer[35]);
            Assert.Equal(0, buffer[36]);
        }
    }
}
