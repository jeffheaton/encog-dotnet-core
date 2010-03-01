// Encog(tm) Artificial Intelligence Framework v2.3
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
using Encog.Neural.Networks.Synapse;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Layers;
using Encog.MathUtil;
using Encog.Neural.Data;

namespace Encog.Neural.Networks.Training.Simple
{
    /// <summary>
    /// Train an ADALINE neural network.
    /// </summary>
    public class TrainAdaline : BasicTraining, ILearningRate
    {
        /// <summary>
        /// The network to train.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// The synapse to train.
        /// </summary>
        private ISynapse synapse;

        /// <summary>
        /// The training data.
        /// </summary>
        private INeuralDataSet training;

        /// <summary>
        /// The learning rate.
        /// </summary>
        private double learningRate;

        /// <summary>
        /// Construct he ADALINE trainer.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training set.</param>
        /// <param name="learningRate">The learning rate.</param>
        public TrainAdaline(BasicNetwork network, INeuralDataSet training,
                double learningRate)
        {
            if (network.Structure.Layers.Count > 2)
                throw new NeuralNetworkError(
                        "An ADALINE network only has two layers.");
            this.network = network;

            ILayer input = network.GetLayer(BasicNetwork.TAG_INPUT);

            this.synapse = input.Next[0];
            this.training = training;
            this.learningRate = learningRate;
        }

        /// <summary>
        /// The network to be trained.
        /// </summary>
        public override BasicNetwork Network
        {
            get
            {
                return this.network;
            }
        }

        /// <summary>
        /// Perform a training iteration.
        /// </summary>
        public override void Iteration()
        {

            ErrorCalculation errorCalculation = new ErrorCalculation();

            ILayer inputLayer = network.GetLayer(BasicNetwork.TAG_INPUT);
            ILayer outputLayer = network.GetLayer(BasicNetwork.TAG_OUTPUT);

            foreach (INeuralDataPair pair in this.training)
            {
                // calculate the error
                INeuralData output = this.network.Compute(pair.Input);

                for (int currentAdaline = 0; currentAdaline < output.Count; currentAdaline++)
                {
                    double diff = pair.Ideal[currentAdaline]
                            - output[currentAdaline];

                    // weights
                    for (int i = 0; i < inputLayer
                            .NeuronCount; i++)
                    {
                        double input = pair.Input[i];
                        synapse.WeightMatrix.Add(i, currentAdaline,
                                learningRate * diff * input);
                    }

                    // threshold (bias)
                    double t = outputLayer.Threshold[
                            currentAdaline];
                    t += learningRate * diff;
                    outputLayer.Threshold[currentAdaline] = t;
                }

                errorCalculation.UpdateError(output.Data, pair.Ideal.Data);
            }

            // set the global error
            this.Error = errorCalculation.CalculateRMS();
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
