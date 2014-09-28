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
using Encog.ML.Prg.ExpValue;
using Encog.MathUtil.Randomize;

namespace Encog.ML.Prg.Ext
{
    /// <summary>
    ///     Defines an opcode. Opcodes are used to extend Encog programs.
    /// </summary>
    public interface IProgramExtensionTemplate
    {
        /// <summary>
        ///     Get the number of child nodes that this opcode requires.
        /// </summary>
        int ChildNodeCount { get; }

        /// <summary>
        ///     The size of extra data that is stored by this node.
        /// </summary>
        int DataSize { get; }

        /// <summary>
        ///     Get the name of this opcode.
        /// </summary>
        String Name { get; }

        /// <summary>
        ///     The node type.
        /// </summary>
        NodeType NodeType { get; }

        /// <summary>
        ///     The parameters (child nodes) required by this node.
        /// </summary>
        IList<ParamTemplate> Params { get; }

        /// <summary>
        ///     The operator precedence.
        /// </summary>
        int Precedence { get; }

        /// <summary>
        ///     The return value for this opcode.
        /// </summary>
        ParamTemplate ReturnValue { get; }

        /// <summary>
        ///     Returns true if this node is variable.
        /// </summary>
        bool IsVariable { get; }

        /// <summary>
        ///     Evaluate the specified actual program node, using this opcode template.
        /// </summary>
        /// <param name="actual">The tree node in the actual program.</param>
        /// <returns>The result of the evaluation.</returns>
        ExpressionValue Evaluate(ProgramNode actual);

        /// <summary>
        ///     Determines if the specified return type is a possible return type.
        /// </summary>
        /// <param name="context">The program context.</param>
        /// <param name="rtn">The potential return type to check.</param>
        /// <returns>True, if the specified type is a possible return type.</returns>
        bool IsPossibleReturnType(EncogProgramContext context, EPLValueType rtn);

        /// <summary>
        ///     Randomize this actual tree node.
        /// </summary>
        /// <param name="rnd">Random number generator.</param>
        /// <param name="desiredType"> The desired type of the randomization, if allowed.</param>
        /// <param name="actual">The actual program node to randomize.</param>
        /// <param name="minValue">The minimum value to use for randomization.</param>
        /// <param name="maxValue">The maximum value to use for randomization.</param>
        void Randomize(EncogRandom rnd, IList<EPLValueType> desiredType, ProgramNode actual,
                       double minValue, double maxValue);
    }
}
