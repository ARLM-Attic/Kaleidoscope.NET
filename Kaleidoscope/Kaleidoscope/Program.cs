using Kaleidoscope.Core;
using System;
using System.Collections.Generic;
using System.IO;
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

			//var tokens = lexer.Tokenize("def fib(x) if x < 3 then 1 else fib(x-1)+fib(x-2); for i = 0, i < 15, 1 in println(fib(i));");
			//var tokens = lexer.Tokenize("def fib(x) if x < 3 then 1 else fib(x-1)+fib(x-2); fib(8)");
			//var tokens = lexer.Tokenize("extern putchard(x) :: System.Console.Write; putchard(5)");
			var tokens = lexer.Tokenize(File.ReadAllText("mandelbrot.txt"));

			Parser parser = new Parser(tokens);

			CodeGenerator codeGenerator = new CodeGenerator(parser);
			StandardLibrary.AddStandardLibrary(codeGenerator);

			foreach (var currentTree in parser.Parse())
			{
				currentTree.GenerateCode(codeGenerator, SyntaxTreeGeneratorData.Empty);
			}

			if (codeGenerator.Methods.ContainsKey("main"))
			{
				var mainMethod = codeGenerator.Methods["main"];
				mainMethod.Invoke(null, null);
			}

			Console.ReadLine();

			//First add the standard library
			//Lexer lexer = new Lexer();
			//Parser parser = new Parser(lexer.Tokenize(File.ReadAllText("standardlibrary.txt")));
			//CodeGenerator codeGenerator = new CodeGenerator(parser);
			//StandardLibrary.AddStandardLibrary(codeGenerator);

			//foreach (var currentTree in parser.Parse())
			//{
			//	currentTree.GenerateCode(codeGenerator, SyntaxTreeGeneratorData.Empty);
			//}

			//while (true)
			//{
			//	string input = Console.ReadLine();
			//	var tokens = lexer.Tokenize(input);

			//	parser.Reset(tokens);

			//	foreach (var currentTree in parser.Parse())
			//	{
			//		currentTree.GenerateCode(codeGenerator, SyntaxTreeGeneratorData.Empty);
			//	}

			//	if (codeGenerator.Methods.ContainsKey("main"))
			//	{
			//		var mainMethod = codeGenerator.Methods["main"];
			//		mainMethod.Invoke(null, null);
			//	}
			//}
        }
    }
}
