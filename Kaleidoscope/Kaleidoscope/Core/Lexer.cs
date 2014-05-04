using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kaleidoscope.Core
{
    /// <summary>
    /// Represents a lexer
    /// </summary>
    public class Lexer
    {

        #region Fields                                                              
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new lexer
        /// </summary>
        public Lexer()
        {

        }
        #endregion

        #region Properties

        #endregion

        #region Methods
		/// <summary>
		/// Indicates if the given character is whitespace
		/// </summary>
		/// <param name="getChar">The character</param>
        private bool IsWhiteSpace(char getChar)
        {
            return getChar == ' ' || getChar == '	' || getChar == '\n' || getChar == '\r';
        }
        
		/// <summary>
		/// Indicates if the given character is a letter
		/// </summary>
		/// <param name="getChar">The character</param>
		private bool IsAlpha(char getChar)
		{
			return Regex.IsMatch("" + getChar, "[a-zA-Z]");
		}

		/// <summary>
		/// Indicates if the given character is a letter or number
		/// </summary>
		/// <param name="getChar">The character</param>
		private bool IsAlphaNumeric(char getChar)
		{
			return Regex.IsMatch(getChar.ToString(), "[a-zA-Z0-9]");
		}

		/// <summary>
		/// Indicates if the given character is a digit
		/// </summary>
		/// <param name="getChar">The character</param>
		private bool IsDigit(char getChar)
		{
			return Regex.IsMatch(getChar.ToString(), "[0-9]");
		}

		/// <summary>
		/// Returns the character at the given position
		/// </summary>
		/// <param name="str">The string</param>
		/// <param name="index">The index</param>
		/// <returns>The character or null</returns>
		private char? GetChar(string str, int index)
		{
			if (index < str.Length)
			{
				return str[index];
			}
			else
			{
				return null;
			}
		}

        /// <summary>
        /// Tokenizes the given string
        /// </summary>
        /// <param name="str">The string</param>
        /// <returns>A list of the tokens</returns>
        public IEnumerable<Token> TokenizeString(string str)
        {
			char? currentChar = ' ';
			int i = -1;

			while (true)
			{
				if (currentChar.HasValue)
				{
					//Skip any whitespace
					while (IsWhiteSpace(currentChar.Value))
					{
						currentChar = GetChar(str, ++i);

						if (!currentChar.HasValue)
						{
							break;
						}
					}

					//Identifier
					if (currentChar != null && IsAlpha(currentChar.Value))
					{
						string identifierStr = currentChar.ToString();

						while (true)
						{
							currentChar = GetChar(str, ++i);

							if (currentChar.HasValue && IsAlphaNumeric(currentChar.Value))
							{
								identifierStr += currentChar;
							}
							else
							{
								break;
							}
						}

						if (identifierStr == "def")
						{
							yield return new Token(TokenType.Def);
							continue;
						}

						if (identifierStr == "extern")
						{
							yield return new Token(TokenType.Extern);
							continue;
						}

						if (identifierStr == "if")
						{
							yield return new Token(TokenType.If);
							continue;
						}

						if (identifierStr == "then")
						{
							yield return new Token(TokenType.Then);
							continue;
						}

						if (identifierStr == "else")
						{
							yield return new Token(TokenType.Else);
							continue;
						}

						yield return new IdentifierToken(identifierStr);
						continue;
					}

					//Number
					if (currentChar != null && (IsDigit(currentChar.Value) || currentChar.Value == '.'))
					{
						string numStr = "";

						do
						{
							numStr += currentChar.ToString();
							currentChar = GetChar(str, ++i);
						} while (currentChar.HasValue && IsDigit(currentChar.Value) || currentChar == '.');

						yield return new NumberToken(double.Parse(numStr, System.Globalization.CultureInfo.InvariantCulture));
						continue;
					}

					//Comment
					if (currentChar == '#')
					{
						//Skip until the end of the line
						do
						{
							currentChar = GetChar(str, ++i);
						} while (currentChar.HasValue && currentChar != '\n' && currentChar != '\r');

						if (i < str.Length)
						{
							continue;
						}
					}

					char? thisChar = currentChar;
					currentChar = GetChar(str, ++i);

					if (thisChar.HasValue)
					{
						yield return new CharacterToken(thisChar.Value);
						continue;
					}
				}
				else
				{
					yield return new Token(TokenType.Eof);
					break;
				}
			}
        }
        #endregion

    }
}
