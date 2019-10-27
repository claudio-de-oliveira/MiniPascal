using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntermediateCode
{
    #region Código Intermediário - Tuplas Indiretas
    public class AbsMachine
    {
        private List<IntermediateInstruction> instructions;

        public AbsMachine()
        {
            instructions = new List<IntermediateInstruction>();
        }

        public IntermediateInstruction this[int i]
        {
            get { return instructions[i]; }
        }

        public int Count
        { get { return instructions.Count; } }

        public IEnumerator<object> GetEnumerator()
        {
            for (int i = 0; i < instructions.Count; i++)
                yield return instructions[i];
        }

        public void AddInstruction(IntermediateInstruction inst)
        {
            instructions.Add(inst);
        }

        public void ReplaceInstruction(int i, IntermediateInstruction inst)
        {
            instructions.Remove(instructions[i]);
            instructions.Insert(i, inst);
        }

        public void InsertInstruction(int i, IntermediateInstruction inst)
        {
            instructions.Insert(i, inst);
        }

        public IntermediateInstruction CreateBinary(Operator op, Address x, Address y, Address z)
        // x = y op z
        {
            if (x.GetType() != typeof(Name))
                throw new InvalidCastException("Invalid Parameter Type");
            return new Binary(op, x, y, z);
        }

        public IntermediateInstruction CreateUnary(Operator op, Address x, Address y)
        // op x
        {
            if (x.GetType() != typeof(Name))
                throw new InvalidCastException("Invalid Parameter Type");
            return new Unary(op, x, y);
        }

        public IntermediateInstruction CreateCopy(Address x, Address y)
        // x = y
        {
            if (x.GetType() != typeof(Name))
                throw new InvalidCastException("Invalid Parameter Type");
            return new Copy(x, y);
        }

        public IntermediateInstruction CreateGoto(Label L)
        // goto L
        {
            return new Goto(L);
        }

        public IntermediateInstruction CreateIfTrue(Address x, Label L)
        // if x goto L
        {
            return new IfTrue(x, L);
        }

        public IntermediateInstruction CreateIfFalse(Address x, Label L)
        // ifFalse x goto L
        {
            return new IfFalse(x, L);
        }

        public IntermediateInstruction CreateIfExp(Operator oprel, Address x, Address y, Label L)
        // if x oprel y goto L ===> tmp := x oprel y, if tmp goto L
        {
            return new IfExp(oprel, x, y, L);
        }

        public IntermediateInstruction CreateParam(Address x)
        // param x
        {
            return new Param(x);
        }

        public IntermediateInstruction CreateCall(Label p, int n)
        {
            return new Call(p, n);
        }

        public IntermediateInstruction CreateReturn()
        // return
        {
            return new Return();
        }

        public IntermediateInstruction CreateRetVal(Address x)
        // return x
        {
            return new RetVal(x);
        }

        public IntermediateInstruction CreateFromArray(Address x, Address i, Address y)
        // x = y[i] ===> tmp = y[i], x = tmp
        {
            if (x.GetType() != typeof(Name))
                throw new InvalidCastException("Invalid Parameter Type");
            return new FromArray(x, i, y);
        }

        public IntermediateInstruction CreateToArray(Address x, Address i, Address y)
        // x[i] = y
        {
            return new ToArray(x, i, y);
        }

        public IntermediateInstruction CreateAddressOf(Address x, Address y)
        // x = &y
        {
            if (x.GetType() != typeof(Name))
                throw new InvalidCastException("Invalid Parameter Type");
            return new AddressOf(x, y);
        }

        public IntermediateInstruction CreateFromMemory(Address x, Address y)
        // x = *y
        {
            if (x.GetType() != typeof(Name))
                throw new InvalidCastException("Invalid Parameter Type");
            return new FromMemory(x, y);
        }

        public IntermediateInstruction CreateToMemory(Address x, Address y)
        // *x = y
        {
            return new ToMemory(x, y);
        }

        public override string ToString()
        {
            string str = "";

            for (int i = 0; i < instructions.Count; i++)
                str += instructions[i].ToString();
            return str;
        }
    }
    #endregion
}
