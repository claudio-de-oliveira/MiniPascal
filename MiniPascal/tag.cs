using System;

namespace MiniPascal
{
    public class Tag
    {
        protected const int VN_MASK = 0x8000;
        protected const int VT_MASK = 0x4000;
        protected const int AC_MASK = 0x2000;

        private readonly int tag;
        private readonly string name;

        private object[] inherited;

        protected Tag(int tag, string name, int iCounter = 0)
        { 
            this.tag = tag; 
            this.name = name;
            inherited = new object[iCounter];
        }

        public Tag Clone()
        {
            if (inherited.Length > 0)
                return new Tag(tag, name, inherited.Length);
            else
                return this;
        }

        public object[] Inherited
        { get { return inherited; } }


        public bool IsTerminal()
        { return (tag & VT_MASK) != 0; }
        public bool IsVariable()
        { return (tag & VN_MASK) != 0; }
        public bool IsAction()
        { return (tag & AC_MASK) != 0; }

        // Conversion from Tag to int
        public static implicit operator int(Tag t)
        { return t.tag & ~(AC_MASK | VT_MASK | VN_MASK); }

#region Teste de igualdade
        public override bool Equals(Object obj)
        {
            // If parameter is null return false
            if (obj == null)
              return false;

            // If parameter cannot be cast to Tag return false
            object p = obj as Tag;
            if ((object)p == null)
              return false;

            // Return true if the fields match
            return tag == ((Tag)p).tag;
        }

        public bool Equals(Tag p)
        {
            // If parameter is null return false
            if ((object)p == null)
              return false;

            // Return true if the fields match
            return tag == p.tag;
        }

        public override int GetHashCode()
        {
            return tag;
        }

        public static bool operator ==(Tag a, Tag b)
        {
            // If both are null, or both are same instance, return true
            if (Object.ReferenceEquals(a, b))
              return true;

            if ((object)a == null)
              return false;

            // Return true if the fields match
            return a.Equals(b);
        }

        public static bool operator !=(Tag a, Tag b)
        {
            return !(a == b);
        }
#endregion

        public override string ToString()
        { return name; }

        public static Tag
            vtSharp = new Tag(VT_MASK | 0, "'#'"),
            PROGRAM = new Tag(VT_MASK | 1, "'program'"),
            IDNEW = new Tag(VT_MASK | 2, "'idnew'"),
            IDVAR = new Tag(VT_MASK | 3, "'idvar'"),
            IDPROC = new Tag(VT_MASK | 4, "'idproc'"),
            IDFUNC = new Tag(VT_MASK | 5, "'idfunc'"),
            LPAR = new Tag(VT_MASK | 6, "'('"),
            RPAR = new Tag(VT_MASK | 7, "')'"),
            SEMICOLON = new Tag(VT_MASK | 8, "';'"),
            DOT = new Tag(VT_MASK | 9, "'.'"),
            COMMA = new Tag(VT_MASK | 10, "','"),
            VAR = new Tag(VT_MASK | 11, "'var'"),
            COLON = new Tag(VT_MASK | 12, "':'"),
            ARRAY = new Tag(VT_MASK | 13, "'array'"),
            LCOL = new Tag(VT_MASK | 14, "'['"),
            NUM = new Tag(VT_MASK | 15, "'num'"),
            RANGE = new Tag(VT_MASK | 16, "'..'"),
            RCOL = new Tag(VT_MASK | 17, "']'"),
            OF = new Tag(VT_MASK | 18, "'of'"),
            INTEGER = new Tag(VT_MASK | 19, "'integer'"),
            REAL = new Tag(VT_MASK | 20, "'real'"),
            FUNCTION = new Tag(VT_MASK | 21, "'function'"),
            PROCEDURE = new Tag(VT_MASK | 22, "'procedure'"),
            BEGIN = new Tag(VT_MASK | 23, "'begin'"),
            END = new Tag(VT_MASK | 24, "'end'"),
            ASSIGNOP = new Tag(VT_MASK | 25, "'assignop'"),
            IF = new Tag(VT_MASK | 26, "'if'"),
            THEN = new Tag(VT_MASK | 27, "'then'"),
            ELSE = new Tag(VT_MASK | 28, "'else'"),
            WHILE = new Tag(VT_MASK | 29, "'while'"),
            DO = new Tag(VT_MASK | 30, "'do'"),
            RELOP = new Tag(VT_MASK | 31, "'relop'"),
            ADDOP = new Tag(VT_MASK | 32, "'addop'"),
            MULOP = new Tag(VT_MASK | 33, "'mulop'"),
            NOT = new Tag(VT_MASK | 34, "'not'"),
            // SHARP = new Tag(VT_MASK | 35, "#"),
            UK = new Tag(VT_MASK | 36, "UNKNOW");

