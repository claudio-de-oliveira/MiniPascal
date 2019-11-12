using System;
using System.Collections.Generic;
using System.IO;
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

            string texto = File.ReadAllText(@"C:..\..\..\teste.mp");

            parser.Parse(texto);
        }
    }
}
