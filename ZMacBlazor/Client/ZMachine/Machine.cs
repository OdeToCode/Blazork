using System.IO;

namespace ZMacBlazor.Client.ZMachine
{
    public class Machine
    {
        public void Load(Stream memoryBytes)
        {
            Memory = new MachineMemory();
            Memory.Load(memoryBytes);

            PC = Memory.StartingProgramCounter;
        }

        public void Execute()
        {
                
        }

        public ushort PC { get; protected set; }
        public MachineMemory Memory { get; protected set; }  
    }
}
