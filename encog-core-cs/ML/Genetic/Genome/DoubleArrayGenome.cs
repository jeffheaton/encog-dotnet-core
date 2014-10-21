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
using System.Linq;
using System.Text;
using Encog.ML.EA.Genome;
using Encog.Util;

namespace Encog.ML.Genetic.Genome
{
    /// <summary>
    /// A genome made up of continuous doubles.
    /// </summary>
    public class DoubleArrayGenome : BasicGenome, IArrayGenome
    {
        /// <summary>
        /// The data.
        /// </summary>
        private double[] data;

        /// <summary>
        /// Construct a genome of a specific size.
        /// </summary>
        /// <param name="size">The size.</param>
        public DoubleArrayGenome(int size)
        {
            this.data = new double[size];
        }

        /// <summary>
        /// Construct a genome based on another genome.
        /// </summary>
        /// <param name="other">The other genome.</param>
        public DoubleArrayGenome(DoubleArrayGenome other)
        {
            this.data = (double[])other.Data.Clone();
        }

        /// <inheritdoc/>
        public override int Size
        {
            get
            {
                return this.data.Length;
            }
        }

        /// <inheritdoc/>
        public void Copy(IArrayGenome source, int sourceIndex, int targetIndex)
        {
            DoubleArrayGenome sourceInt = (DoubleArrayGenome)source;
            this.data[targetIndex] = sourceInt.data[sourceIndex];

        }

        /// <summary>
        /// The data.
        /// </summary>
        public double[] Data
        {
            get
            {
                return this.data;
            }
        }

        /// <inheritdoc/>
        public override void Copy(IGenome source)
        {
            DoubleArrayGenome sourceDouble = (DoubleArrayGenome)source;
            EngineArray.ArrayCopy(sourceDouble.data, this.data);
            Score = source.Score;
            AdjustedScore = source.AdjustedScore;

        }

        /// <inheritdoc/>
        public void Swap(int iswap1, int iswap2)
        {
            double temp = this.data[iswap1];
            this.data[iswap1] = this.data[iswap2];
            this.data[iswap2] = temp;

        }
    }
}
