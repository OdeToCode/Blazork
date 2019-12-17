using System;

namespace ZMacBlazor.Client.ZMachine.Instructions
{
    public class Op2Instruction : Instruction
    {
        private readonly Op2OperandResolver op2OperandResolver;
        private readonly VarOperandResolver varOperandResolver;
        private readonly IndirectOperandResolver indirectOperandResolver;
        private readonly BranchResolver branchResolver;

        public Op2Instruction(Machine machine) : base(machine)
        {
            op2OperandResolver = new Op2OperandResolver();
            varOperandResolver = new VarOperandResolver();
            indirectOperandResolver = new IndirectOperandResolver();
            branchResolver = new BranchResolver();
        }

        public override void Execute(SpanLocation memory)
        {
            OpCode = Bits.BottomFive(memory.Bytes[0]);
            Operation = OpCode switch
            {
                0x01 => new Operation(nameof(JE), JE, hasBranch: true),
                0x03 => new Operation(nameof(JG), JG, hasBranch: true),
                0x04 => new Operation(nameof(DecChk), DecChk, hasBranch: true),
                0x05 => new Operation(nameof(IncChk), IncChk, hasBranch: true),
                0x06 => new Operation(nameof(Jin), Jin, hasBranch: true),
                0x09 => new Operation(nameof(And), And, hasStore: true),
                0x0A => new Operation(nameof(TestAttr), TestAttr, hasBranch: true),
                0x0B => new Operation(nameof(SetAttr), SetAttr),
                0x0D => new Operation(nameof(StoreB), StoreB),
                0x0E => new Operation(nameof(InsertObj), InsertObj),
                0x0F => new Operation(nameof(LoadW), LoadW, hasStore: true),
                0x10 => new Operation(nameof(LoadB), LoadB, hasStore: true),
                0x11 => new Operation(nameof(GetProp), GetProp, hasStore: true),
                0x14 => new Operation(nameof(Add), Add, hasStore: true),
                _ => throw new InvalidOperationException($"Unknown OP2 opcode {OpCode:X}")
            };

            if(OpCode == 0x05)
            {
                // 😒 seven Z-machine opcodes access variables but by their numbers ... 
                // inc, dec, inc_chk, dec_chk, store, pull, load. 😒
                indirectOperandResolver.AddOperands(Operands, memory.Bytes.Slice(1));
                Size = 1 + Operands.Size;
            }
            else if (Bits.SevenSixSet(memory.Bytes[0]) == true && 
                     Bits.FiveSet(memory.Bytes[0]) == false)
            {
                // 😒 2OPS, but VAR operands 😒
                varOperandResolver.AddOperands(Operands, memory.Bytes.Slice(1));
                Size = 2 + Operands.Size;
            }
            else
            {
                op2OperandResolver.AddOperands(Operands, memory.Bytes);
                Size = 1 + Operands.Size;
            }
            if (Operation.HasStore)
            {
                StoreResult = memory.Bytes[Size];
                Size += 1;
            }
            if (Operation.HasBranch)
            {
                var branchData = machine.Memory.SpanAt(memory.Address + Size);
                Branch = branchResolver.ResolveBranch(branchData);
                Size += Branch.Size;
            }

            DumpToLog(memory, Size);
            Operation.Execute(memory);
        }

        public void GetProp(SpanLocation location)
        {
            var objecNumber = Operands[0].Value;
            var propertyNumber = Operands[1].Value;
            var gameObject = machine.ObjectTable.GetObject(objecNumber);
            if (gameObject.Properties.ContainsKey(propertyNumber))
            {
                var propertyValue = gameObject.Properties[propertyNumber];
                if(propertyValue.Size == 1)
                {
                    log.Verbose($"\tGetProp for {objecNumber}[{propertyNumber}] found {propertyValue.Value.Span[0]:X} => {StoreResult}");
                    machine.SetVariable(StoreResult, propertyValue.Value.Span[0]);
                }
                else if(propertyValue.Size == 2)
                {
                    log.Verbose($"\tGetProp for {objecNumber}[{propertyNumber}] found {Bits.MakeWord(propertyValue.Value.Span)} => {StoreResult}");
                    machine.SetVariable(StoreResult, Bits.MakeWord(propertyValue.Value.Span));
                }
                else
                {
                    throw new InvalidOperationException($"GetProp should look for property with length of 1 or 2, not {propertyValue.Size}. Looking at prop {propertyNumber} on {objecNumber}"); ;
                }
            }
            else
            {
                var propertyValue = machine.ObjectTable.GetDefault(propertyNumber);
                log.Verbose($"\tGetProp for {objecNumber}[{propertyNumber}] took default {propertyValue} => {StoreResult}");

                machine.SetVariable(StoreResult, propertyValue);
            }

            machine.SetPC(location.Address + Size);

        }

