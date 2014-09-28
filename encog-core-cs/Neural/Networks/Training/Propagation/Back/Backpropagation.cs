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
using Encog.ML.Data;
using Encog.Neural.Networks.Training.Strategy;
using Encog.Util.Validate;

namespace Encog.Neural.Networks.Training.Propagation.Back
{
    /// <summary>
    /// This class implements a backpropagation training algorithm for feed forward
    /// neural networks. It is used in the same manner as any other training class
    /// that implements the Train interface.
    /// Backpropagation is a common neural network training algorithm. It works by
    /// analyzing the error of the output of the neural network. Each neuron in the
    /// output layer's contribution, according to weight, to this error is
    /// determined. These weights are then adjusted to minimize this error. This
    /// process continues working its way backwards through the layers of the neural
    /// network.
    /// This implementation of the backpropagation algorithm uses both momentum and a
    /// learning rate. The learning rate specifies the degree to which the weight
    /// matrixes will be modified through each iteration. The momentum specifies how
    /// much the previous learning iteration affects the current. To use no momentum
    /// at all specify zero.
    /// One primary problem with backpropagation is that the magnitude of the partial
    /// derivative is often detrimental to the training of the neural network. The
    /// other propagation methods of Manhatten and Resilient address this issue in
    /// different ways. In general, it is suggested that you use the resilient
    /// propagation technique for most Encog training tasks over back propagation.
    /// </summary>
    ///
    public class Backpropagation : Propagation, IMomentum,
                                   ILearningRate
    {
        /// <summary>
        /// The resume key for backpropagation.
        /// </summary>
        ///
        public const String PropertyLastDelta = "LAST_DELTA";

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
        /// Create a class to train using backpropagation. Use auto learn rate and
        /// momentum. Use the CPU to train.
        /// </summary>
        ///
        /// <param name="network">The network that is to be trained.</param>
        /// <param name="training">The training data to be used for backpropagation.</param>
        public Backpropagation(IContainsFlat network, IMLDataSet training) : this(network, training, 0, 0)
        {
            AddStrategy(new SmartLearningRate());
            AddStrategy(new SmartMomentum());
        }


        /// <param name="network">The network that is to be trained</param>
        /// <param name="training">The training set</param>
        /// <param name="learnRate"></param>
        /// <param name="momentum"></param>
        public Backpropagation(IContainsFlat network,
                               IMLDataSet training, double learnRate,
                               double momentum) : base(network, training)
        {
            ValidateNetwork.ValidateMethodToData(network, training);
            _momentum = momentum;
            _learningRate = learnRate;
            _lastDelta = new double[Network.Flat.Weights.Length];
        }

        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return true; }
        }


        /// <value>Ther last delta values.</value>
        public double[] LastDelta
        {
            get { return _lastDelta; }
        }

        #region ILearningRate Members

        /// <summary>
        /// Set the learning rate, this is value is essentially a percent. It is the
        /// degree to which the gradients are applied to the weight matrix to allow
        /// learning.
        /// </summary>
        public virtual double LearningRate
        {
            get { return _learningRate; }
            set { _learningRate = value; }
        }

        #endregion

        #region IMomentum Members

        /// <summary>
        /// Set the momentum for training. This is the degree to which changes from
        /// which the previous training iteration will affect this training
        /// iteration. This can be useful to overcome local minima.
        /// </summary>
        public virtual double Momentum
        {
            get { return _momentum; }
            set { _momentum = value; }
        }

        #endregion

        /// <summary>
        /// Determine if the specified continuation object is valid to resume with.
        /// </summary>
        ///
        /// <param name="state">The continuation object to check.</param>
        /// <returns>True if the specified continuation object is valid for this
        /// training method and network.</returns>
        public bool IsValidResume(TrainingContinuation state)
        {
            if (!state.Contents.ContainsKey(PropertyLastDelta))
            {
                return false;
            }

            if (!state.TrainingType.Equals(GetType().Name))
            {
                return false;
            }

            var d = (double[])state.Get(PropertyLastDelta);
            return d.Length == ((IContainsFlat) Method).Flat.Weights.Length;
        }

        /// <summary>
        /// Pause the training.
        /// </summary>
        ///
        /// <returns>A training continuation object to continue with.</returns>
        public override sealed TrainingContinuation Pause()
        {
            var result = new TrainingContinuation {TrainingType = GetType().Name};
            result.Set(PropertyLastDelta, _lastDelta);
            return result;
        }

        /// <summary>
        /// Resume training.
        /// </summary>
        ///
        /// <param name="state">The training state to return to.</param>
        public override sealed void Resume(TrainingContinuation state)
        {
            if (!IsValidResume(state))
            {
                throw new TrainingError("Invalid training resume data length");
            }

            _lastDelta = (double[])state.Get(PropertyLastDelta);
        }

        /// <summary>
        /// Update a weight.
        /// </summary>
        ///
        /// <param name="gradients">The gradients.</param>
        /// <param name="lastGradient">The last gradients.</param>
        /// <param name="index">The index.</param>
        /// <returns>The weight delta.</returns>
        public override double UpdateWeight(double[] gradients,
                                                   double[] lastGradient, int index)
        {
            double delta = (gradients[index] * _learningRate)
                           + (_lastDelta[index] * _momentum);
            _lastDelta[index] = delta;
            return delta;
        }

        /// <summary>
        /// Not needed for this training type.
        /// </summary>
        public override void InitOthers()
        {
        }
    }
}
