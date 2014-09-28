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

namespace Encog.ML.Prg.Ext
{
    /// <summary>
    ///     A function factory maps the opcodes contained in the EncogOpcodeRegistry into
    ///     an EncogProgram. You rarely want to train with every available opcode. For
    ///     example, if you are only looking to produce an equation, you should not make
    ///     use of the if-statement and logical operators.
    /// </summary>
    [Serializable]
    public class FunctionFactory
    {
        /// <summary>
        ///     The opcodes.
        /// </summary>
        private readonly IList<IProgramExtensionTemplate> _opcodes = new List<IProgramExtensionTemplate>();

        /// <summary>
        ///     A map for quick lookup.
        /// </summary>
        private readonly IDictionary<String, IProgramExtensionTemplate> _templateMap =
            new Dictionary<String, IProgramExtensionTemplate>();

        /// <summary>
        ///     The opcode list.
        /// </summary>
        public IList<IProgramExtensionTemplate> OpCodes
        {
            get { return _opcodes; }
        }

        /// <summary>
        ///     The template map.
        /// </summary>
        public IDictionary<String, IProgramExtensionTemplate> TemplateMap
        {
            get { return _templateMap; }
        }

        /// <summary>
        ///     The number of opcodes.
        /// </summary>
        public int Count
        {
            get { return _opcodes.Count; }
        }

        /// <summary>
        ///     Add an opcode to the function factory. The opcode must exist in the
        ///     opcode registry.
        /// </summary>
        /// <param name="ext">The opcode to add.</param>
        public void AddExtension(IProgramExtensionTemplate ext)
        {
            AddExtension(ext.Name, ext.ChildNodeCount);
        }

        /// <summary>
        ///     Add an opcode to the function factory from the opcode registry.
        /// </summary>
        /// <param name="name">The name of the opcode.</param>
        /// <param name="args">The number of arguments.</param>
        public void AddExtension(String name, int args)
        {
            String key = EncogOpcodeRegistry.CreateKey(name, args);
            if (!_templateMap.ContainsKey(key))
            {
                IProgramExtensionTemplate temp = EncogOpcodeRegistry.Instance
                                                                    .FindOpcode(name, args);
                if (temp == null)
                {
                    throw new EACompileError("Unknown extension " + name + " with "
                                             + args + " arguments.");
                }
                _opcodes.Add(temp);
                _templateMap[key] = temp;
            }
        }

        /// <summary>
        ///     Factor a new program node, based in a template object.
        /// </summary>
        /// <param name="temp">The opcode.</param>
        /// <param name="program">The program.</param>
        /// <param name="args">The arguments for this node.</param>
        /// <returns>The newly created ProgramNode.</returns>
        public ProgramNode FactorProgramNode(IProgramExtensionTemplate temp,
                                             EncogProgram program, ProgramNode[] args)
        {
            return new ProgramNode(program, temp, args);
        }

        /// <summary>
        ///     Factor a new program node, based on an opcode name and arguments.
        /// </summary>
        /// <param name="name">The name of the opcode.</param>
        /// <param name="program">The program to factor for.</param>
        /// <param name="args">The arguements.</param>
        /// <returns>The newly created ProgramNode.</returns>
        public ProgramNode FactorProgramNode(String name,
                                             EncogProgram program, ProgramNode[] args)
        {
            String key = EncogOpcodeRegistry.CreateKey(name, args.Length);

            if (!_templateMap.ContainsKey(key))
            {
                throw new EACompileError("Undefined function/operator: " + name
                                         + " with " + args.Length + " args.");
            }

            IProgramExtensionTemplate temp = _templateMap[key];
            return new ProgramNode(program, temp, args);
        }

        /// <summary>
        ///     Find a function with the specified name.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <returns>The function opcode.</returns>
        public IProgramExtensionTemplate FindFunction(String name)
        {
            return _opcodes.Where(opcode => opcode.NodeType == NodeType.Function).FirstOrDefault(opcode => opcode.Name.Equals(name));
        }

        /// <summary>
        ///     Find all opcodes that match the search criteria.
        /// </summary>
        /// <param name="types">The return types to consider.</param>
        /// <param name="context">The program context.</param>
        /// <param name="includeTerminal">True, to include the terminals.</param>
        /// <param name="includeFunction">True, to include the functions.</param>
        /// <returns>The opcodes found.</returns>
        public IList<IProgramExtensionTemplate> FindOpcodes(
            IList<EPLValueType> types, EncogProgramContext context,
            bool includeTerminal, bool includeFunction)
        {
            IList<IProgramExtensionTemplate> result = new List<IProgramExtensionTemplate>();

            foreach (IProgramExtensionTemplate temp in _opcodes)
            {
                foreach (EPLValueType rtn in types)
                {
                    // it is a possible return type, but given our variables, is it
                    // possible
                    if (temp.IsPossibleReturnType(context, rtn))
                    {
                        if (temp.ChildNodeCount == 0 && includeTerminal)
                        {
                            result.Add(temp);
                        }
                        else if (includeFunction)
                        {
                            result.Add(temp);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     This method is used when parsing an expression. Consider x>=2. The parser
        ///     first sees the > symbol. But it must also consider the =. So we first
        ///     look for a 2-char operator, in this case there is one.
        /// </summary>
        /// <param name="ch1">The first character of the potential operator.</param>
        /// <param name="ch2">The second character of the potential operator. Zero if none.</param>
        /// <returns>The expression template for the operator found.</returns>
        public IProgramExtensionTemplate FindOperator(char ch1, char ch2)
        {
            // if ch2==0 then we are only looking for a single char operator.
            // this is rare, but supported.
            if (ch2 == 0)
            {
                return FindOperatorExact("" + ch1);
            }

            // first, see if we can match an operator with both ch1 and ch2
            IProgramExtensionTemplate result = FindOperatorExact("" + ch1 + ch2) ?? FindOperatorExact("" + ch1);

            // return the operator if we have one.
            return result;
        }

        /// <summary>
        ///     Find an exact match on opcode.
        /// </summary>
        /// <param name="str">The string to match.</param>
        /// <returns>The opcode found.</returns>
        private IProgramExtensionTemplate FindOperatorExact(String str)
        {
            return _opcodes.Where(opcode => opcode.NodeType == NodeType.OperatorLeft || opcode.NodeType == NodeType.OperatorRight).FirstOrDefault(opcode => opcode.Name.Equals(str));
        }

        /// <summary>
        ///     Get the specified opcode.
        /// </summary>
        /// <param name="theOpCode">The opcode index.</param>
        /// <returns>The opcode.</returns>
        public IProgramExtensionTemplate GetOpCode(int theOpCode)
        {
            return _opcodes[theOpCode];
        }

        /// <summary>
        ///     Determine if an opcode is in the function factory.
        /// </summary>
        /// <param name="name">The name of the opcode.</param>
        /// <param name="l">The argument count for the opcode.</param>
        /// <returns>True if the opcode exists.</returns>
        public bool IsDefined(String name, int l)
        {
            String key = EncogOpcodeRegistry.CreateKey(name, l);
            return _templateMap.ContainsKey(key);
        }
    }
}
