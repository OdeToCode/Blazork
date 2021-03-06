﻿using Serilog;
using System;
using System.Collections.Generic;

namespace Blazork.ZMachine
{
    public class FrameCollection 
    {
        public FrameCollection(ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            this.log = logger.ForContext<FrameCollection>();
        }

        public StackFrame PopFrame()
        {
            log.Debug($"\tPop frame to {innerStack.Peek().ReturnPC:X} Size:{innerStack.Count}");
            return innerStack.Pop();
        }

        public void PushFrame(StackFrame newFrame)
        {
            if (newFrame == null) throw new ArgumentNullException(nameof(newFrame));

            log.Debug($"\tPush frame {newFrame.ToString()} Size:{innerStack.Count} ");
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
        private readonly ILogger log;
    }
}
