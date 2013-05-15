//
// Encog(tm) Core v3.2 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2013 Heaton Research, Inc.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.Util.Simple;
using Encog.ML.Data.Basic;
using Encog.Util;

namespace Encog.Neural.NEAT
{
    [Serializable]
    public class NEATNetwork : IMLRegression, IMLError
    {
        /// <summary>
        /// The neuron links.
        /// </summary>
        private NEATLink[] links;

        /// <summary>
        /// The activation functions.
        /// </summary>
        private IActivationFunction[] activationFunctions;

        /// <summary>
        /// The pre-activation values, used to feed the neurons.
        /// </summary>
        private double[] preActivation;

        /// <summary>
        /// The post-activation values, used as the output from the neurons.
        /// </summary>
        private double[] postActivation;

        /// <summary>
        /// The index to the starting location of the output neurons.
        /// </summary>
        private int outputIndex;

        /// <summary>
        /// The input count.
        /// </summary>
        private int inputCount;

        /// <summary>
        /// The output count.
        /// </summary>
        private int outputCount;

        /// <summary>
        /// The number of activation cycles to use.
        /// </summary>
        public int ActivationCycles { get; set; }

        /// <summary>
        /// True, if the network has relaxed and values no longer changing. Used when
        /// activationCycles is set to zero for auto.
        /// </summary>
        public bool HasRelaxed { get; set; }

        /// <summary>
        /// The amount of change allowed before the network is considered to have
        /// relaxed.
        /// </summary>
        private double RelaxationThreshold { get; set; }

        /// <summary>
        /// Construct a NEAT network. The links that are passed in also define the
        /// neurons. 
        /// </summary>
        /// <param name="inputNeuronCount">The input neuron count.</param>
        /// <param name="outputNeuronCount">The output neuron count.</param>
        /// <param name="connectionArray">The links.</param>
        /// <param name="theActivationFunctions">The activation functions.</param>
        public NEATNetwork(int inputNeuronCount, int outputNeuronCount,
                IList<NEATLink> connectionArray,
                IActivationFunction[] theActivationFunctions)
        {

            ActivationCycles = NEATPopulation.DEFAULT_CYCLES;
            HasRelaxed = false;

            this.links = new NEATLink[connectionArray.Count];
            for (int i = 0; i < connectionArray.Count; i++)
            {
                this.links[i] = connectionArray[i];
            }

            this.activationFunctions = theActivationFunctions;
            int neuronCount = this.activationFunctions.Length;

            this.preActivation = new double[neuronCount];
            this.postActivation = new double[neuronCount];

            this.inputCount = inputNeuronCount;
            this.outputIndex = inputNeuronCount + 1;
            this.outputCount = outputNeuronCount;

            // bias
            this.postActivation[0] = 1.0;
        }

        /// <summary>
        /// Calculate the error for this neural network. 
        /// </summary>
        /// <param name="data">The training set.</param>
        /// <returns>The error percentage.</returns>
        public double CalculateError(IMLDataSet data)
        {
            return EncogUtility.CalculateRegressionError(this, data);
        }

        /// <summary>
        /// Compute the output from this synapse.
        /// </summary>
        /// <param name="input">The input to this synapse.</param>
        /// <returns>The output from this synapse.</returns>
        public IMLData Compute(IMLData input)
        {
            double[] result = new double[this.outputCount];

            // clear from previous
            EngineArray.Fill(this.preActivation, 0.0);
            EngineArray.Fill(this.postActivation, 0.0);
            this.postActivation[0] = 1.0;

            // copy input
            for (int i = 0; i < this.inputCount; i++)
            {
                this.postActivation[i+1] = input[i];
            }
            
            // iterate through the network activationCycles times
            for (int i = 0; i < this.ActivationCycles; ++i)
            {
                InternalCompute();
            }

            // copy output
            for (int i = 0; i < this.outputCount; i++)
            {
                result[i] = this.postActivation[this.outputIndex+i];
            }

            return new BasicMLData(result);
        }

        /// <summary>
        /// The activation functions.
        /// </summary>
        public IActivationFunction[] ActivationFunctions
        {
            get
            {
                return this.activationFunctions;
            }
        }

        /// <inheritdoc/>
        public int InputCount
        {
            get
            {
                return this.inputCount;
            }
        }

        /// <inheritdoc/>
        public NEATLink[] Links
        {
            get
            {
                return this.links;
            }
        }

        /// <inheritdoc/>
        public int OutputCount
        {
            get
            {
                return this.outputCount;
            }
        }

        /// <summary>
        /// The starting location of the output neurons.
        /// </summary>
        public int OutputIndex
        {
            get
            {
                return this.outputIndex;
            }
        }

        /// <summary>
        /// The post-activation values, used as the output from the neurons.
        /// </summary>
        public double[] PostActivation
        {
            get
            {
                return this.postActivation;
            }
        }

        /// <summary>
        /// The pre-activation values, used to feed the neurons.
        /// </summary>
        public double[] PreActivation
        {
            get
            {
                return this.preActivation;
            }
        }
  
        /// <summary>
        /// Perform one activation cycle.
        /// </summary>
        private void InternalCompute()
        {
            for (int j = 0; j < this.links.Length; j++)
            {
                this.preActivation[this.links[j].ToNeuron] += this.postActivation[this.links[j]
                        .FromNeuron] * this.links[j].Weight;
            }

            for (int j = this.outputIndex; j < this.preActivation.Length; j++)
            {
                this.postActivation[j] = this.preActivation[j];
                this.activationFunctions[j].ActivationFunction(this.postActivation,
                        j, 1);
                this.preActivation[j] = 0.0F;
            }
        }
    }
}
