using System;

namespace MiniPascal
{
    public class Token
    {
        public readonly Tag tag;

        public Token(Tag tag)
        {
            this.tag = tag;
        }

        public Tag TAG
        { get { return tag; } }

        public override string ToString()
        {
            return "<" + tag.ToString() + ">";
        }

        // Conversion from Token to int
        public static implicit operator int(Token t)
        { return (int) t.tag; }

        #region Teste de igualdade
        public override bool Equals(Object obj)
        {
            // If parameter is null return false
            if (obj == null)
              return false;

            // If parameter cannot be cast to Token return false
            object p = obj as Token;
            if (p == null)
              return false;

            // Return true if the fields match
            return TAG == ((Token)p).TAG;
        }

        public override int GetHashCode()
        {
            return TAG;
        }

        public static bool operator ==(Token a, Token b)
        {
            // If both are null, or both are same instance, return true
            if (Object.ReferenceEquals(a, b))
              return true;

            if ((object)a == null)
              return false;

            if (a.GetType() == b.GetType())
                return a.Equals(b);

            // Return true if the fields match
            return false;
        }

        public static bool operator !=(Token a, Token b)
        {
            return !(a == b);
        }
        #endregion

        public virtual string ToCode()
        { throw new NotImplementedException("Não implementado!"); }

        public virtual string ToLaTeX()
        { throw new NotImplementedException("Não implementado!"); }

        public static Token
            vtSharp = new Token(Tag.vtSharp), // '#'
            PROGRAM = new Token(Tag.PROGRAM), // 'program'
            // IDNEW = new Token(Tag.IDNEW), // 'idnew'
            // IDVAR = new Token(Tag.IDVAR), // 'idvar'
            // IDPROC = new Token(Tag.IDPROC), // 'idproc'
            // IDFUNC = new Token(Tag.IDFUNC), // 'idfunc'
            LPAR = new Token(Tag.LPAR), // '('
            RPAR = new Token(Tag.RPAR), // ')'
            SEMICOLON = new Token(Tag.SEMICOLON), // ';'
            DOT = new Token(Tag.DOT), // '.'
            COMMA = new Token(Tag.COMMA), // ','
            VAR = new Token(Tag.VAR), // 'var'
            COLON = new Token(Tag.COLON), // ':'
            ARRAY = new Token(Tag.ARRAY), // 'array'
            LCOL = new Token(Tag.LCOL), // '['
            // NUM = new Token(Tag.NUM), // 'num'
            RANGE = new Token(Tag.RANGE), // '..'
            RCOL = new Token(Tag.RCOL), // ']'
            OF = new Token(Tag.OF), // 'of'
            INTEGER = new Token(Tag.INTEGER), // 'integer'
            REAL = new Token(Tag.REAL), // 'real'
            FUNCTION = new Token(Tag.FUNCTION), // 'function'
            PROCEDURE = new Token(Tag.PROCEDURE), // 'procedure'
            BEGIN = new Token(Tag.BEGIN), // 'begin'
            END = new Token(Tag.END), // 'end'
            ASSIGNOP = new Token(Tag.ASSIGNOP), // 'assignop'
            IF = new Token(Tag.IF), // 'if'
            THEN = new Token(Tag.THEN), // 'then'
            ELSE = new Token(Tag.ELSE), // 'else'
            WHILE = new Token(Tag.WHILE), // 'while'
            DO = new Token(Tag.DO), // 'do'
            // RELOP = new Token(Tag.RELOP), // 'relop'
            // ADDOP = new Token(Tag.ADDOP), // 'addop'
            // MULOP = new Token(Tag.MULOP), // 'mulop'
            NOT = new Token(Tag.NOT), // 'not'
            UNKNOW = new Token(Tag.UK);
    }

    // Especialize aqui os tokens COM INFORMACOES COMPLEMENTARES conforme o exemplo comentado a seguir
    // OBS: Remova das declaracoes estaticas os Tokens estaticos COM INFORMACOES COMPLEMENTARES

    public class IdNew : Token
    {
        public string Lexema { get; private set; }

        public IdNew(string lexema) : base(Tag.IDNEW)
        {
            Lexema = lexema;
        }

        public override string ToString()
        {
            return "<" + tag.ToString() + ", " + Lexema + ">";
        }
    }

    public class IdVar : Token
    {
        public string Lexema { get; private set; }
        public AbsType IdType { get; private set; }

        public IdVar(string lexema, AbsType idType) : base(Tag.IDVAR)
        {
            Lexema = lexema;
            IdType = idType;
        }

        public override string ToString()
        {
            return "<" + tag.ToString() + ", " + Lexema + ">";
        }
    }

    public class IdProc : Token
    {
        public string Lexema { get; private set; }
        public Environment Context { get; private set; }

        public IdProc(string lexema, Environment env) : base(Tag.IDPROC)
        {
            Lexema = lexema;
            Context = new Environment(env);
        }

        public override string ToString()
        {
            return "<" + tag.ToString() + ", " + Lexema + ">";
        }
    }

    public class IdFunc : Token
    {
        public string Lexema { get; private set; }
        public Environment Context { get; private set; }

        public IdFunc(string lexema, Environment parent) : base(Tag.IDFUNC)
        {
            Lexema = lexema;
            Context = new Environment(parent);
        }

        public override string ToString()
        {
            return "<" + tag.ToString() + ", " + Lexema + ">";
        }
    }

    public class Integer : Token
    {
        public int Value { get; private set; }

        public Integer(int val) : base(Tag.NUM)
        {
            Value = val;
        }

        public override string ToString()
        {
            return "<" + tag.ToString() + ", " + Value.ToString() + ">";
        }
    }

    public class Real : Token
    {
        public double Value { get; private set; }

        public Real(double val) : base(Tag.NUM)
        {
            Value = val;
        }

        public override string ToString()
        {
            return "<" + tag.ToString() + ", " + Value.ToString() + ">";
        }
    }

    public class AddOp : Token
    {
        public char Value { get; private set; }

        public AddOp(char val) : base(Tag.ADDOP)
        {
            Value = val;
        }

        public override string ToString()
        {
            return "<" + tag.ToString() + ", " + Value.ToString() + ">";
        }
    }

    public class MulOp : Token
    {
        public char Value { get; private set; }

        public MulOp(char val) : base(Tag.MULOP)
        {
            Value = val;
        }

        public override string ToString()
        {
            return "<" + tag.ToString() + ", " + Value.ToString() + ">";
        }
    }

    public class RelOp : Token
    {
        public string Value { get; private set; }

        public RelOp(string val) : base(Tag.RELOP)
        {
            Value = val;
        }

        public override string ToString()
        {
            return "<" + tag.ToString() + ", " + Value.ToString() + ">";
        }
    }
}
