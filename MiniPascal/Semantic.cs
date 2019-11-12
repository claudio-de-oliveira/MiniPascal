using System;
using System.Collections.Generic;
using System.Linq;

using IntermediateCode;


namespace MiniPascal
{
    class Semantic
    {
        AbsMachine ic = new AbsMachine();
        string programName = "";

        public void Execute(Tag action, Stack<Tag> stk, Token tk, ref Environment env)
        {
            #region Sentenças Declarativas
            if (action == Tag._Begin)
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
                ProcType type = new ProcType(env, env.machine);

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
                FuncType type = new FuncType(env, env.machine);

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
                IntermediateInstruction code = ic.CreateReturn();
                // Console.WriteLine(code.ToString());
                env.machine.AddInstruction(code);

                env = env.parent;
            }

            else if (action == Tag._Args)
            {
                // Recebe a lista de argumentos do programa

                foreach (string arg in (List<object>)action.Inherited[0])
                    env.parameters.Add(arg, AbsType.Untyped);
            }
            #endregion

            #region Sentenças Imperativas
            else if (action == Tag._Number)
            {
                stk.ElementAt<Tag>(0).Inherited[0] = Constant.Create(((Integer)tk).Value);
            }
            else if (action == Tag._RValue)
            {
                Address address = (Address)action.Inherited[0];
                stk.ElementAt<Tag>(0).Inherited[0] = address;
            }
            else if (action == Tag._Not)
            {
                Name tmp = new Name();

                IntermediateInstruction code = ic.CreateUnary(Operator.NOT, (Address)action.Inherited[0], tmp);
                // Console.WriteLine(code.ToString());
                env.machine.AddInstruction(code);

                stk.ElementAt<Tag>(0).Inherited[0] = tmp;
            }
            else if (action == Tag._Skip1)
            {
                stk.ElementAt<Tag>(1).Inherited[0] = action.Inherited[0];
            }
            else if (action == Tag._FAddress) // Endereço da função
            {
                FuncType func = (FuncType)Environment.Search(env, ((IdFunc)tk).Lexema);

                stk.ElementAt<Tag>(1).Inherited[1] = func;
            }
            else if (action == Tag._PAddress) // Endereço do procedimento
            {
                ProcType proc = (ProcType)Environment.Search(env, ((IdProc)tk).Lexema);

                stk.ElementAt<Tag>(1).Inherited[1] = proc.Env.CodeAddress;
            }
            else if (action == Tag._FCall) // Chamada de função
            {
                // Destino
                FuncType func = (FuncType)action.Inherited[1];
                Label label = new Label(func.Env.CodeAddress);
                // Número de argumentos
                int n = (int)action.Inherited[0];

                IntermediateInstruction code = ic.CreateCall(label, n);
                // Console.WriteLine(code.ToString());
                env.machine.AddInstruction(code);

                stk.ElementAt<Tag>(0).Inherited[0] = func.Env.returnvalue;
            }
            else if (action == Tag._PCall) // Chamada de procedimento
            {
                // Destino
                Label label = new Label((int)action.Inherited[1]);
                // Número de argumentos
                int n = (int)action.Inherited[0];

                IntermediateInstruction code = ic.CreateCall(label, n);
                // Console.WriteLine(code.ToString());
                env.machine.AddInstruction(code);
            }
            else if (action == Tag._FirstActualPar) // Primeiro parâmetro atual
            {
                IntermediateInstruction code = ic.CreateParam((Address)action.Inherited[0]);
                // Console.WriteLine(code.ToString());
                env.machine.AddInstruction(code);

                stk.ElementAt<Tag>(0).Inherited[0] = 1; // Primeiro parametro
            }
            else if (action == Tag._NextActualPar) // Próximo parâmetro atual
            {
                IntermediateInstruction code = ic.CreateParam((Address)action.Inherited[0]);
                // Console.WriteLine(code.ToString());
                env.machine.AddInstruction(code);

                stk.ElementAt<Tag>(0).Inherited[0] = (int)action.Inherited[1] + 1;
            }
            else if (action == Tag._EndActualPar) // Lista de parâmetros atuais completa
            {
                stk.ElementAt<Tag>(1).Inherited[0] = (int)action.Inherited[0]; // Número de parâmetros
            }
            else if (action == Tag._NoArgs)
            {
                stk.ElementAt<Tag>(0).Inherited[0] = 0; // Não há parâmetros atuais
            }
            else if (action == Tag._AddOp)
            {
                if (((AddOp)tk).Value == '+')
                    stk.ElementAt<Tag>(1).Inherited[1] = Operator.ADD;
                else // if (((AddOp)tk).Value == '-')
                    stk.ElementAt<Tag>(1).Inherited[1] = Operator.SUB;
            }
            else if (action == Tag._Add)
            {
                Address x = (Address)action.Inherited[2];
                Operator op = (Operator)action.Inherited[1];
                Address y = (Address)action.Inherited[0];

                Name tmp = new Name();

                IntermediateInstruction code = ic.CreateBinary(op, tmp, x, y);
                // Console.WriteLine(code.ToString());
                env.machine.AddInstruction(code);

                stk.ElementAt<Tag>(0).Inherited[0] = tmp;
            }
            else if (action == Tag._MulOp)
            {
                if (((MulOp)tk).Value == '*')
                    stk.ElementAt<Tag>(1).Inherited[1] = Operator.MUL;
                else // if (((MulOp)tk).Value == '/')
                    stk.ElementAt<Tag>(1).Inherited[1] = Operator.DIV;
            }
            else if (action == Tag._Mul)
            {
                Address x = (Address)action.Inherited[2];
                Operator op = (Operator)action.Inherited[1];
                Address y = (Address)action.Inherited[0];

                Name tmp = new Name();

                IntermediateInstruction code = ic.CreateBinary(op, tmp, x, y);
                // Console.WriteLine(code.ToString());
                env.machine.AddInstruction(code);

                stk.ElementAt<Tag>(0).Inherited[0] = tmp;
            }
            else if (action == Tag._RelOp)
            {
                if (((RelOp)tk).Value == "<")
                    stk.ElementAt<Tag>(1).Inherited[1] = Operator.LT;
                else if (((RelOp)tk).Value == "<=")
                    stk.ElementAt<Tag>(1).Inherited[1] = Operator.LE;
                else if (((RelOp)tk).Value == ">")
                    stk.ElementAt<Tag>(1).Inherited[1] = Operator.GT;
                else if (((RelOp)tk).Value == ">=")
                    stk.ElementAt<Tag>(1).Inherited[1] = Operator.GE;
                else if (((RelOp)tk).Value == "=")
                    stk.ElementAt<Tag>(1).Inherited[1] = Operator.EQ;
                else // if (((RelOp)tk).Value == "<>")
                    stk.ElementAt<Tag>(1).Inherited[1] = Operator.NEQ;
            }
            else if (action == Tag._Rel)
            {
                Address x = (Address)action.Inherited[2];
                Operator op = (Operator)action.Inherited[1];
                Address y = (Address)action.Inherited[0];

                Name tmp = new Name();

                IntermediateInstruction code = ic.CreateBinary(op, tmp, x, y);
                // Console.WriteLine(code.ToString());
                env.machine.AddInstruction(code);

                stk.ElementAt<Tag>(0).Inherited[0] = tmp;
            }

