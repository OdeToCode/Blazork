using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using ZMacBlazor.Client.ZMachine;

namespace ZMacBlazor.Tests.ZMachine
{
    public class MachineStackTests
    {
        [Fact]
        public void CanPushAndPop()
        {
            var s = new FrameCollection();
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
