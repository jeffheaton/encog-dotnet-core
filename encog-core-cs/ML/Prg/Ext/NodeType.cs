using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.ML.Prg.Ext
{
    /// <summary>
    /// The node type. This mostly determines how opcodes are parsed and rendered.
    /// Node types do not generally affect execution.
    /// </summary>
    public enum NodeType
    {
        /// <summary>
        /// A left associated operator. Operators are actually functions, however,
        /// setting to this type does affect how the opcode is rendered.
        /// http://en.wikipedia.org/wiki/Operator_associativity
        /// </summary>
        OperatorLeft,
        /// <summary>
        /// A right associated operator. Operators are actually functions, however,
        /// setting to this type does affect how the opcode is rendered.
        /// http://en.wikipedia.org/wiki/Operator_associativity
        /// </summary>
        OperatorRight,
        /// <summary>
        /// A leaf, or terminal node. No children.
        /// </summary>
        Leaf,
        /// <summary>
        /// A function.
        /// </summary>
        Function,
        /// <summary>
        /// An unary operator.
        /// </summary>
        Unary,
        /**
         * Unknown.
         */
        None

    }
}
