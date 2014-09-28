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
using Encog.MathUtil;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Train;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Util;

namespace Encog.Neural.CPN.Training
{
    /// <summary>
    /// Used for Instar training of a CPN neural network. A CPN network is a hybrid
    /// supervised/unsupervised network. The Instar training handles the unsupervised
    /// portion of the training.
    /// </summary>
    ///
    public class TrainInstar : BasicTraining, ILearningRate
    {
        /// <summary>
        /// The network being trained.
        /// </summary>
        ///
        private readonly CPNNetwork _network;

        /// <summary>
        /// The training data. This is unsupervised training, so only the input
        /// portion of the training data will be used.
        /// </summary>
        ///
        private readonly IMLDataSet _training;

        /// <summary>
        /// The learning rate.
        /// </summary>
        ///
        private double _learningRate;

        /// <summary>
        /// If the weights have not been initialized, then they can be initialized
        /// before training begins. This will be done on the first iteration.
        /// </summary>
        ///
        private bool _mustInit;

        /// <summary>
        /// Construct the instar training object.
        /// </summary>
        ///
        /// <param name="theNetwork">The network to be trained.</param>
        /// <param name="theTraining">The training data.</param>
        /// <param name="theLearningRate">The learning rate.</param>
        /// <param name="theInitWeights">training elements as instar neurons.</param>
        public TrainInstar(CPNNetwork theNetwork, IMLDataSet theTraining,
                           double theLearningRate, bool theInitWeights) : base(TrainingImplementationType.Iterative)
        {
            _network = theNetwork;
            _training = theTraining;
            _learningRate = theLearningRate;
            _mustInit = theInitWeights;
        }

        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return false; }
        }


        /// <inheritdoc />
        public override IMLMethod Method
        {
            get { return _network; }
        }

        #region ILearningRate Members

        /// <inheritdoc />
        public double LearningRate
        {
            get { return _learningRate; }
            set { _learningRate = value; }
        }

        #endregion

        /// <summary>
        /// Approximate the weights based on the input values.
        /// </summary>
        ///
        private void InitWeights()
        {
            if (_training.Count != _network.InstarCount)
            {
                throw new NeuralNetworkError(
                    "If the weights are to be set from the "
                    + "training data, then there must be one instar "
                    + "neuron for each training element.");
            }

            int i = 0;

            foreach (IMLDataPair pair  in  _training)
            {
                for (int j = 0; j < _network.InputCount; j++)
                {
                    _network.WeightsInputToInstar[j, i] =
                        pair.Input[j];
                }
                i++;
            }
            _mustInit = false;
        }

        /// <inheritdoc />
        public override sealed void Iteration()
        {
            if (_mustInit)
            {
                InitWeights();
            }

            double worstDistance = Double.NegativeInfinity;


            foreach (IMLDataPair pair  in  _training)
            {
                IMLData xout = _network.ComputeInstar(pair.Input);

                // determine winner
                int winner = EngineArray.IndexOfLargest(xout);

                // calculate the distance
                double distance = 0;
                for (int i = 0; i < pair.Input.Count; i++)
                {
                    double diff = pair.Input[i]
                                  - _network.WeightsInputToInstar[i, winner];
                    distance += diff*diff;
                }
                distance = BoundMath.Sqrt(distance);

                if (distance > worstDistance)
                {
                    worstDistance = distance;
                }

                // train
                for (int j = 0; j < _network.InputCount; j++)
                {
                    double delta = _learningRate
                                   *(pair.Input[j] - _network.WeightsInputToInstar[j, winner]);

                    _network.WeightsInputToInstar.Add(j, winner, delta);
                }
            }

            Error = worstDistance;
        }

        /// <inheritdoc />
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc />
        public override void Resume(TrainingContinuation state)
        {
        }
    }
}
