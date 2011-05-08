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
using Encog.Neural.Data;
using Encog.MathUtil;
using Encog.Engine.Util;

namespace Encog.Neural.Networks.Training.CPN
{
    /// <summary>
    /// Used for Instar training of a CPN neural network. A CPN network is a hybrid
    /// supervised/unsupervised network. The Instar training handles the unsupervised
    /// portion of the training.
    /// </summary>
    public class TrainInstar : BasicTraining, ILearningRate
    {
        /// <summary>
        /// The network being trained.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// The training data. This is unsupervised training, so only the input
        /// portion of the training data will be used.
        /// </summary>
        private MLDataSet training;

        /// <summary>
        /// The learning rate.
        /// </summary>
        private double learningRate;

        /// <summary>
        /// If the weights have not been initialized, then they must be initialized
        /// before training begins. This will be done on the first iteration.
        /// </summary>
        private bool mustInit = true;

        /// <summary>
        /// Used to find the parts of the CPN network.
        /// </summary>
        private FindCPN parts;

        /// <summary>
        /// Construct the instar training object.
        /// </summary>
        /// <param name="network">The network to be trained.</param>
        /// <param name="training">The training data.</param>
        /// <param name="learningRate">The learning rate.</param>
        public TrainInstar(BasicNetwork network, MLDataSet training,
                double learningRate)
        {
            this.network = network;
            this.training = training;
            this.learningRate = learningRate;
            this.parts = new FindCPN(network);
        }

        /// <summary>
        /// The network being trained.
        /// </summary>
        public override BasicNetwork Network
        {
            get
            {
                return this.network;
            }
        }

        /// <summary>
        /// Approximate the weights based on the input values.
        /// </summary>
        private void InitWeights()
        {
            int i = 0;
            foreach (MLDataPair pair in this.training)
            {
                for (int j = 0; j < this.parts.InputLayer.NeuronCount; j++)
                {
                    this.parts.InstarSynapse.WeightMatrix[j, i] =
                            pair.Input[j];
                }
                i++;
            }

            this.mustInit = false;
        }

        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        public override void Iteration()
        {
            if (this.mustInit)
            {
                InitWeights();
            }

            double worstDistance = Double.NegativeInfinity;

            foreach (MLDataPair pair in this.training)
            {
                MLData output = this.parts.InstarSynapse.Compute(
                        pair.Input);

                // determine winner
                int winner = this.parts.Winner(output);

                // calculate the distance
                double distance = 0;
                for (int i = 0; i < pair.Input.Count; i++)
                {
                    double diff = pair.Input[i]
                    - this.parts.InstarSynapse.WeightMatrix[i, winner];
                    distance += diff * diff;
                }
                distance = BoundMath.Sqrt(distance);

                if (distance > worstDistance)
                    worstDistance = distance;

                // train

                for (int j = 0; j < this.parts.InstarSynapse
                        .FromNeuronCount; j++)
                {
                    double delta = this.learningRate
                            * (pair.Input[j] - this.parts
                                    .InstarSynapse.WeightMatrix[j, winner]);
                    this.parts.InstarSynapse.WeightMatrix.Add(j, winner, delta);
                }
            }
        }

        /// <summary>
        /// The learning rate.
        /// </summary>
        public double LearningRate
        {
            get
            {
                return this.learningRate;
            }
            set
            {
                this.learningRate = value;
            }
        }
    }
}
