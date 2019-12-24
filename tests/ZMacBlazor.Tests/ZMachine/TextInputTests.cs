using Blazork.ZMachine;
using Blazork.ZMachine.Text;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Blazork.Tests.ZMachine
{
    public class TextInputTests
    {
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
