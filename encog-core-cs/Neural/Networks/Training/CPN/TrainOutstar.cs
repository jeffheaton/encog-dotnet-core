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
    /// supervised/unsupervised network. The Outstar training handles the supervised
    /// portion of the training.
    /// </summary>
    public class TrainOutstar : BasicTraining, ILearningRate
    {
        /// <summary>
        /// The learning rate.
        /// </summary>
        private double learningRate;

        /// <summary>
        /// The network being trained.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// The training data.  Supervised training, so both input and ideal must
        /// be provided.
        /// </summary>
        private INeuralDataSet training;

        /// <summary>
        /// If the weights have not been initialized, then they must be initialized
        /// before training begins. This will be done on the first iteration.
        /// </summary>
        private bool mustInit = true;

        /// <summary>
        /// The parts of this CPN network.
        /// </summary>
        private FindCPN parts;

        /// <summary>
        /// Construct the outstar trainer.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data, must provide ideal outputs.</param>
        /// <param name="learningRate">The learning rate.</param>
        public TrainOutstar(BasicNetwork network, INeuralDataSet training,
                double learningRate)
        {
            this.network = network;
            this.training = training;
            this.learningRate = learningRate;
            this.parts = new FindCPN(this.network);
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
        private void InitWeight()
        {
            for (int i = 0; i < this.parts.OutstarLayer.NeuronCount; i++)
            {
                int j = 0;
                foreach (INeuralDataPair pair in this.training)
                {
                    this.parts.OutstarSynapse.WeightMatrix[j++, i] =
                            pair.Ideal[i];
                }
            }
            this.mustInit = false;
        }

        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        public override void Iteration()
        {

            if (this.mustInit)
                InitWeight();

            ErrorCalculation error = new ErrorCalculation();

            foreach (INeuralDataPair pair in this.training)
            {
                INeuralData output = this.parts.InstarSynapse.Compute(
                        pair.Input);
                int j = this.parts.Winner(output);
                for (int i = 0; i < this.parts.OutstarLayer.NeuronCount; i++)
                {
                    double delta = this.learningRate
                            * (pair.Ideal[i] - this.parts
                                    .OutstarSynapse.WeightMatrix[j, i]);
                    this.parts.OutstarSynapse.WeightMatrix.Add(j, i, delta);
                }

                error.UpdateError(output.Data, pair.Ideal.Data);
            }

            this.Error = error.Calculate();
        }
    }
}
