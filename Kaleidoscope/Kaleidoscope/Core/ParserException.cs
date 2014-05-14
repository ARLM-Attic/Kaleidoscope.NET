using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaleidoscope.Core
{
	/// <summary>
	/// Represents a parser exception
	/// </summary>
	public class ParserException : Exception
	{
		/// <summary>
		/// Creates a new parser exception
		/// </summary>
		/// <param name="message">The error message</param>
		public ParserException(string message)
			: base(message)
		{

		}
	}
}
