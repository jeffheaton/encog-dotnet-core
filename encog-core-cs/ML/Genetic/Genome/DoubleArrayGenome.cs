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
