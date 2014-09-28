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
    ///     Because neither the grow or full method provide a very wide array of sizes or
    ///     shapes on their own, Koza (1992) proposed a combination called ramped
    ///     half-and-half. Half the initial population is constructed using full and half
    ///     is constructed using grow. This is done using a range of depth limits (hence
    ///     the term "ramped") to help ensure that we generate trees having a variety of
    ///     sizes and shapes. (from: A field guide to genetic programming)
    ///     This algorithm was implemented as described in the following publication:
    ///     Genetic programming: on the programming of computers by means of natural
    ///     selection MIT Press Cambridge, MA, USA (c)1992 ISBN:0-262-11170-5
    /// </summary>
    public class RampedHalfAndHalf : AbstractPrgGenerator
    {
        /// <summary>
        ///     The full generator.
        /// </summary>
        private readonly PrgFullGenerator _fullGenerator;

        /// <summary>
        ///     The grow generator.
        /// </summary>
        private readonly PrgGrowGenerator _growGenerator;

        /// <summary>
        ///     The minimum depth.
        /// </summary>
        private readonly int _minDepth;

        /// <summary>
        ///     Construct the ramped half-and-half generator.
        /// </summary>
        /// <param name="theContext">The context.</param>
        /// <param name="theMinDepth">The minimum depth.</param>
        /// <param name="theMaxDepth">The maximum depth.</param>
        public RampedHalfAndHalf(EncogProgramContext theContext,
                                 int theMinDepth, int theMaxDepth)
            : base(theContext, theMaxDepth)
        {
            _minDepth = theMinDepth;

            _fullGenerator = new PrgFullGenerator(theContext, theMaxDepth);
            _growGenerator = new PrgGrowGenerator(theContext, theMaxDepth);
        }

        /// <summary>
        ///     The minimum depth.
        /// </summary>
        public int MinDepth
        {
            get { return _minDepth; }
        }

        /// <inheritdoc />
        public override ProgramNode CreateNode(EncogRandom rnd, EncogProgram program,
                                               int depthRemaining, IList<EPLValueType> types)
        {
            int actualDepthRemaining = depthRemaining;

            if (rnd.NextDouble() > 0.5)
            {
                return _fullGenerator.CreateNode(rnd, program,
                                                actualDepthRemaining, types);
            }
            return _growGenerator.CreateNode(rnd, program,
                                             actualDepthRemaining, types);
        }

        /// <inheritdoc />
        public override int DetermineMaxDepth(EncogRandom rnd)
        {
            int range = MaxDepth - _minDepth;
            return rnd.Next(range) + _minDepth;
        }
    }
}
