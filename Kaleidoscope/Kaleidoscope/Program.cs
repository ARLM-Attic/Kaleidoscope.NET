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

            var tokens = lexer.Tokenize("def fib(x) if x < 3 then 1 else fib(x-1)+fib(x-2); println(fib(8))");
            //var tokens = lexer.Tokenize("def fib(x) if x < 3 then 1 else fib(x-1)+fib(x-2); fib(8)");

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
