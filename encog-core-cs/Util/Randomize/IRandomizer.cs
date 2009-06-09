using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks;

namespace Encog.Util.Randomize
{
    /// <summary>
    /// Defines the interface for a class that is capable of randomizing the weights
    /// and thresholds of a neural network.
    /// </summary>
    public interface IRandomizer
    {
        /// <summary>
        /// Starting with the specified number, randomize it to the degree specified
        /// by this randomizer. This could be a totally new random number, or it
        /// could be based on the specified number.
        /// </summary>
        /// <param name="d">The number to randomize.</param>
        /// <returns>A randomized number.</returns>
        double Randomize(double d);

        /// <summary>
        /// Randomize the array based on an array, modify the array. Previous values
        /// may be used, or they may be discarded, depending on the randomizer.
        /// </summary>
        /// <param name="d">An array to randomize.</param>
        void Randomize(double[] d);

        /// <summary>
        /// Randomize the 2d array based on an array, modify the array. Previous
        /// values may be used, or they may be discarded, depending on the
        /// randomizer.
        /// </summary>
        /// <param name="d">An array to randomize.</param>
        void Randomize(double[][] d);


        /// <summary>
        /// Randomize the matrix based on an array, modify the array. Previous values
        /// may be used, or they may be discarded, depending on the randomizer.
        /// </summary>
        /// <param name="m">A matrix to randomize.</param>
        void Randomize(Matrix.Matrix m);

        /// <summary>
        /// Randomize the synapses and thresholds in the basic network based on an
        /// array, modify the array. Previous values may be used, or they may be
        /// discarded, depending on the randomizer.
        /// </summary>
        /// <param name="network">A network to randomize.</param>
        void Randomize(BasicNetwork network);

    }

}
