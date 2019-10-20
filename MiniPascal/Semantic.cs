using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniPascal
{
    class Semantic
    {
        public void Execute(Tag action, Stack<Tag> stk, Token tk, ref Environment env)
        {
            if (action == Tag._Echo)
            {
                stk.ElementAt<Tag>(0).Inherited[0] = action.Inherited[0];
            }

            else if (action == Tag._Begin)
            {
                // Sinaliza o início de uma sentença imperativa
                env.SentencaImperativa = true;
            }
            else if (action == Tag._End)
            {
                // Sinaliza o final de uma sentença imperativa
                env.SentencaImperativa = false;
            }

            else if (action == Tag._CreateList)
            {
                List<object> list = new List<object>();

                list.Add(((IdNew)tk).Lexema);

                stk.ElementAt<Tag>(0).Inherited[0] = list;
            }
            else if (action == Tag._InsertList)
            {
                List<object> list = (List<object>)action.Inherited[0];

                list.Add(((IdNew)tk).Lexema);

                stk.ElementAt<Tag>(0).Inherited[0] = list;
            }

            else if (action == Tag._Integer)
            {
                stk.ElementAt<Tag>(0).Inherited[0] = new IntegerType();
                // stk.ElementAt<Tag>(0).Inherited[0] = AbsType.IntegerType;
            }
            else if (action == Tag._Real)
            {
                stk.ElementAt<Tag>(0).Inherited[0] = new RealType();
                // stk.ElementAt<Tag>(0).Inherited[0] = AbsType.RealType;
            }

            else if (action == Tag._BeginRange)
            {
                stk.ElementAt<Tag>(6).Inherited[2] = ((Integer)tk).Value;
            }
            else if (action == Tag._EndRange)
            {
                stk.ElementAt<Tag>(3).Inherited[1] = ((Integer)tk).Value;
            }
            else if (action == Tag._ArrayDec)
            {
                AbsType type = (AbsType)action.Inherited[0];
                int rangeEnd = (int)action.Inherited[1];
                int rangeBegin = (int)action.Inherited[2];

                stk.ElementAt<Tag>(0).Inherited[0] = new ArrayType(type, rangeBegin, rangeEnd);
            }

            else if (action == Tag._IdVar)
            {
                stk.ElementAt<Tag>(2).Inherited[1] = action.Inherited[0];
            }
            else if (action == Tag._VarDec)
            {
                List<object> lexemas = (List<object>)action.Inherited[1];
                AbsType type = (AbsType)action.Inherited[0];

                foreach (string v in lexemas)
                    env.locals.Add(v, type);
            }

            else if (action == Tag._IdProc)
            {
                // Verificar se o identificador é novo
                if (tk.tag != Tag.IDNEW)
                    throw new NotImplementedException("O Id deve ser novo!");
                string lexema = ((IdNew)tk).Lexema;

                // O contexto deve ser alterado logo antes dos parâmetros serem declarados
                ProcType type = new ProcType(env);

                env.locals.Add(lexema, type);

                env = type.Env;

                stk.ElementAt<Tag>(1).Inherited[0] = lexema;
            }
            else if (action == Tag._IdFunc)
            {
                // Verificar se o identificador é novo
                if (tk.tag != Tag.IDNEW)
                    throw new NotImplementedException("O Id deve ser novo!");
                string lexema = ((IdNew)tk).Lexema;

                // O contexto deve ser alterado logo antes dos parâmetros serem declarados
                FuncType type = new FuncType(env);

                env.locals.Add(lexema, type);

                env = type.Env;

                stk.ElementAt<Tag>(3).Inherited[1] = lexema;
            }
            else if (action == Tag._ProcDec)
            {
                // Identificador do procedimento
                string lexema = (string)action.Inherited[0];

                ProcType type = (ProcType)Environment.SearchLocal(env.parent, lexema);
            }
            else if (action == Tag._FuncDec)
            {
                // Identificador da função
                string lexema = (string)action.Inherited[1];

                FuncType type = (FuncType)Environment.SearchLocal(env.parent, lexema);

                type.ReturnType = (AbsType)action.Inherited[0];
            }

            else if (action == Tag._EndParList)
            {
                stk.ElementAt<Tag>(2).Inherited[1] = action.Inherited[0];
            }
            else if (action == Tag._ParDec)
            {
                foreach (string arg in (List<object>)action.Inherited[1])
                    env.parameters.Add(arg, (AbsType)action.Inherited[0]);
            }
            else if (action == Tag._EnvRestore)
            {
                env = env.parent;
            }

            ////
            else if (action == Tag._Args)
            {
                // Recebe a lista de argumentos do programa

                foreach (string arg in (List<object>)action.Inherited[0])
                    env.parameters.Add(arg, AbsType.Untyped);
            }
            else 
                throw new NotImplementedException("Não implementado!");
        }
    }
}
