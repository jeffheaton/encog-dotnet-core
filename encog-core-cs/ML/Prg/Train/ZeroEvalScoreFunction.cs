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
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks.Training;

namespace Encog.ML.Prg.Train
{
    /// <summary>
    ///     This is a very simple evaluation function that simply passes in all zeros for
    ///     the input arguments. Make sure that the genome you are testing supports
    ///     floating point numbers for inputs.
    ///     This evaluation function is useful to test to very quickly verify that a
    ///     genome is valid and does not generate any obvious division by zero issues.
    ///     This allows a population generator to quickly eliminate some invalid genomes.
    /// </summary>
    [Serializable]
    public class ZeroEvalScoreFunction : ICalculateScore
    {
        /// <inheritdoc />
        public double CalculateScore(IMLMethod genome)
        {
            var prg = (EncogProgram) genome;
            var pop = (PrgPopulation) prg.Population;
            IMLData inputData = new BasicMLData(pop.Context.DefinedVariables.Count);
            prg.Compute(inputData);
            return 0;
        }

        /// <inheritdoc />
        public bool RequireSingleThreaded
        {
            get { return false; }
        }

        /// <inheritdoc />
        public bool ShouldMinimize
        {
            get { return true; }
        }
    }
}
