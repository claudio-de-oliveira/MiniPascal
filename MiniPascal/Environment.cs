using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntermediateCode;

namespace MiniPascal
{
    public class Environment
    {
        public Environment parent;
        public Dictionary<string, AbsType> locals;
        public Dictionary<string, AbsType> parameters;
        public Address returnvalue;
        public bool SentencaImperativa;
        public AbsMachine machine;
        public int CodeAddress;

        public static Environment root = new Environment(new AbsMachine());

        private Environment(AbsMachine machine)
        {
            this.parent = null;
            parameters = new Dictionary<string, AbsType>();
            locals = new Dictionary<string, AbsType>();
            SentencaImperativa = false;
            this.machine = machine;
            // O CodeAddress é inicializado pela ação semântica @MainCode
            this.returnvalue = null;
        }

        public Environment(Environment parent)
        {
            this.parent = parent;
            this.machine = parent.machine;

            EntryPoint();

            parameters = new Dictionary<string, AbsType>();
            locals = new Dictionary<string, AbsType>();
            SentencaImperativa = false;
            this.returnvalue = null;
        }

        public void EntryPoint()
        { 
            CodeAddress = machine.Count;
        }

        public static AbsType SearchLocal(Environment corrente, string simbolo)
        {
            if (corrente.parameters.ContainsKey(simbolo))
                return corrente.parameters[simbolo];
            else if (corrente.locals.ContainsKey(simbolo))
                return corrente.locals[simbolo];
            else
                return null;
        }

        public static AbsType Search(Environment corrente, string simbolo)
        {
            if (corrente == null)
                return null;

            AbsType type = SearchLocal(corrente, simbolo);

            if (type == null)
                type = Search(corrente.parent, simbolo);

            return type;
        }

        public override string ToString()
        {
            string str = "";

            // Parte do cabeçalho
            str += "(";

            int i = 0;

            for (i = 0; i < parameters.Values.Count - 1; i++)
                str += parameters.Keys.ElementAt<string>(i) + " : " + parameters.Values.ElementAt<AbsType>(i).ToString() + "; ";
            if (i < parameters.Values.Count)
                str += parameters.Keys.ElementAt<string>(i) + " : " + parameters.Values.ElementAt<AbsType>(i).ToString();
            str += ")";

            if (returnvalue != null)
                str += " : " + returnvalue.ToString();

            str += ";\n";
            //

            // Variáveis Locais
            foreach (string id in locals.Keys)
            {
                if (locals[id].GetType() == typeof(FuncType))
                {
                    str += "\n(* Corpo da funcao " + id + " *)";
                    str += "\nfunction " + id + " : " + locals[id].ToString() + "\n";
                }
                else if (locals[id].GetType() == typeof(ProcType))
                {
                    str += "\n(* Corpo do procedimento " + id + " *)";
                    str += "\nprocedure " + id + " : " + locals[id].ToString() + "\n";
                }
                else
                {
                    str += "var\n\t" + id + " : " + locals[id].ToString() + "\n";
                }
            }

            str += "\n(* Entry-point do programa principal *)";
            str += "\nbegin\n";
            i = CodeAddress;
            str += "\t$" + i + ":\t" + this.machine[i].ToString() + "\n";
            do
            {
                i++;
                str += "\t$" + i + ":\t" + this.machine[i].ToString() + "\n";
            }
            while (this.machine[i].GetType() != typeof(Return));
            str += "end.\n";

            return str;
        }
    }
}
