using System;
using System.Collections.Generic;

namespace MiniPascal
{
    class Scanner
    {
        public int ErrorLine { get; private set; }

        private Dictionary<string, Token> reserved_words;

        public Scanner()
        {
            reserved_words = new Dictionary<string, Token>();

            reserved_words.Add("program", new Token(Tag.PROGRAM));
            reserved_words.Add("var", new Token(Tag.VAR));
            reserved_words.Add("array", new Token(Tag.ARRAY));
            reserved_words.Add("of", new Token(Tag.OF));
            reserved_words.Add("integer", new Token(Tag.INTEGER));
            reserved_words.Add("real", new Token(Tag.REAL));
            reserved_words.Add("function", new Token(Tag.FUNCTION));
            reserved_words.Add("procedure", new Token(Tag.PROCEDURE));
            reserved_words.Add("begin", new Token(Tag.BEGIN));
            reserved_words.Add("end", new Token(Tag.END));
            reserved_words.Add("if", new Token(Tag.IF));
            reserved_words.Add("then", new Token(Tag.THEN));
            reserved_words.Add("else", new Token(Tag.ELSE));
            reserved_words.Add("while", new Token(Tag.WHILE));
            reserved_words.Add("do", new Token(Tag.DO));
            reserved_words.Add("not", new Token(Tag.NOT));
        }

