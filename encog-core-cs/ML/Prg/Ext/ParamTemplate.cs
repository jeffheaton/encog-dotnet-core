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
    ///     Provides a template for parameters to the opcodes. This defines the accepted
    ///     types and if type of a given parameter passes through to the return type.
    /// </summary>
    [Serializable]
    public class ParamTemplate
    {
        /// <summary>
        ///     Possible types for this parameter.
        /// </summary>
        private readonly HashSet<EPLValueType> _possibleTypes = new HashSet<EPLValueType>();

        /// <summary>
        ///     Is this a pass through argument. If so, then the return type of the
        ///     parent opcode will depend on the actual type of this parameter.
        /// </summary>
        public bool PassThrough { get; set; }

        /// <summary>
        ///     All possible types.
        /// </summary>
        public HashSet<EPLValueType> PossibleTypes
        {
            get { return _possibleTypes; }
        }

        /// <summary>
        ///     Add all known types.
        /// </summary>
        public void AddAllTypes()
        {
            foreach (EPLValueType t in Enum.GetValues(typeof (EPLValueType)))
            {
                AddType(t);
            }
        }

        /// <summary>
        ///     Add the specified type.
        /// </summary>
        /// <param name="theType">The type to add.</param>
        public void AddType(String theType)
        {
            if (theType.Equals("b"))
            {
                AddType(EPLValueType.BooleanType);
            }
            else if (theType.Equals("e"))
            {
                AddType(EPLValueType.EnumType);
            }
            else if (theType.Equals("f"))
            {
                AddType(EPLValueType.FloatingType);
            }
            else if (theType.Equals("i"))
            {
                AddType(EPLValueType.IntType);
            }
            else if (theType.Equals("s"))
            {
                AddType(EPLValueType.StringType);
            }
            else if (theType.Equals("*"))
            {
                AddAllTypes();
            }
            else
            {
                throw new EACompileError("Unknown type: " + theType);
            }
        }

        /// <summary>
        ///     Add a type using a type enum.
        /// </summary>
        /// <param name="theType">The type to add.</param>
        public void AddType(EPLValueType theType)
        {
            _possibleTypes.Add(theType);
        }

        /// <summary>
        ///     Determine the possable argument types, given the parent types.
        /// </summary>
        /// <param name="parentTypes">The parent types.</param>
        /// <returns>The possable types.</returns>
        public IList<EPLValueType> DetermineArgumentTypes(
            IList<EPLValueType> parentTypes)
        {
            if (PassThrough)
            {
                return parentTypes;
            }

            IList<EPLValueType> result = new List<EPLValueType>();
            result = result.Union(PossibleTypes).ToList();
            return result;
        }
    }
}
