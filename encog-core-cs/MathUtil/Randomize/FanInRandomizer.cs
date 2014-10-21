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
using Encog.MathUtil.Randomize;
using Encog.Neural.Networks;

namespace Encog.MathUtil.Randomize
{
    /// <summary>
    /// A randomizer that attempts to create starting weight values that are
    /// conducive to propagation training.
    /// This is one of the best randomizers offered in Encog, however, the Nguyen
    /// Widrow method generally performs better.
    /// From:
    /// Neural Networks - A Comprehensive Foundation, Haykin, chapter 6.7
    /// </summary>
    ///
    public class FanInRandomizer : BasicRandomizer
    {
        /// <summary>
        /// Error message. Can't use fan-in on a single number.
        /// </summary>
        ///
        internal const String Error = "To use FanInRandomizer you must "
                                      + "present a Matrix or 2D array type value.";

        /// <summary>
        /// The default boundary.
        /// </summary>
        ///
        private const double DefaultBoundary = 2.4d;

        /// <summary>
        /// The lower bound. 
        /// </summary>
        ///
        private readonly double _lowerBound;

        /// <summary>
        /// Should the square root of the number of rows be used?
        /// </summary>
        ///
        private readonly bool _sqrt;

        /// <summary>
        /// The upper bound. 
        /// </summary>
        ///
        private readonly double _upperBound;

        /// <summary>
        /// Create a fan-in randomizer with default values.
        /// </summary>
        ///
        public FanInRandomizer() : this(-DefaultBoundary, DefaultBoundary, false)
        {
        }

        /// <summary>
        /// Construct a fan-in randomizer along the specified boundary. The min will
        /// be -boundary and the max will be boundary.
        /// </summary>
        ///
        /// <param name="boundary">The boundary for the fan-in.</param>
        /// <param name="sqrt"></param>
        public FanInRandomizer(double boundary, bool sqrt) : this(-boundary, boundary, sqrt)
        {
        }

        /// <summary>
        /// Construct a fan-in randomizer. Use the specified bounds.
        /// </summary>
        ///
        /// <param name="aLowerBound">The lower bound.</param>
        /// <param name="anUpperBound">The upper bound.</param>
        /// <param name="sqrt"></param>
        public FanInRandomizer(double aLowerBound, double anUpperBound,
                               bool sqrt)
        {
            _lowerBound = aLowerBound;
            _upperBound = anUpperBound;
            _sqrt = sqrt;
        }

        /// <summary>
        /// Calculate the fan-in value.
        /// </summary>
        ///
        /// <param name="rows">The number of rows.</param>
        /// <returns>The fan-in value.</returns>
        private double CalculateValue(int rows)
        {
            double rowValue = _sqrt ? Math.Sqrt(rows) : rows;

            return (_lowerBound/rowValue) + NextDouble()
                   *((_upperBound - _lowerBound)/rowValue);
        }

        /// <summary>
        /// Throw an error if this class is used improperly.
        /// </summary>
        ///
        private static void CauseError()
        {
            throw new EncogError(Error);
        }

        /// <summary>
        /// Starting with the specified number, randomize it to the degree specified
        /// by this randomizer. This could be a totally new random number, or it
        /// could be based on the specified number.
        /// </summary>
        ///
        /// <param name="d">The number to randomize.</param>
        /// <returns>A randomized number.</returns>
        public override double Randomize(double d)
        {
            CauseError();
            return 0;
        }

        /// <summary>
        /// Randomize the array based on an array, modify the array. Previous values
        /// may be used, or they may be discarded, depending on the randomizer.
        /// </summary>
        ///
        /// <param name="d">An array to randomize.</param>
        public override void Randomize(double[] d)
        {
            for (int i = 0; i < d.Length; i++)
            {
                d[i] = CalculateValue(1);
            }
        }

        /// <summary>
        /// Randomize the 2d array based on an array, modify the array. Previous
        /// values may be used, or they may be discarded, depending on the
        /// randomizer.
        /// </summary>
        ///
        /// <param name="d">An array to randomize.</param>
        public override void Randomize(double[][] d)
        {
            foreach (double[] t in d)
            {
                for (var col = 0; col < d[0].Length; col++)
                {
                    t[col] = CalculateValue(d.Length);
                }
            }
        }

        /// <summary>
        /// Randomize the matrix based on an array, modify the array. Previous values
        /// may be used, or they may be discarded, depending on the randomizer.
        /// </summary>
        ///
        /// <param name="m">A matrix to randomize.</param>
        public override void Randomize(Matrix m)
        {
            for (int row = 0; row < m.Rows; row++)
            {
                for (int col = 0; col < m.Cols; col++)
                {
                    m[row, col] = CalculateValue(m.Rows);
                }
            }
        }

        /// <summary>
        /// Randomize one level of a neural network.
        /// </summary>
        ///
        /// <param name="network">The network to randomize</param>
        /// <param name="fromLayer">The from level to randomize.</param>
        public override void Randomize(BasicNetwork network, int fromLayer)
        {
            int fromCount = network.GetLayerTotalNeuronCount(fromLayer);
            int toCount = network.GetLayerNeuronCount(fromLayer + 1);

            for (int fromNeuron = 0; fromNeuron < fromCount; fromNeuron++)
            {
                for (int toNeuron = 0; toNeuron < toCount; toNeuron++)
                {
                    double v = CalculateValue(toCount);
                    network.SetWeight(fromLayer, fromNeuron, toNeuron, v);
                }
            }
        }
    }
}
