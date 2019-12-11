using System;
using System.IO;
using Xunit;
using ZMacBlazor.Client.ZMachine;
using ZMacBlazor.Tests.Logging;

namespace ZMacBlazor.Tests.ZMachine
{
    public class ObjectTableTests
    {
        [Fact]
        public void CanLoadObjectTable()
        {
            using var file = File.OpenRead(@"Data\ZORK1.DAT");
            using var logger = new NullLogger();
            var machine = new Machine(logger);
            machine.Load(file);

            Assert.Equal(31, machine.ObjectTable.Defaults.Length);
            Assert.Equal(0, machine.ObjectTable.Defaults[10]);
            Assert.Equal(0, machine.ObjectTable.Defaults[30]);
            Assert.Equal(0, machine.ObjectTable.Defaults[0]);
            Assert.Equal(250, machine.ObjectTable.GameObjects.Count);

            var object1 = machine.ObjectTable.GameObjects[0];
            Assert.Equal(0xBB8, object1.PropertyPointer);
            Assert.Equal(247, object1.Parent);
            Assert.Equal(2, object1.Sibling);
            Assert.Equal(0, object1.Child);
            Assert.True(object1.ReadAttribute(14));
            Assert.True(object1.ReadAttribute(28));
            object1.SetAttribute(28, false);
            Assert.False(object1.ReadAttribute(28));
            Assert.False(object1.ReadAttribute(0));
            Assert.Equal("pair of hands", object1.Description);
            Assert.False(object1.Properties.ContainsKey(17));
            Assert.True(MemoryEqual(new byte[] { 0x46, 0xDC, 0x42, 0xC2, 0x42, 0xB4 }, object1.Properties[18].Value));
            Assert.True(MemoryEqual(new byte[] { 0x82 }, object1.Properties[16].Value));

            var object227 = machine.ObjectTable.GameObjects[226];
            Assert.Equal(0x205F, object227.PropertyPointer);
            Assert.True(object227.ReadAttribute(11));
            Assert.True(object227.ReadAttribute(12));
            Assert.False(object227.ReadAttribute(14));
            object227.SetAttribute(14, true);
            Assert.True(object227.ReadAttribute(14));
            Assert.Equal("basket", object227.PropertyTable.Description);

            var object250 = machine.ObjectTable.GameObjects[249];
            Assert.Equal(249, object250.Parent);
            Assert.Equal(73, object250.Sibling);
            Assert.Equal(0, object250.Child);
            Assert.Equal(0x2263, object250.PropertyPointer);
            Assert.Equal("board", object250.PropertyTable.Description);
            Assert.True(MemoryEqual(new byte[] { 0x3C, 0xDA, 0x3C, 0xCC }, object250.Properties[18].Value));
        }

        private bool MemoryEqual(byte[] value1, Memory<byte> value2)
        {
            if (value1.Length != value2.Length) return false;

            var value2Span = value2.Span;
            for(var i = 0; i < value2Span.Length; i++)
            {
                if(value1[i] != value2Span[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
