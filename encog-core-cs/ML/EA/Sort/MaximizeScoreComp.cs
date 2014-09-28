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
using Encog.ML.EA.Genome;

namespace Encog.ML.EA.Sort
{
    /// <summary>
    ///     Use this comparator to maximize the score.
    /// </summary>
    [Serializable]
    public class MaximizeScoreComp : AbstractGenomeComparer
    {
        /// <inheritdoc />
        public override bool ShouldMinimize
        {
            get { return false; }
        }

        /// <inheritdoc />
        public override int Compare(IGenome p1, IGenome p2)
        {
            return p2.Score.CompareTo(p1.Score);
        }

        /// <inheritdoc />
        public override bool IsBetterThan(IGenome prg, IGenome betterThan)
        {
            return IsBetterThan(prg.AdjustedScore,
                                betterThan.AdjustedScore);
        }
    }
}
