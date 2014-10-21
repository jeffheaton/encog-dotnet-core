//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.Collections.Generic;
using System.Text;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.EA.Exceptions;
using Encog.ML.EA.Genome;
using Encog.ML.Prg.ExpValue;
using Encog.ML.Prg.Ext;
using Encog.ML.Prg.Train;
using Encog.ML.Tree.Traverse.Tasks;
using Encog.MathUtil.Randomize;
using Encog.Parse.Expression.Common;
using Encog.Parse.Expression.EPL;
using Encog.Parse.Expression.RPN;
using Encog.Util.Simple;

namespace Encog.ML.Prg
{
    /// <summary>
    ///     Holds an Encog Programming Language (EPL) program. A Encog program is
    ///     internally represented as a tree structure. It can be rendered to a variety
    ///     of forms, such as RPN, common infix expressions, or Latex. The Encog
    ///     Workbench also allows display as a graphical tree. An Encog Program is both a
    ///     genome and phenome. No decoding is necessary.
    ///     Every Encog Program has a context. The context is the same for every Encog
    ///     Program in a population. The context defines which opcodes should be used, as
    ///     well as the defined variables.
    ///     The actual values for the variables are not stored in the context. Rather
    ///     they are stored in a variable holder. Each program usually has its own
    ///     variable holder, though it is possible to share.
    /// </summary>
    [Serializable]
    public class EncogProgram : BasicGenome, IMLRegression, IMLError
    {
        /// <summary>
        ///     The context that this Encog program executes in, the context is typically
        ///     shared with other Encog programs.
        /// </summary>
        private readonly EncogProgramContext _context = new EncogProgramContext();

        /// <summary>
        ///     Holds extra data that might be needed by user extended opcodes.
        /// </summary>
        private readonly IDictionary<String, Object> _extraData = new Dictionary<String, Object>();

        /// <summary>
        ///     The variables that will be used by this Encog program.
        /// </summary>
        private readonly EncogProgramVariables _variables = new EncogProgramVariables();

        /// <summary>
        ///     Construct the Encog program and create a default context and variable
        ///     holder. Use all available opcodes.
        /// </summary>
        public EncogProgram()
            : this(new EncogProgramContext(), new EncogProgramVariables())
        {
            StandardExtensions.CreateAll(_context);
        }

        /// <summary>
        ///     Construct the Encog program with the specified context, but create a new
        ///     variable holder.
        /// </summary>
        /// <param name="theContext">The context.</param>
        public EncogProgram(EncogProgramContext theContext)
            : this(theContext, new EncogProgramVariables())
        {
        }

        /// <summary>
        ///     Construct an Encog program using the specified context and variable
        ///     holder.
        /// </summary>
        /// <param name="theContext">The context.</param>
        /// <param name="theVariables">The variable holder.</param>
        public EncogProgram(EncogProgramContext theContext,
                            EncogProgramVariables theVariables)
        {
            _context = theContext;
            _variables = theVariables;

            // define variables
            foreach (VariableMapping v in _context.DefinedVariables)
            {
                _variables.DefineVariable(v);
            }
        }

        /// <summary>
        ///     Construct an Encog program using the specified expression, but create an
        ///     empty context and variable holder.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public EncogProgram(String expression)
            : this()
        {
            CompileExpression(expression);
        }

        /// <summary>
        ///     The root node of the program.
        /// </summary>
        public ProgramNode RootNode { get; set; }

        /// <summary>
        ///     The program context. The program context may be shared over
        ///     multiple programs.
        /// </summary>
        public EncogProgramContext Context
        {
            get { return _context; }
        }

        /// <summary>
        ///     The function factory from the context.
        /// </summary>
        public FunctionFactory Functions
        {
            get { return _context.Functions; }
        }

        /// <summary>
        ///     The variable mapping for the result type. This is obtained from
        ///     the context.
        /// </summary>
        private VariableMapping ResultType
        {
            get { return ((PrgPopulation) Population).Context.Result; }
        }

        /// <summary>
        ///     The return type, from the context.
        /// </summary>
        public EPLValueType ReturnType
        {
            get { return _context.Result.VariableType; }
        }

        /// <summary>
        ///     The variable holder.
        /// </summary>
        public EncogProgramVariables Variables
        {
            get { return _variables; }
        }

