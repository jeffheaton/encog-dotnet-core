using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;
using Encog.ML.Prg.ExpValue;
using Encog.ML.Prg.Ext;
using Encog.ML.EA.Exceptions;

namespace Encog.ML.Prg
{
    /// <summary>
    /// Every EncogProgram must belong to a context. When programs are in a
    /// population, they must all share a common context. The context defines
    /// attributes that are common to all programs. The following information is
    /// stored in a context.
    /// 
    /// The number formatting used. Namely, what type of radix point should strings
    /// be parsed/rendered to.
    /// 
    /// The functions, or opcodes, that are available to the program. This defines
    /// the set of functions & operators that a program might use. For an Encog
    /// Program all operators are treated as functions internally. A operator is
    /// essentially a shortcut notation for common functions.
    /// 
    /// The defined variables. These variables are constant for the run of the
    /// program, but typically change for each run. They are essentially the
    /// variables that make up an algebraic expression.
    /// 
    /// ly, the return value mapping for the programs.
    /// </summary>
    [Serializable]
    public class EncogProgramContext
    {
        /// <summary>
        /// The number formatting used. Namely, what type of radix point should
	    /// strings be parsed/rendered to.
        /// </summary>
        private CSVFormat format;

        /// <summary>
        /// The functions, or opcodes, that are available to the program. This
        /// defines the set of functions & operators that a program might use. For an
        /// Encog Program all operators are treated as functions internally. A
        /// operator is essentially a shortcut notation for common functions.
        /// </summary>
        private FunctionFactory functions;

        /// <summary>
        /// The defined variables. These variables are constant for the run of the
        /// program, but typically change for each run. They are essentially the
        /// variables that make up an algebraic expression.
        /// </summary>
        private IList<VariableMapping> definedVariables = new List<VariableMapping>();

        /// <summary>
        /// Lookup map for the defined variables.
        /// </summary>
        private IDictionary<String, VariableMapping> map = new Dictionary<String, VariableMapping>();

        /// <summary>
        /// The return value mapping for the programs.
        /// </summary>
        private VariableMapping result = new VariableMapping(null,
                EPLValueType.floatingType);

        /// <summary>
        /// Construct the context with an English number format and an empty function
        /// factory.
        /// </summary>
        public EncogProgramContext()
            : this(CSVFormat.English, new FunctionFactory())
        {
        }

        /// <summary>
        /// Construct a context with the specified number format and an empty
        /// function factory.
        /// </summary>
        /// <param name="format">The format.</param>
        public EncogProgramContext(CSVFormat format)
            : this(format, new FunctionFactory())
        {

        }

        /// <summary>
        /// Construct the context with the specified format and function factory.
        /// </summary>
        /// <param name="theFormat">The format.</param>
        /// <param name="theFunctions">The function factory.</param>
        public EncogProgramContext(CSVFormat theFormat,
                 FunctionFactory theFunctions)
        {
            this.format = theFormat;
            this.functions = theFunctions;
        }

        /// <summary>
        /// Clear the defined variables.
        /// </summary>
        public void ClearDefinedVariables()
        {
            this.definedVariables.Clear();
            this.map.Clear();
        }

        /// <summary>
        /// Clone a branch of the program from the specified node.
        /// </summary>
        /// <param name="targetProgram">The program that this branch will be "grafted" into.</param>
        /// <param name="sourceBranch">The branch to clone, from the source program.</param>
        /// <returns>The cloned branch.</returns>
        public ProgramNode CloneBranch(EncogProgram targetProgram,
                 ProgramNode sourceBranch)
        {
            if (sourceBranch == null)
            {
                throw new EncogError("Can't clone null branch.");
            }

            String name = sourceBranch.Name;

            // create any subnodes
            ProgramNode[] args = new ProgramNode[sourceBranch.ChildNodes.Count];
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = CloneBranch(targetProgram, (ProgramNode)sourceBranch
                        .ChildNodes[i]);
            }

            ProgramNode result = targetProgram.Context.Functions
                   .FactorProgramNode(name, targetProgram, args);

            // now copy the expression data for the node
            for (int i = 0; i < sourceBranch.Data.Length; i++)
            {
                result.Data[i] = new ExpressionValue(sourceBranch.Data[i]);
            }

            // return the new node
            return result;
        }

        /// <summary>
        /// Clone an entire program, keep the same context.
        /// </summary>
        /// <param name="sourceProgram">The source program.</param>
        /// <returns>The cloned program.</returns>
        public EncogProgram CloneProgram(EncogProgram sourceProgram)
        {
            ProgramNode rootNode = sourceProgram.RootNode;
            EncogProgram result = new EncogProgram(this);
            result.RootNode = CloneBranch(result, rootNode);
            return result;
        }

        /// <summary>
        /// Create a new program, using this context.
        /// </summary>
        /// <param name="expression">The common expression to compile.</param>
        /// <returns>The resulting program.</returns>
        public EncogProgram CreateProgram(String expression)
        {
            EncogProgram result = new EncogProgram(this);
            result.CompileExpression(expression);
            return result;
        }

