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
using System.Text;
using Encog.ML.Prg;

namespace Encog.Parse.Expression
{
    /// <summary>
    /// Common functions for some renders.
    /// </summary>
    public class CommonRender
    {
        /// <summary>
        /// Determine the expression type.
        /// </summary>
        /// <param name="node">The program node.</param>
        /// <returns>The expression type.</returns>
        public ExpressionNodeType DetermineNodeType(ProgramNode node)
        {

            if (node.Name.Equals("#const"))
            {
                return ExpressionNodeType.ConstVal;
            }

            if (node.Name.Equals("#var"))
            {
                return ExpressionNodeType.Variable;
            }

            if (node.ChildNodes.Count != 2)
            {
                return ExpressionNodeType.Function;
            }

            String name = node.Name;

            if (!char.IsLetterOrDigit(name[0]))
            {
                return ExpressionNodeType.Operator;
            }

            return ExpressionNodeType.Function;
        }
    }
}
