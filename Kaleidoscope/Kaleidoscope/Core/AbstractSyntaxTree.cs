using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Kaleidoscope.Core
{
	/// <summary>
	/// Represents an abstract syntax tree
	/// </summary>
	public abstract class AbstractSyntaxTree
	{

		#region Fields

		#endregion

		#region Constructors

		#endregion

		#region Properties

		#endregion

		#region Methods
		/// <summary>
		/// Generates code for the current syntax tree
		/// </summary>
		/// <param name="codeGenerator">The code generator</param>
		/// <param name="ilGenerator">The generator data</param>
		public abstract void GenerateCode(CodeGenerator codeGenerator, SyntaxTreeGeneratorData generatorData);
		#endregion

	}

	/// <summary>
	/// Represents an expression syntax tree
	/// </summary>
	public abstract class ExpressionSyntaxTree : AbstractSyntaxTree
	{

		#region Fields

		#endregion

		#region Constructors

		#endregion

		#region Properties

		#endregion

		#region Methods

		#endregion

	}

	/// <summary>
	/// Represents a number expression syntax tree
	/// </summary>
	public class NumberExpressionSyntaxTree : ExpressionSyntaxTree
	{

		#region Fields
		private readonly double value;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new number syntax tree
		/// </summary>
		/// <param name="value">The value</param>
		public NumberExpressionSyntaxTree(double value)
		{
			this.value = value;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Returns the value
		/// </summary>
		public double Value
		{
			get { return this.value; }
		}
		#endregion

		#region Methods
		public override void GenerateCode(CodeGenerator codeGenerator, SyntaxTreeGeneratorData generatorData)
		{
			generatorData.ILGenerator.Emit(OpCodes.Ldc_R8, this.Value);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
		#endregion

	}

	/// <summary>
	/// Represents a variable expression syntax tree
	/// </summary>
	public class VariableExpressionSyntaxTree : ExpressionSyntaxTree
	{

		#region Fields
		private readonly string name;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new variable expression syntax tree
		/// </summary>
		/// <param name="value">The name of the variable</param>
		public VariableExpressionSyntaxTree(string name)
		{
			this.name = name;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Returns the name
		/// </summary>
		public string Name
		{
			get { return this.name; }
		}
		#endregion

		#region Methods
		public override void GenerateCode(CodeGenerator codeGenerator, SyntaxTreeGeneratorData generatorData)
		{
			if (generatorData.SymbolTable.ContainsKey(this.Name))
			{
				FunctionArgumentSymbol argSymbol = generatorData.SymbolTable[this.Name] as FunctionArgumentSymbol;
				LocalVariableSymbol varSymbol = generatorData.SymbolTable[this.Name] as LocalVariableSymbol;

				if (argSymbol != null)
				{
					generatorData.ILGenerator.Emit(OpCodes.Ldarg, argSymbol.ArgumentIndex);
				}

				if (varSymbol != null)
				{
					generatorData.ILGenerator.Emit(OpCodes.Ldloc, varSymbol.LocalVariable);
				}
			}
			else
			{
				throw new CodeGeneratorException("Variable '" + this.Name + "' not found.", this);
			}
		}

		public override string ToString()
		{
			return this.Name;
		}
		#endregion

	}

	/// <summary>
	/// Represents a binary expression syntax tree
	/// </summary>
	public class BinaryExpressionSyntaxTree : ExpressionSyntaxTree
	{

		#region Fields
		private readonly char op;
		private readonly ExpressionSyntaxTree leftHandSide;
		private readonly ExpressionSyntaxTree rightHandSide;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new binary expression syntax tree
		/// </summary>
		/// <param name="op">The operator</param>
		/// <param name="leftHandSide">The left hand side</param>
		/// <param name="rightHandSide">The right hand side</param>
		public BinaryExpressionSyntaxTree(char op, ExpressionSyntaxTree leftHandSide, ExpressionSyntaxTree rightHandSide)
		{
			this.op = op;
			this.leftHandSide = leftHandSide;
			this.rightHandSide = rightHandSide;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Returns the operator
		/// </summary>
		public char Operator
		{
			get { return this.op; }
		}

		/// <summary>
		/// Returns the left hand side
		/// </summary>
		public ExpressionSyntaxTree LeftHandSide
		{
			get { return this.leftHandSide; }
		}

		/// <summary>
		/// Returns the right hand side
		/// </summary>
		public ExpressionSyntaxTree RightHandSide
		{
			get { return this.rightHandSide; }
		}
		#endregion

		#region Methods
		public override void GenerateCode(CodeGenerator codeGenerator, SyntaxTreeGeneratorData generatorData)
		{
			this.LeftHandSide.GenerateCode(codeGenerator, generatorData);
			this.RightHandSide.GenerateCode(codeGenerator, generatorData);
			bool isBuiltinOperator = true;

			switch (this.Operator)
			{
				case '+':
					generatorData.ILGenerator.Emit(OpCodes.Add);
					break;
				case '-':
					generatorData.ILGenerator.Emit(OpCodes.Sub);
					break;
				case '*':
					generatorData.ILGenerator.Emit(OpCodes.Mul);
					break;
				case '<':
					Label endLabel = generatorData.ILGenerator.DefineLabel();
					Label trueLabel = generatorData.ILGenerator.DefineLabel();

					generatorData.ILGenerator.Emit(OpCodes.Blt, trueLabel);
					generatorData.ILGenerator.Emit(OpCodes.Ldc_R8, 0.0);
					generatorData.ILGenerator.Emit(OpCodes.Br, endLabel);
					generatorData.ILGenerator.MarkLabel(trueLabel);
					generatorData.ILGenerator.Emit(OpCodes.Ldc_R8, 1.0);
					generatorData.ILGenerator.MarkLabel(endLabel);
					break;
				default:
					isBuiltinOperator = false;
					break;
			}

			//If not a builtin operator, generate a function call
			if (!isBuiltinOperator)
			{
				codeGenerator.GenerateFunctionCall(
					"binary" + this.op,
					this,
					generatorData);
			}
		}

		public override string ToString()
		{
			return this.LeftHandSide.ToString() + this.Operator + this.RightHandSide.ToString();
		}
		#endregion

	}

	/// <summary>
	/// Represents a unary expression syntax tree
	/// </summary>
	public class UnaryExpressionSyntaxTree : ExpressionSyntaxTree
	{

		#region Fields
		private readonly char op;
		private readonly ExpressionSyntaxTree operand;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new unary expression syntax tree
		/// </summary>
		/// <param name="op">The operator</param>
		/// <param name="operand">The operand</param>
		public UnaryExpressionSyntaxTree(char op, ExpressionSyntaxTree operand)
		{
			this.op = op;
			this.operand = operand;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Returns the operator
		/// </summary>
		public char Operator
		{
			get { return this.op; }
		}

		/// <summary>
		/// Returns the operand
		/// </summary>
		public ExpressionSyntaxTree Operand
		{
			get { return this.operand; }
		}
		#endregion

		#region Methods
		public override void GenerateCode(CodeGenerator codeGenerator, SyntaxTreeGeneratorData generatorData)
		{
			this.Operand.GenerateCode(codeGenerator, generatorData);

			codeGenerator.GenerateFunctionCall(
				"unary" + this.op,
				this,
				generatorData);
		}

		public override string ToString()
		{
			return this.Operator + this.Operand.ToString();
		}
		#endregion

	}

	/// <summary>
	/// Represents a call expression syntax tree
	/// </summary>
	public class CallExpressionSyntaxTree : ExpressionSyntaxTree
	{

		#region Fields
		private readonly string funcName;
		private readonly List<ExpressionSyntaxTree> arguments;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new call expression syntax tree
		/// </summary>
		/// <param name="funcName">The function to call</param>
		/// <param name="arguments">The arguments to call with</param>
		public CallExpressionSyntaxTree(string funcName, List<ExpressionSyntaxTree> arguments)
		{
			this.funcName = funcName;
			this.arguments = arguments;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Returns the name of the function to call
		/// </summary>
		public string FunctionName
		{
			get { return this.funcName; }
		}

		/// <summary>
		/// Returns the arguments
		/// </summary>
		public IReadOnlyList<ExpressionSyntaxTree> Arguments
		{
			get { return this.arguments.AsReadOnly(); }
		}
		#endregion

		#region Methods
		public override void GenerateCode(CodeGenerator codeGenerator, SyntaxTreeGeneratorData generatorData)
		{
			foreach (var currentArg in arguments)
			{
				currentArg.GenerateCode(codeGenerator, generatorData);
			}

			codeGenerator.GenerateFunctionCall(
				this.funcName,
				this,
				generatorData);
		}

		public override string ToString()
		{
			string str = "";

			str += this.FunctionName;
			str += "(" + string.Join(" ", this.Arguments) + ")";

			return str;
		}
		#endregion

	}

	/// <summary>
	/// Represents a function prototype syntax tree
	/// </summary>
	public class PrototypeSyntaxTree : ExpressionSyntaxTree
	{

		#region Fields
		private readonly string name;
		private readonly IImmutableList<string> arguments;

		private readonly bool isOperator;
		private readonly int precedence;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new call prototype syntax tree
		/// </summary>
		/// <param name="name">The name of the function</param>
		/// <param name="arguments">The arguments</param>
		/// <param name="isOperator">Indicates if the current prototype is a operator</param>
		/// <param name="precedence">The operator precedence</param>
		public PrototypeSyntaxTree(string name, IEnumerable<string> arguments, bool isOperator = false, int precedence = 0)
		{
			this.name = name;
			this.arguments = arguments.ToImmutableList();
			this.isOperator = isOperator;
			this.precedence = precedence;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Returns the name of the function
		/// </summary>
		public string Name
		{
			get { return this.name; }
		}

		/// <summary>
		/// Returns the arguments
		/// </summary>
		public IImmutableList<string> Arguments
		{
			get { return this.arguments; }
		}

		/// <summary>
		/// Indicates if the current prototype is an operator
		/// </summary>
		public bool IsOperator
		{
			get { return this.isOperator; }
		}

		/// <summary>
		/// Returns the precedence of the operator
		/// </summary>
		public int Precedence
		{
			get { return this.precedence; }
		}

		/// <summary>
		/// Indicates if the current prototype is an unary operator
		/// </summary>
		public bool IsUnaryOperator
		{
			get { return this.isOperator && this.arguments.Count == 1; }
		}

		/// <summary>
		/// Indicates if the current prototype is a binary operator
		/// </summary>
		public bool IsBinaryOperator
		{
			get { return this.isOperator && this.arguments.Count == 2; }
		}

		/// <summary>
		/// Returns the name of operator if an operator else null
		/// </summary>
		public char? OperatorName
		{
			get
			{
				if (this.IsOperator)
				{
					return this.Name[this.Name.Length - 1];
				}

				return null;
			}
		}
		#endregion

		#region Methods
		public override void GenerateCode(CodeGenerator codeGenerator, SyntaxTreeGeneratorData generatorData)
		{

		}

		/// <summary>
		/// Returns a string representation of the current function prototype
		/// </summary>
		/// <param name="includeDef">True if to include the 'def' keyword</param>
		public string ToString(bool includeDef)
		{
			string str = "";

			if (this.Name != "" && this.Arguments.Count > 0)
			{
				if (includeDef)
				{
					str += "def ";
				}

				str += this.Name;
				str += "(" + string.Join(" ", this.Arguments) + ")";
			}

			return str;
		}

		public override string ToString()
		{
			return this.ToString(true);
		}
		#endregion

	}

	/// <summary>
	/// Represents a function syntax tree
	/// </summary>
	public class FunctionSyntaxTree : ExpressionSyntaxTree
	{

		#region Fields
		private readonly PrototypeSyntaxTree prototype;
		private readonly ExpressionSyntaxTree body;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new function syntax tree
		/// </summary>
		/// <param name="name">The prototype</param>
		/// <param name="arguments">The body</param>
		public FunctionSyntaxTree(PrototypeSyntaxTree prototype, ExpressionSyntaxTree body)
		{
			this.prototype = prototype;
			this.body = body;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Returns the prototype
		/// </summary>
		public PrototypeSyntaxTree Prototype
		{
			get { return this.prototype; }
		}

		/// <summary>
		/// Returns the body
		/// </summary>
		public ExpressionSyntaxTree Body
		{
			get { return this.body; }
		}
		#endregion

		#region Methods
		public override void GenerateCode(CodeGenerator codeGenerator, SyntaxTreeGeneratorData generatorData)
		{
			Type[] argumentTypes = new Type[this.Prototype.Arguments.Count];

			for (int i = 0; i < argumentTypes.Length; i++)
			{
				argumentTypes[i] = typeof(double);
			}

			//Create the function
            string funcName = this.Prototype.Name;
            Type returnType = typeof(double);

            if (funcName == "")
            {
                funcName = "main";
            }

            var function = new DynamicMethod(funcName, returnType, argumentTypes);
			var generator = function.GetILGenerator();

			var symbolTable = ImmutableDictionary.Create<string, Symbol>();

			for (int i = 0; i < this.Prototype.Arguments.Count; i++)
			{
				string currentArg = this.Prototype.Arguments[i];
				symbolTable = symbolTable.Add(currentArg, new FunctionArgumentSymbol(i));
			}

			if (!codeGenerator.Methods.ContainsKey(funcName))
			{
				codeGenerator.Methods.Add(funcName, function);

				//Emit the method
				this.Body.GenerateCode(
					codeGenerator,
					new SyntaxTreeGeneratorData(generator, symbolTable));

				generator.Emit(OpCodes.Ret);

				//If a binary operator, add it to the operator table
				if (this.Prototype.IsBinaryOperator)
				{
					codeGenerator.Session.DefineBinaryOperator(
						this.Prototype.OperatorName.Value,
						this.Prototype.Precedence);
				}
			}
			else
			{
				throw new CodeGeneratorException("Function '" + funcName + "' is already defined.", this);
			}
		}

		public override string ToString()
		{
			string firstPart = this.Prototype.ToString();

			if (firstPart != "")
			{
				firstPart += " ";
			}

			return firstPart + this.Body.ToString();
		}
		#endregion

	}

	/// <summary>
	/// Represents a external function syntax tree
	/// </summary>
	public class ExternalFunctionSyntaxTree : ExpressionSyntaxTree
	{

		#region Fields
		private readonly PrototypeSyntaxTree prototype;
		private readonly string funcReference;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new external function syntax tree
		/// </summary>
		/// <param name="name">The prototype</param>
		/// <param name="funcReference">The name of the external function</param>
		public ExternalFunctionSyntaxTree(PrototypeSyntaxTree prototype, string funcReference)
		{
			this.prototype = prototype;
			this.funcReference = funcReference;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Returns the prototype
		/// </summary>
		public PrototypeSyntaxTree Prototype
		{
			get { return this.prototype; }
		}

		/// <summary>
		/// Returns the name of the external function
		/// </summary>
		public string FuncReference
		{
			get { return this.funcReference; }
		}
		#endregion

		#region Methods
		public override void GenerateCode(CodeGenerator codeGenerator, SyntaxTreeGeneratorData generatorData)
		{
			//Find the .NET class and method reference
			string[] splitedFuncRef = this.FuncReference.Split('.');
			string funcClassName = string.Join(".", splitedFuncRef.Take(splitedFuncRef.Length - 1));
			string funcName = splitedFuncRef.Last();

			Type[] typeArgs = this.Prototype.Arguments.Select(_ => typeof(double)).ToArray();

			Type funcClass = Type.GetType(funcClassName);
			
			if (funcClass == null)
			{
				throw new CodeGeneratorException("Could not find class '" + funcClassName + "'.", this);
			}

			MethodInfo func = funcClass.GetMethod(funcName, typeArgs);

			if (func == null)
			{
				throw new CodeGeneratorException("Could not find method '" + funcName + "' in class '" + funcClassName + "'.", this);
			}

			if (func.ReturnType == typeof(double) || func.ReturnType == typeof(void))
			{
				if (!codeGenerator.Methods.ContainsKey(this.Prototype.Name))
				{
					codeGenerator.Methods[this.Prototype.Name] = func;
				}
				else
				{
					throw new CodeGeneratorException("The function '" + this.Prototype.Name + "' is already defined.", this);
				}
			}
			else
			{
				throw new CodeGeneratorException("External function must return type of double or void. Return type: " + func.ReturnType.Name, this);
			}
		}

		public override string ToString()
		{
			return "extern " + this.Prototype.ToString(false) + " :: " +  this.FuncReference;
		}
		#endregion

	}

	/// <summary>
	/// Represents an if expression syntax tree
	/// </summary>
	public class IfExpressionSyntaxTree : ExpressionSyntaxTree
	{

		#region Fields
		private readonly ExpressionSyntaxTree condition;
		private readonly ExpressionSyntaxTree thenBody;
		private readonly ExpressionSyntaxTree elseBody;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new if expression syntax tree
		/// </summary>
		/// <param name="condition">The condition</param>
		/// <param name="thenBody">The then body</param>
		/// <param name="elseBody">The else body</param>
		public IfExpressionSyntaxTree(ExpressionSyntaxTree condition, ExpressionSyntaxTree thenBody, ExpressionSyntaxTree elseBody)
		{
			this.condition = condition;
			this.thenBody = thenBody;
			this.elseBody = elseBody;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Returns the condition
		/// </summary>
		public ExpressionSyntaxTree Condition
		{
			get { return this.condition; }
		}

		/// <summary>
		/// Returns the then body
		/// </summary>
		public ExpressionSyntaxTree ThenBody
		{
			get { return this.thenBody; }
		}

		/// <summary>
		/// Returns the else body
		/// </summary>
		public ExpressionSyntaxTree ElseBody
		{
			get { return this.elseBody; }
		}
		#endregion

		#region Methods
		public override void GenerateCode(CodeGenerator codeGenerator, SyntaxTreeGeneratorData generatorData)
		{
			Label endLabel = generatorData.ILGenerator.DefineLabel();
			Label elseLabel = generatorData.ILGenerator.DefineLabel();

			this.Condition.GenerateCode(codeGenerator, generatorData);
			generatorData.ILGenerator.Emit(OpCodes.Ldc_R8, 0.0);
			generatorData.ILGenerator.Emit(OpCodes.Beq, elseLabel);

			this.ThenBody.GenerateCode(codeGenerator, generatorData);
			generatorData.ILGenerator.Emit(OpCodes.Br, endLabel);

			generatorData.ILGenerator.MarkLabel(elseLabel);
			this.ElseBody.GenerateCode(codeGenerator, generatorData);
			generatorData.ILGenerator.MarkLabel(endLabel);
		}

		public override string ToString()
		{
			return "if " + this.Condition.ToString() + " then " + this.ThenBody.ToString() + " else " + this.ElseBody.ToString();
		}
		#endregion

	}

    /// <summary>
    /// Represents a for expression syntax tree
    /// </summary>
    public class ForExpressionSyntaxTree : ExpressionSyntaxTree
    {

        #region Fields
        private readonly string variableName;
        private readonly ExpressionSyntaxTree start;
        private readonly ExpressionSyntaxTree end;
        private readonly ExpressionSyntaxTree step;
        private readonly ExpressionSyntaxTree body;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new for expression syntax tree
        /// </summary>
        /// <param name="variableName">The name of the loop variable</param>
        /// <param name="start">The start value</param>
        /// <param name="end">The stop value</param>
        /// <param name="step">The step</param>
        /// <param name="body">The body</param>
        public ForExpressionSyntaxTree(string variableName, ExpressionSyntaxTree start, ExpressionSyntaxTree end, ExpressionSyntaxTree step, ExpressionSyntaxTree body)
        {
            this.variableName = variableName;
            this.start = start;
            this.end = end;
            this.step = step;
            this.body = body;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Returns the name of the loop variable
        /// </summary>
        public string VariableName
        {
            get { return this.variableName; }
        }

        /// <summary>
        /// Returns the start value
        /// </summary>
        public ExpressionSyntaxTree Start
        {
            get { return this.start; }
        }

        /// <summary>
        /// Returns the end value
        /// </summary>
        public ExpressionSyntaxTree End
        {
            get { return this.end; }
        }

        /// <summary>
        /// Returns the step value
        /// </summary>
        public ExpressionSyntaxTree Step
        {
            get { return this.step; }
        }

        /// <summary>
        /// Returns the body
        /// </summary>
        public ExpressionSyntaxTree Body
        {
            get { return this.body; }
        }
        #endregion

        #region Methods
        public override void GenerateCode(CodeGenerator codeGenerator, SyntaxTreeGeneratorData generatorData)
        {
			LocalBuilder loopVar = generatorData.ILGenerator.DeclareLocal(typeof(double));
			this.Start.GenerateCode(codeGenerator, generatorData);
			generatorData.ILGenerator.Emit(OpCodes.Stloc, loopVar);
			
			//Add the loop variable
			generatorData = generatorData.WithSymbol(this.VariableName, new LocalVariableSymbol(loopVar));

			Label loopStart = generatorData.ILGenerator.DefineLabel();
			Label end = generatorData.ILGenerator.DefineLabel();

			//Emit the loop condition
			generatorData.ILGenerator.MarkLabel(loopStart);
			this.End.GenerateCode(codeGenerator, generatorData);
			generatorData.ILGenerator.Emit(OpCodes.Ldc_R8, 0.0);
			generatorData.ILGenerator.Emit(OpCodes.Beq, end);

			//The body
			this.Body.GenerateCode(codeGenerator, generatorData);

			//Pop any value returned from the body
			generatorData.ILGenerator.Emit(OpCodes.Pop);

			//The step
			generatorData.ILGenerator.Emit(OpCodes.Ldloc, loopVar);
			this.Step.GenerateCode(codeGenerator, generatorData);
			generatorData.ILGenerator.Emit(OpCodes.Add);
			generatorData.ILGenerator.Emit(OpCodes.Stloc, loopVar);
			generatorData.ILGenerator.Emit(OpCodes.Br, loopStart);

			generatorData.ILGenerator.MarkLabel(end);
			generatorData.ILGenerator.Emit(OpCodes.Ldc_R8, 0.0);
        }        
        #endregion

    }
}
