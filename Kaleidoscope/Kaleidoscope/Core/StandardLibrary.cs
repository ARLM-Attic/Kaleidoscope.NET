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
        /// Adds the standard library to the given code generator
        /// </summary>
        /// <param name="codeGenerator">The code generator</param>
        public static void AddStandardLibrary(CodeGenerator codeGenerator)
        {
            Type[] doubleArgumentType = new Type[] { typeof(double) };

            codeGenerator.Methods["println"] = typeof(StandardLibrary).GetMethod("Println", doubleArgumentType);
			codeGenerator.Methods["print"] = typeof(StandardLibrary).GetMethod("Print", doubleArgumentType);

            Type mathType = typeof(Math);
            codeGenerator.Methods["sin"] = mathType.GetMethod("Sin", doubleArgumentType);
            codeGenerator.Methods["cos"] = mathType.GetMethod("Cos", doubleArgumentType);
            codeGenerator.Methods["tan"] = mathType.GetMethod("Tan", doubleArgumentType);
        }
    }
}
