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
using System.Linq;
using Encog.Engine.Network.Activation;
using Encog.MathUtil.Randomize;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Freeform.Basic;
using Encog.Neural.Freeform.Factory;
using Encog.Neural.Networks;
using Encog.Util;
using Encog.Util.Simple;

namespace Encog.Neural.Freeform
{
    /// <summary>
    ///     Implements a freefrom neural network. A freeform neural network can represent
    ///     much more advanced structures than the flat networks that the Encog
    ///     BasicNetwork implements. However, while freeform networks are more advanced
    ///     than the BasicNetwork, they are also much slower.
    ///     Freeform networks allow just about any neuron to be connected to another
    ///     neuron. You can have neuron layers if you want, but they are not required.
    /// </summary>
    [Serializable]
    public class FreeformNetwork : BasicML, IMLContext,
        IMLRegression, IMLEncodable, IMLResettable, IMLClassification, IMLError
    {
        /// <summary>
        /// Perform a task for each connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public delegate void FreeformConnectionTask(IFreeformConnection connection);

        /// <summary>
        /// Perofmr a task for each neuron.
        /// </summary>
        /// <param name="neuron">The neuron.</param>
        public delegate void FreeformNeuronTask(IFreeformNeuron neuron);

        /// <summary>
        /// The connection factory.
        /// </summary>
        private readonly IFreeformConnectionFactory _connectionFactory = new BasicFreeformConnectionFactory();

        /// <summary>
        /// The layer factory.
        /// </summary>
        private readonly IFreeformLayerFactory _layerFactory = new BasicFreeformLayerFactory();

        /// <summary>
        /// The neuron factory.
        /// </summary>
        private readonly IFreeformNeuronFactory _neuronFactory = new BasicFreeformNeuronFactory();

        /// <summary>
        /// The input summation factory.
        /// </summary>
        private readonly IInputSummationFactory _summationFactory = new BasicActivationSummationFactory();
        
        /// <summary>
        /// The input layer.
        /// </summary>
        private IFreeformLayer _inputLayer;

        /// <summary>
        /// The output layer.
        /// </summary>
        private IFreeformLayer _outputLayer;

        /// <summary>
        /// Default constructor. Typically should not be directly used.
        /// </summary>
        public FreeformNetwork()
        {
        }

        /// <summary>
        /// Craete a freeform network from a basic network. 
        /// </summary>
        /// <param name="network">The basic network to use.</param>
        public FreeformNetwork(BasicNetwork network)
        {
            if (network.LayerCount < 2)
            {
                throw new FreeformNetworkError(
                    "The BasicNetwork must have at least two layers to be converted.");
            }

            // handle each layer
            IFreeformLayer previousLayer = null;

            for (int currentLayerIndex = 0;
                currentLayerIndex < network
                    .LayerCount;
                currentLayerIndex++)
            {
                // create the layer
                IFreeformLayer currentLayer = _layerFactory.Factor();

                // Is this the input layer?
                if (_inputLayer == null)
                {
                    _inputLayer = currentLayer;
                }

                // Add the neurons for this layer
                for (int i = 0; i < network.GetLayerNeuronCount(currentLayerIndex); i++)
                {
                    // obtain the summation object.
                    IInputSummation summation = null;

                    if (previousLayer != null)
                    {
                        summation = _summationFactory.Factor(network
                            .GetActivation(currentLayerIndex));
                    }

                    // add the new neuron
                    currentLayer.Add(_neuronFactory.FactorRegular(summation));
                }

                // Fully connect this layer to previous
                if (previousLayer != null)
                {
                    ConnectLayersFromBasic(network, currentLayerIndex - 1,
                        previousLayer, currentLayer);
                }

                // Add the bias neuron
                // The bias is added after connections so it has no inputs
                if (network.IsLayerBiased(currentLayerIndex))
                {
                    IFreeformNeuron biasNeuron = _neuronFactory
                        .FactorRegular(null);
                    biasNeuron.IsBias = true;
                    biasNeuron.Activation = network
                        .GetLayerBiasActivation(currentLayerIndex);
                    currentLayer.Add(biasNeuron);
                }

                // update previous layer
                previousLayer = currentLayer;
            }

            // finally, set the output layer.
            _outputLayer = previousLayer;
        }

        /// <summary>
        /// The output layer.
        /// </summary>
        public IFreeformLayer OutputLayer
        {
            get { return _outputLayer; }
        }

        /// <inheritdoc/>
        public int Classify(IMLData input)
        {
            IMLData output = Compute(input);
            return EngineArray.MaxIndex(output);
        }

        /// <inheritdoc/>
        public void ClearContext()
        {
            PerformNeuronTask(
                neuron =>
                {
                    if (neuron is FreeformContextNeuron)
                    {
                        neuron.Activation = 0;
                    }
                });
        }

        /// <inheritdoc/>
        public void DecodeFromArray(double[] encoded)
        {
            int index = 0;
            var visited = new HashSet<IFreeformNeuron>();
            IList<IFreeformNeuron> queue = _outputLayer.Neurons.ToList();

            // first copy outputs to queue

            while (queue.Count > 0)
            {
                // pop a neuron off the queue
                IFreeformNeuron neuron = queue[0];
                queue.RemoveAt(0);
                visited.Add(neuron);

                // find anymore neurons and add them to the queue.
                if (neuron.InputSummation != null)
                {
                    foreach (IFreeformConnection connection in neuron
                        .InputSummation.List)
                    {
                        connection.Weight = encoded[index++];
                        IFreeformNeuron nextNeuron = connection.Source;
                        if (!visited.Contains(nextNeuron))
                        {
                            queue.Add(nextNeuron);
                        }
                    }
                }
            }
        }

        /// <inheritdoc/>
        public int EncodedArrayLength()
        {
            int result = 0;
            var visited = new HashSet<IFreeformNeuron>();
            IList<IFreeformNeuron> queue = _outputLayer.Neurons.ToList();

            // first copy outputs to queue

            while (queue.Count > 0)
            {
                // pop a neuron off the queue
                IFreeformNeuron neuron = queue[0];
                queue.RemoveAt(0);
                visited.Add(neuron);

                // find anymore neurons and add them to the queue.
                if (neuron.InputSummation != null)
                {
                    foreach (IFreeformConnection connection in neuron
                        .InputSummation.List)
                    {
                        result++;
                        IFreeformNeuron nextNeuron = connection.Source;
                        if (!visited.Contains(nextNeuron))
                        {
                            queue.Add(nextNeuron);
                        }
                    }
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public void EncodeToArray(double[] encoded)
        {
            int index = 0;
            var visited = new HashSet<IFreeformNeuron>();
            IList<IFreeformNeuron> queue = _outputLayer.Neurons.ToList();

            // first copy outputs to queue

            while (queue.Count > 0)
            {
                // pop a neuron off the queue
                IFreeformNeuron neuron = queue[0];
                queue.RemoveAt(0);
                visited.Add(neuron);

                // find anymore neurons and add them to the queue.
                if (neuron.InputSummation != null)
                {
                    foreach (IFreeformConnection connection in neuron
                        .InputSummation.List)
                    {
                        encoded[index++] = connection.Weight;
                        IFreeformNeuron nextNeuron = connection.Source;
                        if (!visited.Contains(nextNeuron))
                        {
                            queue.Add(nextNeuron);
                        }
                    }
                }
            }
        }

        /// <inheritdoc/>
        public double CalculateError(IMLDataSet data)
        {
            return EncogUtility.CalculateRegressionError(this, data);
        }

        /// <inheritdoc/>
        public IMLData Compute(IMLData input)
        {
            // Allocate result
            var result = new BasicMLData(_outputLayer.Count);

            // Copy the input
            for (int i = 0; i < input.Count; i++)
            {
                _inputLayer.SetActivation(i, input[i]);
            }

            // Request calculation of outputs
            for (int i = 0; i < _outputLayer.Count; i++)
            {
                IFreeformNeuron outputNeuron = _outputLayer.Neurons[i];
                outputNeuron.PerformCalculation();
                result[i] = outputNeuron.Activation;
            }

            UpdateContext();

            return result;
        }

        /// <inheritdoc/>
        public int InputCount
        {
            get { return _inputLayer.CountNonBias; }
        }

        /// <inheritdoc/>
        public int OutputCount
        {
            get { return _outputLayer.CountNonBias; }
        }

        /// <inheritdoc/>
        public void Reset()
        {
            Reset((int) (DateTime.Now.Ticks%int.MaxValue));
        }

        /// <inheritdoc/>
        public void Reset(int seed)
        {
            var randomizer = new ConsistentRandomizer(-1, 1,
                seed);

            /**
             * {@inheritDoc}
             */
            PerformConnectionTask(connection => { connection.Weight = randomizer.NextDouble(); });
        }
        
        /// <summary>
        /// Construct an Elmann recurrent neural network.
        /// </summary>
        /// <param name="input">The input count.</param>
        /// <param name="hidden1">The hidden count.</param>
        /// <param name="output">The output count.</param>
        /// <param name="af">The activation function.</param>
        /// <returns>The newly created network.</returns>
        public static FreeformNetwork CreateElman(int input,
            int hidden1, int output, IActivationFunction af)
        {
            var network = new FreeformNetwork();
            IFreeformLayer inputLayer = network.CreateInputLayer(input);
            IFreeformLayer hiddenLayer1 = network.CreateLayer(hidden1);
            IFreeformLayer outputLayer = network.CreateOutputLayer(output);

            network.ConnectLayers(inputLayer, hiddenLayer1, af, 1.0, false);
            network.ConnectLayers(hiddenLayer1, outputLayer, af, 1.0, false);
            network.CreateContext(hiddenLayer1, hiddenLayer1);
            network.Reset();

            return network;
        }

        /// <summary>
        /// Create a feedforward freeform neural network.
        /// </summary>
        /// <param name="input">The input count.</param>
        /// <param name="hidden1">The first hidden layer count, zero if none.</param>
        /// <param name="hidden2">The second hidden layer count, zero if none.</param>
        /// <param name="output">The output count.</param>
        /// <param name="af">The activation function.</param>
        /// <returns>The newly crated network.</returns>
        public static FreeformNetwork CreateFeedforward(int input,
            int hidden1, int hidden2, int output,
            IActivationFunction af)
        {
            var network = new FreeformNetwork();
            IFreeformLayer lastLayer = network.CreateInputLayer(input);
            IFreeformLayer currentLayer;

            if (hidden1 > 0)
            {
                currentLayer = network.CreateLayer(hidden1);
                network.ConnectLayers(lastLayer, currentLayer, af, 1.0, false);
                lastLayer = currentLayer;
            }

            if (hidden2 > 0)
            {
                currentLayer = network.CreateLayer(hidden2);
                network.ConnectLayers(lastLayer, currentLayer, af, 1.0, false);
                lastLayer = currentLayer;
            }

            currentLayer = network.CreateOutputLayer(output);
            network.ConnectLayers(lastLayer, currentLayer, af, 1.0, false);

            network.Reset();

            return network;
        }

        /// <inheritdoc/>
        public object Clone()
        {
            var result = (BasicNetwork) ObjectCloner.DeepCopy(this);
            return result;
        }

        /// <summary>
        /// Connect two layers. These layers will be connected with a TANH activation
        /// function in a non-recurrent way. A bias activation of 1.0 will be used,
        /// if needed. 
        /// </summary>
        /// <param name="source">The source layer.</param>
        /// <param name="target">The target layer.</param>
        public void ConnectLayers(IFreeformLayer source,
            IFreeformLayer target)
        {
            ConnectLayers(source, target, new ActivationTANH(), 1.0, false);
        }

        /// <summary>
        /// Connect two layers. 
        /// </summary>
        /// <param name="source">The source layer.</param>
        /// <param name="target">The target layer.</param>
        /// <param name="theActivationFunction">The activation function to use.</param>
        /// <param name="biasActivation">The bias activation to use.</param>
        /// <param name="isRecurrent">True, if this is a recurrent connection.</param>
        public void ConnectLayers(IFreeformLayer source,
            IFreeformLayer target,
            IActivationFunction theActivationFunction,
            double biasActivation, bool isRecurrent)
        {
            // create bias, if requested
            if (biasActivation > EncogFramework.DefaultDoubleEqual)
            {
                // does the source already have a bias?
                if (source.HasBias)
                {
                    throw new FreeformNetworkError(
                        "The source layer already has a bias neuron, you cannot create a second.");
                }
                IFreeformNeuron biasNeuron = _neuronFactory
                    .FactorRegular(null);
                biasNeuron.Activation = biasActivation;
                biasNeuron.IsBias = true;
                source.Add(biasNeuron);
            }

            // create connections
            foreach (IFreeformNeuron targetNeuron in target.Neurons)
            {
                // create the summation for the target
                IInputSummation summation = targetNeuron.InputSummation;

                // do not create a second input summation
                if (summation == null)
                {
                    summation = _summationFactory.Factor(theActivationFunction);
                    targetNeuron.InputSummation = summation;
                }

                // connect the source neurons to the target neuron
                foreach (IFreeformNeuron sourceNeuron in source.Neurons)
                {
                    IFreeformConnection connection = _connectionFactory
                        .Factor(sourceNeuron, targetNeuron);
                    sourceNeuron.AddOutput(connection);
                    targetNeuron.AddInput(connection);
                }
            }
        }

        /// <summary>
        /// Connect two layers, assume bias activation of 1.0 and non-recurrent
        /// connection. 
        /// </summary>
        /// <param name="source">The source layer.</param>
        /// <param name="target">The target layer.</param>
        /// <param name="theActivationFunction">The activation function.</param>
        public void ConnectLayers(IFreeformLayer source,
            IFreeformLayer target,
            IActivationFunction theActivationFunction)
        {
            ConnectLayers(source, target, theActivationFunction, 1.0, false);
        }

        /// <summary>
        /// Connect layers from a BasicNetwork. Used internally only.
        /// </summary>
        /// <param name="network">The BasicNetwork.</param>
        /// <param name="fromLayerIdx">The from layer index.</param>
        /// <param name="source">The from layer.</param>
        /// <param name="target">The target.</param>
        private void ConnectLayersFromBasic(BasicNetwork network,
            int fromLayerIdx, IFreeformLayer source, IFreeformLayer target)
        {
            for (int targetNeuronIdx = 0; targetNeuronIdx < target.Count; targetNeuronIdx++)
            {
                for (int sourceNeuronIdx = 0; sourceNeuronIdx < source.Count; sourceNeuronIdx++)
                {
                    IFreeformNeuron sourceNeuron = source.Neurons[sourceNeuronIdx];
                    IFreeformNeuron targetNeuron = target.Neurons[targetNeuronIdx];

                    // neurons with no input (i.e. bias neurons)
                    if (targetNeuron.InputSummation == null)
                    {
                        continue;
                    }

                    IFreeformConnection connection = _connectionFactory
                        .Factor(sourceNeuron, targetNeuron);
                    sourceNeuron.AddOutput(connection);
                    targetNeuron.AddInput(connection);
                    double weight = network.GetWeight(fromLayerIdx,
                        sourceNeuronIdx, targetNeuronIdx);
                    connection.Weight = weight;
                }
            }
        }


        /// <summary>
        /// Create a context connection, such as those used by Jordan/Elmann.
        /// </summary>
        /// <param name="source">The source layer.</param>
        /// <param name="target">The target layer.</param>
        /// <returns>The newly created context layer.</returns>
        public IFreeformLayer CreateContext(IFreeformLayer source,
            IFreeformLayer target)
        {
            const double biasActivation = 0.0;

            if (source.Neurons[0].Outputs.Count < 1)
            {
                throw new FreeformNetworkError(
                    "A layer cannot have a context layer connected if there are no other outbound connections from the source layer.  Please connect the source layer somewhere else first.");
            }

            IActivationFunction activatonFunction = source.Neurons[0].InputSummation
                .ActivationFunction;

            // first create the context layer
            IFreeformLayer result = _layerFactory.Factor();

            for (int i = 0; i < source.Count; i++)
            {
                IFreeformNeuron neuron = source.Neurons[i];
                if (neuron.IsBias)
                {
                    IFreeformNeuron biasNeuron = _neuronFactory
                        .FactorRegular(null);
                    biasNeuron.IsBias = true;
                    biasNeuron.Activation = neuron.Activation;
                    result.Add(biasNeuron);
                }
                else
                {
                    IFreeformNeuron contextNeuron = _neuronFactory
                        .FactorContext(neuron);
                    result.Add(contextNeuron);
                }
            }

            // now connect the context layer to the target layer

            ConnectLayers(result, target, activatonFunction, biasActivation, false);

            return result;
        }

        /// <summary>
        /// Create the input layer. 
        /// </summary>
        /// <param name="neuronCount">The input neuron count.</param>
        /// <returns>The newly created layer.</returns>
        public IFreeformLayer CreateInputLayer(int neuronCount)
        {
            if (neuronCount < 1)
            {
                throw new FreeformNetworkError(
                    "Input layer must have at least one neuron.");
            }
            _inputLayer = CreateLayer(neuronCount);
            return _inputLayer;
        }

        /// <summary>
        /// Create a hidden layer. 
        /// </summary>
        /// <param name="neuronCount">The neuron count.</param>
        /// <returns>The newly created layer.</returns>
        public IFreeformLayer CreateLayer(int neuronCount)
        {
            if (neuronCount < 1)
            {
                throw new FreeformNetworkError(
                    "Layer must have at least one neuron.");
            }

            IFreeformLayer result = _layerFactory.Factor();

            // Add the neurons for this layer
            for (int i = 0; i < neuronCount; i++)
            {
                result.Add(_neuronFactory.FactorRegular(null));
            }

            return result;
        }

        /// <summary>
        /// Create the output layer. 
        /// </summary>
        /// <param name="neuronCount">The neuron count.</param>
        /// <returns>The newly created output layer.</returns>
        public IFreeformLayer CreateOutputLayer(int neuronCount)
        {
            if (neuronCount < 1)
            {
                throw new FreeformNetworkError(
                    "Output layer must have at least one neuron.");
            }
            _outputLayer = CreateLayer(neuronCount);
            return _outputLayer;
        }

        /// <summary>
        /// Perform the specified connection task. This task will be performed over
        /// all connections. 
        /// </summary>
        /// <param name="task">The connection task.</param>
        public void PerformConnectionTask(FreeformConnectionTask task)
        {
            var visited = new HashSet<IFreeformNeuron>();

            foreach (IFreeformNeuron neuron in _outputLayer.Neurons)
            {
                PerformConnectionTask(visited, neuron, task);
            }
        }


        /// <summary>
        /// Perform the specified connection task. 
        /// </summary>
        /// <param name="visited">The list of visited neurons.</param>
        /// <param name="parentNeuron"></param>
        /// <param name="task"></param>
        private void PerformConnectionTask(HashSet<IFreeformNeuron> visited,
            IFreeformNeuron parentNeuron, FreeformConnectionTask task)
        {
            visited.Add(parentNeuron);

            // does this neuron have any inputs?
            if (parentNeuron.InputSummation != null)
            {
                // visit the inputs
                foreach (IFreeformConnection connection in parentNeuron
                    .InputSummation.List)
                {
                    task(connection);
                    IFreeformNeuron neuron = connection.Source;
                    // have we already visited this neuron?
                    if (!visited.Contains(neuron))
                    {
                        PerformConnectionTask(visited, neuron, task);
                    }
                }
            }
        }
        
        /// <summary>
        /// Perform the specified neuron task. This task will be executed over all
        /// neurons. 
        /// </summary>
        /// <param name="task">The task to perform.</param>
        public void PerformNeuronTask(FreeformNeuronTask task)
        {
            var visited = new HashSet<IFreeformNeuron>();

            foreach (IFreeformNeuron neuron in _outputLayer.Neurons)
            {
                PerformNeuronTask(visited, neuron, task);
            }
        }

        /// <summary>
        /// Perform the specified neuron task.
        /// </summary>
        /// <param name="visited">The visited list.</param>
        /// <param name="parentNeuron">The neuron to start with.</param>
        /// <param name="task">The task to perform.</param>
        private void PerformNeuronTask(HashSet<IFreeformNeuron> visited,
            IFreeformNeuron parentNeuron, FreeformNeuronTask task)
        {
            visited.Add(parentNeuron);
            task(parentNeuron);

            // does this neuron have any inputs?
            if (parentNeuron.InputSummation != null)
            {
                // visit the inputs
                foreach (IFreeformConnection connection in parentNeuron
                    .InputSummation.List)
                {
                    IFreeformNeuron neuron = connection.Source;
                    // have we already visited this neuron?
                    if (!visited.Contains(neuron))
                    {
                        PerformNeuronTask(visited, neuron, task);
                    }
                }
            }
        }

        /// <summary>
        ///  Allocate temp training space. 
        /// </summary>
        /// <param name="neuronSize">The number of elements to allocate on each neuron.</param>
        /// <param name="connectionSize">The number of elements to allocate on each connection.</param>
        public void TempTrainingAllocate(int neuronSize,
            int connectionSize)
        {
            PerformNeuronTask(neuron =>
            {
                neuron.AllocateTempTraining(neuronSize);
                if (neuron.InputSummation != null)
                {
                    foreach (IFreeformConnection connection in neuron
                        .InputSummation.List)
                    {
                        connection.AllocateTempTraining(connectionSize);
                    }
                }
            });
        }

        /// <summary>
        /// Clear the temp training data.
        /// </summary>
        public void TempTrainingClear()
        {
            PerformNeuronTask(neuron =>
            {
                neuron.ClearTempTraining();
                if (neuron.InputSummation != null)
                {
                    foreach (IFreeformConnection connection in neuron
                        .InputSummation.List)
                    {
                        connection.ClearTempTraining();
                    }
                }
            });
        }

        /// <inheritdoc/>
        public void UpdateContext()
        {
            PerformNeuronTask(neuron => neuron.UpdateContext());
        }

        /// <inheritdoc/>
        public override void UpdateProperties()
        {
            // not needed
        }
    }
}
