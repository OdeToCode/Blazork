using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazork.ZMachine
{
    public static class Header
    {
        public static readonly int VERSION = 0x0;
        public static readonly int FLAGSONE = 0x1;
        public static readonly int HIGHMEMORY = 0x4;
        public static readonly int PC = 0x6;
        public static readonly int DICTIONARY = 0x8;
        public static readonly int OBJECTTABLE = 0xA;
        public static readonly int GLOBALS = 0xC;
        public static readonly int STATICMEMORY = 0xE;
        public static readonly int FLAGSTWO = 0x10;
        public static readonly int ABBREVIATIONS = 0x18;
        public static readonly int FILELENGTH = 0x1A;
        public static readonly int CHECKSUM = 0x1C;
        public static readonly int INTERPRETERNUMBER = 0x1E;
        public static readonly int INTERPRETERVERSION = 0x1F;
        public static readonly int SCREENHEIGHT = 0x20;
        public static readonly int SCREENWIDTH = 0x21;
        public static readonly int SCREENWIDTHUNITS = 0x22;
        public static readonly int SCREENHEIGHTUNITS = 0x24;
        public static readonly int FONTWIDTHUNITSV5 = 0x26;
        public static readonly int FONTHEIGHTUNITSV6 = 0x26;
        public static readonly int FONTHEIGHTUNITSV5 = 0x27;
        public static readonly int FONTWIDTHUNITSV6 = 0x27;
        public static readonly int ROUTINESOFFSET = 0x28;
        public static readonly int STATICSTRINGSOFFSET = 0x2A;
        public static readonly int DEFAULTBACKGROUND = 0x2A;
        public static readonly int DEFAULTFOREGROUND = 0x2D;
        public static readonly int TERMINATINGCHARACTERS = 0x2E;
        public static readonly int WIDTHOFPIXELSSTREAMTHREE = 0x30;
        public static readonly int REVISION = 0x32;
        public static readonly int ALPHABETTABLE = 0x34;
        public static readonly int HEADEREXTENSION = 0x36;
    }
}
