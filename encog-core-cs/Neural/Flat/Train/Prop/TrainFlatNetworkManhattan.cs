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
    /// Train the flat network using Manhattan update rule.
    /// </summary>
    ///
    public class TrainFlatNetworkManhattan : TrainFlatNetworkProp
    {
        /// <summary>
        /// The zero tolerance to use.
        /// </summary>
        ///
        private readonly double _zeroTolerance;

        /// <summary>
        /// The learning rate.
        /// </summary>
        ///
        private double _learningRate;

        /// <summary>
        /// Construct a trainer for flat networks to use the Manhattan update rule.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="theLearningRate">The learning rate to use.</param>
        public TrainFlatNetworkManhattan(FlatNetwork network,
                                         IMLDataSet training, double theLearningRate) : base(network, training)
        {
            _learningRate = theLearningRate;
            _zeroTolerance = RPROPConst.DefaultZeroTolerance;
        }


        /// <value>the learningRate to set</value>
        public double LearningRate
        {
            get { return _learningRate; }
            set { _learningRate = value; }
        }


        /// <summary>
        /// Calculate the amount to change the weight by.
        /// </summary>
        ///
        /// <param name="gradients">The gradients.</param>
        /// <param name="lastGradient">The last gradients.</param>
        /// <param name="index">The index to update.</param>
        /// <returns>The amount to change the weight by.</returns>
        public override sealed double UpdateWeight(double[] gradients,
                                                   double[] lastGradient, int index)
        {
            if (Math.Abs(gradients[index]) < _zeroTolerance)
            {
                return 0;
            }
            else if (gradients[index] > 0)
            {
                return _learningRate;
            }
            else
            {
                return -_learningRate;
            }
        }
    }
}
