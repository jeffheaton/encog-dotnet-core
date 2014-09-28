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
using System.Text;
using Encog.ML.EA.Population;
using Encog.ML.EA.Species;

namespace Encog.ML.EA.Genome
{
    /// <summary>
    ///     A basic abstract genome. Provides base functionality.
    /// </summary>
    [Serializable]
    public abstract class BasicGenome : IGenome
    {
        /// <summary>
        ///     Construct a basic genome.
        /// </summary>
        public BasicGenome()
        {
            Score = double.NaN;
            AdjustedScore = double.NaN;
        }

        /// <summary>
        ///     The adjusted score. If unknown, it is set to NaN.
        /// </summary>
        public double AdjustedScore { get; set; }

        /// <summary>
        ///     The score of this genome.
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        ///     The population this genome belongs to.
        /// </summary>
        public IPopulation Population { get; set; }

        /// <summary>
        ///     The birth generation for this genome.
        /// </summary>
        public int BirthGeneration { get; set; }

        /// <summary>
        ///     The species of this genome.
        /// </summary>
        public ISpecies Species { get; set; }


        public abstract void Copy(IGenome source);

        public abstract int Size { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("[");
            builder.Append(GetType().Name);
            builder.Append(": score=");
            builder.Append(Score);
            return builder.ToString();
        }
    }
}
