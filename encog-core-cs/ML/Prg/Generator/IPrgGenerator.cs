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
using Encog.ML.EA.Population;
using Encog.ML.Prg.ExpValue;
using Encog.MathUtil.Randomize;

namespace Encog.ML.Prg.Generator
{
    /// <summary>
    ///     Generate a random Encog Program.
    /// </summary>
    public interface IPrgGenerator : IPopulationGenerator
    {
        /// <summary>
        ///     The maximum number of errors to allow during generation.
        /// </summary>
        int MaxGenerationErrors { get; set; }

        /// <summary>
        ///     Create a random node for an Encog Program.
        /// </summary>
        /// <param name="rnd">Random number generator.</param>
        /// <param name="program">The program that the node should be generated for.</param>
        /// <param name="depthRemaining">The depth remaining to generate.</param>
        /// <param name="types">The types to generate.</param>
        /// <returns>The newly created node.</returns>
        ProgramNode CreateNode(EncogRandom rnd, EncogProgram program, int depthRemaining,
                               IList<EPLValueType> types);
    }
}
