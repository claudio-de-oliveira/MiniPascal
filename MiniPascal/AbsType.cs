using System.Collections.Generic;
using IntermediateCode;

namespace MiniPascal
{
    public enum EnumType { INTEGER, REAL, FUNC, PROC, ARRAY, UNTYPED }

    public class AbsType
    {
        public EnumType IdType { get; private set; }

        protected AbsType(EnumType type)
        { this.IdType = type; }

        // Tipos padrões
        public static AbsType Untyped = new AbsType(EnumType.UNTYPED);
        // public static AbsType IntegerType = new AbsType(EnumType.INTEGER);
        // public static AbsType RealType = new AbsType(EnumType.REAL);

        public override string ToString()
        {
            if (IdType == EnumType.INTEGER)
                return "Integer";
            if (IdType == EnumType.REAL)
                return "Real";

            return "Untyped";
        }
    }

    public class ProcType : AbsType
    {
        public Environment Env { get; private set; }

        public ProcType(Environment env, AbsMachine machine) : base(EnumType.PROC)
        { 
            Env = new Environment(env); 
        }

        public override string ToString()
        {
            string str = "";
            List<string> keys = new List<string>();

            // cabeçalho
            int i = 0;

            str += "(";
            foreach (string key in Env.parameters.Keys)
            {
                str += key + " : " + Env.parameters[key].ToString();

                if (++i < Env.parameters.Keys.Count)
                    str += "; ";
            }
            str += ")";

            str += ";\n";

            // Locais
            foreach (string id in Env.locals.Keys)
                str += "var\n\t" + id + " : " + Env.locals[id].ToString() + ";\n";

            // Código
            str += "begin\n";
            i = Env.CodeAddress;
            str += "\t$" + i + ":\t" + Env.machine[i].ToString() + "\n";
            do
            {
                i++;
                str += "\t$" + i + ":\t" + Env.machine[i].ToString() + "\n";
            }
            while (Env.machine[i].GetType() != typeof(Return));
            str += "end;";

            return str;
        }
    }

    public class FuncType : AbsType
    {
        public Environment Env { get; private set; }
        public AbsType ReturnType { get; set; }

        public FuncType(Environment env, AbsMachine machine) : base(EnumType.FUNC)
        { 
            Env = new Environment(env); 
        }

        public override string ToString()
        {
            string str = "";
            List<string> keys = new List<string>();

            // cabeçalho
            int i = 0;

            str += "(";
            foreach (string key in Env.parameters.Keys)
            {
                str += key + " : " + Env.parameters[key].ToString();

                if (++i < Env.parameters.Keys.Count)
                    str += "; ";
            }
            str += ") : ";

            str += ReturnType.ToString() + ";\n";

            // Locais
            foreach (string id in Env.locals.Keys)
                str += "var\n\t" + id + " : " + Env.locals[id].ToString() + ";\n";

            // Código
            str += "begin\n";
            i = Env.CodeAddress;
            str += "\t$" + i + ":\t" + Env.machine[i].ToString() + "\n";
            do
            {
                i++;
                str += "\t$" + i + ":\t" + Env.machine[i].ToString() + "\n";
            }
            while (Env.machine[i].GetType() != typeof(Return));
            str += "end;";

            return str;
        }
    }

    public class ArrayType : AbsType
    {
        public AbsType ItemType { get; private set; }
        public int BeginRange { get; private set; }
        public int EndRange { get; private set; }

        public ArrayType(AbsType type, int beginRange, int endRange) : base(EnumType.ARRAY)
        {
            ItemType = type;
            BeginRange = beginRange;
            EndRange = endRange;
        }

        public override string ToString()
        {
            return string.Format("array [{0}..{1}] of {2}", BeginRange, EndRange, ItemType.ToString());
        }
    }

    public class IntegerType : AbsType
    {
        public IntegerType() : base(EnumType.INTEGER)
        { /* Nothing more todo */  }
    
        public override string ToString()
        {
            return "Integer";
        }
    }
    
    public class RealType : AbsType
    {
        public RealType() : base(EnumType.REAL)
        { /* Nothing more todo */  }
    
        public override string ToString()
        {
            return "Real";
        }
    }
}
