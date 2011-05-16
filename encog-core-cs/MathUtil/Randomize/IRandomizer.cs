using Encog.MathUtil.Matrices;
using Encog.ML;

namespace Encog.MathUtil.Randomize
{
    /// <summary>
    /// Defines the interface for a class that is capable of randomizing the weights
    /// and bias values of a neural network.
    /// </summary>
    ///
    public interface IRandomizer
    {
        /// <summary>
        /// Randomize the synapses and bias values in the basic network based on an
        /// array, modify the array. Previous values may be used, or they may be
        /// discarded, depending on the randomizer.
        /// </summary>
        ///
        /// <param name="network">A network to randomize.</param>
        void Randomize(MLMethod network);

        /// <summary>
        /// Starting with the specified number, randomize it to the degree specified
        /// by this randomizer. This could be a totally new random number, or it
        /// could be based on the specified number.
        /// </summary>
        ///
        /// <param name="d">The number to randomize.</param>
        /// <returns>A randomized number.</returns>
        double Randomize(double d);

        /// <summary>
        /// Randomize the array based on an array, modify the array. Previous values
        /// may be used, or they may be discarded, depending on the randomizer.
        /// </summary>
        ///
        /// <param name="d">An array to randomize.</param>
        void Randomize(double[] d);

        /// <summary>
        /// Randomize the 2d array based on an array, modify the array. Previous
        /// values may be used, or they may be discarded, depending on the
        /// randomizer.
        /// </summary>
        ///
        /// <param name="d">An array to randomize.</param>
        void Randomize(double[][] d);

        /// <summary>
        /// Randomize the matrix based on an array, modify the array. Previous values
        /// may be used, or they may be discarded, depending on the randomizer.
        /// </summary>
        ///
        /// <param name="m">A matrix to randomize.</param>
        void Randomize(Matrix m);

        /// <summary>
        /// Randomize an array.
        /// </summary>
        ///
        /// <param name="d">The array to randomize.</param>
        /// <param name="begin">The beginning element.</param>
        /// <param name="size">The size of the array.</param>
        void Randomize(double[] d, int begin, int size);
    }
}