        public Token NextToken(string txt, Environment env, ref int pos)
        {
            ErrorLine = 1;

            // I M P L E M E N T A R
            // throw new NotImplementedException("Não implementado!");

            int state = 0;
            string lexema = "";
            int integer_part = 0;
            double real_part = 0.0;
            int divisor = 10;

            while (true)
            {
                switch (state)
                {
                    case 0:
                        if (txt[pos] == '\n')
                        {
                            ErrorLine++;
                            state = 0;
                            pos++;
                            break;
                        }
                        if (Char.IsWhiteSpace(txt[pos]))
                        {
                            state = 0;
                            pos++;
                            break;
                        }
                        // 'id' = ID 
                        if (Char.IsLetter(txt[pos]))
                        {
                            lexema = "" + txt[pos];
                            state = 1;
                            pos++;
                            break;
                        }
                        // 'num' = NUM 
                        if (Char.IsDigit(txt[pos]))
                        {
                            integer_part = txt[pos] - '0';
                            state = 5;
                            pos++;
                            break;
                        }

                        // '(' = LPAR 
                        if (txt[pos] == '(')
                        {
                            state = 20;
                            pos++;
                            break;
                        }
                        // ')' = RPAR 
                        if (txt[pos] == ')')
                        {
                            state = 21;
                            pos++;
                            break;
                        }
                        // ';' = SEMICOLON 
                        if (txt[pos] == ';')
                        {
                            state = 22;
                            pos++;
                            break;
                        }
                        // ',' = COMMA 
                        if (txt[pos] == ',')
                        {
                            state = 23;
                            pos++;
                            break;
                        }
                        // '[' = LCOL 
                        if (txt[pos] == '[')
                        {
                            state = 24;
                            pos++;
                            break;
                        }
                        // ']' = RCOL 
                        if (txt[pos] == ']')
                        {
                            state = 25;
                            pos++;
                            break;
                        }
                        // 'addop' = ADDOP 
                        if (txt[pos] == '+')
                        {
                            state = 26;
                            pos++;
                            break;
                        }
                        if (txt[pos] == '-')
                        {
                            state = 27;
                            pos++;
                            break;
                        }
                        // 'mulop' = MULOP 
                        if (txt[pos] == '*')
                        {
                            state = 28;
                            pos++;
                            break;
                        }
                        if (txt[pos] == '/')
                        {
                            state = 29;
                            pos++;
                            break;
                        }

                        // 'relop' = RELOP 
                        if (txt[pos] == '=')
                        {
                            lexema = "" + txt[pos];
                            state = 40;
                            pos++;
                            break;
                        }
                        if (txt[pos] == '<')
                        {
                            lexema = "" + txt[pos];
                            state = 41;
                            pos++;
                            break;
                        }
                        if (txt[pos] == '>')
                        {
                            lexema = "" + txt[pos];
                            state = 42;
                            pos++;
                            break;
                        }
                        if (txt[pos] == '#')
                        {
                            state = 90;
                            pos++;
                            break;
                        }

                        // ':' = COLON 
                        // 'assignop' = ASSIGNOP 
                        if (txt[pos] == ':')
                        {
                            state = 50;
                            pos++;
                            break;
                        }

                        // '.' = DOT 
                        // '..' = RANGE 
                        if (txt[pos] == '.')
                        {
                            state = 60;
                            pos++;
                            break;
                        }

                        // ERROR
                        pos++;
                        state = 99;
                        break;

                    case 1:
                        // 'id' = ID 
                        // 'idproc' = IDPROC 
                        if (Char.IsLetterOrDigit(txt[pos]))
                        {
                            lexema += txt[pos];
                            state = 1;
                            pos++;
                            break;
                        }
                        state = 2;
                        pos++;
                        break;

                    case 2:
                        pos--; // RETRACT
                        if (reserved_words.ContainsKey(lexema.ToLower()))
                            return (Token)reserved_words[lexema.ToLower()];

                        AbsType obj;

                        if (env.SentencaImperativa)
                        {
                            obj = Environment.Search(env, lexema);

                            if (obj == null)
                                throw new NotImplementedException("O Id deve existir!");

                            if (obj.IdType == EnumType.INTEGER)
                                return new IdVar(lexema, obj);
                            if (obj.IdType == EnumType.REAL)
                                return new IdVar(lexema, obj);
                            if (obj.IdType == EnumType.ARRAY)
                                return new IdVar(lexema, obj);
                            if (obj.IdType == EnumType.FUNC)
                                return new IdFunc(lexema, env);
                            if (obj.IdType == EnumType.PROC)
                                return new IdProc(lexema, env);

                            throw new NotImplementedException("Não foi possível identificar o tipo de " + lexema);
                        }
                        else // Sentença declarativa
                        {
                            obj = Environment.SearchLocal(env, lexema);

                            if (obj != null)
                                throw new NotImplementedException("O Id deve ser novo!");

                            /////////////////////////////
                            // NOTA: O Scanner não vai adicionar o identificador novo na tabela de símbolos corrente.
                            // Alguns autores fazem assim, mas nós deixaremos isso para o semântico.

                            return new IdNew(lexema);
                        }

                    case 5:
                        // 'num' = NUM 
                        if (Char.IsDigit(txt[pos]))
                        {
                            integer_part = (integer_part * 10) + (txt[pos] - '0');
                            state = 5;
                            pos++;
                            break;
                        }
                        if (txt[pos] == '.')
                        {
                            state = 6;
                            pos++;
                            break;
                        }
                        state = 8;
                        pos++;
                        break;

                    case 6:
                        if (Char.IsDigit(txt[pos]))
                        {
                            real_part += (txt[pos] - '0') / divisor;
                            divisor *= 10;
                            state = 7;
                            pos++;
                            break;
                        }
                        state = 10;
                        pos++;
                        break;

                    case 7:
                        if (Char.IsDigit(txt[pos]))
                        {
                            real_part += (txt[pos] - '0') / divisor;
                            divisor *= 10;
                            state = 7;
                            pos++;
                            break;
                        }
                        state = 8;
                        pos++;
                        break;

                    case 8:
                        pos--; // RETRACT
                        return new Integer(integer_part);

                    case 9:
                        pos--; // RETRACT
                        return new Real(integer_part + real_part);

                    case 10:
                        pos--; // RETRACT
                        pos--; // RETRACT
                        return new Integer(integer_part);

                    case 20:
                        return Token.LPAR;

                    case 21:
                        return Token.RPAR;

                    case 22:
                        return Token.SEMICOLON;

                    case 23:
                        return Token.COMMA;

                    case 24:
                        return Token.LCOL;

                    case 25:
                        return Token.RCOL;

                    case 26:
                        return new AddOp('+');

                    case 27:
                        return new AddOp('-');

                    case 28:
                        return new MulOp('*');

                    case 29:
                        return new MulOp('/');

                    case 40:
                        // RELOP 
                        return new RelOp(lexema);

                    case 41:
                        if (txt[pos] == '=')
                        {
                            lexema += txt[pos];
                            state = 40;
                            pos++;
                            break;
                        }
                        if (txt[pos] == '>')
                        {
                            lexema += txt[pos];
                            state = 40;
                            pos++;
                            break;
                        }
                        pos--; // RETRACT
                        state = 40;
                        break;

                    case 42:
                        if (txt[pos] == '=')
                        {
                            lexema += txt[pos];
                            state = 40;
                            pos++;
                            break;
                        }
                        // pos--; // RETRACT
                        state = 40;
                        break;

                    case 50:
                        if (txt[pos] == '=')
                        {
                            state = 52;
                            pos++;
                            break;
                        }
                        pos++;
                        state = 51;
                        break;

                    case 51:
                        pos--; // RETRACT
                        return Token.COLON;

                    case 52:
                        return Token.ASSIGNOP;

                    case 60:
                        // '.' = DOT 
                        // '..' = RANGE 
                        if (txt[pos] == '.')
                        {
                            state = 62;
                            pos++;
                            break;
                        }
                        pos++;
                        state = 61;
                        break;

                    case 61:
                        pos--; // RETRACT
                        return Token.DOT;

                    case 62:
                        return Token.RANGE;

                    case 90:
                        return Token.vtSharp;

                    case 99:
                        pos--; // RETRACT
                        return Token.UNKNOW;

                }
            }
        }
    }
}