        /// <inheritdoc />
        public override int Size
        {
            get { return RootNode.Count; }
        }

        /// <inheritdoc />
        public double CalculateError(IMLDataSet data)
        {
            return EncogUtility.CalculateRegressionError(this, data);
        }

        /// <summary>
        ///     Compute the output from the input MLData. The individual values of the
        ///     input will be mapped to the variables defined in the context. The order
        ///     is the same between the input and the defined variables. The input will
        ///     be mapped to the appropriate types. Enums will use their ordinal number.
        ///     The result will be a single number MLData.
        /// </summary>
        /// <param name="input">The input to the program.</param>
        /// <returns>A single numer MLData.</returns>
        public IMLData Compute(IMLData input)
        {
            if (input.Count != InputCount)
            {
                throw new EACompileError("Invalid input count.");
            }

            for (int i = 0; i < input.Count; i++)
            {
                _variables.SetVariable(i, input[i]);
            }

            ExpressionValue v = RootNode.Evaluate();
            VariableMapping resultMapping = ResultType;

            var result = new BasicMLData(1);
            bool success = false;

            switch (resultMapping.VariableType)
            {
                case EPLValueType.FloatingType:
                    if (v.IsNumeric)
                    {
                        result.Data[0] = v.ToFloatValue();
                        success = true;
                    }
                    break;
                case EPLValueType.StringType:
                    result.Data[0] = v.ToFloatValue();
                    success = true;
                    break;
                case EPLValueType.BooleanType:
                    if (v.IsBoolean)
                    {
                        result.Data[0] = v.ToBooleanValue() ? 1.0 : 0.0;
                        success = true;
                    }
                    break;
                case EPLValueType.IntType:
                    if (v.IsNumeric)
                    {
                        result[0] = v.ToIntValue();
                        success = true;
                    }
                    break;
                case EPLValueType.EnumType:
                    if (v.IsEnum)
                    {
                        result.Data[0] = v.ToIntValue();
                        success = true;
                    }
                    break;
            }

            if (!success)
            {
                throw new EARuntimeError("EncogProgram produced "
                                         + v.ExprType.ToString() + " but "
                                         + resultMapping.VariableType.ToString()
                                         + " was expected.");
            }

            return result;
        }

        /// <inheritdoc />
        public int InputCount
        {
            get { return _variables.Count; }
        }

        /// <inheritdoc />
        public int OutputCount
        {
            get { return 1; }
        }

        /// <summary>
        ///     Parse the specified program, or expression, and return the result. No
        ///     variables can be defined for this as a default context is used. The
        ///     result is returned as a boolean.
        /// </summary>
        /// <param name="str">The program expression.</param>
        /// <returns>The value the expression was evaluated to.</returns>
        public static bool ParseBoolean(String str)
        {
            var holder = new EncogProgram(str);
            return holder.Evaluate().ToBooleanValue();
        }

        /// <summary>
        ///     Parse the specified program, or expression, and return the result. No
        ///     variables can be defined for this as a default context is used. The
        ///     result is returned as a boolean.
        /// </summary>
        /// <param name="str">The program expression.</param>
        /// <returns>The value the expression was evaluated to.</returns>
        public static ExpressionValue ParseExpression(String str)
        {
            var holder = new EncogProgram(str);
            return holder.Evaluate();
        }

        /// <summary>
        ///     Parse the specified program, or expression, and return the result. No
        ///     variables can be defined for this as a default context is used. The
        ///     result is returned as a float.
        /// </summary>
        /// <param name="str">The program expression value.</param>
        /// <returns>The value the expression was evaluated to.</returns>
        public static double ParseFloat(String str)
        {
            var holder = new EncogProgram(str);
            return holder.Evaluate().ToFloatValue();
        }

        /// <summary>
        ///     Parse the specified program, or expression, and return the result. No
        ///     variables can be defined for this as a default context is used. The
        ///     result is returned as a string.
        /// </summary>
        /// <param name="str">The program expression value.</param>
        /// <returns>The value the expression was evaluated to.</returns>
        public static String ParseString(String str)
        {
            var holder = new EncogProgram(str);
            return holder.Evaluate().ToStringValue();
        }

