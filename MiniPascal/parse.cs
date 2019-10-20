using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniPascal
{
    public class LL1_Parser
    {
        #region LL(1) Parse Tables
        public static Tag[][] RHS = new Tag[][] {
            // 0.	<program> ::= 'program' 'idnew' '(' <identifier_list> @Args ')' ';' <declarations> <subprogram_declarations> <compound_statement> '.' '#' .
            new Tag[] {Tag.PROGRAM, Tag.IDNEW, Tag.LPAR, Tag.identifier_list, Tag._Args, Tag.RPAR, Tag.SEMICOLON, Tag.declarations, Tag.subprogram_declarations, Tag.compound_statement, Tag.DOT, Tag.vtSharp, },
            // 1.	<identifier_list> ::= 'idnew' @CreateList <identifier_list'> .
            new Tag[] {Tag.IDNEW, Tag._CreateList, Tag.identifier_list_, },
            // 2.	<identifier_list'> ::= @Echo .
            new Tag[] { Tag._Echo, },
            // 3.	<identifier_list'> ::= ',' 'idnew' @InsertList <identifier_list'> .
            new Tag[] {Tag.COMMA, Tag.IDNEW, Tag._InsertList, Tag.identifier_list_, },
            // 4.	<declarations> ::=  .
            new Tag[] {},
            // 5.	<declarations> ::= 'var' <identifier_list> @IdVar ':' <type> @VarDec  ';' <declarations> .
            new Tag[] {Tag.VAR, Tag.identifier_list, Tag._IdVar, Tag.COLON, Tag.type, Tag._VarDec, Tag.SEMICOLON, Tag.declarations, },
            // 6.	<type> ::= <standard_type> .
            new Tag[] {Tag.standard_type, },
            // 7.	<type> ::= 'array' '[' 'num' @BeginRange '..' 'num' @EndRange ']' 'of' <standard_type> @Array .
            new Tag[] {Tag.ARRAY, Tag.LCOL, Tag.NUM, Tag._BeginRange, Tag.RANGE, Tag.NUM, Tag._EndRange, Tag.RCOL, Tag.OF, Tag.standard_type, Tag._ArrayDec, },
            // 8.	<standard_type> ::= 'integer' @Integer .
            new Tag[] {Tag.INTEGER, Tag._Integer, },
            // 9.	<standard_type> ::= 'real' @Real .
            new Tag[] {Tag.REAL, Tag._Real, },
            // 10.	<subprogram_declarations> ::=  .
            new Tag[] {},
            // 11.	<subprogram_declarations> ::= <subprogram_declaration> ';' <subprogram_declarations> .
            new Tag[] {Tag.subprogram_declaration, Tag.SEMICOLON, Tag.subprogram_declarations, },
            // 12.	<subprogram_declaration> ::= <subprogram_head> <declarations> <compound_statement> @EnvRestore .
            new Tag[] {Tag.subprogram_head, Tag.declarations, Tag.compound_statement, Tag._EnvRestore, },
            // 13.	<subprogram_head> ::= 'function' 'idnew' @IdFunc <arguments> ':' <standard_type> @FuncDec ';' .
            new Tag[] {Tag.FUNCTION, Tag.IDNEW, Tag._IdFunc, Tag.arguments, Tag.COLON, Tag.standard_type, Tag._FuncDec, Tag.SEMICOLON, },
            // 14.	<subprogram_head> ::= 'procedure' 'idnew' @IdProc <arguments> @ProcDec ';' .
            new Tag[] {Tag.PROCEDURE, Tag.IDNEW, Tag._IdProc, Tag.arguments, Tag._ProcDec, Tag.SEMICOLON, },
            // 15.	<arguments> ::=  .
            new Tag[] {},
            // 16.	<arguments> ::= '(' <parameter_list> ')' .
            new Tag[] {Tag.LPAR, Tag.parameter_list, Tag.RPAR, },
            // 17.	<parameter_list> ::= 'idnew' @CreateList <identifier_list'> @EndParList ':' <type> @ParDec <parameter_list'> .
            new Tag[] {Tag.IDNEW, Tag._CreateList, Tag.identifier_list_, Tag._EndParList, Tag.COLON, Tag.type, Tag._ParDec, Tag.parameter_list_, },
            // 18.	<parameter_list'> ::= .
            new Tag[] {},
            // 19.	<parameter_list'> ::= ';' <identifier_list> @EndParList ':' <type> @ParDec <parameter_list'> .
            new Tag[] {Tag.SEMICOLON, Tag.identifier_list, Tag._EndParList, Tag.COLON, Tag.type, Tag._ParDec, Tag.parameter_list_, },
            // 20.	<compound_statement> ::= @Begin 'begin' <optional_statements> @End 'end' .
            new Tag[] {Tag._Begin, Tag.BEGIN, Tag.optional_statements, Tag._End, Tag.END, },
            // 21.	<optional_statements> ::=  .
            new Tag[] {},
            // 22.	<optional_statements> ::= <statement_list> .
            new Tag[] {Tag.statement_list, },
            // 23.	<statement_list> ::= <statement> <statement_list'> .
            new Tag[] {Tag.statement, Tag.statement_list_, },
            // 24.	<statement_list'> ::=  .
            new Tag[] {},
            // 25.	<statement_list'> ::= ';' <statement> <statement_list'> .
            new Tag[] {Tag.SEMICOLON, Tag.statement, Tag.statement_list_, },
            // 26.	<statement> ::= <variable> 'assignop' <expression> .
            new Tag[] {Tag.variable, Tag.ASSIGNOP, Tag.expression, },
            // 27.	<statement> ::= 'idfunc' 'assignop' <expression> .
            new Tag[] {Tag.IDFUNC, Tag.ASSIGNOP, Tag.expression, },
            // 28.	<statement> ::= <procedure_statement> .
            new Tag[] {Tag.procedure_statement, },
            // 29.	<statement> ::= 'begin' <optional_statements> 'end' .
            new Tag[] {Tag.BEGIN, Tag.optional_statements, Tag.END, },
            // 30.	<statement> ::= 'if' <expression> 'then' <statement> 'else' <statement> .
            new Tag[] {Tag.IF, Tag.expression, Tag.THEN, Tag.statement, Tag.ELSE, Tag.statement, },
            // 31.	<statement> ::= 'while' <expression> 'do' <statement> .
            new Tag[] {Tag.WHILE, Tag.expression, Tag.DO, Tag.statement, },
            // 32.	<variable> ::= 'idvar' <variable'> .
            new Tag[] {Tag.IDVAR, Tag.variable_, },
            // 33.	<variable'> ::=  .
            new Tag[] {},
            // 34.	<variable'> ::= '[' <simple_expression> ']' .
            new Tag[] {Tag.LCOL, Tag.simple_expression, Tag.RCOL, },
            // 35.	<procedure_statement> ::= 'idproc' <procedure_statement'> .
            new Tag[] {Tag.IDPROC, Tag.procedure_statement_, },
            // 36.	<procedure_statement'> ::=  .
            new Tag[] {},
            // 37.	<procedure_statement'> ::= '(' <expression_list> ')' .
            new Tag[] {Tag.LPAR, Tag.expression_list, Tag.RPAR, },
            // 38.	<expression_list> ::= <expression> <expression_list'> .
            new Tag[] {Tag.expression, Tag.expression_list_, },
            // 39.	<expression_list'> ::=  .
            new Tag[] {},
            // 40.	<expression_list'> ::= ',' <expression> <expression_list'> .
            new Tag[] {Tag.COMMA, Tag.expression, Tag.expression_list_, },
            // 41.	<expression> ::= <simple_expression> <expression'> .
            new Tag[] {Tag.simple_expression, Tag.expression_, },
            // 42.	<expression'> ::=  .
            new Tag[] {},
            // 43.	<expression'> ::= 'relop' <simple_expression> .
            new Tag[] {Tag.RELOP, Tag.simple_expression, },
            // 44.	<simple_expression> ::= <term> <simple_expression'> .
            new Tag[] {Tag.term, Tag.simple_expression_, },
            // 45.	<simple_expression'> ::=  .
            new Tag[] {},
            // 46.	<simple_expression'> ::= 'addop' <term> <simple_expression'> .
            new Tag[] {Tag.ADDOP, Tag.term, Tag.simple_expression_, },
            // 47.	<term> ::= <factor> <term'> .
            new Tag[] {Tag.factor, Tag.term_, },
            // 48.	<term'> ::=  .
            new Tag[] {},
            // 49.	<term'> ::= 'mulop' <factor> <term'> .
            new Tag[] {Tag.MULOP, Tag.factor, Tag.term_, },
            // 50.	<factor> ::= 'idfunc' <factor'> .
            new Tag[] {Tag.IDFUNC, Tag.factor_, },
            // 51.	<factor> ::= 'num' .
            new Tag[] {Tag.NUM, },
            // 52.	<factor> ::= 'idvar' <variable'> .
            new Tag[] {Tag.IDVAR, Tag.variable_, },
            // 53.	<factor> ::= '(' <expression> ')' .
            new Tag[] {Tag.LPAR, Tag.expression, Tag.RPAR, },
            // 54.	<factor> ::= 'not' <factor> .
            new Tag[] {Tag.NOT, Tag.factor, },
            // 55.	<factor'> ::=  .
            new Tag[] {},
            // 56.	<factor'> ::= '(' <expression_list> ')' .
            new Tag[] {Tag.LPAR, Tag.expression_list, Tag.RPAR, },
        };

        public static int[][] M = new int[][] {
            // 0.	<program> ::= 'program' 'idnew' '(' <identifier_list> ')' ';' <declarations> <subprogram_declarations> <compound_statement> '.' '#' .
            new int[] {-1, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
            // 1.	<identifier_list> ::= 'idnew' <identifier_list'> .
            new int[] {-1, -1, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
            // 2.	<identifier_list'> ::=  .
            // 3.	<identifier_list'> ::= ',' 'idnew' <identifier_list'> .
            new int[] {-1, -1, -1, -1, -1, -1, -1, 2, -1, -1, 3, -1, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
            // 4.	<declarations> ::=  .
            // 5.	<declarations> ::= 'var' <identifier_list> ':' <type> ';' <declarations> .
            new int[] {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, 4, 4, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
            // 6.	<type> ::= <standard_type> .
            // 7.	<type> ::= 'array' '[' 'num' '..' 'num' ']' 'of' <standard_type> @Array .
            new int[] {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 7, -1, -1, -1, -1, -1, 6, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
            // 8.	<standard_type> ::= 'integer' .
            // 9.	<standard_type> ::= 'real' .
            new int[] {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
            // 10.	<subprogram_declarations> ::=  .
            // 11.	<subprogram_declarations> ::= <subprogram_declaration> ';' <subprogram_declarations> .
            new int[] {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, 11, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
            // 12.	<subprogram_declaration> ::= <subprogram_head> <declarations> <compound_statement> .
            new int[] {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 12, 12, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
            // 13.	<subprogram_head> ::= 'function' 'idnew' <arguments> ':' <standard_type> ';' .
            // 14.	<subprogram_head> ::= 'procedure' 'idnew' <arguments> ';' .
            new int[] {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 13, 14, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
            // 15.	<arguments> ::=  .
            // 16.	<arguments> ::= '(' <parameter_list> ')' .
            new int[] {-1, -1, -1, -1, -1, -1, 16, -1, 15, -1, -1, -1, 15, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
            // 17.	<parameter_list> ::= 'idnew' <identifier_list'> ':' <type> <parameter_list'> .
            new int[] {-1, -1, 17, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
            // 18.	<parameter_list'> ::=  .
            // 19.	<parameter_list'> ::= ';' <identifier_list> ':' <type> <parameter_list'> .
            new int[] {-1, -1, -1, -1, -1, -1, -1, 18, 19, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
            // 20.	<compound_statement> ::= 'begin' <optional_statements> 'end' .
            new int[] {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 20, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
            // 21.	<optional_statements> ::=  .
            // 22.	<optional_statements> ::= <statement_list> .
            new int[] {-1, -1, -1, 22, 22, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 22, 21, -1, 22, -1, -1, 22, -1, -1, -1, -1, -1, },
            // 23.	<statement_list> ::= <statement> <statement_list'> .
            new int[] {-1, -1, -1, 23, 23, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 23, -1, -1, 23, -1, -1, 23, -1, -1, -1, -1, -1, },
            // 24.	<statement_list'> ::=  .
            // 25.	<statement_list'> ::= ';' <statement> <statement_list'> .
            new int[] {-1, -1, -1, -1, -1, -1, -1, -1, 25, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 24, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
            // 26.	<statement> ::= <variable> 'assignop' <expression> .
            // 27.	<statement> ::= 'idfunc' 'assignop' <expression> .
            // 28.	<statement> ::= <procedure_statement> .
            // 29.	<statement> ::= 'begin' <optional_statements> 'end' .
            // 30.	<statement> ::= 'if' <expression> 'then' <statement> 'else' <statement> .
            // 31.	<statement> ::= 'while' <expression> 'do' <statement> .
            new int[] {-1, -1, -1, 26, 28, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 29, -1, -1, 30, -1, -1, 31, -1, -1, -1, -1, -1, },
            // 32.	<variable> ::= 'idvar' <variable'> .
            new int[] {-1, -1, -1, 32, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
            // 33.	<variable'> ::=  .
            // 34.	<variable'> ::= '[' <simple_expression> ']' .
            new int[] {-1, -1, -1, -1, -1, -1, -1, 33, 33, -1, 33, -1, -1, -1, 34, -1, -1, 33, -1, -1, -1, -1, -1, -1, 33, 33, -1, 33, 33, -1, 33, 33, 33, 33, -1, },
            // 35.	<procedure_statement> ::= 'idproc' <procedure_statement'> .
            new int[] {-1, -1, -1, -1, 35, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
            // 36.	<procedure_statement'> ::=  .
            // 37.	<procedure_statement'> ::= '(' <expression_list> ')' .
            new int[] {-1, -1, -1, -1, -1, -1, 37, -1, 36, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 36, -1, -1, -1, 36, -1, -1, -1, -1, -1, -1, },
            // 38.	<expression_list> ::= <expression> <expression_list'> .
            new int[] {-1, -1, -1, 38, -1, 38, 38, -1, -1, -1, -1, -1, -1, -1, -1, 38, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 38, },
            // 38.	<expression_list'> ::=  .
            // 39.	<expression_list'> ::= ',' <expression> <expression_list'> .
            new int[] {-1, -1, -1, -1, -1, -1, -1, 39, -1, -1, 40, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
            // 41.	<expression> ::= <simple_expression> <expression'> .
            new int[] {-1, -1, -1, 41, -1, 41, 41, -1, -1, -1, -1, -1, -1, -1, -1, 41, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 41, },
            // 42.	<expression'> ::=  .
            // 43.	<expression'> ::= 'relop' <simple_expression> .
            new int[] {-1, -1, -1, -1, -1, -1, -1, 42, 42, -1, 42, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 42, -1, -1, 42, 42, -1, 42, 43, -1, -1, -1, },
            // 44.	<simple_expression> ::= <term> <simple_expression'> .
            new int[] {-1, -1, -1, 44, -1, 44, 44, -1, -1, -1, -1, -1, -1, -1, -1, 44, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 44, },
            // 45.	<simple_expression'> ::=  .
            // 46.	<simple_expression'> ::= 'addop' <term> <simple_expression'> .
            new int[] {-1, -1, -1, -1, -1, -1, -1, 45, 45, -1, 45, -1, -1, -1, -1, -1, -1, 45, -1, -1, -1, -1, -1, -1, 45, -1, -1, 45, 45, -1, 45, 45, 46, -1, -1, },
            // 47.	<term> ::= <factor> <term'> .
            new int[] {-1, -1, -1, 47, -1, 47, 47, -1, -1, -1, -1, -1, -1, -1, -1, 47, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 47, },
            // 48.	<term'> ::=  .
            // 49.	<term'> ::= 'mulop' <factor> <term'> .
            new int[] {-1, -1, -1, -1, -1, -1, -1, 48, 48, -1, 48, -1, -1, -1, -1, -1, -1, 48, -1, -1, -1, -1, -1, -1, 48, -1, -1, 48, 48, -1, 48, 48, 48, 49, -1, },
            // 50.	<factor> ::= 'idfunc' <factor'> .
            // 51.	<factor> ::= 'num' .
            // 52.	<factor> ::= 'idvar' <variable'> .
            // 53.	<factor> ::= '(' <expression> ')' .
            // 54.	<factor> ::= 'not' <factor> .
            new int[] {-1, -1, -1, 52, -1, 50, 53, -1, -1, -1, -1, -1, -1, -1, -1, 51, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 54, },
            // 55.	<factor'> ::=  .
            // 56.	<factor'> ::= '(' <expression_list> ')' .
            new int[] {-1, -1, -1, -1, -1, -1, 56, 55, 55, -1, 55, -1, -1, -1, -1, -1, -1, 55, -1, -1, -1, -1, -1, -1, 55, -1, -1, 55, 55, -1, 55, 55, 55, 55, -1, },
        };
        #endregion

        private int line;

        public int ErrorLine
        { get { return line; } }

        private static void PushRHS(Stack<Tag> stk, Tag[] rhs)
        {
            for (int i = rhs.Length - 1; i >= 0; i--)
                stk.Push(rhs[i]);
        }

        public bool Parse(string text)
        {
            Scanner lex = new Scanner();
            Semantic sem = new Semantic();

            text += "#";

            int pos = 0;
            Stack<Tag> stk = new Stack<Tag>();

            // Criação da Tabela de Símbolos (Principal)
            Environment simbolTable = new Environment(null);

            // Local para introduzir símbolos globais
            // ....

            Token previous = null;

            Token current = lex.NextToken(text, simbolTable, ref pos);

            Console.WriteLine(current.ToString());

            PushRHS(stk, RHS[0]);

            while (true)
            {
                Tag A = stk.Pop();

                if (A.IsVariable())
                {
                    int rule = M[(int)A][(int)current.TAG];

                    if (rule == -1)
                    {
                        line = lex.ErrorLine;
                        return false;
                    }

                    PushRHS(stk, RHS[rule]);

                    // Attributes adjustment
                    switch (rule)
                    {
                        // 2.	<identifier_list'> ::= @Echo .
                        case 2:
                            stk.ElementAt<Tag>(0).Inherited[0] = A.Inherited[0];
                            break;

                        // 3.	<identifier_list'> ::= ',' 'idnew' @InsertList <identifier_list'> .
                        case 3:
                            stk.ElementAt<Tag>(2).Inherited[0] = A.Inherited[0];
                            break;

                        default:
                            break;
                    }
                }
                else if (A.IsTerminal())
                {
                    if (A != current.TAG)
                    {
                        line = lex.ErrorLine;
                        return false;
                    }

                    if (A == Tag.vtSharp)
                        return true;

                    // pop
                    previous = current;
                    current = lex.NextToken(text, simbolTable, ref pos);

                    Console.WriteLine(current.ToString());

                    if (current == Token.UNKNOW)
                    {
                        line = lex.ErrorLine;
                        return false;
                    }
                }
                else 
                {
                    sem.Execute(A, stk, previous, ref simbolTable);
                }
            }
        }
    }
}
