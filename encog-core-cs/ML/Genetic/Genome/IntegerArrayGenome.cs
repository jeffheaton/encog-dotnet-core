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
    /// A genome that is an array of discrete integer values.
    /// </summary>
    public class IntegerArrayGenome : BasicGenome, IArrayGenome
    {
        /// <summary>
        /// The genome data.
        /// </summary>
        private int[] data;

        /// <summary>
        /// Construct the genome.
        /// </summary>
        /// <param name="size">The size of the genome.</param>
        public IntegerArrayGenome(int size)
        {
            this.data = new int[size];
        }

        /// <summary>
        /// Construct the genome by copying another.
        /// </summary>
        /// <param name="other">The other genome.</param>
        public IntegerArrayGenome(IntegerArrayGenome other)
        {
            this.data = (int[])other.Data.Clone();
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
            IntegerArrayGenome sourceInt = (IntegerArrayGenome)source;
            this.data[targetIndex] = sourceInt.data[sourceIndex];

        }

        /// <summary>
        /// The data.
        /// </summary>
        public int[] Data
        {
            get
            {
                return this.data;
            }
        }

        /// <inheritdoc/>
        public override void Copy(IGenome source)
        {
            IntegerArrayGenome sourceInt = (IntegerArrayGenome)source;
            EngineArray.ArrayCopy(sourceInt.data, this.data);
            this.Score = source.Score;
            this.AdjustedScore = source.AdjustedScore;

        }

        /// <inheritdoc/>
        public void Swap(int iswap1, int iswap2)
        {
            int temp = this.data[iswap1];
            this.data[iswap1] = this.data[iswap2];
            this.data[iswap2] = temp;

        }
    }
}
