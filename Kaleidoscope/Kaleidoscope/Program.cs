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
			Session session = new Session();
			CodeGenerator codeGenerator = new CodeGenerator(session);

			//Add the standard library
			StandardLibrary.AddStandardLibrary(codeGenerator);

			while (true)
			{
				try
				{
					string input = Console.ReadLine();
					var tokens = lexer.Tokenize(input);

					Parser parser = new Parser(tokens, session.BinaryOperatorPrecedence);

					foreach (var currentTree in parser.Parse())
					{
						currentTree.GenerateCode(codeGenerator, SyntaxTreeGeneratorData.Empty);

						if (currentTree is FunctionSyntaxTree)
						{
							FunctionSyntaxTree funcTree = (FunctionSyntaxTree)currentTree;

							if (funcTree.Prototype.Name != "")
							{
								Console.WriteLine("Defined function '" + funcTree.Prototype.Name + "'");
							}
						}

						if (currentTree is ExternalFunctionSyntaxTree)
						{
							ExternalFunctionSyntaxTree funcTree = (ExternalFunctionSyntaxTree)currentTree;

							if (funcTree.Prototype.Name != "")
							{
								Console.WriteLine("Defined external function '" + funcTree.Prototype.Name + "' referencing: " + funcTree.FuncReference);
							}
						}
					}

					if (codeGenerator.Methods.ContainsKey("main"))
					{
						var mainMethod = codeGenerator.Methods["main"];
						mainMethod.Invoke(null, null);
					}
				}
				catch (ParserException e)
				{
					Console.WriteLine(e.Message);
				}
				catch (CodeGeneratorException e)
				{
					Console.WriteLine(e.Message);
				}

				codeGenerator.Methods.Remove("main");
			}
		}
    }
}
