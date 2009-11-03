using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Normalize.Target
{
    /// <summary>
    /// Output the normalized data to a 1D array.
    /// </summary>
    public class NormalizationStorageArray1D : INormalizationStorage
    {
        /// <summary>
        /// The array to store to.
        /// </summary>
        private double[] array;

        /// <summary>
        /// The current index.
        /// </summary>
        private int currentIndex;


        /// <summary>
        /// Construct an object to store to a 2D array.
        /// </summary>
        /// <param name="array">The array to store to.</param>
        public NormalizationStorageArray1D(double[] array)
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
            this.array[this.currentIndex++] = data[0];
        }
    }
}
