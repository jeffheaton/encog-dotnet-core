//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
using Encog.ML.Data;

namespace Encog.Neural.Flat.Train.Prop
{
    /// <summary>
    /// Train a flat network using RPROP.
    /// </summary>
    ///
    public class TrainFlatNetworkResilient : TrainFlatNetworkProp
    {
        /// <summary>
        /// The maximum step value for rprop.
        /// </summary>
        ///
        private readonly double _maxStep;

        /// <summary>
        /// The update values, for the weights and thresholds.
        /// </summary>
        ///
        private readonly double[] _updateValues;

        /// <summary>
        /// The zero tolerance.
        /// </summary>
        ///
        private readonly double _zeroTolerance;

        /// <summary>
        /// Construct a resilient trainer for flat networks.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="zeroTolerance">How close a number should be to zero to be counted as zero.</param>
        /// <param name="initialUpdate">The initial update value.</param>
        /// <param name="maxStep">The maximum step value.</param>
        public TrainFlatNetworkResilient(FlatNetwork network,
                                         IMLDataSet training, double zeroTolerance,
                                         double initialUpdate, double maxStep) : base(network, training)
        {
            _updateValues = new double[network.Weights.Length];
            _zeroTolerance = zeroTolerance;
            _maxStep = maxStep;

            for (int i = 0; i < _updateValues.Length; i++)
            {
                _updateValues[i] = initialUpdate;
            }
        }

        /// <summary>
        /// Tran a network using RPROP.
        /// </summary>
        ///
        /// <param name="flat">The network to train.</param>
        /// <param name="trainingSet">The training data to use.</param>
        public TrainFlatNetworkResilient(FlatNetwork flat,
                                         IMLDataSet trainingSet)
            : this(
                flat, trainingSet, RPROPConst.DefaultZeroTolerance, RPROPConst.DefaultInitialUpdate,
                RPROPConst.DefaultMaxStep)
        {
        }

        /// <value>The RPROP update values.</value>
        public double[] UpdateValues
        {
            get { return _updateValues; }
        }

        /// <summary>
        /// Determine the sign of the value.
        /// </summary>
        ///
        /// <param name="v">The value to check.</param>
        /// <returns>-1 if less than zero, 1 if greater, or 0 if zero.</returns>
        private int Sign(double v)
        {
            if (Math.Abs(v) < _zeroTolerance)
            {
                return 0;
            }
            if (v > 0)
            {
                return 1;
            }
            return -1;
        }

        /// <summary>
        /// Calculate the amount to change the weight by.
        /// </summary>
        ///
        /// <param name="gradients">The gradients.</param>
        /// <param name="lastGradient">The last gradients.</param>
        /// <param name="index">The index to update.</param>
        /// <returns>The amount to change the weight by.</returns>
        public override double UpdateWeight(double[] gradients,
                                            double[] lastGradient, int index)
        {
            // multiply the current and previous gradient, and take the
            // sign. We want to see if the gradient has changed its sign.
            int change = Sign(gradients[index]*lastGradient[index]);
            double weightChange = 0;

            // if the gradient has retained its sign, then we increase the
            // delta so that it will converge faster
            if (change > 0)
            {
                double delta = _updateValues[index]*RPROPConst.PositiveEta;
                delta = Math.Min(delta, _maxStep);
                weightChange = Sign(gradients[index])*delta;
                _updateValues[index] = delta;
                lastGradient[index] = gradients[index];
            }
            else if (change < 0)
            {
                // if change<0, then the sign has changed, and the last
                // delta was too big
                double delta = _updateValues[index]*RPROPConst.NegativeEta;
                delta = Math.Max(delta, RPROPConst.DeltaMin);
                _updateValues[index] = delta;
                // set the previous gradent to zero so that there will be no
                // adjustment the next iteration
                lastGradient[index] = 0;
            }
            else if (change == 0)
            {
                // if change==0 then there is no change to the delta
                double delta = _updateValues[index];
                weightChange = Sign(gradients[index])*delta;
                lastGradient[index] = gradients[index];
            }

            // apply the weight change, if any
            return weightChange;
        }
    }
}
