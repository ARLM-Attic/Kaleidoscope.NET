using Kaleidoscope.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Kaleidoscope
{
    class Program
    {
        static void Main(string[] args)
        {
			Lexer lexer = new Lexer();

			var tokens = lexer.Tokenize("def fib(x) if x < 3 then 1 else fib(x-1)+fib(x-2); for i = 0, i < 15, 1.0 in println(fib(i));");
            //var tokens = lexer.Tokenize("def fib(x) if x < 3 then 1 else fib(x-1)+fib(x-2); fib(8)");
			//var tokens = lexer.Tokenize("for i = 0, i < 5, 1.0 in print(5);");

			Parser parser = new Parser(tokens);

			CodeGenerator codeGenerator = new CodeGenerator();
            StandardLibrary.AddStandardLibrary(codeGenerator);

			foreach (var currentTree in parser.Parse())
			{
				currentTree.GenerateCode(codeGenerator, SyntaxTreeGeneratorData.Empty);
			}

			var mainMethod = codeGenerator.Methods["main"];
            mainMethod.Invoke(null, null);

			Console.ReadLine();
        }
    }
}
