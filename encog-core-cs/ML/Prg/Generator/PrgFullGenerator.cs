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
using System.Collections.Generic;
using Encog.ML.Prg.ExpValue;
using Encog.MathUtil.Randomize;

namespace Encog.ML.Prg.Generator
{
    /// <summary>
    ///     The full generator works by creating program trees that do not stop
    ///     prematurely. To do this a node is randomly selected from the "function set"
    ///     until the tree reaches the maximum depth. Once the tree reaches the maximum
    ///     depth only nodes from the "terminal set" are chosen.
    ///     This algorithm was implemented as described in the following publication:
    ///     Genetic programming: on the programming of computers by means of natural
    ///     selection MIT Press Cambridge, MA, USA (c)1992 ISBN:0-262-11170-5
    /// </summary>
    public class PrgFullGenerator : AbstractPrgGenerator
    {
        /// <summary>
        ///     Construct the full generator.
        /// </summary>
        /// <param name="theContext">The context.</param>
        /// <param name="theMaxDepth">The max depth.</param>
        public PrgFullGenerator(EncogProgramContext theContext,
                                int theMaxDepth)
            : base(theContext, theMaxDepth)
        {
        }

        /// <inheritdoc />
        public override ProgramNode CreateNode(EncogRandom rnd, EncogProgram program,
                                               int depthRemaining, IList<EPLValueType> types)
        {
            return CreateRandomNode(rnd, program, depthRemaining, types, false,
                                    true);
        }
    }
}
