using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMacBlazor.Client.ZMachine.Text
{
    /*
      
     --first byte-------   --second byte---
       7    6 5 4 3 2  1 0   7 6 5  4 3 2 1 0
       bit  --first--  --second---  --third--
       
     */

    public class ZStringDecoder
    {
        private readonly Machine machine;

        static ZStringDecoder()
        {
            alphabetMap = new Dictionary<int, char>[3];
            alphabetMap[0] = new Dictionary<int, char>
            {
                { 0x06, 'a' }, { 0x07, 'b' }, { 0x08, 'c' }, { 0x09, 'd' }, { 0x0A, 'e' }, { 0x0B, 'f' }, 
                { 0x0C, 'g' }, { 0x0D, 'h' }, { 0x0E, 'i' }, { 0x0F, 'j' }, { 0x10, 'k' }, { 0x11, 'l' }, 
                { 0x12, 'm' }, { 0x13, 'n' }, { 0x14, 'o' }, { 0x15, 'p' }, { 0x16, 'q' }, { 0x17, 'r' }, 
                { 0x18, 's' }, { 0x19, 't' }, { 0x1A, 'u' }, { 0x1B, 'v' }, { 0x1C, 'w' }, { 0x1D, 'x' }, 
                { 0x1E, 'y' }, { 0x1F, 'z' }
            };

            alphabetMap[1] = new Dictionary<int, char>
            {
                { 0x06, 'A' }, { 0x07, 'B' }, { 0x08, 'C' }, { 0x09, 'D' }, { 0x0A, 'E' }, { 0x0B, 'F' },
                { 0x0C, 'G' }, { 0x0D, 'H' }, { 0x0E, 'I' }, { 0x0F, 'J' }, { 0x10, 'K' }, { 0x11, 'L' },
                { 0x12, 'M' }, { 0x13, 'N' }, { 0x14, 'O' }, { 0x15, 'P' }, { 0x16, 'Q' }, { 0x17, 'R' },
                { 0x18, 'S' }, { 0x19, 'T' }, { 0x1A, 'U' }, { 0x1B, 'V' }, { 0x1C, 'W' }, { 0x1D, 'X' },
                { 0x1E, 'Y' }, { 0x1F, 'Z' }
            };

            alphabetMap[2] = new Dictionary<int, char>
            {
                { 0x07, '\n'}, { 0x08, '0' }, { 0x09, '1' }, { 0x0A, '2' }, { 0x0B, '3' },
                { 0x0C, '4' }, { 0x0D, '5' }, { 0x0E, '6' }, { 0x0F, '7' }, { 0x10, '8' }, { 0x11, '9' },
                { 0x12, '.' }, { 0x13, ',' }, { 0x14, '!' }, { 0x15, '?' }, { 0x16, '_' }, { 0x17, '#' },
                { 0x18, '`' }, { 0x19, '"' }, { 0x1A, '/' }, { 0x1B, '\\'}, { 0x1C, '-' }, { 0x1D, ':' },
                { 0x1E, '(' }, { 0x1F, ')' }
            };
        } 

        public ZStringDecoder(Machine machine)
        {
            if (machine == null) throw new ArgumentNullException(nameof(machine));

            this.machine = machine;
            if(machine.Version >= 5)
            {
                throw new NotImplementedException("Must check for custom alphabet");
            }
        }

        public string Decode(ReadOnlySpan<byte> bytes)
        {
            var result = new ZString();
            bool end = false;

            while(!end)
            {
                var value = Bits.MakeWord(bytes.Slice(0, 2));
                end = (value & 80) > 0;
                var char1 = (value >> 10) & 0x1F;
                var char2 = (value >> 5) & 0x1F;
                var char3 = value & 0x1F;

                result.Append(Translate(char1));
                
            }

            return result.ToString();
        }

        private (char, bool) Translate(int value)
        {
            if(value == 6)
            {
                throw new NotImplementedException("This is 10 but encoding, I think");
            }

            if (alphabetMap[nextAlphabet].ContainsKey(value))
            {
                var result = (alphabetMap[nextAlphabet][value], true);
                nextAlphabet = 0;
                return result;
            }

            if(value == 5)
            {
                nextAlphabet = 2;
            }
            if(value == 4)
            {
                nextAlphabet = 1;
            }

            return (' ', false);
        }

        int nextAlphabet = 0;
        readonly static Dictionary<int, char>[] alphabetMap;
    }

    public class ZString
    {
        private readonly StringBuilder builder;

        public ZString()
        {
            builder = new StringBuilder();
        }

        public void Append((char letter, bool used) decoded)
        {
            if(decoded.used)
            {
                builder.Append(decoded.letter);
            }
        }

        public override string ToString()
        {
            return builder.ToString();
        }
    }

    // In Versions 3 and later, the current alphabet is always A0 unless changed for 1 character only: 
    // Z-characters 4 and 5 are shift characters. Thus 4 means "the next character is in A1" a
    // and 5 means "the next is in A2". There are no shift lock characters.
    public class AlphabetA0 { }
    public class AlphabetA1 { }

    // Z-character 6 from A2 means that the two subsequent Z-characters specify a ten-bit 
    // ZSCII character code: the next Z-character gives the top 5 bits and the one after the 
    // bottom 5
    public class AlphabetA2 { }

    // In Versions 3 and later, Z-characters 1, 2 and 3 represent abbreviations, 
    // sometimes also called 'synonyms' (for traditional reasons): the next Z-character indicates 
    // which abbreviation string to print.If z is the first Z-character(1, 2 or 3) and x the 
    // subsequent one, then the interpreter must look up entry 32(z-1)+x in the abbreviations 
    // table and print the string at that word address.In Version 2, Z-character 1 has this 
    // effect (but 2 and 3 do not, so there are only 32 abbreviations).

}
