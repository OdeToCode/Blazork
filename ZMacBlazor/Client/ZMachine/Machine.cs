using Microsoft.Extensions.Logging;
using System.IO;

namespace ZMacBlazor.Client.ZMachine
{
    public class Machine
    {
        public void Load(Stream memoryBytes)
        {
            Memory = new MachineMemory(memoryBytes);
            PC = Memory.StartingProgramCounter;
        }

        public void Execute()
        {
                
        }

        public void SetPC(int newValue)
        {
            PC = newValue;
        }

        public byte Version => Memory.Version;
        public int PC { get; protected set; }
        public MachineMemory Memory { get; protected set; }

    }
}
