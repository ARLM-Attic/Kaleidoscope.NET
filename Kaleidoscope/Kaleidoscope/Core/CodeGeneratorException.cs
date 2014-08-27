using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaleidoscope.Core
{
	/// <summary>
	/// Represents a code generator exception
	/// </summary>
	public class CodeGeneratorException : Exception
	{
		/// <summary>
		/// Returns the syntax tree that caused the exception
		/// </summary>
		public SyntaxTrees SyntaxTree { get; private set; }

		/// <summary>
		/// Creates a new code generator exception
		/// </summary>
		/// <param name="message">The error message</param>
		/// <param name="syntaxTree">The syntax tree that caused the exception</param>
		public CodeGeneratorException(string message, SyntaxTrees syntaxTree)
			: base(message)
		{
			this.SyntaxTree = syntaxTree;
		}
	}
}
