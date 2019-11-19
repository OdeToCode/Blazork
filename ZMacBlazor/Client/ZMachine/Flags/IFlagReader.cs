using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZMacBlazor.Client.ZMachine.Flags
{
    public class IFlagSetOne
    {
        public StatusLine StatusLine { get; }
        public bool StorySplit { get; }
        public bool StatusLineNotAvailable { get; set; }
        public bool ScreenSplitAvailable { get; set; }
        public bool VariablePitchDefault { get; set; }
        public bool ColorsAvailable { get; set; }
        public bool PicturesAvailable { get; set; }
        public bool BoldAvailable { get; set; }
        public bool ItalicAvailable { get; set; }
        public bool FixedSpaceAvailable { get; set; }
        public bool SoundEffectsAvailable { get; set; }
        public bool TimeKeyboardAvailable { get; set; }
    }
}
