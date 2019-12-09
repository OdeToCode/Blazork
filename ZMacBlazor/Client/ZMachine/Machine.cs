using Microsoft.Extensions.Logging;
using System;
using System.IO;
using ZMacBlazor.Client.ZMachine.Instructions;

namespace ZMacBlazor.Client.ZMachine
{
    public class Machine
    {
        public Machine(ILogger logger)
        {
            Memory = new MachineMemory(Stream.Null);
            StackFrames = new FrameCollection();
            Logger = logger;
        }

        public void Load(Stream memoryBytes)
        {
            Memory = new MachineMemory(memoryBytes);
            PC = Memory.StartingProgramCounter;
        }

        public void Execute()
        {
            var decoder = new InstructionDecoder(this);

            var i = 100;
            while(--i > 0)
            {
                var memory = Memory.LocationAt(PC);
                var instruction = decoder.Decode(memory);
                instruction.Execute(memory);
            }
        }

        public void SetWordVariable(int variableNumber, int value)
        {
            if (variableNumber == 0)
            {
                StackFrames.RoutineStack.Push(value);
            }
            else if (variableNumber <= 15)
            {
                StackFrames.Locals[variableNumber - 1] = value;
            }
            else if (variableNumber <= 255)
            {
                var msAddress = (variableNumber - 16) * 2;
                var lsAddress = msAddress + 1;

                var (msb, lsb) = Bits.BreakWord(value);

                Memory.Globals[msAddress] = msb;
                Memory.Globals[lsAddress] = lsb;
            }
            else
            {
                throw new InvalidOperationException($"Illegal variable number {variableNumber}");
            }
        }

        public int ReadVariable(int variableNumber)
        {
            // • 0: the top of the routine stack;
            // • 1 - 15: the local variable with that number;
            // • 16 - 255: the global variable with that number minus 16.
            //   globals are 0-239 (240 words starting $0C)

            if (variableNumber == 0)
            {
                return StackFrames.RoutineStack.Pop();
            }
            else if(variableNumber <= 15)
            {
                return StackFrames.Locals[variableNumber - 1];
            }
            else if(variableNumber <= 255)
            {
                var bytes = Memory.Globals.Slice((variableNumber - 16) * 2, 2);
                return Bits.MakeWord(bytes);
            }
            else
            {
                throw new InvalidOperationException($"Illegal variable number {variableNumber}");
            }
        }

        public void SetPC(int newValue)
        {
            PC = newValue;
            Logger.Log(LogLevel.Trace, $"Setting machine PC to {newValue:X}");
        }

        public byte Version => Memory.Version;
        public int PC { get; protected set; }
        public MachineMemory Memory { get; protected set; }
        public FrameCollection StackFrames { get; protected set; }
        public ILogger Logger { get; }
    }
}