        /// <summary>
        /// Define the specified variable as floating point.
        /// </summary>
        /// <param name="theName">The variable name to define.</param>
        public void DefineVariable(String theName)
        {
            DefineVariable(theName, EPLValueType.floatingType, 0, 0);
        }

        /// <summary>
        /// Define the specified variable as the specified type. Don't use this for
        /// enums.
        /// </summary>
        /// <param name="theName">The name of the variable.</param>
        /// <param name="theVariableType">The variable type.</param>
        public void DefineVariable(String theName,
                 EPLValueType theVariableType)
        {
            DefineVariable(theName, theVariableType, 0, 0);
        }

        /// <summary>
        /// Define a variable. 
        /// </summary>
        /// <param name="theName">The name of the variable.</param>
        /// <param name="theVariableType">The type of variable.</param>
        /// <param name="theEnumType">The enum type, not used if not an enum type.</param>
        /// <param name="theEnumValueCount">The number of values for the enum, not used if not an enum
        /// type.</param>
        public void DefineVariable(String theName,
                 EPLValueType theVariableType, int theEnumType,
                 int theEnumValueCount)
        {
            VariableMapping mapping = new VariableMapping(theName,
                   theVariableType, theEnumType, theEnumValueCount);
            DefineVariable(mapping);

        }

        /// <summary>
        /// Define a variable, based on a mapping.
        /// </summary>
        /// <param name="mapping">The variable mapping.</param>
        public void DefineVariable(VariableMapping mapping)
        {
            if (this.map.ContainsKey(mapping.Name))
            {
                throw new EACompileError("Variable " + mapping.Name
                        + " already defined.");
            }
            this.map[mapping.Name] = mapping;
            this.definedVariables.Add(mapping);
        }

        /// <summary>
        /// Find all of the variables of the specified types.
        /// </summary>
        /// <param name="desiredTypes">The types to look for.</param>
        /// <returns>The variables that matched the specified types.</returns>
        public IList<VariableMapping> FindVariablesByTypes(
                 IList<EPLValueType> desiredTypes)
        {
            IList<VariableMapping> result = new List<VariableMapping>();

            foreach (VariableMapping mapping in this.definedVariables)
            {
                if (desiredTypes.Contains(mapping.VariableType))
                {
                    result.Add(mapping);
                }
            }

            return result;
        }

        /// <summary>
        /// The defined variables.
        /// </summary>
        public IList<VariableMapping> DefinedVariables
        {
            get
            {
                return this.definedVariables;
            }
        }

        /// <summary>
        /// Get the enum ordinal count for the specified enumeration type.
        /// </summary>
        /// <param name="enumType">The enumeration type.</param>
        /// <returns>The ordinal count for the specified enumeration type.</returns>
        public int GetEnumCount(int enumType)
        {

            // make sure we consider the result
            if (this.result.VariableType == EPLValueType.enumType
                    && this.result.EnumType == enumType)
            {
                return this.result.EnumValueCount;
            }

            foreach (VariableMapping mapping in this.definedVariables)
            {
                if (mapping.VariableType == EPLValueType.enumType)
                {
                    if (mapping.EnumType == enumType)
                    {
                        return mapping.EnumValueCount;
                    }
                }
            }
            throw new EACompileError("Undefined enum type: " + enumType);
        }

        /// <summary>
        /// The number formatting used. Namely, what type of radix point
        /// should strings be parsed/rendered to.
        /// </summary>
        public CSVFormat Format
        {
            get
            {
                return this.format;
            }
        }

        /// <summary>
        /// The functions, or opcodes, that are available to the program.
        /// This defines the set of functions & operators that a program
        /// might use. For an Encog Program all operators are treated as
        /// functions internally. A operator is essentially a shortcut
        /// notation for common functions.
        /// </summary>
        public FunctionFactory Functions
        {
            get
            {
                return this.functions;
            }
        }

        /// <summary>
        /// Get the max enum type for all defined variables.
        /// </summary>
        /// <returns>The max enumeration type.</returns>
        public int GetMaxEnumType()
        {
            int r = -1;

            // make sure we consider the result
            if (this.result.VariableType == EPLValueType.enumType)
            {
                r = this.result.EnumType;
            }

            // loop over all mappings and find the max enum type
            foreach (VariableMapping mapping in this.definedVariables)
            {
                if (mapping.VariableType == EPLValueType.enumType)
                {
                    r = Math.Max(r, mapping.EnumType);
                }
            }

            // if we did not find one then there are no enum types
            if (r == -1)
            {
                throw new EACompileError("No enum types defined in context.");
            }

            return r;
        }

        /// <summary>
        /// The result of the program.
        /// </summary>
        public VariableMapping Result
        {
            get
            {
                return this.result;
            }
            set
            {
                this.result = value;
            }
        }

        /// <summary>
        /// True, if enums are defined.
        /// </summary>
        public bool HasEnum
        {
            get
            {
                if (this.result.VariableType == EPLValueType.enumType)
                {
                    return true;
                }

                foreach (VariableMapping mapping in this.definedVariables)
                {
                    if (mapping.VariableType == EPLValueType.enumType)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Load all known functions as opcodes.
        /// </summary>
        public void LoadAllFunctions()
        {
            StandardExtensions.CreateAll(this);
        }
    }
}
