using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Normalize.Target
{
    /// <summary>
    /// Output the normalized data to a 2D array.
    /// </summary>
    public class NormalizationStorageArray2D : INormalizationStorage
    {
        /// <summary>
        /// The array to output to.
        /// </summary>
        private double[][] array;

        /// <summary>
        /// The current data.
        /// </summary>
        private int currentIndex;

        /// <summary>
        /// Construct an object to store to a 2D array.
        /// </summary>
        /// <param name="array">The array to store to.</param>
        public NormalizationStorageArray2D(double[][] array)
        {
            this.array = array;
            this.currentIndex = 0;
        }

        /// <summary>
        /// Not needed for this storage type.
        /// </summary>
        public void Close()
        {

        }

        /// <summary>
        /// Not needed for this storage type.
        /// </summary>
        public void Open()
        {

        }

        /// <summary>
        /// Write an array.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="inputCount">How much of the data is input.</param>
        public void Write(double[] data, int inputCount)
        {
            for (int i = 0; i < data.Length; i++)
            {
                this.array[this.currentIndex][i] = data[i];
            }
            this.currentIndex++;
        }

    }
}
