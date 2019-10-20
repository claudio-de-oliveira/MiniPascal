using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPascal
{
    public class Environment
    {
        public Environment parent;
        public Dictionary<string, AbsType> locals;
        public Dictionary<string, AbsType> parameters;
        public bool SentencaImperativa; 

        public Environment(Environment parent)
        {
            this.parent = parent;
            parameters = new Dictionary<string, AbsType>();
            locals = new Dictionary<string, AbsType>();
            SentencaImperativa = false;
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
    }
}