        public void Jin(SpanLocation location)
        {
            // Branch if n is the parent object of obj, or if n is 0 and the object has no parent. 
            var childNumber = Operands[0].Value;
            var parentNumber = Operands[1].Value;

            var child = machine.ObjectTable.GetObject(childNumber);
            var result = child.Parent == parentNumber;

            Branch.Go(result, machine, Size, location);
        }

        public void SetAttr(SpanLocation location)
        {
            var objectNumber = Operands[0].Value;
            var attribute = Operands[1].Value;
            machine.ObjectTable.GetObject(objectNumber).SetAttribute(attribute, true);
            machine.SetPC(location.Address + Size);
        }

        public void InsertObj(SpanLocation location)
        {
            var target = Operands[0].Value;
            var destination = Operands[1].Value;

            machine.ObjectTable.InsertObject(target, destination);
            machine.SetPC(location.Address + Size);
        }

        public void DecChk(SpanLocation location)
        {
            // dec_chk is indirect
            var variable = Operands[0].RawValue;
            var value = machine.ReadVariable(variable);

            value -= 1;
            machine.SetVariable(variable, value);

            var result = value < Operands[1].Value;
            Branch.Go(result, machine, Size, location);
        }
        
        public void IncChk(SpanLocation location)
        {
            // inc_chk is indirect
            var variable = Operands[0].RawValue;
            var value = machine.ReadVariable(variable);
            
            value += 1;
            machine.SetVariable(variable, value);

            var result = value > Operands[1].Value;
            Branch.Go(result, machine, Size, location);
        }

        public void TestAttr(SpanLocation location)
        {
            var objectNumber = Operands[0].Value;
            var gameObject = machine.ObjectTable.GetObject(objectNumber);
            var attributeNumber = Operands[1].Value;
            var result = gameObject.ReadAttribute(attributeNumber);

            log.Verbose($"\tTestAttr for {objectNumber}[{attributeNumber}] got {result}");

            Branch.Go(result, machine, Size, location);
        }

        public void And(SpanLocation location)
        {
            var a = Operands[0].Value;
            var b = Operands[1].Value;
            var result = a & b;

            machine.SetVariable(StoreResult, result);
            machine.SetPC(location.Address + Size);
        }

        public void StoreB(SpanLocation location)
        {
            if (Operands[0].Type != OperandType.Small)
            {
                throw new NotImplementedException("Need to rethink what this means!");
            }

            var variable = Operands[0].Value;
            var value = Operands[1].Value;
            
            machine.SetVariable(variable, value);
            machine.SetPC(location.Address + Size);
        }

        public void LoadB(SpanLocation location)
        {
            var baseArray = Operands[0].Value;
            var index = Operands[1].Value;
            var arrayLocation = baseArray + index;
            var value = machine.Memory.ByteAt(arrayLocation);

            machine.SetVariable(StoreResult, value);
            machine.SetPC(location.Address + Size);
        }

        public void LoadW(SpanLocation location)
        {
            var baseArray = Operands[0].Value;
            var index = Operands[1].Value;
            var arrayLocation = baseArray + (2 * index);
            var word = machine.Memory.WordAt(arrayLocation);

            log.Verbose($"\tLoadW {baseArray} {index} is at {arrayLocation} loaded {word} => {StoreResult}");

            machine.SetVariable(StoreResult, word);
            machine.SetPC(location.Address + Size);
        }

        public void Add(SpanLocation memory)
        {
            var a = Operands[0].Value;
            var b = Operands[1].Value;
            var result = a + b;

            log.Verbose($"\tAdd {a} {b} is {result} => {StoreResult}");

            machine.SetVariable(StoreResult, result);
            machine.SetPC(memory.Address + Size);
        }
        
        public void JG(SpanLocation location)
        {
            var a = Operands[0].SignedValue;
            var b = Operands[1].SignedValue;
            var result = a > b;

            Branch.Go(result, machine, Size, location);
        }

        public void JE(SpanLocation location)
        {
            var a = Operands[0].Value;
            var b = Operands[1].Value;
            var result = a == b;

            log.Verbose($"\tJE: {a} {b} is {result}");

            Branch.Go(result, machine, Size, location);
        }
    }
}
