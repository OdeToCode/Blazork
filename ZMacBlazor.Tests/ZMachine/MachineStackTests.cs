using Xunit;
using ZMacBlazor.Client.ZMachine;
using ZMacBlazor.Tests.Logging;

namespace ZMacBlazor.Tests.ZMachine
{
    public class MachineStackTests
    {
        [Fact]
        public void CanPushAndPop()
        {
            var m = new Machine(NullLoggerFactory.GetLogger());
            var s = new FrameCollection(m);
            s.PushFrame(new StackFrame(0x55FF, 2, 1));
            s.Locals[0] = 22;
            s.RoutineStack.Push(42);

            var frame = s.PopFrame();
            Assert.Equal(0x55FF, frame.ReturnPC);
            Assert.Equal(22, frame.Locals[0]);
            Assert.Equal(42, frame.RoutineStack.Pop());
        }
    }
}
