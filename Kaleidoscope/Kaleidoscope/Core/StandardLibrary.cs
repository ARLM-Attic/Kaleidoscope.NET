using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaleidoscope.Core
{
    /// <summary>
    /// Represents the standard library
    /// </summary>
    public static class StandardLibrary
    {
		/// <summary>
		/// Prints the given value on a new line to standard output and returns it
		/// </summary>
		/// <param name="value">The value to print</param>
		/// <returns>The printed value</returns>
		public static double Println(double value)
		{
			Console.WriteLine(value);
			return value;
		}

		/// <summary>
		/// Prints the given value to standard output and returns it
		/// </summary>
		/// <param name="value">The value to print</param>
		/// <returns>The printed value</returns>
		public static double Print(double value)
		{
			Console.Write(value);
			return value;
		}

		/// <summary>
		/// Prints the given ASCII character
		/// </summary>
		/// <param name="value">The ASCII character to print</param>
		/// <returns>The printed value</returns>
		public static double AsciiPrint(double value)
		{
			Console.Write((char)value);
			return value;
		}

		/// <summary>
		/// Closes the current program
		/// </summary>
		/// <param name="value">The exitCode</param>
		public static void Exit()
		{
			System.Environment.Exit(0);
		}

		/// <summary>
		/// Negates the given value
		/// </summary>
		/// <param name="value">The value</param>
		/// <returns>The negated value</returns>
		public static double Negate(double value)
		{
			return -value;
		}

		/// <summary>
		/// Adds the standard library to the given code generator
		/// </summary>
		/// <param name="codeGenerator">The code generator</param>
		public static void AddStandardLibrary(CodeGenerator codeGenerator)
		{
			Type standardLibraryType = typeof(StandardLibrary);
		
			Type[] doubleArgumentType = new Type[] { typeof(double) };

			codeGenerator.Methods["println"] = standardLibraryType.GetMethod("Println", doubleArgumentType);
			codeGenerator.Methods["print"] = standardLibraryType.GetMethod("Print", doubleArgumentType);
			codeGenerator.Methods["exit"] = standardLibraryType.GetMethod("Exit");

			Type mathType = typeof(Math);
			codeGenerator.Methods["sin"] = mathType.GetMethod("Sin", doubleArgumentType);
			codeGenerator.Methods["cos"] = mathType.GetMethod("Cos", doubleArgumentType);
			codeGenerator.Methods["tan"] = mathType.GetMethod("Tan", doubleArgumentType);

			//codeGenerator.DefineUnaryOperator('-', standardLibraryType.GetMethod("Negate"));
		}
    }
}