            else if (action == Tag._Variable)
            {
                stk.ElementAt<Tag>(0).Inherited[0] = new Name(((IdVar)tk).Lexema);
            }
            else if (action == Tag._ToArray)
            {
                Name tmp = new Name();

                Address y = (Address)action.Inherited[1];
                Address i = (Address)action.Inherited[0];

                IntermediateInstruction code = ic.CreateToArray(tmp, i, y);
                // Console.WriteLine(code.ToString());
                env.machine.AddInstruction(code);

                stk.ElementAt<Tag>(1).Inherited[0] = tmp;
            }
            else if (action == Tag._FromArray)
            {
                Name tmp = new Name();

                Address y = (Address)action.Inherited[1];
                Address i = (Address)action.Inherited[0];

                IntermediateInstruction code = ic.CreateFromArray(tmp, i, y);
                // Console.WriteLine(code.ToString());
                env.machine.AddInstruction(code);

                stk.ElementAt<Tag>(1).Inherited[0] = tmp;
            }
            else if (action == Tag._LValue)
            {
                stk.ElementAt<Tag>(2).Inherited[1] = action.Inherited[0];
            }
            else if (action == Tag._Assign)
            {
                Address lValue = (Address)action.Inherited[1];
                Address rValue = (Address)action.Inherited[0];

                IntermediateInstruction code = ic.CreateCopy(lValue, rValue);
                // Console.WriteLine(code.ToString());
                env.machine.AddInstruction(code);
            }
            else if (action == Tag._RetAssign)
            {
                Address lValue = env.returnvalue;
                Address rValue = (Address)action.Inherited[0];

                if (lValue == null)
                    lValue = new Name();

                IntermediateInstruction code1 = ic.CreateCopy(lValue, rValue);
                // Console.WriteLine(code1.ToString());
                env.machine.AddInstruction(code1);

                env.returnvalue = lValue;

                IntermediateInstruction code2 = ic.CreateRetVal(lValue); // ic.CreateCopy(lValue, rValue);
                // Console.WriteLine(code2.ToString());
                env.machine.AddInstruction(code2);
            }

