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

namespace Encog.ML.Prg.Ext
{
    /// <summary>
    ///     Holds all known EPL opcodes. Extension programs should add new opcodes here.
    ///     The FunctionFactory selects a subset of opcodes from here that will be run.
    ///     An opcode is identified by its name, and the number of parameters it accepts.
    ///     It is okay to add an opcode multiple times, the new opcode replaces the
    ///     previous.
    /// </summary>
    public class EncogOpcodeRegistry
    {
        /// <summary>
        ///     The instance.
        /// </summary>
        private static EncogOpcodeRegistry _instance;

        /// <summary>
        ///     A lookup for all of the opcodes.
        /// </summary>
        private readonly IDictionary<String, IProgramExtensionTemplate> _registry =
            new Dictionary<String, IProgramExtensionTemplate>();

        /// <summary>
        ///     Construct the opcode registry with all known opcodes. User programs can
        ///     always add additional opcodes later.
        /// </summary>
        private EncogOpcodeRegistry()
        {
            Add(StandardExtensions.EXTENSION_NOT_EQUAL);
            Add(StandardExtensions.EXTENSION_NOT);
            Add(StandardExtensions.EXTENSION_VAR_SUPPORT);
            Add(StandardExtensions.EXTENSION_CONST_SUPPORT);
            Add(StandardExtensions.EXTENSION_NEG);
            Add(StandardExtensions.EXTENSION_ADD);
            Add(StandardExtensions.EXTENSION_SUB);
            Add(StandardExtensions.EXTENSION_MUL);
            Add(StandardExtensions.EXTENSION_DIV);
            Add(StandardExtensions.EXTENSION_POWER);
            Add(StandardExtensions.EXTENSION_AND);
            Add(StandardExtensions.EXTENSION_OR);
            Add(StandardExtensions.EXTENSION_EQUAL);
            Add(StandardExtensions.EXTENSION_GT);
            Add(StandardExtensions.EXTENSION_LT);
            Add(StandardExtensions.EXTENSION_GTE);
            Add(StandardExtensions.EXTENSION_LTE);
            Add(StandardExtensions.EXTENSION_ABS);
            Add(StandardExtensions.EXTENSION_ACOS);
            Add(StandardExtensions.EXTENSION_ASIN);
            Add(StandardExtensions.EXTENSION_ATAN);
            Add(StandardExtensions.EXTENSION_ATAN2);
            Add(StandardExtensions.EXTENSION_CEIL);
            Add(StandardExtensions.EXTENSION_COS);
            Add(StandardExtensions.EXTENSION_COSH);
            Add(StandardExtensions.EXTENSION_EXP);
            Add(StandardExtensions.EXTENSION_FLOOR);
            Add(StandardExtensions.EXTENSION_LOG);
            Add(StandardExtensions.EXTENSION_LOG10);
            Add(StandardExtensions.EXTENSION_MAX);
            Add(StandardExtensions.EXTENSION_MIN);
            Add(StandardExtensions.EXTENSION_POWFN);
            Add(StandardExtensions.EXTENSION_RANDOM);
            Add(StandardExtensions.EXTENSION_ROUND);
            Add(StandardExtensions.EXTENSION_SIN);
            Add(StandardExtensions.EXTENSION_SINH);
            Add(StandardExtensions.EXTENSION_SQRT);
            Add(StandardExtensions.EXTENSION_TAN);
            Add(StandardExtensions.EXTENSION_TANH);
            Add(StandardExtensions.EXTENSION_TODEG);
            Add(StandardExtensions.EXTENSION_TORAD);
            Add(StandardExtensions.EXTENSION_LENGTH);
            Add(StandardExtensions.EXTENSION_FORMAT);
            Add(StandardExtensions.EXTENSION_LEFT);
            Add(StandardExtensions.EXTENSION_RIGHT);
            Add(StandardExtensions.EXTENSION_CINT);
            Add(StandardExtensions.EXTENSION_CFLOAT);
            Add(StandardExtensions.EXTENSION_CSTR);
            Add(StandardExtensions.EXTENSION_CBOOL);
            Add(StandardExtensions.EXTENSION_IFF);
            Add(StandardExtensions.EXTENSION_CLAMP);
        }

        /// <summary>
        ///     Get the instance.
        /// </summary>
        public static EncogOpcodeRegistry Instance
        {
            get { return _instance ?? (_instance = new EncogOpcodeRegistry()); }
        }

        /// <summary>
        ///     Construct a lookup key for the hash map.
        /// </summary>
        /// <param name="functionName">The name of the opcode.</param>
        /// <param name="argCount">The number of parameters this opcode accepts.</param>
        /// <returns>Return the string key.</returns>
        public static String CreateKey(String functionName, int argCount)
        {
            return functionName + '`' + argCount;
        }

        /// <summary>
        ///     Add an opcode. User programs should add opcodes here.
        /// </summary>
        /// <param name="ext">The opcode to add.</param>
        public void Add(IProgramExtensionTemplate ext)
        {
            _registry[
                CreateKey(ext.Name,
                          ext.ChildNodeCount)] = ext;
        }

        /// <summary>
        ///     Find all opcodes.
        /// </summary>
        /// <returns>The opcodes.</returns>
        public ICollection<IProgramExtensionTemplate> FindAllOpcodes()
        {
            return _registry.Values;
        }

        /// <summary>
        ///     Find the specified opcode.
        /// </summary>
        /// <param name="name">The name of the opcode.</param>
        /// <param name="args">The number of arguments.</param>
        /// <returns>The opcode if found, null otherwise.</returns>
        public IProgramExtensionTemplate FindOpcode(String name, int args)
        {
            String key = CreateKey(name, args);
            if (_registry.ContainsKey(key))
            {
                return _registry[key];
            }
            return null;
        }


        /// <summary>
        ///     True, if this is an operator.
        /// </summary>
        /// <param name="t">Node type to check.</param>
        /// <returns>True, if this is an operator.</returns>
        public static bool IsOperator(NodeType t)
        {
            return t == NodeType.OperatorLeft || t == NodeType.OperatorRight || t == NodeType.Unary;
        }
    }
}
