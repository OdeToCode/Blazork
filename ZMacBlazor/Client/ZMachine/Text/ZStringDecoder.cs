﻿using System;
using System.Collections.Generic;

namespace ZMacBlazor.Client.ZMachine.Text
{
    public class ZStringDecoder
    {
        private readonly Machine machine;

        static ZStringDecoder()
        {
            alphabetMap = new Dictionary<int, char>[3];
            alphabetMap[0] = new Dictionary<int, char>
            {
                { 0x00, ' ' },
                { 0x06, 'a' }, { 0x07, 'b' }, { 0x08, 'c' }, { 0x09, 'd' }, { 0x0A, 'e' }, { 0x0B, 'f' },
                { 0x0C, 'g' }, { 0x0D, 'h' }, { 0x0E, 'i' }, { 0x0F, 'j' }, { 0x10, 'k' }, { 0x11, 'l' },
                { 0x12, 'm' }, { 0x13, 'n' }, { 0x14, 'o' }, { 0x15, 'p' }, { 0x16, 'q' }, { 0x17, 'r' },
                { 0x18, 's' }, { 0x19, 't' }, { 0x1A, 'u' }, { 0x1B, 'v' }, { 0x1C, 'w' }, { 0x1D, 'x' },
                { 0x1E, 'y' }, { 0x1F, 'z' }
            };

            alphabetMap[1] = new Dictionary<int, char>
            {
                { 0x00, ' ' },
                { 0x06, 'A' }, { 0x07, 'B' }, { 0x08, 'C' }, { 0x09, 'D' }, { 0x0A, 'E' }, { 0x0B, 'F' },
                { 0x0C, 'G' }, { 0x0D, 'H' }, { 0x0E, 'I' }, { 0x0F, 'J' }, { 0x10, 'K' }, { 0x11, 'L' },
                { 0x12, 'M' }, { 0x13, 'N' }, { 0x14, 'O' }, { 0x15, 'P' }, { 0x16, 'Q' }, { 0x17, 'R' },
                { 0x18, 'S' }, { 0x19, 'T' }, { 0x1A, 'U' }, { 0x1B, 'V' }, { 0x1C, 'W' }, { 0x1D, 'X' },
                { 0x1E, 'Y' }, { 0x1F, 'Z' }
            };

            alphabetMap[2] = new Dictionary<int, char>
            {
                { 0x00, ' ' },
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
            if (machine.Version >= 5)
            {
                throw new NotImplementedException("Must check for custom alphabet");
            }
        }

        public string Decode(ReadOnlySpan<byte> bytes)
        {
            var failSafe = 0;
            var position = 0;
            bool end = false;
            var result = new ZString();
            
            while (!end)
            {
                var value = Bits.MakeWord(bytes.Slice(position, 2));
                end = (value & 0x8000) > 0;
                
                for(var i = 10; i >= 0; i -=5)
                {
                    var zchar = (value >> i) & 0x1F;
                    var (letter, abbreviation) = Translate(zchar);

                    if (letter.HasValue)
                    {
                        result.Append(letter.Value);
                    }
                    else if (abbreviation != null)
                    {
                        result.Append(abbreviation);
                    }
                }

                position += 2;
                if(++failSafe > 1024)
                {
                    throw new InvalidOperationException("Probably in an infinite loop trying to decode");
                }
            }

            return result.ToString();
        }

        // In Versions 3 and later, Z-characters 1, 2 and 3 represent abbreviations, 
        // sometimes also called 'synonyms' (for traditional reasons): the next Z-character indicates 
        // which abbreviation string to print.If z is the first Z-character(1, 2 or 3) and x the 
        // subsequent one, then the interpreter must look up entry 32(z-1)+x in the abbreviations 
        // table and print the string at that word address.
        public string DecodeAbbreviation(int index, int number)
        {
            var offset = (32 * (index - 1)) + (number * 2);
            var ppAbbreviation = machine.Memory.WordAt(Header.ABBREVIATIONS);
            var pAbbreviation = machine.Memory.WordAddressAt(ppAbbreviation + offset);
            var abbreviationBytes = machine.Memory.SpanAt(pAbbreviation).Bytes;

            var result = Decode(abbreviationBytes);
            return result;
        }

        private (char? letter, string? abbreviation) Translate(int value)
        {
            if(state == State.Abbr)
            {
                var set = abbreviationSet;

                abbreviationSet = 0;
                state = State.A0;
                
                var abbreviation = DecodeAbbreviation(set, value);
                return (null, abbreviation);
            }

            var letter = ' ';
            var mapped = state switch
            {
                State.A0 => alphabetMap[0].TryGetValue(value, out letter),
                State.A1 => alphabetMap[1].TryGetValue(value, out letter),
                State.A2 => alphabetMap[2].TryGetValue(value, out letter),
                State.Abbr => false,
                _ => throw new InvalidOperationException("Invalid string decoder state")
            };

            if(mapped)
            {
                state = State.A0;
                return (letter, null);
            }

            state = value switch
            {
                1 => State.Abbr,
                2 => State.Abbr,
                3 => State.Abbr,
                4 => State.A1,
                5 => State.A2,
                6 when state == State.A2 => throw new NotImplementedException("10 bit encoding not implemented yet"),
                _ => throw new InvalidOperationException($"Unknown ZSCII translate value {value:X}")
            };
           
            if(state == State.Abbr)
            {
                abbreviationSet = value;
            }

            return (null, null);
        }

        int abbreviationSet = 0;
        State state = State.A0;
        readonly static Dictionary<int, char>[] alphabetMap;

        enum State { A0, A1, A2, Abbr };
    }
}