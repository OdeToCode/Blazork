using System.IO;

namespace ZMacBlazor.Client.ZMachine
{
    public class Machine
    {
        public void Load(Stream memoryBytes)
        {
            memory = new MachineMemory();
            memory.Load(memoryBytes);

            programCounter = memory.StartingProgramCounter;
        }

        public void Execute()
        {
            
        }

        ushort programCounter;
        MachineMemory memory;
        MachineStack stack;
    }
}
