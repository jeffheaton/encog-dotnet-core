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
using Encog.ML.Data;

namespace Encog.Neural.Flat.Train.Prop
{
    /// <summary>
    /// Train a flat network, using backpropagation.
    /// </summary>
    ///
    public class TrainFlatNetworkBackPropagation : TrainFlatNetworkProp
    {
        /// <summary>
        /// The last delta values.
        /// </summary>
        ///
        private double[] _lastDelta;

        /// <summary>
        /// The learning rate.
        /// </summary>
        ///
        private double _learningRate;

        /// <summary>
        /// The momentum.
        /// </summary>
        ///
        private double _momentum;

        /// <summary>
        /// Construct a backprop trainer for flat networks.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data.</param>
        /// <param name="theLearningRate">The learning rate.</param>
        /// <param name="theMomentum">The momentum.</param>
        public TrainFlatNetworkBackPropagation(FlatNetwork network,
                                               IMLDataSet training, double theLearningRate,
                                               double theMomentum) : base(network, training)
        {
            _momentum = theMomentum;
            _learningRate = theLearningRate;
            _lastDelta = new double[network.Weights.Length];
        }

        /// <summary>
        /// Set the last delta.
        /// </summary>
        public double[] LastDelta
        {
            get { return _lastDelta; }
            set { _lastDelta = value; }
        }


        /// <summary>
        /// Set the learning rate.
        /// </summary>
        public double LearningRate
        {
            get { return _learningRate; }
            set { _learningRate = value; }
        }


        /// <summary>
        /// Set the momentum.
        /// </summary>
        public double Momentum
        {
            get { return _momentum; }
            set { _momentum = value; }
        }


        /// <summary>
        /// Update a weight.
        /// </summary>
        ///
        /// <param name="gradients">The gradients.</param>
        /// <param name="lastGradient">The last gradients.</param>
        /// <param name="index">The index.</param>
        /// <returns>The weight delta.</returns>
        public override sealed double UpdateWeight(double[] gradients,
                                                   double[] lastGradient, int index)
        {
            double delta = (gradients[index]*_learningRate)
                           + (_lastDelta[index]*_momentum);
            _lastDelta[index] = delta;
            return delta;
        }
    }
}
