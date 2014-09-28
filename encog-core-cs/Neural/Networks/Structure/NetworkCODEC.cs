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
using Encog.ML;

namespace Encog.Neural.Networks.Structure
{
    /// <summary>
    /// This class will extract the "long term memory" of a neural network, that is
    /// the weights and bias values into an array. This array can be used to view the
    /// neural network as a linear array of doubles. These values can then be
    /// modified and copied back into the neural network. This is very useful for
    /// simulated annealing, as well as genetic algorithms.
    /// </summary>
    ///
    public static class NetworkCODEC
    {
        /// <summary>
        /// Error message.
        /// </summary>
        ///
        private const String Error = "This machine learning method cannot be encoded:";

        /// <summary>
        /// Use an array to populate the memory of the neural network.
        /// </summary>
        ///
        /// <param name="array">An array of doubles.</param>
        /// <param name="network">The network to encode.</param>
        public static void ArrayToNetwork(double[] array,
                                          IMLMethod network)
        {
            if (network is IMLEncodable)
            {
                ((IMLEncodable) network).DecodeFromArray(array);
                return;
            }
            throw new NeuralNetworkError(Error
                                         + network.GetType().FullName);
        }

        /// <summary>
        /// Determine if the two neural networks are equal. Uses exact precision
        /// required by Arrays.equals.
        /// </summary>
        ///
        /// <param name="network1">The first network.</param>
        /// <param name="network2">The second network.</param>
        /// <returns>True if the two networks are equal.</returns>
        public static bool Equals(BasicNetwork network1,
                                  BasicNetwork network2)
        {
            return Equals(network1, network2, EncogFramework.DefaultPrecision);
        }

        /// <summary>
        /// Determine if the two neural networks are equal.
        /// </summary>
        ///
        /// <param name="network1">The first network.</param>
        /// <param name="network2">The second network.</param>
        /// <param name="precision">How many decimal places to check.</param>
        /// <returns>True if the two networks are equal.</returns>
        public static bool Equals(BasicNetwork network1,
                                  BasicNetwork network2, int precision)
        {
            double[] array1 = NetworkToArray(network1);
            double[] array2 = NetworkToArray(network2);

            if (array1.Length != array2.Length)
            {
                return false;
            }

            double test = Math.Pow(10.0d, precision);
            if (Double.IsInfinity(test) || (test > Int64.MaxValue))
            {
                throw new NeuralNetworkError("Precision of " + precision
                                             + " decimal places is not supported.");
            }

            for (int i = 0; i < array1.Length; i++)
            {
                var l1 = (long) (array1[i]*test);
                var l2 = (long) (array2[i]*test);
                if (l1 != l2)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determine the network size.
        /// </summary>
        ///
        /// <param name="network">The network.</param>
        /// <returns>The size.</returns>
        public static int NetworkSize(IMLMethod network)
        {
            if (network is IMLEncodable)
            {
                return ((IMLEncodable) network).EncodedArrayLength();
            }
            throw new NeuralNetworkError(Error
                                         + network.GetType().FullName);
        }

        /// <summary>
        /// Convert to an array. This is used with some training algorithms that
        /// require that the "memory" of the neuron(the weight and bias values) be
        /// expressed as a linear array.
        /// </summary>
        ///
        /// <param name="network">The network to encode.</param>
        /// <returns>The memory of the neuron.</returns>
        public static double[] NetworkToArray(IMLMethod network)
        {
            int size = NetworkSize(network);

            if (network is IMLEncodable)
            {
                var encoded = new double[size];
                ((IMLEncodable) network).EncodeToArray(encoded);
                return encoded;
            }
            throw new NeuralNetworkError(Error
                                         + network.GetType().FullName);
        }
    }
}
