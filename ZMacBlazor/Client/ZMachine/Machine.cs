using Serilog;
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
            StackFrames = new FrameCollection(logger);
            ObjectTable = new GameObjectTable(this);
            Decoder = new InstructionDecoder(this);
            Output = new CompositeOutputStream(new DebugOutputStream(logger));
            Logger = logger.ForContext<Machine>();
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
            var i = 4000;
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
                Logger.Information($"\tSetWordVariable varNum:{variableNumber} value:{value} localsCount:{StackFrames.Locals.Length}");
                StackFrames.Locals[variableNumber - 1] = value;
            }
            else if (variableNumber <= 255)
            {
                var msAddress = (variableNumber - 16) * 2;
                var lsAddress = msAddress + 1;

                var (msb, lsb) = Bits.BreakWord(value);

                var address = Bits.MakeWord(Memory.Globals) + ((variableNumber - 16) * 2);
                var place = Memory.SpanAt(address, 2);
                place.Bytes[0] = msb;
                place.Bytes[1] = lsb;
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
                var address = Bits.MakeWord(Memory.Globals) + ((variableNumber - 16) * 2);
                var value = Memory.SpanAt(address, 2);
                return Bits.MakeWord(value.Bytes);
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

        public SpanLocation GetAbbreviation(int index, int number)
        {
            var offset = (64 * (index - 1)) + number * 2;
            var ppAbbreviation = Memory.WordAt(Header.ABBREVIATIONS);
            var pAbbreviation = Memory.WordAddressAt(ppAbbreviation + offset);
            var location = Memory.SpanAt(pAbbreviation);
            return location;
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