        public static Tag
            program = new Tag(VN_MASK | 0, "<program>", 0),
            identifier_list = new Tag(VN_MASK | 1, "<identifier_list>", 0),
            identifier_list_ = new Tag(VN_MASK | 2, "<identifier_list'>", 1),
            declarations = new Tag(VN_MASK | 3, "<declarations>", 0),
            type = new Tag(VN_MASK | 4, "<type>", 0),
            standard_type = new Tag(VN_MASK | 5, "<standard_type>", 0),
            subprogram_declarations = new Tag(VN_MASK | 6, "<subprogram_declarations>", 0),
            subprogram_declaration = new Tag(VN_MASK | 7, "<subprogram_declaration>", 0),
            subprogram_head = new Tag(VN_MASK | 8, "<subprogram_head>", 0),
            arguments = new Tag(VN_MASK | 9, "<arguments>", 0),
            parameter_list = new Tag(VN_MASK | 10, "<parameter_list>", 0),
            parameter_list_ = new Tag(VN_MASK | 11, "<parameter_list'>", 0),
            compound_statement = new Tag(VN_MASK | 12, "<compound_statement>", 0),
            optional_statements = new Tag(VN_MASK | 13, "<optional_statements>", 0),
            statement_list = new Tag(VN_MASK | 14, "<statement_list>", 0),
            statement_list_ = new Tag(VN_MASK | 15, "<statement_list'>", 1),
            statement = new Tag(VN_MASK | 16, "<statement>", 0),
            variable = new Tag(VN_MASK | 17, "<variable>", 0),
            variable_ = new Tag(VN_MASK | 18, "<variable'>", 1),
            procedure_statement = new Tag(VN_MASK | 19, "<procedure_statement>", 0),
            procedure_statement_ = new Tag(VN_MASK | 20, "<procedure_statement'>", 0),
            expression_list = new Tag(VN_MASK | 21, "<expression_list>", 0),
            expression_list_ = new Tag(VN_MASK | 22, "<expression_list'>", 1),

            expression = new Tag(VN_MASK | 23, "<expression>", 0),
            expression_ = new Tag(VN_MASK | 24, "<expression'>", 1),
            simple_expression = new Tag(VN_MASK | 25, "<simple_expression>", 0),
            simple_expression_ = new Tag(VN_MASK | 26, "<simple_expression'>", 1),
            term = new Tag(VN_MASK | 27, "<term>", 0),
            term_ = new Tag(VN_MASK | 28, "<term'>", 1),
            factor = new Tag(VN_MASK | 29, "<factor>", 0),
            factor_ = new Tag(VN_MASK | 30, "<factor'>", 1),
            varexp = new Tag(VN_MASK | 31, "<varexp>", 1)
            ;

