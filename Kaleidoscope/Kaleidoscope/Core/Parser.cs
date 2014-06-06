using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaleidoscope.Core
{
	/// <summary>
	/// Represents a parser
	/// </summary>
	public class Parser
	{

		#region Fields
		private IEnumerable<Token> tokens;
		private Token currentToken;
		private IDictionary<char, int> binaryOperatorPrecedence;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new parser
		/// </summary>
		/// <param name="tokens">The tokens</param>
		/// <param name="binaryOperatorPrecedences">The binary operator precedence table</param>
		public Parser(IEnumerable<Token> tokens, IDictionary<char, int> binaryOperatorPrecedences = null)
		{
			this.tokens = tokens;
			this.currentToken = null;

			if (binaryOperatorPrecedences == null)
			{
				this.binaryOperatorPrecedence = new Dictionary<char, int>();
			}
			else
			{
				this.binaryOperatorPrecedence = binaryOperatorPrecedences;
			}

			//This operators must exist
			if (!this.binaryOperatorPrecedence.ContainsKey('<'))
			{
				this.binaryOperatorPrecedence.Add('<', 10);
			}

			if (!this.binaryOperatorPrecedence.ContainsKey('+'))
			{
				this.binaryOperatorPrecedence.Add('+', 20);
			}

			if (!this.binaryOperatorPrecedence.ContainsKey('-'))
			{
				this.binaryOperatorPrecedence.Add('-', 20);
			}

			if (!this.binaryOperatorPrecedence.ContainsKey('*'))
			{
				this.binaryOperatorPrecedence.Add('*', 40);
			}
		}
		#endregion

		#region Properties

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

		/// <summary>
		/// Advances to the next token
		/// </summary>
		private void NextToken()
		{
			this.currentToken = this.tokens.FirstOrDefault();
			this.tokens = this.tokens.Skip(1);
		}

		/// <summary>
		/// Parses an expression
		/// </summary>
		/// <param name="funcCall">Indicates if the primary is parsed in a function call</param>
		/// <returns>An expression syntax tree</returns>
		private ExpressionSyntaxTree ParseExpression(bool funcCall = false)
		{
			ExpressionSyntaxTree leftHandSide = this.ParseUnary(funcCall);

			if (leftHandSide == null)
			{
				return null;
			}

			return this.ParseBinaryOpRHS(0, leftHandSide);
		}

		/// <summary>
		/// Parses an unary operator expression
		/// </summary>
		/// <param name="funcCall">Indicates if the primary is parsed in a function call</param>
		/// <returns>An expression syntax tree</returns>
		private ExpressionSyntaxTree ParseUnary(bool funcCall = false)
		{
			//If the current token is not an operator, it must be a primary expression
			CharacterToken charToken = this.currentToken as CharacterToken;

			if (charToken == null)
			{
				return this.ParsePrimary(funcCall);
			}

			if (charToken != null && (charToken.Value == '(' || charToken.Value == ')' || charToken.Value == ','))
			{
				return this.ParsePrimary(funcCall);
			}

			charToken = this.currentToken as CharacterToken;
			char op = charToken.Value;
			this.NextToken();

			ExpressionSyntaxTree operand = this.ParseUnary(funcCall);

			if (operand == null)
			{
				return null;
			}

			return new UnaryExpressionSyntaxTree(op, operand);
		}

		/// <summary>
		/// Parses the right hand side of binary operator expression
		/// </summary>
		/// <param name="expressionPrecedence">The expression precedence</param>
		/// <param name="leftHandSide">The left hand side</param>
		/// <returns>An expression syntax tree</returns>
		private ExpressionSyntaxTree ParseBinaryOpRHS(int expressionPrecedence, ExpressionSyntaxTree leftHandSide)
		{
			//If this is a binop, find its precedence
			while (true)
			{
				int tokenPrec = this.GetTokenPrecedence();

				//If this is a binop that binds at least as tightly as the current binop,
				//consume it, otherwise we are done.

				if (tokenPrec < expressionPrecedence)
				{
					return leftHandSide;
				}

				//Okay, we know this is a binop.
				CharacterToken opToken = this.currentToken as CharacterToken;
				this.NextToken(); //Consume the binop

				//Parse the primary expression after the binary operator.
				ExpressionSyntaxTree rightHandSide = this.ParseUnary();

				if (rightHandSide == null)
				{
					return null;
				}

				if (rightHandSide == null)
				{
					return null;
				}

				//If the bin op binds less tightly with RHS than the operator after RHS, let
				//the pending operator take RHS as its LHS.
				int nextPrec = this.GetTokenPrecedence();
				if (tokenPrec < nextPrec)
				{
					rightHandSide = this.ParseBinaryOpRHS(tokenPrec + 1, rightHandSide);

					if (rightHandSide == null)
					{
						return null;
					}
				}

				//Merge the left and right hand side
				leftHandSide = new BinaryExpressionSyntaxTree(opToken.Value, leftHandSide, rightHandSide);
			}
		}

		/// <summary>
		/// Returns the precedence for the current token
		/// </summary>
		/// <returns>The precedence or -1 if not an defined operator</returns>
		private int GetTokenPrecedence()
		{
			CharacterToken charToken = this.currentToken as CharacterToken;

			if (charToken != null)
			{
				if (this.binaryOperatorPrecedence.ContainsKey(charToken.Value))
				{
					return this.binaryOperatorPrecedence[charToken.Value];
				}
			}

			return -1;
		}

		/// <summary>
		/// Parses a primary expression
		/// </summary>
		/// <param name="funcCall">Indicates if the primary is parsed in a function call</param>
		/// <returns>An expression syntax tree</returns>
		private	ExpressionSyntaxTree ParsePrimary(bool funcCall = false)
		{
			if (this.currentToken.Type == TokenType.Identifier)
			{
				return this.ParseIdentifierExpression();
			}

			if (this.currentToken.Type == TokenType.Number)
			{
				return this.ParseNumberExpression();
			}

			if (this.currentToken.Type == TokenType.Character)
			{
				CharacterToken charToken = (CharacterToken)this.currentToken;

				if (charToken.Value == '(')
				{
					return this.ParseParentheseExpression();
				}
			}

			if (this.currentToken.Type == TokenType.If)
			{
				return this.ParseIfExpression();
			}

			if (this.currentToken.Type == TokenType.For)
			{
				return this.ParseForExpression();
			}

			if (!funcCall)
			{
				throw new ParserException("unknown token when expecting an expression");
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Parses a number expression
		/// </summary>
		/// <returns>An number syntax tree</returns>
		private ExpressionSyntaxTree ParseNumberExpression()
		{
			NumberToken numToken = (NumberToken)this.currentToken;
			this.NextToken(); //Consume the number
			return new NumberExpressionSyntaxTree(numToken.Value);
		}

		/// <summary>
		/// Parses a parenthese expression
		/// </summary>
		/// <returns>An expression syntax tree</returns>
		private ExpressionSyntaxTree ParseParentheseExpression()
		{
			this.NextToken(); //Consume (
			ExpressionSyntaxTree expr = this.ParseExpression();

			if (expr == null)
			{
				return null;
			}

			CharacterToken charToken = this.currentToken as CharacterToken;

			if ((charToken != null && charToken.Value != ')') ||charToken == null)
			{
				Console.WriteLine("expected ')'");
				return null;
			}

			this.NextToken(); //Consume )

			return expr;
		}

		/// <summary>
		/// Parses a identifier expression
		/// </summary>
		/// <returns>An expression syntax tree</returns>
		private ExpressionSyntaxTree ParseIdentifierExpression()
		{
			string identifierName = ((IdentifierToken)this.currentToken).Value;

			this.NextToken(); //Consume the identifier

			CharacterToken charToken = this.currentToken as CharacterToken;

			//Simple variable ref
			if ((charToken != null && charToken.Value != '(') || charToken == null)
			{
				return new VariableExpressionSyntaxTree(identifierName);
			}

			//Call
			this.NextToken(); //Consume the (
			List<ExpressionSyntaxTree> args = new List<ExpressionSyntaxTree>();

			if (charToken != null && charToken.Value != ')')
			{
				while (true)
				{
					ExpressionSyntaxTree arg = this.ParseExpression(true);

					if (arg != null)
					{
						args.Add(arg);
					}

					charToken = this.currentToken as CharacterToken;

					if (charToken != null && charToken.Value == ')')
					{
						break;
					}

					if ((charToken != null && charToken.Value != ',') || charToken == null)
					{
						throw new ParserException("Expected ')' or ',' in argument list");
					}

					this.NextToken();
				}
			}

			this.NextToken(); //Consume the )

			return new CallExpressionSyntaxTree(identifierName, args);
		}

		/// <summary>
		/// Parses a prototype
		/// </summary>
		/// <returns>A prototype syntax tree</returns>
		private PrototypeSyntaxTree ParsePrototype()
		{
			int kind = 0;
			int binaryPrecedence = 30;
			string funcName = "";
			CharacterToken charToken = null;
			IdentifierToken identToken = null;

			switch (this.currentToken.Type)
			{
				default:
					throw new ParserException("Expected function name in prototype");
				case TokenType.Identifier:
					funcName = ((IdentifierToken)this.currentToken).Value;
					kind = 0;
					this.NextToken(); //Consume the func name
					break;
				case TokenType.Binary:
					this.NextToken();

					charToken = this.currentToken as CharacterToken;

					if (charToken == null)
					{
						throw new ParserException("Expected binary operator");
					}

					funcName = "binary" + charToken.Value;
					kind = 2;
					this.NextToken();

					//Read the precedence
					NumberToken numToken = this.currentToken as NumberToken;

					if (numToken != null)
					{
						if (numToken.Value < 1 || numToken.Value > 100)
						{
							throw new ParserException("Invalid precedecnce: must be in the range: 1..100");
						}
						
						binaryPrecedence = (int)numToken.Value;
						this.NextToken();
					}
					else
					{
						throw new ParserException("Expected a number");
					}
					break;
				case TokenType.Unary:
					this.NextToken();

					charToken = this.currentToken as CharacterToken;

					if (charToken == null)
					{
						throw new ParserException("Expected unary operator");
					}

					funcName = "unary" + charToken.Value;
					kind = 1;

					this.NextToken();
					break;
			}

			charToken = this.currentToken as CharacterToken;

			if ((charToken != null && charToken.Value != '(') || charToken == null)
			{
				throw new ParserException("Expected '(' in prototype");
			}

			//Read the arguments
			List<string> args = new List<string>();

			while (true)
			{
				this.NextToken();

				identToken = this.currentToken as IdentifierToken;

				if (identToken != null)
				{
					args.Add(identToken.Value);
				}
				else
				{
					break;
				}
			}

			charToken = this.currentToken as CharacterToken;

			if ((charToken != null && charToken.Value != ')') || charToken == null)
			{
				throw new ParserException("Expected ')' in prototype");
			}

			this.NextToken(); //Consume the )

			//Verify the arguments is the same as the operator
			if (kind != 0 && args.Count != kind)
			{
				throw new ParserException("Invalid number of operands for operator");
			}

			return new PrototypeSyntaxTree(funcName, args, kind != 0, binaryPrecedence);
		}

		/// <summary>
		/// Parses a function definition
		/// </summary>
		/// <returns>A function syntax tree</returns>
		private FunctionSyntaxTree ParseDefinition()
		{
			this.NextToken(); //Consume the def
			PrototypeSyntaxTree prototype = this.ParsePrototype();

			if (prototype == null)
			{
				return null;
			}

			//Parse the body
			ExpressionSyntaxTree bodyExpression = this.ParseExpression();

			if (bodyExpression != null)
			{
				return new FunctionSyntaxTree(prototype, bodyExpression);
			}

			return null;
		}

		/// <summary>
		/// Parses an extern 
		/// </summary>
		/// <returns>A external function syntax tree</returns>
		private ExternalFunctionSyntaxTree ParseExtern()
		{
			this.NextToken(); //Consume the extern

			PrototypeSyntaxTree protoType = this.ParsePrototype();

			CharacterToken charToken = this.currentToken as CharacterToken;

			if ((charToken != null && charToken.Value != ':') || charToken == null)
			{
				throw new ParserException("Expected ':' after extern");
			}

			//Consume the :
			this.NextToken();

			charToken = this.currentToken as CharacterToken;

			if ((charToken != null && charToken.Value != ':') || charToken == null)
			{
				throw new ParserException("Expected ':' after extern");
			}

			//Consume the :
			this.NextToken();

			if (this.currentToken.Type != TokenType.Identifier)
			{
				throw new ParserException("Expected indentifier.");
			}

			string funcRef = ((IdentifierToken)this.currentToken).Value;

			//Consume the identifier
			this.NextToken();

			return new ExternalFunctionSyntaxTree(protoType, funcRef);
		}

		/// <summary>
		/// Parses an if expression
		/// </summary>
		/// <returns>An expression syntax tree</returns>
		private ExpressionSyntaxTree ParseIfExpression()
		{
			this.NextToken(); //Consume the if

			//Condition
			ExpressionSyntaxTree cond = this.ParseExpression();

			if (cond == null)
			{
				return null;
			}

			if (this.currentToken.Type != TokenType.Then)
			{
				throw new ParserException("expected then");
			}

			this.NextToken(); //Consume the then

			ExpressionSyntaxTree thenBody = this.ParseExpression();

			if (thenBody == null)
			{
				return null;
			}

			if (this.currentToken.Type != TokenType.Else)
			{
				throw new ParserException("expected else");
			}

			this.NextToken(); //Consume the else

			ExpressionSyntaxTree elseBody = this.ParseExpression();

			if (elseBody == null)
			{
				return null;
			}

			return new IfExpressionSyntaxTree(cond, thenBody, elseBody);
		}

        /// <summary>
        /// Parses a for expression
        /// </summary>
        /// <returns>An expression syntax tree</returns>
        private ExpressionSyntaxTree ParseForExpression()
        {
            this.NextToken(); //Consume the for

            if (this.currentToken.Type != TokenType.Identifier)
            {
				throw new ParserException("expected identifier after for");
            }

            string varName = ((IdentifierToken)this.currentToken).Value;

            this.NextToken(); //Consume the identifier

            CharacterToken charToken = this.currentToken as CharacterToken;

            if ((charToken != null && charToken.Value != '=') ||charToken == null)
            {
				throw new ParserException("expected '=' after for");
            }

            this.NextToken(); //Consume the '='

            ExpressionSyntaxTree startExpression = this.ParseExpression();

            if (startExpression == null)
            {
                return null;
            }

            charToken = this.currentToken as CharacterToken;

            if ((charToken != null && charToken.Value != ',') ||charToken == null)
            {
				throw new ParserException("expected ',' after for start value");
            }

            this.NextToken(); //Consume the ','

            ExpressionSyntaxTree endExpression = this.ParseExpression();

            if (endExpression == null)
            {
                return null;
            }

            //The step value is optional
            ExpressionSyntaxTree stepExpression = null;

            charToken = this.currentToken as CharacterToken;

            if ((charToken != null && charToken.Value == ',') ||charToken == null)
            {
                this.NextToken(); //Consume the ','
                stepExpression = this.ParseExpression();

                if (stepExpression == null)
                {
                    return null;
                }
            }

            if (this.currentToken.Type != TokenType.In)
            {
				throw new ParserException("expected 'in' after for");
            }

            this.NextToken(); //Consume the 'in'

            ExpressionSyntaxTree bodyExpression = this.ParseExpression();

            if (bodyExpression == null)
            {
                return null;
            }

            return new ForExpressionSyntaxTree(varName, startExpression, endExpression, stepExpression, bodyExpression);
        }

		/// <summary>
		/// Parses a top level expression
		/// </summary>
		/// <returns>An expression syntax tree</returns>
		private ExpressionSyntaxTree ParseTopLevelExpression()
		{
			ExpressionSyntaxTree expression = this.ParseExpression();

			if (expression != null)
			{
				//Make an anonymous prototype
				PrototypeSyntaxTree prototype = new PrototypeSyntaxTree("", new List<string>());
				return new FunctionSyntaxTree(prototype, expression);
			}

			return null;
		}

		/// <summary>
		/// Parses the loaded tokens
		/// </summary>
		/// <returns>A syntax tree</returns>
		/// <exception cref="ParserException">If the input was invalid</exception>
		public IEnumerable<AbstractSyntaxTree> Parse()
		{
			this.NextToken();
			bool doParse = true;

			while (doParse)
			{
				if (currentToken == null)
				{
					doParse = false;
					break;
				}

				CharacterToken charToken = this.currentToken as CharacterToken;

				if (charToken != null && charToken.Value == ';')
				{
					this.NextToken();
				}

				switch (this.currentToken.Type)
				{
					case TokenType.Eof:
						doParse = false;
						break;
					case TokenType.Def:
						yield return this.ParseDefinition();
						break;
					case TokenType.Extern:
						yield return this.ParseExtern();
						break;
					default:
						yield return this.ParseTopLevelExpression();
						break;
				}
			}
		}
		#endregion

	}
}
