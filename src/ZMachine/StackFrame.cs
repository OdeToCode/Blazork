using System;
using System.Collections.Generic;

namespace Blazork.ZMachine
{
    public class StackFrame
    {
        public StackFrame(int returnPC, int localsCount, int storeVariable)
        {
            locals = new int[localsCount];
            ReturnPC = returnPC;
            RoutineStack = new Stack<int>();
            StoreVariable = storeVariable;
        }

        public override string ToString()
        {
            return $"ReturnPC:{ReturnPC:X} LocalsCount:{Locals.Length}";
        }

        int[] locals;
        public int ReturnPC { get; }
        public Span<int> Locals => locals.AsSpan();
        public Stack<int> RoutineStack { get; }
        public int StoreVariable { get; }
    }
}
