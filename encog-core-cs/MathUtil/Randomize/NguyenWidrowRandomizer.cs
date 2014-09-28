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
using Encog.Neural.Networks;
using Encog.Engine.Network.Activation;
using Encog.MathUtil.Matrices;

namespace Encog.MathUtil.Randomize
{
    /// <summary>
    /// Implementation of <i>Nguyen-Widrow</i> weight initialization. This is the
    /// default weight initialization used by Encog, as it generally provides the
    /// most trainable neural network.
    /// </summary>
    ///
    public class NguyenWidrowRandomizer : IRandomizer
    {
        /// <summary>
        /// Message to indicate which operations are not allowed.
        /// </summary>
        public static String MSG = "This type of randomization is not supported by Nguyen-Widrow";

        /// <summary>
        /// Randomize the specified BasicNetwork.
        /// </summary>
        /// <param name="method">The network to randomize.</param>
        public void Randomize(IMLMethod method)
        {
            if (!(method is BasicNetwork))
            {
                throw new EncogError("Nguyen-Widrow only supports BasicNetwork.");
            }

            BasicNetwork network = (BasicNetwork)method;

            for (int fromLayer = 0; fromLayer < network.LayerCount - 1; fromLayer++)
            {
                RandomizeSynapse(network, fromLayer);
            }

        }

        /// <summary>
        /// Calculate the range of an activation function.
        /// </summary>
        /// <param name="af">The actionvation function to calculate for.</param>
        /// <param name="r">The value to collect the range at.</param>
        /// <returns>The range.</returns>
        private double CalculateRange(IActivationFunction af, double r)
        {
            double[] d = { r };
            af.ActivationFunction(d, 0, 1);
            return d[0];
        }

        /// <summary>
        /// Randomize the connections between two layers.
        /// </summary>
        /// <param name="network">The network to randomize.</param>
        /// <param name="fromLayer">The starting layer.</param>
        private void RandomizeSynapse(BasicNetwork network, int fromLayer)
        {
            int toLayer = fromLayer + 1;
            int toCount = network.GetLayerNeuronCount(toLayer);
            int fromCount = network.GetLayerNeuronCount(fromLayer);
            int fromCountTotalCount = network.GetLayerTotalNeuronCount(fromLayer);
            IActivationFunction af = network.GetActivation(toLayer);
            double low = CalculateRange(af, Double.NegativeInfinity);
            double high = CalculateRange(af, Double.PositiveInfinity);

            double b = 0.7d * Math.Pow(toCount, (1d / fromCount)) / (high - low);

            for (int toNeuron = 0; toNeuron < toCount; toNeuron++)
            {
                if (fromCount != fromCountTotalCount)
                {
                    double w = RangeRandomizer.Randomize(-b, b);
                    network.SetWeight(fromLayer, fromCount, toNeuron, w);
                }
                for (int fromNeuron = 0; fromNeuron < fromCount; fromNeuron++)
                {
                    double w = RangeRandomizer.Randomize(0, b);
                    network.SetWeight(fromLayer, fromNeuron, toNeuron, w);
                }
            }
        }

        /// <inheritdoc/>
        public double Randomize(double d)
        {
            throw new EncogError(MSG);
        }

        /// <inheritdoc/>
        public void Randomize(double[] d)
        {
            throw new EncogError(MSG);
        }

        /// <inheritdoc/>
        public void Randomize(double[][] d)
        {
            throw new EncogError(MSG);
        }

        /// <inheritdoc/>
        public void Randomize(Matrix m)
        {
            throw new EncogError(MSG);
        }

        /// <inheritdoc/>
        public void Randomize(double[] d, int begin, int size)
        {
            throw new EncogError(MSG);
        }
    }
}
