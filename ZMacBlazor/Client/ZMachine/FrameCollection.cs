using System;
using System.Collections.Generic;

namespace ZMacBlazor.Client.ZMachine
{
    public class FrameCollection 
    {
        public FrameCollection(Machine machine)
        {
            this.machine = machine;
        }

        public StackFrame PopFrame()
        {
            machine.Logger.Information($"Pop frame to {innerStack.Peek().ReturnPC:X} Size:{innerStack.Count}");
            return innerStack.Pop();
        }

        public void PushFrame(StackFrame newFrame)
        {
            if (newFrame == null) throw new ArgumentNullException(nameof(newFrame));

            machine.Logger.Information($"Push frame {newFrame.ToString()} Size:{innerStack.Count} ");
            innerStack.Push(newFrame);
        }

        public void Initialize()
        {
            var startingStackFrame = new StackFrame(0, 0, -1);
            PushFrame(startingStackFrame);
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
        private readonly Machine machine;
    }
}
