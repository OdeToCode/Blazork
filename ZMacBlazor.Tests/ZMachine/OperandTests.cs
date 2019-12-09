using Xunit;
using Xunit.Abstractions;
using ZMacBlazor.Client.ZMachine;
using ZMacBlazor.Client.ZMachine.Instructions;
using ZMacBlazor.Tests.Logging;

namespace ZMacBlazor.Tests.ZMachine
{
    public class OperandTests
    {
        private readonly LogAdapter logger;

        public OperandTests(ITestOutputHelper outputHelper)
        {
            logger = new LogAdapter(outputHelper);
        }

        [Fact]
        public void CanReturnValue()
        {
            var machine = new Machine(logger);
            var operand = new Operand(OperandType.Large, 0xFFFF, machine);

            Assert.Equal(65535, operand.Value);
        }

        [Fact]
        public void CanReturnSignedValue()
        {
            var machine = new Machine(logger);
            var operand1 = new Operand(OperandType.Large, 0xFFFF, machine);
            var operand2 = new Operand(OperandType.Large, 0xFFFE, machine);
            var operand3 = new Operand(OperandType.Large, 0xFFD7, machine);
            var operand4 = new Operand(OperandType.Large, 0x0001, machine);
            var operand5 = new Operand(OperandType.Large, 0x007F, machine);
            var operand6 = new Operand(OperandType.Large, 0x7FFF, machine);

            Assert.Equal(-1, operand1.SignedValue);
            Assert.Equal(-2, operand2.SignedValue);
            Assert.Equal(-41, operand3.SignedValue);
            Assert.Equal(1, operand4.SignedValue);
            Assert.Equal(127, operand5.SignedValue);
            Assert.Equal(32767, operand6.SignedValue);
        }
    }
}
