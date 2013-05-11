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
