using System;
using Xunit;
using ZMacBlazor.Client.ZMachine;
using ZMacBlazor.Client.ZMachine.Instructions;
using ZMacBlazor.Tests.Logging;

namespace ZMacBlazor.Tests.ZMachine
{
    public class OperandTests 
    {
        [Fact]
        public void CanReturnSmallValue()
        {
            var logger = NullLoggerFactory.GetLogger();
            var machine = new Machine(logger);

            var operand1 = new Operand(OperandType.Small, 0xFF, machine);
            var operand2 = new Operand(OperandType.Small, 0xFE, machine);
            var operand3 = new Operand(OperandType.Small, 0xD7, machine);
            var operand4 = new Operand(OperandType.Small, 0x01, machine);
            var operand5 = new Operand(OperandType.Small, 0x7F, machine);
            var operand6 = new Operand(OperandType.Small, 0x10, machine);

            Assert.Equal(-1, operand1.SignedValue);
            Assert.Equal(255, operand1.Value);

            Assert.Equal(-2, operand2.SignedValue);
            Assert.Equal(254, operand2.Value);

            Assert.Equal(-41, operand3.SignedValue);
            Assert.Equal(215, operand3.Value);

            Assert.Equal(1, operand4.SignedValue);
            Assert.Equal(1, operand4.Value);

            Assert.Equal(127, operand5.SignedValue);
            Assert.Equal(127, operand5.Value);

            Assert.Equal(16, operand6.SignedValue);
            Assert.Equal(16, operand6.Value);
        }


        [Fact]
        public void CanReturnLargeValue()
        {
            var logger = NullLoggerFactory.GetLogger();
            var machine = new Machine(logger);

            var operand1 = new Operand(OperandType.Large, 0xFFFF, machine);
            var operand2 = new Operand(OperandType.Large, 0xFFFE, machine);
            var operand3 = new Operand(OperandType.Large, 0xFFD7, machine);
            var operand4 = new Operand(OperandType.Large, 0x0001, machine);
            var operand5 = new Operand(OperandType.Large, 0x007F, machine);
            var operand6 = new Operand(OperandType.Large, 0x7FFF, machine);

            Assert.Equal(-1, operand1.SignedValue);
            Assert.Equal(65535, operand1.Value);

            Assert.Equal(-2, operand2.SignedValue);
            Assert.Equal(65534, operand2.Value);

            Assert.Equal(-41, operand3.SignedValue);
            Assert.Equal(65495, operand3.Value);

            Assert.Equal(1, operand4.SignedValue);
            Assert.Equal(1, operand4.Value);

            Assert.Equal(127, operand5.SignedValue);
            Assert.Equal(127, operand5.Value);

            Assert.Equal(32767, operand6.SignedValue);
            Assert.Equal(32767, operand6.Value);
        }
    }
}