        public static Tag
            _Begin = new Tag(AC_MASK | 1, "@Begin", 0),
            _End = new Tag(AC_MASK | 2, "@End", 0),
            _IdFunc = new Tag(AC_MASK | 3, "@IdFunc", 0),
            _FuncDec = new Tag(AC_MASK | 4, "@FuncDec", 2),
            _IdProc = new Tag(AC_MASK | 5, "@IdProc", 0),
            _ProcDec = new Tag(AC_MASK | 6, "@ProcDec", 1),
            _IdVar = new Tag(AC_MASK | 7, "@IdVar", 1),
            _VarDec = new Tag(AC_MASK | 8, "@VarDec", 2),
            _CreateList = new Tag(AC_MASK | 9, "@CreateList", 0),
            _InsertList = new Tag(AC_MASK | 10, "@InsertList", 1),
            _BeginRange = new Tag(AC_MASK | 11, "@BeginRange", 0),
            _EndRange = new Tag(AC_MASK | 12, "@EndRange", 0),
            _ArrayDec = new Tag(AC_MASK | 13, "@ArrayDec", 3),
            _Integer = new Tag(AC_MASK | 14, "@Integer", 0),
            _Real = new Tag(AC_MASK | 15, "@Real", 0),
            _Array = new Tag(AC_MASK | 16, "@Array", 3),
            _EndParList = new Tag(AC_MASK | 17, "@EndParList", 1),
            _ParDec = new Tag(AC_MASK | 18, "@ParDec", 2),
            _Args = new Tag(AC_MASK | 19, "@Args", 1),
            _EnvRestore = new Tag(AC_MASK | 20, "@EnvRestore", 0),
            _Number = new Tag(AC_MASK | 30, "@Number", 0),
            _Not = new Tag(AC_MASK | 31, "@Number", 1),
            _RValue = new Tag(AC_MASK | 32, "@RValue", 1),
            _Skip1 = new Tag(AC_MASK | 33, "@Skip1", 1),
            _FCall = new Tag(AC_MASK | 34, "@FCall", 2),
            _PCall = new Tag(AC_MASK | 35, "@PCall", 2),
            _FAddress = new Tag(AC_MASK | 36, "@FAddress", 0),
            _PAddress = new Tag(AC_MASK | 37, "@PAddress", 0),
            _FirstActualPar = new Tag(AC_MASK | 38, "@FirstActualPar", 1),
            _NextActualPar = new Tag(AC_MASK | 39, "@NextActualPar", 2),
            _EndActualPar = new Tag(AC_MASK | 40, "@EndActualPar", 1),
            _NoArgs = new Tag(AC_MASK | 41, "@NoArgs", 0),
            _AddOp = new Tag(AC_MASK | 42, "@AddOp", 0),
            _Add = new Tag(AC_MASK | 43, "@Add", 3),
            _MulOp = new Tag(AC_MASK | 44, "@MulOp", 0),
            _Mul = new Tag(AC_MASK | 45, "@Mul", 3),
            _RelOp = new Tag(AC_MASK | 46, "@RelOp", 0),
            _Rel = new Tag(AC_MASK | 47, "@Rel", 3),

            _Variable = new Tag(AC_MASK | 48, "@Variable", 0),
            _ToArray = new Tag(AC_MASK | 49, "@Indexed", 2),
            _LValue = new Tag(AC_MASK | 50, "@LValue", 1),
            _Assign = new Tag(AC_MASK | 51, "@Assign", 2),
            _RetAssign = new Tag(AC_MASK | 52, "@RetAssign", 2),
            _FromArray = new Tag(AC_MASK | 53, "@FromArray", 2),

            _IfExp = new Tag(AC_MASK | 60, "@IfExp", 1),
            _Then = new Tag(AC_MASK | 61, "@Then", 1),
            _Else = new Tag(AC_MASK | 62, "@Else", 1),
            _ExitIf = new Tag(AC_MASK | 63, "@ExitIf", 1),

            _WhileExp = new Tag(AC_MASK | 70, "@WhileExp", 1),
            _Do = new Tag(AC_MASK | 71, "@Do", 1),
            _ExitWhile = new Tag(AC_MASK | 72, "@ExitWhile", 2),
            _Loop = new Tag(AC_MASK | 73, "@Loop", 0),

            _MainCode = new Tag(AC_MASK | 90, "@MainCode", 0),
            _Done = new Tag(AC_MASK | 91, "@Done", 0),
            _ProgramName = new Tag(AC_MASK | 92, "@ProgramName", 0),

            _Echo = new Tag(AC_MASK | 99, "@Echo", 1);
    }
}
