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
using System.Linq;
using Encog.ML.EA.Exceptions;
using Encog.ML.Prg.ExpValue;
using Encog.ML.Prg.Ext;
using Encog.Util.CSV;

namespace Encog.ML.Prg
{
    /// <summary>
    ///     Every EncogProgram must belong to a context. When programs are in a
    ///     population, they must all share a common context. The context defines
    ///     attributes that are common to all programs. The following information is
    ///     stored in a context.
    ///     The number formatting used. Namely, what type of radix point should strings
    ///     be parsed/rendered to.
    ///     The functions, or opcodes, that are available to the program. This defines
    ///     the set of functions & operators that a program might use. For an Encog
    ///     Program all operators are treated as functions internally. A operator is
    ///     essentially a shortcut notation for common functions.
    ///     The defined variables. These variables are constant for the run of the
    ///     program, but typically change for each run. They are essentially the
    ///     variables that make up an algebraic expression.
    ///     ly, the return value mapping for the programs.
    /// </summary>
    [Serializable]
    public class EncogProgramContext
    {
        /// <summary>
        ///     The defined variables. These variables are constant for the run of the
        ///     program, but typically change for each run. They are essentially the
        ///     variables that make up an algebraic expression.
        /// </summary>
        private readonly IList<VariableMapping> _definedVariables = new List<VariableMapping>();

        /// <summary>
        ///     The number formatting used. Namely, what type of radix point should
        ///     strings be parsed/rendered to.
        /// </summary>
        private readonly CSVFormat _format;

        /// <summary>
        ///     The functions, or opcodes, that are available to the program. This
        ///     defines the set of functions & operators that a program might use. For an
        ///     Encog Program all operators are treated as functions internally. A
        ///     operator is essentially a shortcut notation for common functions.
        /// </summary>
        private readonly FunctionFactory _functions;

        /// <summary>
        ///     Lookup map for the defined variables.
        /// </summary>
        private readonly IDictionary<String, VariableMapping> _map = new Dictionary<String, VariableMapping>();

        /// <summary>
        ///     The return value mapping for the programs.
        /// </summary>
        private VariableMapping _result = new VariableMapping(null,
                                                             EPLValueType.FloatingType);

        /// <summary>
        ///     Construct the context with an English number format and an empty function
        ///     factory.
        /// </summary>
        public EncogProgramContext()
            : this(CSVFormat.English, new FunctionFactory())
        {
        }

        /// <summary>
        ///     Construct a context with the specified number format and an empty
        ///     function factory.
        /// </summary>
        /// <param name="format">The format.</param>
        public EncogProgramContext(CSVFormat format)
            : this(format, new FunctionFactory())
        {
        }

        /// <summary>
        ///     Construct the context with the specified format and function factory.
        /// </summary>
        /// <param name="theFormat">The format.</param>
        /// <param name="theFunctions">The function factory.</param>
        public EncogProgramContext(CSVFormat theFormat,
                                   FunctionFactory theFunctions)
        {
            _format = theFormat;
            _functions = theFunctions;
        }

        /// <summary>
        ///     The defined variables.
        /// </summary>
        public IList<VariableMapping> DefinedVariables
        {
            get { return _definedVariables; }
        }

        /// <summary>
        ///     The number formatting used. Namely, what type of radix point
        ///     should strings be parsed/rendered to.
        /// </summary>
        public CSVFormat Format
        {
            get { return _format; }
        }

        /// <summary>
        ///     The functions, or opcodes, that are available to the program.
        ///     This defines the set of functions & operators that a program
        ///     might use. For an Encog Program all operators are treated as
        ///     functions internally. A operator is essentially a shortcut
        ///     notation for common functions.
        /// </summary>
        public FunctionFactory Functions
        {
            get { return _functions; }
        }

        /// <summary>
        ///     The result of the program.
        /// </summary>
        public VariableMapping Result
        {
            get { return _result; }
            set { _result = value; }
        }

        /// <summary>
        ///     True, if enums are defined.
        /// </summary>
        public bool HasEnum
        {
            get
            {
                if (_result.VariableType == EPLValueType.EnumType)
                {
                    return true;
                }

                return _definedVariables.Any(mapping => mapping.VariableType == EPLValueType.EnumType);
            }
        }

        /// <summary>
        ///     Clear the defined variables.
        /// </summary>
        public void ClearDefinedVariables()
        {
            _definedVariables.Clear();
            _map.Clear();
        }

