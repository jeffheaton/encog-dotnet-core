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
using Encog.ML.EA.Exceptions;
using Encog.ML.Prg.ExpValue;

namespace Encog.ML.Prg
{
    /// <summary>
    ///     This class stores the actual variable values for an Encog Program. The
    ///     definitions for the variables are stored in the context.
    /// </summary>
    [Serializable]
    public class EncogProgramVariables
    {
        /// <summary>
        ///     A map to the index values of each variable name.
        /// </summary>
        private readonly IDictionary<String, int> _varMap = new Dictionary<String, int>();

        /// <summary>
        ///     The values of each variable. The order lines up to the order defined in
        ///     the context.
        /// </summary>
        private readonly IList<ExpressionValue> _variables = new List<ExpressionValue>();

        /// <summary>
        /// The number of variables.
        /// </summary>
        public int Count
        {
            get { return _varMap.Count; }
        }

        /// <summary>
        ///     Define the specified variable mapping. This is to be used by the context
        ///     to setup the variable definitions. Do not call it directly. You will have
        ///     unexpected results if you have a variable defined in this class, but not
        ///     in the context.
        /// </summary>
        /// <param name="mapping">The variable mapping.</param>
        public void DefineVariable(VariableMapping mapping)
        {
            if (_varMap.ContainsKey(mapping.Name))
            {
                throw new EARuntimeError(
                    "Variable "
                    + mapping.Name
                    + " already defined, simply set its value, do not redefine.");
            }
            _varMap[mapping.Name] = _variables.Count;
            _variables.Add(new ExpressionValue(mapping.VariableType));
        }

        /// <summary>
        ///     Get a variable value by index.
        /// </summary>
        /// <param name="i">The index of the variable we are using.</param>
        /// <returns>The variable at the specified index.</returns>
        public ExpressionValue GetVariable(int i)
        {
            return _variables[i];
        }

        /// <summary>
        ///     Get a variable value by name.
        /// </summary>
        /// <param name="name">The name of the variable we are using.</param>
        /// <returns>The variable at the specified index.</returns>
        public ExpressionValue GetVariable(String name)
        {
            if (_varMap.ContainsKey(name))
            {
                int index = _varMap[name];
                return _variables[index];
            }
            return null;
        }

        /// <summary>
        ///     Get a variable index by name.
        /// </summary>
        /// <param name="varName">The variable name.</param>
        /// <returns>The index of the specified variable.</returns>
        public int GetVariableIndex(String varName)
        {
            if (!VariableExists(varName))
            {
                throw new EARuntimeError("Undefined variable: " + varName);
            }

            return _varMap[varName];
        }

        /// <summary>
        ///     Get a variable name by index.
        /// </summary>
        /// <param name="idx">The variable index.</param>
        /// <returns>The variable name.</returns>
        public String GetVariableName(int idx)
        {
            foreach (string key in _varMap.Keys)
            {
                int value = _varMap[key];
                if (value == idx)
                {
                    return key;
                }
            }

            throw new EARuntimeError("No variable defined for index " + idx);
        }

        /// <summary>
        ///     Set a variable floating point value by index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public void SetVariable(int index, double value)
        {
            _variables[index] = new ExpressionValue(value);
        }

        /// <summary>
        ///     Set a floating point variable value by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="d">The value.</param>
        public void SetVariable(String name, double d)
        {
            SetVariable(name, new ExpressionValue(d));
        }

        /// <summary>
        ///     Set a variable value by name.
        /// </summary>
        /// <param name="name">The variable name.</param>
        /// <param name="value">The value.</param>
        public void SetVariable(String name,
                                ExpressionValue value)
        {
            lock (this)
            {
                if (_varMap.ContainsKey(name))
                {
                    int index = _varMap[name];
                    _variables[index] = value;
                }
                else
                {
                    _varMap[name] = _variables.Count;
                    _variables.Add(value);
                }
            }
        }

        /**
         * @return Get the number of variables defined.
         */

        /// <summary>
        ///     Determine if the specified variable name exists.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <returns>True if the name already exists.</returns>
        public bool VariableExists(String name)
        {
            return _varMap.ContainsKey(name);
        }
    }
}
