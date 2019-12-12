﻿using Microsoft.Extensions.Logging;
using System;
using System.IO;
using ZMacBlazor.Client.ZMachine.Instructions;
using ZMacBlazor.Client.ZMachine.Objects;
using ZMacBlazor.Client.ZMachine.Streams;

namespace ZMacBlazor.Client.ZMachine
{
    public class Machine
    {
        public Machine(ILogger logger)
        {
            Memory = new MachineMemory(Stream.Null);
            StackFrames = new FrameCollection(this);
            ObjectTable = new GameObjectTable(this);
            Decoder = new InstructionDecoder(this);
            Output = new CompositeOutputStream(new DebugOutputStream(logger));
            Logger = logger;
        }

        public void Load(Stream memoryBytes)
        {
            Memory = new MachineMemory(memoryBytes);
            PC = Memory.StartingProgramCounter;

            ObjectTable.Initialize();
            StackFrames.Initialize();
        }

        public void Execute()
        {   
            var i = 2000;
            while(--i > 0)
            {
                var memory = Memory.SpanAt(PC);
                var instruction = Decoder.Decode(memory);
                instruction.Execute(memory);
            }
        }

        public void SetVariable(int variableNumber, int value)
        {
            if (variableNumber == 0)
            {
                StackFrames.RoutineStack.Push(value);
            }
            else if (variableNumber <= 15)
            {
                Logger.Log(LogLevel.Information, $"SetWordVariable varNum:{variableNumber} value:{value} localsCount:{StackFrames.Locals.Length}");
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
        }

        public byte Version => Memory.Version;
        public int PC { get; protected set; }
        public GameObjectTable ObjectTable { get; protected set; }
        public InstructionDecoder Decoder { get; protected set; }
        public MachineMemory Memory { get; protected set; }
        public FrameCollection StackFrames { get; protected set; }
        public CompositeOutputStream Output { get; }
        public ILogger Logger { get; }
    }
}