        /// <summary>
        ///     Clone a branch of the program from the specified node.
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
            var args = new ProgramNode[sourceBranch.ChildNodes.Count];
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = CloneBranch(targetProgram, (ProgramNode) sourceBranch
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
        ///     Clone an entire program, keep the same context.
        /// </summary>
        /// <param name="sourceProgram">The source program.</param>
        /// <returns>The cloned program.</returns>
        public EncogProgram CloneProgram(EncogProgram sourceProgram)
        {
            ProgramNode rootNode = sourceProgram.RootNode;
            var result = new EncogProgram(this);
            result.RootNode = CloneBranch(result, rootNode);
            return result;
        }

        /// <summary>
        ///     Create a new program, using this context.
        /// </summary>
        /// <param name="expression">The common expression to compile.</param>
        /// <returns>The resulting program.</returns>
        public EncogProgram CreateProgram(String expression)
        {
            var result = new EncogProgram(this);
            result.CompileExpression(expression);
            return result;
        }

        /// <summary>
        ///     Define the specified variable as floating point.
        /// </summary>
        /// <param name="theName">The variable name to define.</param>
        public void DefineVariable(String theName)
        {
            DefineVariable(theName, EPLValueType.FloatingType, 0, 0);
        }

        /// <summary>
        ///     Define the specified variable as the specified type. Don't use this for
        ///     enums.
        /// </summary>
        /// <param name="theName">The name of the variable.</param>
        /// <param name="theVariableType">The variable type.</param>
        public void DefineVariable(String theName,
                                   EPLValueType theVariableType)
        {
            DefineVariable(theName, theVariableType, 0, 0);
        }

        /// <summary>
        ///     Define a variable.
        /// </summary>
        /// <param name="theName">The name of the variable.</param>
        /// <param name="theVariableType">The type of variable.</param>
        /// <param name="theEnumType">The enum type, not used if not an enum type.</param>
        /// <param name="theEnumValueCount">
        ///     The number of values for the enum, not used if not an enum
        ///     type.
        /// </param>
        public void DefineVariable(String theName,
                                   EPLValueType theVariableType, int theEnumType,
                                   int theEnumValueCount)
        {
            var mapping = new VariableMapping(theName,
                                              theVariableType, theEnumType, theEnumValueCount);
            DefineVariable(mapping);
        }

        /// <summary>
        ///     Define a variable, based on a mapping.
        /// </summary>
        /// <param name="mapping">The variable mapping.</param>
        public void DefineVariable(VariableMapping mapping)
        {
            if (_map.ContainsKey(mapping.Name))
            {
                throw new EACompileError("Variable " + mapping.Name
                                         + " already defined.");
            }
            _map[mapping.Name] = mapping;
            _definedVariables.Add(mapping);
        }

        /// <summary>
        ///     Find all of the variables of the specified types.
        /// </summary>
        /// <param name="desiredTypes">The types to look for.</param>
        /// <returns>The variables that matched the specified types.</returns>
        public IList<VariableMapping> FindVariablesByTypes(
            IList<EPLValueType> desiredTypes)
        {
            return _definedVariables.Where(mapping => desiredTypes.Contains(mapping.VariableType)).ToList();
        }

        /// <summary>
        ///     Get the enum ordinal count for the specified enumeration type.
        /// </summary>
        /// <param name="enumType">The enumeration type.</param>
        /// <returns>The ordinal count for the specified enumeration type.</returns>
        public int GetEnumCount(int enumType)
        {
            // make sure we consider the result
            if (_result.VariableType == EPLValueType.EnumType
                && _result.EnumType == enumType)
            {
                return _result.EnumValueCount;
            }

            foreach (VariableMapping mapping in _definedVariables)
            {
                if (mapping.VariableType == EPLValueType.EnumType)
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
        ///     Get the max enum type for all defined variables.
        /// </summary>
        /// <returns>The max enumeration type.</returns>
        public int GetMaxEnumType()
        {
            int r = -1;

            // make sure we consider the result
            if (_result.VariableType == EPLValueType.EnumType)
            {
                r = _result.EnumType;
            }

            // loop over all mappings and find the max enum type
            r = (from mapping in _definedVariables where mapping.VariableType == EPLValueType.EnumType select mapping.EnumType).Concat(new[] {r}).Max();

            // if we did not find one then there are no enum types
            if (r == -1)
            {
                throw new EACompileError("No enum types defined in context.");
            }

            return r;
        }

        /// <summary>
        ///     Load all known functions as opcodes.
        /// </summary>
        public void LoadAllFunctions()
        {
            StandardExtensions.CreateAll(this);
        }
    }
}
