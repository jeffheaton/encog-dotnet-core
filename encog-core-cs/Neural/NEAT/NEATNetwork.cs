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
using System.Collections.Generic;
using Encog.ML;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.Util.Simple;
using Encog.ML.Data.Basic;
using Encog.Util;

namespace Encog.Neural.NEAT
{
    /// <summary>
    /// NEAT networks relieve the programmer of the need to define the hidden layer
    /// structure of the neural network.
    /// 
    /// The output from the neural network can be calculated normally or using a
    /// snapshot. The snapshot mode is slower, but it can be more accurate. The
    /// snapshot handles recurrent layers better, as it takes the time to loop
    /// through the network multiple times to "flush out" the recurrent links.
    /// 
    /// NeuroEvolution of Augmenting Topologies (NEAT) is a genetic algorithm for the
    /// generation of evolving artificial neural networks. It was developed by Ken
    /// Stanley while at The University of Texas at Austin.
    /// 
    /// http://www.cs.ucf.edu/~kstanley/
    /// 
    /// The following Journal articles were used to implement NEAT/HyperNEAT in
    /// Encog. Provided in BibTeX form.
    /// 
    /// Article{stanley:ec02,title={Evolving Neural Networks Through Augmenting
    /// Topologies}, author={Kenneth O. Stanley and Risto Miikkulainen}, volume={10},
    /// journal={Evolutionary Computation}, number={2}, pages={99-127}, url=
    /// "http://nn.cs.utexas.edu/?stanley:ec02" , year={2002}}
    /// 
    /// MISC{Gauci_abstractgenerating, author = {Jason Gauci and Kenneth Stanley},
    /// title = {ABSTRACT Generating Large-Scale Neural Networks Through Discovering
    /// Geometric Regularities}, year = {}}
    /// 
    /// INPROCEEDINGS{Whiteson05automaticfeature, author = {Shimon Whiteson and
    /// Kenneth O. Stanley and Risto Miikkulainen}, title = {Automatic feature
    /// selection in neuroevolution}, booktitle = {In Genetic and Evolutionary
    /// Computation Conference}, year = {2005}, pages = {1225--1232}, publisher =
    /// {ACM Press} }
    /// </summary>
    [Serializable]
    public class NEATNetwork : IMLRegression, IMLClassification, IMLError
    {
        /// <summary>
        /// The neuron links.
        /// </summary>
        private readonly NEATLink[] _links;

        /// <summary>
        /// The activation functions.
        /// </summary>
        private readonly IActivationFunction[] _activationFunctions;

        /// <summary>
        /// The pre-activation values, used to feed the neurons.
        /// </summary>
        private readonly double[] _preActivation;

        /// <summary>
        /// The post-activation values, used as the output from the neurons.
        /// </summary>
        private readonly double[] _postActivation;

        /// <summary>
        /// The index to the starting location of the output neurons.
        /// </summary>
        private readonly int _outputIndex;

        /// <summary>
        /// The input count.
        /// </summary>
        private readonly int _inputCount;

        /// <summary>
        /// The output count.
        /// </summary>
        private readonly int _outputCount;

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

            ActivationCycles = NEATPopulation.DefaultCycles;
            HasRelaxed = false;

            _links = new NEATLink[connectionArray.Count];
            for (int i = 0; i < connectionArray.Count; i++)
            {
                _links[i] = connectionArray[i];
            }

            _activationFunctions = theActivationFunctions;
            int neuronCount = _activationFunctions.Length;

            _preActivation = new double[neuronCount];
            _postActivation = new double[neuronCount];

            _inputCount = inputNeuronCount;
            _outputIndex = inputNeuronCount + 1;
            _outputCount = outputNeuronCount;

            // bias
            _postActivation[0] = 1.0;
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
            var result = new double[_outputCount];

            // clear from previous
            EngineArray.Fill(_preActivation, 0.0);
            EngineArray.Fill(_postActivation, 0.0);
            _postActivation[0] = 1.0;

            // copy input
            for (int i = 0; i < _inputCount; i++)
            {
                _postActivation[i + 1] = input[i];
            }

            // iterate through the network activationCycles times
            for (int i = 0; i < ActivationCycles; ++i)
            {
                InternalCompute();
            }

            // copy output
            for (int i = 0; i < _outputCount; i++)
            {
                result[i] = _postActivation[_outputIndex + i];
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
                return _activationFunctions;
            }
        }

        /// <inheritdoc/>
        public int InputCount
        {
            get
            {
                return _inputCount;
            }
        }

        /// <inheritdoc/>
        public NEATLink[] Links
        {
            get
            {
                return _links;
            }
        }

        /// <inheritdoc/>
        public int OutputCount
        {
            get
            {
                return _outputCount;
            }
        }

        /// <summary>
        /// The starting location of the output neurons.
        /// </summary>
        public int OutputIndex
        {
            get
            {
                return _outputIndex;
            }
        }

        /// <summary>
        /// The post-activation values, used as the output from the neurons.
        /// </summary>
        public double[] PostActivation
        {
            get
            {
                return _postActivation;
            }
        }

        /// <summary>
        /// The pre-activation values, used to feed the neurons.
        /// </summary>
        public double[] PreActivation
        {
            get
            {
                return _preActivation;
            }
        }

        /// <summary>
        /// Perform one activation cycle.
        /// </summary>
        private void InternalCompute()
        {
            foreach (NEATLink t in _links)
            {
                _preActivation[t.ToNeuron] += _postActivation[t.FromNeuron] * t.Weight;
            }

            for (int j = _outputIndex; j < _preActivation.Length; j++)
            {
                _postActivation[j] = _preActivation[j];
                _activationFunctions[j].ActivationFunction(_postActivation,
                        j, 1);
                _preActivation[j] = 0.0F;
            }
        }

        /// <inheritdoc/>
        public int Classify(IMLData input)
        {
            IMLData output = Compute(input);
            return EngineArray.MaxIndex(output);
        }
    }
}
