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
            Assert.False(object1.ReadAttribute(0));


            var object227 = machine.ObjectTable.GameObjects[226];
            Assert.Equal(0x205F, object227.PropertyPointer);
            Assert.True(object227.ReadAttribute(11));
            Assert.True(object227.ReadAttribute(12));
            Assert.False(object227.ReadAttribute(14));
            object227.SetAttribute(14, true);
            Assert.True(object227.ReadAttribute(14));

            var object250 = machine.ObjectTable.GameObjects[249];
            Assert.Equal(249, object250.Parent);
            Assert.Equal(73, object250.Sibling);
            Assert.Equal(0, object250.Child);
            Assert.Equal(0x2263, object250.PropertyPointer);
        }
    }
}
