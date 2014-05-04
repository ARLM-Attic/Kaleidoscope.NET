using System;
using System.Collections.Generic;
using System.Linq;
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

				if (argSymbol != null)
				{
					generatorData.ILGenerator.Emit(OpCodes.Ldarg, argSymbol.ArgumentIndex);
				}
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
			}
		}

		public override string ToString()
		{
			return this.LeftHandSide.ToString() + this.Operator + this.RightHandSide.ToString();
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
			foreach (var currentArg in this.Arguments)
			{
				currentArg.GenerateCode(codeGenerator, generatorData);
			}

			generatorData.ILGenerator.EmitCall(OpCodes.Call, codeGenerator.Methods[this.FunctionName], null);
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
		private readonly List<string> arguments;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new call prototype syntax tree
		/// </summary>
		/// <param name="name">The name of the function</param>
		/// <param name="arguments">The arguments</param>
		public PrototypeSyntaxTree(string name, List<string> arguments)
		{
			this.name = name;
			this.arguments = arguments;
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
		public IReadOnlyList<string> Arguments
		{
			get { return this.arguments.AsReadOnly(); }
		}
		#endregion

		#region Methods
		public override void GenerateCode(CodeGenerator codeGenerator, SyntaxTreeGeneratorData generatorData)
		{

		}

		public override string ToString()
		{
			string str = "";

			if (this.Name != "" && this.Arguments.Count > 0)
			{
				str += "def ";
				str += this.Name;
				str += "(" + string.Join(" ", this.Arguments) + ")";
			}

			return str;
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
			DynamicMethod function = new DynamicMethod(this.Prototype.Name, typeof(double), argumentTypes);
			var generator = function.GetILGenerator();

			Dictionary<string, Symbol> symbolTable = new Dictionary<string, Symbol>();

			for (int i = 0; i < this.Prototype.Arguments.Count; i++)
			{
				string currentArg = this.Prototype.Arguments[i];
				symbolTable.Add(currentArg, new FunctionArgumentSymbol(i));
			}

			string funcName = this.Prototype.Name;

			if (funcName == "")
			{
				funcName = "main";
			}

			codeGenerator.Methods.Add(funcName, function);

			//Emit the method
			this.Body.GenerateCode(
				codeGenerator,
				new SyntaxTreeGeneratorData(generator, symbolTable));

			generator.Emit(OpCodes.Ret);
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
}
