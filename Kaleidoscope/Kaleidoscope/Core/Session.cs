using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kaleidoscope.Core
{
	/// <summary>
	/// Represents a session
	/// </summary>
	public class Session
	{

		#region Fields
		private IDictionary<char, int> binaryOperatorPrecedence;
		private IDictionary<string, MethodInfo> methods;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new session
		/// </summary>
		public Session()
		{
			this.binaryOperatorPrecedence = new Dictionary<char, int>();
			this.methods = new Dictionary<string, MethodInfo>();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Returns the binary operator precedence table
		/// </summary>
		public IDictionary<char, int> BinaryOperatorPrecedence
		{
			get { return this.binaryOperatorPrecedence; }
		}

		/// <summary>
		/// Returns the generated methods
		/// </summary>
		public IDictionary<string, MethodInfo> Methods
		{
			get { return this.methods; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Defines a new binary operator
		/// </summary>
		/// <param name="operatorChar">The operator</param>
		/// <param name="precedence">The precedence of the operator</param>
		public void DefineBinaryOperator(char operatorChar, int precedence)
		{
			if (!this.binaryOperatorPrecedence.ContainsKey(operatorChar))
			{
				this.binaryOperatorPrecedence.Add(operatorChar, precedence);
			}
		}
		#endregion

	}
}
