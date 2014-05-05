using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaleidoscope.Core
{
    /// <summary>
    /// The type of the token
    /// </summary>
    public enum TokenType
    {
        Eof,
        Def,
        Extern,
        Identifier,
        Number,
		Character,
		If,
		Then,
		Else,
        For,
        In
    }

    /// <summary>
    /// Represents a token
    /// </summary>
    public class Token
    {

        #region Fields
        private readonly TokenType type;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new token
        /// </summary>
        /// <param name="type">The type of the token</param>
        public Token(TokenType type)
        {
            this.type = type;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Returns the type of the token
        /// </summary>
        public TokenType Type
        {
            get { return this.type; }
        }
        #endregion

        #region Methods
		public override string ToString()
		{
			return "Token: " + this.Type.ToString();
		}
        #endregion

    }

    /// <summary>
    /// Represents a number token
    /// </summary>
    public class NumberToken : Token
    {

        #region Fields
        private readonly double value;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new number token
        /// </summary>
        /// <param name="value">The value</param>
        public NumberToken(double value)
            : base(TokenType.Number)
        {
            this.value = value;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Returns the value of the token
        /// </summary>
        public double Value
        {
            get { return this.value; }
        }
        #endregion

        #region Methods
		public override string ToString()
		{
			return "Number: " + this.value;
		}
        #endregion

    }

    /// <summary>
    /// Represents an identifier token
    /// </summary>
    public class IdentifierToken : Token
    {

        #region Fields
        private readonly string value;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an new identifier token
        /// </summary>
        /// <param name="value">The value of the token</param>
        public IdentifierToken(string value)
            : base(TokenType.Identifier)
        {
            this.value = value;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Returns the value of the token
        /// </summary>
        public string Value
        {
            get { return this.value; }
        }
        #endregion

        #region Methods
		public override string ToString()
		{
			return "Identifier: " + this.value;
		}
        #endregion

    }

	/// <summary>
	/// Represents a character token
	/// </summary>
	public class CharacterToken : Token
	{

		#region Fields
		private readonly char value;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates an new character token
		/// </summary>
		/// <param name="value">The value of the token</param>
		public CharacterToken(char value)
			: base(TokenType.Character)
		{
			this.value = value;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Returns the value of the token
		/// </summary>
		public char Value
		{
			get { return this.value; }
		}
		#endregion

		#region Methods
		public override string ToString()
		{
			return "Character: " + this.value;
		}
		#endregion

	}
}
