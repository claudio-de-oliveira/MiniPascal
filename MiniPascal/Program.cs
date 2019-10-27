using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntermediateCode;

namespace MiniPascal
{
    class Program
    {
        static void Main(string[] args)
        {
            LL1_Parser parser = new LL1_Parser();

            string prg =
                "Program Teste(input, output);\n" +
                "var\n" +
                "   x, y: integer;\n" +
                "var\n" +
                "   a: array [1..10] of integer;\n" +
                "\n" +
                "procedure inicializa;\n" +
                "var\n" +
                "   i : integer;\n" +
                "begin\n" +
                "   i := (1 + i) * 2 / a;\n" +
                "   while i <= 10 do\n" +
                "       begin\n" +
                "           a[i] := a[0];\n" +
                "           i := i + 1\n" +
                "       end\n" +
                "end;\n" +
                "\n" +
                "function max(a, b: integer; i, j: real) : integer;\n" +
                "begin\n" +
                "   if a > b then max := a else max := b\n" +
                "end;\n" +
                "\n" +
                "begin\n" +
                "   inicializa;\n" +
                "   x := 10;\n" +
                "   y := 20;\n" +
                "   if max(x, y) = x then\n" +
                "       x := y\n" +
                "   else\n" +
                "       y := x\n" +
                "end.\n";

            parser.Parse(prg);
        }
    }
}
