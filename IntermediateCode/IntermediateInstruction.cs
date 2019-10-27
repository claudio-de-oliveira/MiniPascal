    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntermediateCode
{
    public enum Operator
    {
        COPY,
        MUL, DIV, ADD, SUB,
        INC, DEC, NEG, NOT,
        GOTO,
        IFTRUE, IFFALSE, IFEXP,
        PARAM,
        CALL,
        RETURN,
        RETVAL,
        ADDRESS,
        LT, LE, GT, GE,
        EQ, NEQ,
        FROMMEMORY, TOMEMORY,
        FROMARRAY, TOARRAY,
        CONTINUE,
    };

    #region Endereços
    public class Address : KeyedData
    {
        private BitSet declaration;
        protected static int count = 0;
        public static HashSet<object> Symbols = new HashSet<object>();
        protected int id;

        public Address()
        {
            id = -1;
            declaration = null;
        }

        public static void Reinitilize()
        {
            count = 0;
            Symbols = new HashSet<object>();
        }

        public int ToInt()
        { return id; }
        public static int Count
        { get { return count; } }
        public override string Key
        { get { return ToString(); } }

        public static bool operator ==(Address lhs, Address rhs)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(lhs, rhs))
                return true;

            // If one is null, but not both, return false.
            if (((object)lhs == null) || ((object)rhs == null))
                return false;

            if (lhs.GetType() != rhs.GetType())
                return false;

            // Return true if the fields match:
            return lhs.Equals(rhs);
        }
        public static bool operator !=(Address s1, Address s2)
        { return !(s1 == s2); }

        public override bool Equals(object obj)
        { return base.Equals(obj); }

        public override int GetHashCode()
        { return base.GetHashCode(); }

        public BitSet Declaration
        { 
            get { return declaration; }
            set { declaration = value; }
        }
    }

    public class Name : Address
    {
        private readonly string name;

        public Name(string name)
        {
            id = count;
            count++;
            this.name = name;
            Symbols.Add(this);
        }

        public Name()
        {
            id = count;
            count++;
            this.name = "tmp" + count.ToString() + "";
            Symbols.Add(this);
        }

        public override bool Equals(object obj)
        {
            // If one is null, but not both, return false.
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            return name == ((Name)obj).name; 
        }

        public override int GetHashCode()
        { return base.GetHashCode(); }

        public override string ToString()
        { return name; }
    }

    public class Label : Address
    {
        private readonly int val;
        private readonly Block blk;

        public Label(int val)
        {
            this.val = val;
            this.blk = null;
        }

        public Label(Block blk)
        {
            this.val = -1;
            this.blk = blk;
        }

        public int Value
        { get { return val; } }

        public Block Blk
        { get { return blk; } }

        public override bool Equals(object obj)
        {
            // If one is null, but not both, return false.
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            if (Value != -1)
                return Value == ((Label)obj).Value; 
            else
                return Blk == ((Label)obj).Blk; 
        }

        public override int GetHashCode()
        { return base.GetHashCode(); }

        public override string ToString()
        {
            if (Value != -1)
                return "$" + val.ToString(); 
            else
                return "@" + blk.Id.ToString(); 
        }
    }

    public abstract class Value : Address
    {
        private static Value binaryOperation(Operator Op, Value v1, Value v2)
        {
            if (v1.GetType() == typeof(Nac) || v2.GetType() == typeof(Nac))
                return Nac.Create();
            if (v1.GetType() == typeof(Undef) || v2.GetType() == typeof(Undef))
                return Undef.Create();
            switch (Op)
            {
                case Operator.MUL:
                    return Constant.Create(((Constant)v1).Value * ((Constant)v2).Value);
                case Operator.DIV:
                    return Constant.Create(((Constant)v1).Value / ((Constant)v2).Value);
                case Operator.ADD:
                    return Constant.Create(((Constant)v1).Value + ((Constant)v2).Value);
                case Operator.SUB:
                    return Constant.Create(((Constant)v1).Value - ((Constant)v2).Value);
            }

            return null;
        }

        private static Value unaryOperation(Operator Op, Value v1)
        {
            if (v1.GetType() == typeof(Nac))
                return Nac.Create();
            if (v1.GetType() == typeof(Undef))
                return Undef.Create();
            switch (Op)
            {
                case Operator.INC:
                    return Constant.Create(((Constant)v1).Value + 1);
                case Operator.DEC:
                    return Constant.Create(((Constant)v1).Value - 1);
                case Operator.NEG:
                    return Constant.Create(-((Constant)v1).Value);
                case Operator.NOT:
                    return Constant.Create(((Constant)v1).Value > 0 ? 0 : 1);
            }

            return null;
        }

        public static Value operator +(Value v1, Value v2)
        { return binaryOperation(Operator.ADD, v1, v2); }
        public static Value operator -(Value v1, Value v2)
        { return binaryOperation(Operator.SUB, v1, v2); }
        public static Value operator *(Value v1, Value v2)
        { return binaryOperation(Operator.MUL, v1, v2); }
        public static Value operator /(Value v1, Value v2)
        { return binaryOperation(Operator.DIV, v1, v2); }
        public static Value operator -(Value v1)
        { return unaryOperation(Operator.NEG, v1); }
        public static Value operator ++(Value v1)
        { return unaryOperation(Operator.INC, v1); }
        public static Value operator --(Value v1)
        { return unaryOperation(Operator.DEC, v1); }
        public static Value operator !(Value v1)
        { return unaryOperation(Operator.NOT, v1); }
    }

    public class Constant : Value
    {
        private readonly int val;

        public static Constant Create(int val)
        { return new Constant(val); }

        private Constant(int val)
        {
            this.val = val;
        }

        public int Value
        { get { return val; } }

        public override bool Equals(object obj)
        {
            // If one is null, but not both, return false.
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            return Value == ((Constant)obj).Value; 
        }

        public override int GetHashCode()
        { return base.GetHashCode(); }

        public override string ToString()
        { return val.ToString(); }
    }

    public class Undef : Value
    {
        private static Undef undef = new Undef();

        public static Undef Create()
        { return undef; }

        protected Undef()
        {
            // Nothing todo
        }

        public override bool Equals(object obj)
        {
            // If one is null, but not both, return false.
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            return true; 
        }

        public override int GetHashCode()
        { return base.GetHashCode(); }

        public override string ToString()
        { return "\\top"; }
    }

    public class Nac : Value
    {
        private static Nac nac = new Nac();

        public static Nac Create()
        { return nac; }

        public override bool Equals(object obj)
        {
            // If one is null, but not both, return false.
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            return true; 
        }

        public override int GetHashCode()
        { return base.GetHashCode(); }

        public override string ToString()
        { return "\\bot"; }
    }

    #endregion

    #region Instruções de Três Endereços
    public class TAC : Address
    {
        public Operator Op { get; set; }

        protected Address arg1;
        protected Address arg2;

        public TAC(Operator op, Address arg1, Address arg2)
        {
            this.Op = op;
            this.arg1 = arg1;
            this.arg2 = arg2;
        }

        public Address Arg1
        {
            get { return arg1; }
            set { arg1 = value; }
        }
        public Address Arg2
        {
            get { return arg2; }
            set { arg2 = value; }
        }

        public override bool Equals(object obj)
        {
            return Op == ((TAC)obj).Op && Arg1 == ((TAC)obj).Arg1 && Arg2 == ((TAC)obj).Arg2;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            string str = "";

            switch (Op)
            {
                case Operator.COPY:
                    str += "(COPY, " + Arg1.ToString() + ", " + Arg2.ToString() + ")";
                    break;
                case Operator.MUL:
                    str += "(*, " + Arg1.ToString() + ", " + Arg2.ToString() + ")";
                    break;
                case Operator.DIV:
                    str += "(/, " + Arg1.ToString() + ", " + Arg2.ToString() + ")";
                    break;
                case Operator.ADD:
                    str += "(+, " + Arg1.ToString() + ", " + Arg2.ToString() + ")";
                    break;
                case Operator.SUB:
                    str += "(-, " + Arg1.ToString() + ", " + Arg2.ToString() + ")";
                    break;
                case Operator.NOT:
                    str += "(not, " + Arg1.ToString() + ", " + "---" + ")";
                    break;
                case Operator.PARAM:
                    str += "(PARAM, " + Arg1.ToString() + ", " + "---" + ")";
                    break;
                case Operator.GOTO:
                    str += "(GOTO, " + "---" + ", " + Arg2.ToString() + ")";
                    break;
                case Operator.IFTRUE:
                    str += "(IFTRUE, " + Arg1.ToString() + ", " + Arg2.ToString() + ")";
                    break;
                case Operator.IFFALSE:
                    str += "(IFFALSE, " + Arg1.ToString() + ", " + Arg2.ToString() + ")";
                    break;
                case Operator.CALL:
                    str += "(CALL, " + Arg1.ToString() + ", " + Arg2.ToString() + ")";
                    break;
                case Operator.LT:
                    str += "(<, " + Arg1.ToString() + ", " + Arg2.ToString() + ")";
                    break;
                case Operator.LE:
                    str += "(<=, " + Arg1.ToString() + ", " + Arg2.ToString() + ")";
                    break;
                case Operator.GT:
                    str += "(>, " + Arg1.ToString() + ", " + Arg2.ToString() + ")";
                    break;
                case Operator.GE:
                    str += "(>=, " + Arg1.ToString() + ", " + Arg2.ToString() + ")";
                    break;
                case Operator.EQ:
                    str += "(=, " + Arg1.ToString() + ", " + Arg2.ToString() + ")";
                    break;
                case Operator.NEQ:
                    str += "(<>, " + Arg1.ToString() + ", " + Arg2.ToString() + ")";
                    break;
                case Operator.FROMARRAY:
                    str += "(FROMARRAY, " + Arg1.ToString() + ", " + Arg2.ToString() + ")";
                    break;
                case Operator.TOARRAY:
                    str += "(TOARRAY, " + Arg1.ToString() + ", " + Arg2.ToString() + ")";
                    break;
                case Operator.RETURN:
                    str += "(RETURN, " + "---" + ", " + "---" + ")";
                    break;
                case Operator.RETVAL:
                    str += "(RETURN, " + Arg1.ToString() + ", " + "---" + ")";
                    break;
                case Operator.CONTINUE:
                    if (Arg1 != null && Arg2 != null)
                        str += "(CONTINUE, " + Arg1.ToString() + ", " + Arg2.ToString() + ")";
                    else if (Arg1 != null)
                        str += "(CONTINUE, " + Arg1.ToString() + ", " + "---" + ")";
                    else // if (Arg2 != null)
                        str += "(CONTINUE, " + "---" + ", " + Arg2.ToString() + ")";
                    break;

                case Operator.INC: // Não usado no MiniPascal
                    str += "(++, " + Arg1.ToString() + ", " + "---" + ")";
                    break;
                case Operator.DEC: // Não usado no MiniPascal
                    str += "(--, " + Arg1.ToString() + ", " + "---" + ")";
                    break;
                case Operator.NEG: // Não usado no MiniPascal
                    str += "(-, " + Arg1.ToString() + ", " + "---" + ")";
                    break;
                case Operator.IFEXP: // Não usado no MiniPascal
                    str += "(IFEXP)";
                    break;
                case Operator.ADDRESS: // Não usado no MiniPascal
                    str += "(&, " + Arg1.ToString() + ", " + Arg2.ToString() + ")";
                    break;
                case Operator.FROMMEMORY: // Não usado no MiniPascal
                    str += "(FROMMEMORY)";
                    break;
                case Operator.TOMEMORY: // Não usado no MiniPascal
                    str += "(TOMEMORY)";
                    break;
            }

            return str;
        }
    }
    #endregion

    #region Instruções Intermediárias
    public abstract class IntermediateInstruction
    {
        protected int pos;
        protected List<TAC> tuples;
        protected Address target;

        public IntermediateInstruction()
        {
            this.tuples = new List<TAC>();
            target = null;
        }

        public Address Target
        {
            get { return target; }
            set { target = value; }
        }

        public int NumOfTuples
        { get { return tuples.Count; } }

        public TAC this[int i]
        { get { if (pos < 0) return null; else return tuples[pos + i]; } }

        public override string ToString()
        {
            string str = "";

            for (int i = 0; i < tuples.Count; i++)
                str += String.Format("\t{0}", tuples[i].ToString());

            return str;
        }
    }

    public class Nop : IntermediateInstruction
    {
        public Nop()
        {
            // Nothing more todo
        }

        public override string ToString()
        { 
            return "nop" + base.ToString(); 
        }
    }

    public class Binary : IntermediateInstruction
    {
        public Binary(Operator op, Address x, Address y, Address z)
        {
            TAC ic;
            ic = new TAC(op, y, z);
            pos = tuples.Count;
            tuples.Add(ic);
            ic = new TAC(Operator.CONTINUE, null, x);
            tuples.Add(ic);
            target = ic.Arg2;
        }

        public override string ToString()
        {
            string str = "";

            switch (tuples[pos].Op)
            {
                case Operator.MUL:
                    str += tuples[pos + 1].Arg2.ToString() + " := " + tuples[pos].Arg1.ToString() + "*" + tuples[pos].Arg2.ToString();
                    break;
                case Operator.DIV:
                    str += tuples[pos + 1].Arg2.ToString() + " := " + tuples[pos].Arg1.ToString() + "/" + tuples[pos].Arg2.ToString();
                    break;
                case Operator.ADD:
                    str += tuples[pos + 1].Arg2.ToString() + " := " + tuples[pos].Arg1.ToString() + "+" + tuples[pos].Arg2.ToString();
                    break;
                case Operator.SUB:
                    str += tuples[pos + 1].Arg2.ToString() + " := " + tuples[pos].Arg1.ToString() + "-" + tuples[pos].Arg2.ToString();
                    break;

                case Operator.LT:
                    str += tuples[pos + 1].Arg2.ToString() + " := " + tuples[pos].Arg1.ToString() + "<" + tuples[pos].Arg2.ToString();
                    break;
                case Operator.LE:
                    str += tuples[pos + 1].Arg2.ToString() + " := " + tuples[pos].Arg1.ToString() + "<=" + tuples[pos].Arg2.ToString();
                    break;
                case Operator.GT:
                    str += tuples[pos + 1].Arg2.ToString() + " := " + tuples[pos].Arg1.ToString() + ">" + tuples[pos].Arg2.ToString();
                    break;
                case Operator.GE:
                    str += tuples[pos + 1].Arg2.ToString() + " := " + tuples[pos].Arg1.ToString() + ">=" + tuples[pos].Arg2.ToString();
                    break;
                case Operator.EQ:
                    str += tuples[pos + 1].Arg2.ToString() + " := " + tuples[pos].Arg1.ToString() + "=" + tuples[pos].Arg2.ToString();
                    break;
                case Operator.NEQ:
                    str += tuples[pos + 1].Arg2.ToString() + " := " + tuples[pos].Arg1.ToString() + "<>" + tuples[pos].Arg2.ToString();
                    break;
            }

            return str + "\t" + base.ToString();
        }
    }

    public class Unary : IntermediateInstruction
    {
        public Unary(Operator op, Address x, Address y)
        // x = op y
        {
            TAC ic = new TAC(op, x, y);
            pos = tuples.Count;
            tuples.Add(ic);
            target = ic.Arg1;
        }

        public override string ToString()
        {
            string str = "";

            switch (tuples[pos].Op)
            {
                case Operator.INC:
                    str += tuples[pos].Arg1.ToString() + " := " + tuples[pos].Arg2.ToString() + "+1";
                    break;
                case Operator.DEC:
                    str += tuples[pos].Arg1.ToString() + " := " + tuples[pos].Arg2.ToString() + "-1";
                    break;
                case Operator.NEG:
                    str += tuples[pos].Arg1.ToString() + ":=-" + tuples[pos].Arg2.ToString();
                    break;
                case Operator.NOT:
                    str += tuples[pos].Arg1.ToString() + ":=!" + tuples[pos].Arg2.ToString();
                    break;
            }
            return str + "\t" + base.ToString();
        }
    }

    public class Copy : IntermediateInstruction
    {
        public Copy(Address x, Address y)
        // x = y
        {
            TAC ic = new TAC(Operator.COPY, x, y);
            pos = tuples.Count;
            if (tuples != null)
                tuples.Add(ic);
            else
                pos = -1;
            target = ic.Arg1;
        }

        public override string ToString()
        {
            string str = "";
            str += tuples[pos].Arg1.ToString() + " := " + tuples[pos].Arg2.ToString();
            return str + "\t" + base.ToString();
        }
    }

    public class Goto : IntermediateInstruction
    {
        public Goto(Label L)
        // goto L
        {
            TAC ic = new TAC(Operator.GOTO, null, L);
            pos = tuples.Count;
            tuples.Add(ic);
        }

        public override string ToString()
        {
            string str = "";
            str += "goto " + tuples[pos].Arg2.ToString();
            return str + "\t" + base.ToString();
        }
    }

    public class IfTrue : IntermediateInstruction
    {
        public IfTrue(Address x, Label L)
        // if x goto L
        {
            TAC ic = new TAC(Operator.IFTRUE, x, L);
            pos = tuples.Count;
            tuples.Add(ic);
        }

        public override string ToString()
        {
            string str = "";
            str += "if " + tuples[pos].Arg1.ToString() + " goto " + tuples[pos].Arg2.ToString();
            return str + "\t" + base.ToString();
        }
    }

    public class IfFalse : IntermediateInstruction
    {
        public IfFalse(Address x, Label L)
        // ifFalse x goto L
        {
            TAC ic = new TAC(Operator.IFFALSE, x, L);
            pos = tuples.Count;
            tuples.Add(ic);
        }

        public override string ToString()
        {
            string str = "";
            str += "ifFalse " + tuples[pos].Arg1.ToString() + " goto " + tuples[pos].Arg2.ToString();
            return str + "\t" + base.ToString();
        }
    }

    public class IfExp : IntermediateInstruction
    {
        public IfExp(Operator oprel, Address x, Address y, Label L)
        // if x oprel y goto L ===> tmp := x oprel y, if tmp goto L
        {
            TAC ic;
            ic = new TAC(Operator.IFEXP, null, L);
            pos = tuples.Count;
            tuples.Add(ic);
            ic = new TAC(oprel, x, y);
            tuples.Add(ic);
        }

        public override string ToString()
        {
            string str = "";

            str += "if " + tuples[pos + 1].Arg1.ToString();
            switch (tuples[pos + 1].Op)
            {
                case Operator.LT:
                    str += "<";
                    break;
                case Operator.LE:
                    str += "<=";
                    break;
                case Operator.GT:
                    str += ">";
                    break;
                case Operator.GE:
                    str += ">=";
                    break;
                case Operator.EQ:
                    str += "==";
                    break;
                case Operator.NEQ:
                    str += "!=";
                    break;
                default:
                    break;
            }
            str += tuples[pos + 1].Arg2.ToString() + " goto " + tuples[pos].Arg2.ToString();

            return str + "\t" + base.ToString();
        }
    }

    public class Param : IntermediateInstruction
    {
        public Param(Address x)
        // param x
        {
            TAC ic = new TAC(Operator.PARAM, x, null);
            pos = tuples.Count;
            tuples.Add(ic);
        }

        public override string ToString()
        {
            string str = "";
            str += "param " + tuples[pos].Arg1.ToString();
            return str + "\t" + base.ToString();
        }
    }

    public class Call : IntermediateInstruction
    {
        public Call(Label p, int n)
        {
            TAC ic = new TAC(Operator.CALL, Constant.Create(n), p);
            pos = tuples.Count;
            tuples.Add(ic);
        }

        public override string ToString()
        {
            string str = "";
            str += "call " + tuples[pos].Arg2.ToString() + ", " + tuples[pos].Arg1.ToString();
            return str + "\t" + base.ToString();
        }
    }

    public class Return : IntermediateInstruction
    {
        public Return()
        {
            TAC ic = new TAC(Operator.RETURN, null, null);
            pos = tuples.Count;
            tuples.Add(ic);
        }

        public override string ToString()
        {
            return "return\t" + base.ToString();
        }
    }

    public class RetVal : IntermediateInstruction
    {
        public RetVal(Address x)
        // return x
        {
            TAC ic = new TAC(Operator.RETVAL, x, null);
            pos = tuples.Count;
            tuples.Add(ic);
        }

        public override string ToString()
        {
            string str = "";
            str += "return " + tuples[pos].Arg1.ToString();
            return str + "\t" + base.ToString();
        }
    }

    public class ToArray : IntermediateInstruction
    {
        public ToArray(Address x, Address i, Address y)
        // x[i] = y ===> tmp = x[i], *tmp = y
        {
            TAC ic;
            ic = new TAC(Operator.TOARRAY, x, i);
            pos = tuples.Count;
            tuples.Add(ic);
            ic = new TAC(Operator.CONTINUE, y, null);
            tuples.Add(ic);
        }

        public override string ToString()
        {
            string str = "";
            // str += tuples[pos].Arg1.ToString() + "[" + tuples[pos].Arg2.ToString() + "]:=" + tuples[pos + 1].Arg1.ToString();
            str += tuples[pos].Arg1.ToString() + " := " + tuples[pos + 1].Arg1.ToString() + "[" + tuples[pos].Arg2.ToString() + "]";
            return str + "\t" + base.ToString();
        }
    }

    public class FromArray : IntermediateInstruction
    {
        public FromArray(Address x, Address i, Address y)
        // x = y[i] ===> tmp = y[i], x = tmp
        {
            TAC ic;
            ic = new TAC(Operator.FROMARRAY, y, i);
            pos = tuples.Count;
            tuples.Add(ic); 
            ic = new TAC(Operator.CONTINUE, x, null);
            tuples.Add(ic);
            target = ic.Arg1;
        }

        public override string ToString()
        {
            string str = "";
            str += tuples[pos + 1].Arg1.ToString() + " := " + tuples[pos].Arg1.ToString() + "[" + tuples[pos].Arg2.ToString() + "]";
            return str + "\t" + base.ToString();
        }
    }

    public class AddressOf : IntermediateInstruction
    {
        public AddressOf(Address x, Address y)
        // x = &y
        {
            TAC ic = new TAC(Operator.ADDRESS, x, y);
            pos = tuples.Count;
            tuples.Add(ic);
            target = ic.Arg1;
        }

        public override string ToString()
        {
            string str = "";
            str += tuples[pos].Arg1.ToString() + ":=\\&" + tuples[pos].Arg2.ToString();
            return str + "\t" + base.ToString();
        }
    }

    public class FromMemory : IntermediateInstruction
    {
        public FromMemory(Address x, Address y)
        // x = *y
        {
            TAC ic = new TAC(Operator.FROMMEMORY, x, y);
            pos = tuples.Count;
            tuples.Add(ic);
            target = ic.Arg1;
        }

        public override string ToString()
        {
            string str = "";
            str += tuples[pos].Arg1.ToString() + ":=*" + tuples[pos].Arg2.ToString();
            return str + "\t" + base.ToString();
        }
    }

    public class ToMemory : IntermediateInstruction
    {
        public ToMemory(Address x, Address y)
        // *x = y
        {
            TAC ic = new TAC(Operator.TOMEMORY, x, y);
            pos = tuples.Count;
            tuples.Add(ic);
        }

        public override string ToString()
        {
            string str = "";
            str += "*" + tuples[pos].Arg1.ToString() + " := " + tuples[pos].Arg2.ToString();
            return str + "\t" + base.ToString();
        }
    }
    #endregion
}
