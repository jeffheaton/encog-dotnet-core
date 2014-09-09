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
using Encog.MathUtil.Matrices;
using Encog.ML;
using Encog.Neural.Networks;

namespace Encog.MathUtil.Randomize
{
    /// <summary>
    /// Provides basic functionality that most randomizers will need.
    /// </summary>
    [Serializable]
    public abstract class BasicRandomizer : IRandomizer
    {
        /// <summary>
        /// The random number generator.
        /// </summary>
        ///
        private Random _random;

        /// <summary>
        /// Construct a random number generator with a random(current time) seed. If
        /// you want to set your own seed, just call "getRandom().setSeed".
        /// </summary>
        ///
        protected BasicRandomizer()
        {
            _random = new Random((int) (DateTime.Now.Ticks*100));
        }


        /// <value>the random to set</value>
        public Random Random
        {
            get { return _random; }
            set { _random = value; }
        }

        #region IRandomizer Members

        /// <summary>
        /// Randomize the array based on an array, modify the array. Previous values
        /// may be used, or they may be discarded, depending on the randomizer.
        /// </summary>
        ///
        /// <param name="d">An array to randomize.</param>
        public virtual void Randomize(double[] d)
        {
            Randomize(d, 0, d.Length);
        }

        /// <summary>
        /// Randomize the array based on an array, modify the array. Previous values
        /// may be used, or they may be discarded, depending on the randomizer.
        /// </summary>
        ///
        /// <param name="d">An array to randomize.</param>
        /// <param name="begin">The beginning element of the array.</param>
        /// <param name="size">The size of the array to copy.</param>
        public virtual void Randomize(double[] d, int begin, int size)
        {
            for (int i = 0; i < size; i++)
            {
                d[begin + i] = Randomize(d[begin + i]);
            }
        }

        /// <summary>
        /// Randomize the 2d array based on an array, modify the array. Previous
        /// values may be used, or they may be discarded, depending on the
        /// randomizer.
        /// </summary>
        ///
        /// <param name="d">An array to randomize.</param>
        public virtual void Randomize(double[][] d)
        {
            foreach (double[] t in d)
            {
                for (var c = 0; c < d[0].Length; c++)
                {
                    t[c] = Randomize(t[c]);
                }
            }
        }

        /// <summary>
        /// Randomize the matrix based on an array, modify the array. Previous values
        /// may be used, or they may be discarded, depending on the randomizer.
        /// </summary>
        ///
        /// <param name="m">A matrix to randomize.</param>
        public virtual void Randomize(Matrix m)
        {
            double[][] d = m.Data;
            for (int r = 0; r < m.Rows; r++)
            {
                for (int c = 0; c < m.Cols; c++)
                {
                    d[r][c] = Randomize(d[r][c]);
                }
            }
        }

        /// <summary>
        /// Randomize the synapses and biases in the basic network based on an array,
        /// modify the array. Previous values may be used, or they may be discarded,
        /// depending on the randomizer.
        /// </summary>
        ///
        /// <param name="method">A network to randomize.</param>
        public virtual void Randomize(IMLMethod method)
        {
            if (method is BasicNetwork)
            {
                var network = (BasicNetwork) method;
                for (int i = 0; i < network.LayerCount - 1; i++)
                {
                    Randomize(network, i);
                }
            }
            else if (method is IMLEncodable)
            {
                var encode = (IMLEncodable) method;
                var encoded = new double[encode.EncodedArrayLength()];
                encode.EncodeToArray(encoded);
                Randomize(encoded);
                encode.DecodeFromArray(encoded);
            }
        }

        /// <summary>
        /// from Encog.mathutil.randomize.Randomizer
        /// </summary>
        ///
        public abstract double Randomize(double d);

        #endregion

        /// <returns>The next double.</returns>
        public double NextDouble()
        {
            return _random.NextDouble();
        }

        /// <summary>
        /// Generate a random number in the specified range.
        /// </summary>
        ///
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>A random number.</returns>
        public double NextDouble(double min, double max)
        {
            double range = max - min;
            return (range*_random.NextDouble()) + min;
        }

        /// <summary>
        /// Randomize one level of a neural network.
        /// </summary>
        ///
        /// <param name="network">The network to randomize</param>
        /// <param name="fromLayer">The from level to randomize.</param>
        public virtual void Randomize(BasicNetwork network, int fromLayer)
        {
            int fromCount = network.GetLayerTotalNeuronCount(fromLayer);
            int toCount = network.GetLayerNeuronCount(fromLayer + 1);

            for (int fromNeuron = 0; fromNeuron < fromCount; fromNeuron++)
            {
                for (int toNeuron = 0; toNeuron < toCount; toNeuron++)
                {
                    double v = network.GetWeight(fromLayer, fromNeuron, toNeuron);
                    v = Randomize(v);
                    network.SetWeight(fromLayer, fromNeuron, toNeuron, v);
                }
            }
        }
    }
}
