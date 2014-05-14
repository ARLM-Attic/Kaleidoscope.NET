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
		/// Creates a new code generator exception
		/// </summary>
		/// <param name="message">The error message</param>
		public CodeGeneratorException(string message)
			: base(message)
		{

		}
	}
}
