using System;
using System.Collections.Generic;

namespace ZMacBlazor.Client.ZMachine
{
    public class StackFrame
    {
        public StackFrame(int returnPC, int localsCount, int storeResult)
        {
            locals = new int[localsCount];
            ReturnPC = returnPC;
            RoutineStack = new Stack<int>();
            StoreResult = storeResult;

        }

        int[] locals;
        public int ReturnPC { get; }
        public Span<int> Locals => locals.AsSpan();
        public Stack<int> RoutineStack { get; }
        public int StoreResult { get; }
    }
}
