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
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Train;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Neural.SOM;

namespace Encog.Neural.Som.Training.Clustercopy
{
    /// <summary>
    /// SOM cluster copy is a very simple trainer for SOM's. Using this triner all of
    /// the training data is copied to the SOM weights. This can provide a functional
    /// SOM, or can be used as a starting point for training.
    /// </summary>
    ///
    public class SOMClusterCopyTraining : BasicTraining
    {
        /// <summary>
        /// The SOM to train.
        /// </summary>
        ///
        private readonly SOMNetwork _network;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data.</param>
        public SOMClusterCopyTraining(SOMNetwork network, IMLDataSet training)
            : base(TrainingImplementationType.OnePass)
        {
            _network = network;
            Training = training;
            if (_network.OutputCount < training.Count)
            {
                throw new NeuralNetworkError(
                        "To use cluster copy training you must have at least as many output neurons as training elements.");
            }	
        }

        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return false; }
        }

        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get { return _network; }
        }

        /// <summary>
        /// Copy the specified input pattern to the weight matrix. This causes an
        /// output neuron to learn this pattern "exactly". This is useful when a
        /// winner is to be forced.
        /// </summary>
        ///
        /// <param name="outputNeuron">The output neuron to set.</param>
        /// <param name="input">The input pattern to copy.</param>
        private void CopyInputPattern(int outputNeuron, IMLData input)
        {
            for (int inputNeuron = 0; inputNeuron < _network.InputCount; inputNeuron++)
            {
                _network.Weights[inputNeuron, outputNeuron] = input[inputNeuron];
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed void Iteration()
        {
            int outputNeuron = 0;

            foreach (IMLDataPair pair  in  Training)
            {
                CopyInputPattern(outputNeuron++, pair.Input);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override void Resume(TrainingContinuation state)
        {
        }
    }
}
