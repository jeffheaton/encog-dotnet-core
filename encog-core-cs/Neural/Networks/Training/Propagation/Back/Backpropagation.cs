// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Training.Strategy;
using Encog.Neural.Networks.Structure;
using Encog.Engine.Network.Train.Prop;

namespace Encog.Neural.Networks.Training.Propagation.Back
{
    /// <summary>
    /// This class implements a backpropagation training algorithm for feed forward
    /// neural networks. It is used in the same manner as any other training class
    /// that implements the Train interface.
    /// 
    /// Backpropagation is a common neural network training algorithm. It works by
    /// analyzing the error of the output of the neural network. Each neuron in the
    /// output layer's contribution, according to weight, to this error is
    /// determined. These weights are then adjusted to minimize this error. This
    /// process continues working its way backwards through the layers of the neural
    /// network.
    /// 
    /// This implementation of the backpropagation algorithm uses both momentum and a
    /// learning rate. The learning rate specifies the degree to which the weight
    /// matrixes will be modified through each iteration. The momentum specifies how
    /// much the previous learning iteration affects the current. To use no momentum
    /// at all specify zero.
    /// 
    /// One primary problem with backpropagation is that the magnitude of the partial
    /// derivative is often detrimental to the training of the neural network. The
    /// other propagation methods of Manhatten and Resilient address this issue in
    /// different ways. In general, it is suggested that you use the resilient
    /// propagation technique for most Encog training tasks over back propagation.
    /// </summary>
    public class Backpropagation : Propagation, IMomentum,
            ILearningRate
    {
        /// <summary>
        /// The resume key for backpropagation.
        /// </summary>
        public readonly static String LAST_DELTA = "LAST_DELTA";

        /// <summary>
        /// Set the momentum for training. This is the degree to which changes from
        /// which the previous training iteration will affect this training
        /// iteration. This can be useful to overcome local minima.
        /// </summary>
        public double Momentum
        {
            get
            {
                return ((TrainFlatNetworkBackPropagation)this.FlatTraining).Momentum;
            }
            set
            {
                ((TrainFlatNetworkBackPropagation)this.FlatTraining).Momentum = value;
            }
        }

        /// <summary>
        /// The learning rate, this is value is essentially a percent. It is the
        /// degree to which the gradients are applied to the weight matrix to allow
        /// learning.
        /// </summary>
        public double LearningRate
        {
            get
            {
                return ((TrainFlatNetworkBackPropagation)this.FlatTraining).LearningRate;
            }
            set
            {
                ((TrainFlatNetworkBackPropagation)this.FlatTraining).LearningRate = value;
            }

        }

        /// <summary>
        /// Create a class to train using backpropagation.  Use auto learn rate and momentum.  Use the CPU to train.
        /// </summary>
        /// <param name="network">The network that is to be trained.</param>
        /// <param name="training">The training data to be used for backpropagation.</param>
        public Backpropagation(BasicNetwork network,
                INeuralDataSet training)
            : this(network, training, 0, 0)
        {
            AddStrategy(new SmartLearningRate());
            AddStrategy(new SmartMomentum());
        }

        /// <summary>
        /// The network that is to be trained.
        /// </summary>
        /// <param name="network">The training set.</param>
        /// <param name="training">The OpenCL profile to use, null for CPU.</param>
        /// <param name="profile">The OpenCL profile, or null for none.</param>
        /// <param name="learnRate">The rate at which the weight matrix will be adjusted based on
        /// learning.</param>
        /// <param name="momentum">The influence that previous iteration's training deltas will
        /// have on the current iteration.</param>
        public Backpropagation(BasicNetwork network,
                 INeuralDataSet training, double learnRate,
                 double momentum)
            : base(network, training)
        {
            TrainFlatNetworkBackPropagation backFlat = new TrainFlatNetworkBackPropagation(
                    network.Structure.Flat,
                    this.Training,
                    learnRate,
                    momentum);
            this.FlatTraining = backFlat;
        }

        /// <summary>
        /// Determine if the specified continuation object is valid to resume with. 
        /// </summary>
        /// <param name="state">The continuation object to check.</param>
        /// <returns>True if the specified continuation object is valid for this
        /// training method and network.</returns>
        public override bool IsValidResume(TrainingContinuation state)
        {
            if (!state.Contents.ContainsKey(Backpropagation.LAST_DELTA))
            {
                return false;
            }

            double[] d = (double[])state
                    [Backpropagation.LAST_DELTA];
            return d.Length == Network.Structure.CalculateSize();
        }
        
        /// <summary>
        /// Pause the training. 
        /// </summary>
        /// <returns>A training continuation object to continue with.</returns>
        public override TrainingContinuation Pause()
        {
            TrainingContinuation result = new TrainingContinuation();

            TrainFlatNetworkBackPropagation backFlat = (TrainFlatNetworkBackPropagation)FlatTraining;
            double[] d = backFlat.LastDelta;
            result[Backpropagation.LAST_DELTA] = d;
            return result;
        }
        
        /// <summary>
        /// Resume training. 
        /// </summary>
        /// <param name="state">The training state to return to.</param>
        public override void Resume(TrainingContinuation state)
        {
            if (!IsValidResume(state))
            {
                throw new TrainingError("Invalid training resume data length");
            }

            ((TrainFlatNetworkBackPropagation)this.FlatTraining).LastDelta =
                (double[])state[Backpropagation.LAST_DELTA];

        }

        /// <summary>
        /// The last deltas.
        /// </summary>
        public double[] LastDelta
        {
            get
            {
                return ((TrainFlatNetworkBackPropagation)this.FlatTraining).LastDelta;
            }
        }

    }
}
