using System.Diagnostics.CodeAnalysis;
using System.IO;
using Xunit;
using Blazork.ZMachine;
using Blazork.ZMachine.Text;
using Blazork.Tests.Logging;
using Blazork.Tests.Input;

namespace Blazork.Tests.ZMachine
{
    [SuppressMessage("Usage", "xUnit1000:Test classes must be public")]
    internal class AbbreviationTests
    {
        [Fact]
        public void AbbreviationsMakeSense()
        {
            using var file = File.OpenRead(@"Data\ZORK1.DAT");
            var logger = NullLoggerFactory.GetLogger();
            var machine = new Machine(logger, new SolveZorkInputStream());
            machine.Load(file);

            logger.Error($"ABBREVIATIONS");
            var decoder = new ZStringDecoder(machine);
            for (var index = 1; index <= 3; index++)
            {
                for (var number = 0; number < 32; number++)
                {
                    var offset = (32 * (index - 1)) + (number * 2);
                    logger.Error($"For [{index}][{number}] the offset is {offset}");

                    var ppAbbreviation = machine.Memory.WordAt(Header.ABBREVIATIONS);
                    logger.Error($"For [{index}][{number}] the ppointer is {ppAbbreviation:X}");

                    var pAbbreviation = machine.Memory.WordAddressAt(ppAbbreviation + offset);
                    logger.Error($"For [{index}][{number}] the pointer is {pAbbreviation:X}");

                    var location = machine.Memory.SpanAt(pAbbreviation);
                    var result = decoder.Decode(location).Text;

                    logger.Error($"Abbreviation [{index}][{number}] : {result}");
                }
            }
        }
    }
}