        /// <summary>
        ///     Compile the specified EPL into an actual program node structure, for
        ///     later execution.
        /// </summary>
        /// <param name="code">The code to compile.</param>
        /// <returns>The root node.</returns>
        public ProgramNode CompileEPL(String code)
        {
            var parser = new ParseEPL(this);
            RootNode = parser.Parse(code);
            return RootNode;
        }

        /// <summary>
        ///     Compile the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The program node that this was compiled into.</returns>
        public ProgramNode CompileExpression(String expression)
        {
            var parser = new ParseCommonExpression(this);
            RootNode = parser.Parse(expression);
            return RootNode;
        }

        /// <inheritdoc />
        public override void Copy(IGenome source)
        {
            // not needed, already a genome
        }

        /// <summary>
        ///     The string as a common "infix" expression.
        /// </summary>
        /// <returns>The string as a common "infix" expression.</returns>
        public String DumpAsCommonExpression()
        {
            var render = new RenderCommonExpression();
            return render.Render(this);
        }

        /// <summary>
        ///     Execute the program and return the result.
        /// </summary>
        /// <returns>The result of running the program.</returns>
        public ExpressionValue Evaluate()
        {
            return RootNode.Evaluate();
        }

        /// <summary>
        ///     Find the specified node by index. The tree is traversed to do this. This
        ///     is typically used to select a random node.
        /// </summary>
        /// <param name="index">The index being sought.</param>
        /// <returns>The program node found.</returns>
        public ProgramNode FindNode(int index)
        {
            return (ProgramNode) TaskGetNodeIndex.process(index, RootNode);
        }

        /// <summary>
        ///     The string as an EPL expression. EPL is the format that
        ///     EncogPrograms are usually persisted as.
        /// </summary>
        /// <returns>EPL code.</returns>
        public String GenerateEPL()
        {
            var render = new RenderEPL();
            return render.Render(this);
        }

        /// <summary>
        ///     Replace the specified node with another.
        /// </summary>
        /// <param name="replaceThisNode">The node to replace.</param>
        /// <param name="replaceWith">The node that is replacing that node.</param>
        public void ReplaceNode(ProgramNode replaceThisNode,
                                ProgramNode replaceWith)
        {
            if (replaceThisNode == RootNode)
            {
                RootNode = replaceWith;
            }
            else
            {
                TaskReplaceNode
                    .Process(RootNode, replaceThisNode, replaceWith);
            }
        }

        /// <summary>
        ///     Select a random variable from the defined variables.
        /// </summary>
        /// <param name="rnd">A random number generator.</param>
        /// <param name="desiredTypes">The desired types that the variable can be.</param>
        /// <returns>The index of the defined variable, or -1 if unable to define.</returns>
        public int SelectRandomVariable(EncogRandom rnd,
                                        IList<EPLValueType> desiredTypes)
        {
            IList<VariableMapping> selectionSet = _context
                .FindVariablesByTypes(desiredTypes);
            if (selectionSet.Count == 0
                && desiredTypes.Contains(EPLValueType.IntType))
            {
                IList<EPLValueType> floatList = new List<EPLValueType>();
                floatList.Add(EPLValueType.FloatingType);
                selectionSet = _context.FindVariablesByTypes(floatList);
            }

            if (selectionSet.Count == 0)
            {
                return -1;
            }

            VariableMapping selected = selectionSet[rnd.Next(selectionSet.Count)];
            return Context.DefinedVariables.IndexOf(selected);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var render = new RenderRPN();
            String code = render.Render(this);
            var result = new StringBuilder();
            result.Append("[EncogProgram: size=");
            result.Append(Size);
            result.Append(", score=");
            result.Append(Score);
            result.Append(",code=");
            result.Append(code);
            result.Append("]");
            return result.ToString();
        }

        /// <summary>
        ///     Get extra data that might be needed by user extended opcodes.
        /// </summary>
        /// <param name="name">The name the data was stored under.</param>
        /// <returns>The extra data.</returns>
        public Object GetExtraData(String name)
        {
            return _extraData[name];
        }

        /// <summary>
        ///     Set extra data that might be needed by extensions.
        /// </summary>
        /// <param name="name">The name of the data stored.</param>
        /// <param name="value">The data.</param>
        public void SetExtraData(String name, Object value)
        {
            _extraData[name] = value;
        }
    }
}
