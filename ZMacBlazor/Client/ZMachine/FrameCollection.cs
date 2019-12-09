using System;
using System.Collections.Generic;

namespace ZMacBlazor.Client.ZMachine
{
    public class FrameCollection 
    {
        public StackFrame PopFrame()
        {
            return innerStack.Pop();
        }

        public void PushFrame(StackFrame newFrame)
        {
            innerStack.Push(newFrame);
        }

        public Span<int> Locals
        {
            get
            {
                return innerStack.Peek().Locals;
            }
        }

        public Stack<int> RoutineStack
        {
            get { return innerStack.Peek().RoutineStack; }
        }

        Stack<StackFrame> innerStack = new Stack<StackFrame>();
    }
}