            else if (action == Tag._IfExp)
            {
                IntermediateInstruction code1 = ic.CreateIfTrue((Address)action.Inherited[0], null); ;
                env.machine.AddInstruction(code1);
                IntermediateInstruction code2 = ic.CreateGoto(null);
                env.machine.AddInstruction(code2);

                stk.ElementAt<Tag>(0).Inherited[0] = code1;

                stk.ElementAt<Tag>(3).Inherited[0] = code2;
            }
            else if (action == Tag._Then)
            {
                ((IntermediateInstruction)action.Inherited[0])[0].Arg2 = new Label(env.machine.Count);
                // Console.WriteLine(action.Inherited[0].ToString());
            }
            else if (action == Tag._Else)
            {
                ((IntermediateInstruction)action.Inherited[0])[0].Arg2 = new Label(env.machine.Count + 1);
                // Console.WriteLine(action.Inherited[0].ToString());

                IntermediateInstruction code = ic.CreateGoto(null);
                env.machine.AddInstruction(code);

                stk.ElementAt<Tag>(2).Inherited[0] = code;
            }
            else if (action == Tag._ExitIf)
            {
                Label exit = new Label(env.machine.Count + 1);
                ((IntermediateInstruction)action.Inherited[0])[0].Arg2 = exit;
                // Console.WriteLine(action.Inherited[0].ToString());
                IntermediateInstruction code = ic.CreateGoto(exit);
                env.machine.AddInstruction(code);
            }

            else if (action == Tag._Loop)
            {
                Label loop = new Label(env.machine.Count);
                stk.ElementAt<Tag>(5).Inherited[1] = loop;
            }
            else if (action == Tag._WhileExp)
            {
                IntermediateInstruction code1 = ic.CreateIfTrue((Address)action.Inherited[0], null); ;
                env.machine.AddInstruction(code1);
                IntermediateInstruction code2 = ic.CreateGoto(null);
                env.machine.AddInstruction(code2);

                stk.ElementAt<Tag>(1).Inherited[0] = code1;

                stk.ElementAt<Tag>(3).Inherited[0] = code2;
            }
            else if (action == Tag._Do)
            {
                ((IntermediateInstruction)action.Inherited[0])[0].Arg2 = new Label(env.machine.Count);
                // Console.WriteLine(action.Inherited[0].ToString());
            }
            else if (action == Tag._ExitWhile)
            {
                Label loop = (Label)action.Inherited[1];
                IntermediateInstruction code = ic.CreateGoto(loop);
                env.machine.AddInstruction(code);

                ((IntermediateInstruction)action.Inherited[0])[0].Arg2 = new Label(env.machine.Count);
                // Console.WriteLine(action.Inherited[0].ToString());
            }

            else if (action == Tag._MainCode)
            {
                Environment.root.EntryPoint();
            }
            else if (action == Tag._Done)
            {
                // Insere uma instrução ret no final do código do programa.
                IntermediateInstruction code = ic.CreateReturn();
                // Console.WriteLine(code.ToString());
                env.machine.AddInstruction(code);

                string str = "program " + programName + " " + env.ToString();

                Console.WriteLine(str);

                Console.WriteLine("\nPRESSIONE UMA TECLA PARA CONTINUAR...");

                Console.ReadKey();
            }

            #endregion

            else if (action == Tag._ProgramName)
            {
                programName = ((IdNew)tk).Lexema;
            }

            else if (action == Tag._Echo)
            {
                stk.ElementAt<Tag>(0).Inherited[0] = action.Inherited[0];
            }
            else
                throw new NotImplementedException("Não implementado!");
        }
    }
}
