namespace ZMacBlazor.Client.ZMachine.Instructions
{
    // Instructions which test a condition are called "branch" instructions.
    //The branch information is stored in one or two bytes, 
    //indicating what to do with the result of the test.
    //If bit 7 of the first byte is 0, a branch occurs when the condition was false; 
    //if 1, then branch is on true.If bit 6 is set, then the branch occupies 
    //1 byte only, and the "offset" is in the range 0 to 63, 
    //given in the bottom 6 bits.If bit 6 is clear, then the offset is a 
    //signed 14 - bit number given in bits 0 to 5 of the first byte followed 
    //by all 8 of the second.
    //An offset of 0 means "return false from the current routine", and 1 means 
    //"return true from the current routine".

    // Otherwise, a branch moves execution to the instruction at address
    //Address after branch data + Offset - 2.

    public class BranchResolver
    {
        public BranchDescriptor ResolveBranch(SpanLocation memory)
        {
            var branchOnTrue = Bits.SevenSet(memory.Bytes[0]);
            var oneByteOffset = Bits.SixSet(memory.Bytes[0]);

            var offset = 0;
            if(oneByteOffset)
            {                
                offset = Bits.BottomSix(memory.Bytes[0]);
            }
            else
            {
                offset = Bits.MakeWordFromBottomFourteen(memory.Bytes);
            }

            return new BranchDescriptor(branchOnTrue, offset, oneByteOffset ? 1 : 2);
        }
    }
}
