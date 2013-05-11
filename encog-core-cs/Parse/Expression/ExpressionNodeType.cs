using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Parse.Expression
{
    /// <summary>
    /// Expression node types.
    /// </summary>
    public enum ExpressionNodeType
    {
        ConstVal,
        Operator,
        Variable,
        Function,
        ConstKnown
    }
}
