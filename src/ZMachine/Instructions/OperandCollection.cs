﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Blazork.ZMachine.Instructions
{
    public class OperandCollection : ICollection<Operand>
    {
        public OperandCollection(Machine machine)
        {
            this.machine = machine;
            this.innerList = new List<Operand>();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for(var i =0; i < Count; i++)
            {
                sb.Append($" Arg[{i+1}] : {OperandType.ToString(this[i].Type)} : {this[i].RawValue:X} ");
            }
            return sb.ToString();
        }

        public Operand this[int index]
        {
            get
            {
                return innerList[index];
            }
            protected set
            {
                innerList[index] = value;
            }
        }

        public void Add(Operand item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            Add(item.Type, item.Value);
        }

        public void Add(byte type, int value)
        {
            var operand = new Operand(type, value, machine);
            innerList.Add(operand);
        }

        public void Clear()
        {
            innerList.Clear();
        }

        public bool Contains(Operand item)
        {
            return innerList.Contains(item);
        }

        public void CopyTo(Operand[] array, int arrayIndex)
        {
            innerList.CopyTo(array, arrayIndex);
        }

        public bool Remove(Operand item)
        {
            return innerList.Remove(item);
        }

        public IEnumerator<Operand> GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        public int Count => innerList.Count;
        public bool IsReadOnly => false;

        public int Size 
        { 
            get
            {
                var size = 0;
                foreach(var operand in innerList)
                {
                    if(operand.Type == OperandType.Small)
                    {
                        size += 1;
                    }
                    else if(operand.Type == OperandType.Variable)
                    {
                        size += 1;
                    }
                    else if(operand.Type == OperandType.Large)
                    {
                        size += 2;
                    }
                }
                return size;
            }
        }

        private readonly List<Operand> innerList;
        private readonly Machine machine;
    }
}