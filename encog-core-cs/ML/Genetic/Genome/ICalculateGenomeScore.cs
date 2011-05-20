//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
namespace Encog.ML.Genetic.Genome
{
    /// <summary>
    /// Genetic Algorithms need a class to calculate the score.
    /// </summary>
    ///
    public interface ICalculateGenomeScore
    {
        /// <returns>True if the goal is to minimize the score.</returns>
        bool ShouldMinimize { get; }

        /// <summary>
        /// Calculate this genome's score.
        /// </summary>
        ///
        /// <param name="genome">The genome.</param>
        /// <returns>The score.</returns>
        double CalculateScore(IGenome genome);
    }
}
