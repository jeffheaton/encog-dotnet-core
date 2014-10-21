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
using System.Text;
using Encog.ML.Prg.ExpValue;

namespace Encog.ML.Prg
{
    /// <summary>
    ///     A variable mapping defines the type for each of the variables in an Encog
    ///     program.
    /// </summary>
    [Serializable]
    public class VariableMapping
    {
        /// <summary>
        ///     If this is an enum, what is the type.
        /// </summary>
        private readonly int _enumType;

        /// <summary>
        ///     The count for this given enum. If this is not an enum, then value is not
        ///     used.
        /// </summary>
        private readonly int _enumValueCount;

        /// <summary>
        ///     The name of the variable.
        /// </summary>
        private readonly String _name;

        /// <summary>
        ///     The variable type.
        /// </summary>
        private readonly EPLValueType _variableType;

        /// <summary>
        ///     Construct a variable mapping for a non-enum type.
        /// </summary>
        /// <param name="theName">The variable name.</param>
        /// <param name="theVariableType">The variable type.</param>
        public VariableMapping(String theName, EPLValueType theVariableType)
            : this(theName, theVariableType, 0, 0)
        {
        }

        /// <summary>
        ///     Construct a variable mapping.
        /// </summary>
        /// <param name="theName">The name of the variable.</param>
        /// <param name="theVariableType">The type of the variable.</param>
        /// <param name="theEnumType">The enum type.</param>
        /// <param name="theEnumValueCount">The number of values for an enum.</param>
        public VariableMapping(String theName,
                               EPLValueType theVariableType, int theEnumType,
                               int theEnumValueCount)
        {
            _name = theName;
            _variableType = theVariableType;
            _enumType = theEnumType;
            _enumValueCount = theEnumValueCount;
        }

        /// <summary>
        ///     The enum type.
        /// </summary>
        public int EnumType
        {
            get { return _enumType; }
        }

        /// <summary>
        ///     The enum value count.
        /// </summary>
        public int EnumValueCount
        {
            get { return _enumValueCount; }
        }

        /// <summary>
        ///     The name.
        /// </summary>
        public String Name
        {
            get { return _name; }
        }

        /// <summary>
        ///     The type.
        /// </summary>
        public EPLValueType VariableType
        {
            get { return _variableType; }
        }

        /// <inheritdoc />
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[VariableMapping: name=");
            result.Append(_name);
            result.Append(",type=");
            result.Append(_variableType.ToString());
            result.Append(",enumType=");
            result.Append(_enumType);
            result.Append(",enumCount=");
            result.Append(_enumValueCount);
            result.Append("]");
            return result.ToString();
        }
    }
}
