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
        void Randomize(IMLMethod network);

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
