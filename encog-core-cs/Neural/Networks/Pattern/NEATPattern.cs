using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks.Synapse.NEAT;
using Encog.Neural.Activation;
using Encog.Neural.Networks.Layers;

namespace Encog.Neural.Networks.Pattern
{
    /// <summary>
    /// Pattern to create a NEAT style neural network.
    /// </summary>
    public class NEATPattern : INeuralNetworkPattern
    {
        /// <summary>
        /// The number of input neurons to use. Must be set, default to invalid -1
	    /// value.
        /// </summary>
        public int InputNeurons { get; set; }

        /// <summary>
        /// The number of hidden neurons to use. Must be set, default to invalid -1
        /// value.
        /// </summary>
        public int OutputNeurons { get; set; }

        /// <summary>
        /// The NEAT activation function to use.  This is used on every 
        /// </summary>
        public IActivationFunction NEATActivation { get; set; }

        /// <summary>
        /// The activation function that will be placed on the output layer 
        /// from the entire NEAT network.  If you want the output direct from 
        /// the NEAT network, then set this paramater to a linear activation 
        /// function.
        /// </summary>
        public IActivationFunction ActivationFunction { get; set; }

        /// <summary>
        /// Should snapshot be used when calculating the output of the network.  See
        /// the documentation for NEAT Synapse for more info.
        /// </summary>
        public bool Snapshot { get; set; }

        /// <summary>
        /// The NEAT neurons.
        /// </summary>
        private IList<NEATNeuron> neurons = new List<NEATNeuron>();

        /// <summary>
        /// Construct a NEAT pattern.
        /// </summary>
        public NEATPattern()
        {
            InputNeurons = -1;
            OutputNeurons = -1;
        }

        /// <summary>
        /// Add the hidden layer, this should be called once, as a RBF has a single
        /// hidden layer. 
        /// </summary>
        /// <param name="count">The number of neurons in the hidden layer.</param>
        public void AddHiddenLayer(int count)
        {
            throw new NeuralNetworkError("A NEAT network will evolve its hidden layers, do not specify any.");
        }

        /// <summary>
        /// Clear out any hidden neurons.
        /// </summary>
        public void Clear()
        {
        }


        /// <summary>
        /// Generate the RBF network. 
        /// </summary>
        /// <returns>The neural network.</returns>
        public BasicNetwork Generate()
        {

            int y = PatternConst.START_Y;
            BasicLayer inputLayer = new BasicLayer(new ActivationLinear(),
                    false, this.InputNeurons);
            inputLayer.X = PatternConst.START_X;
            inputLayer.Y = y;
            y += PatternConst.INC_Y;
            BasicLayer outputLayer = new BasicLayer(ActivationFunction, false, this.OutputNeurons);
            outputLayer.X = PatternConst.START_X;
            outputLayer.Y = y;
            NEATSynapse synapse = new NEATSynapse(inputLayer, outputLayer,
                    this.neurons, this.NEATActivation, 0);
            synapse.Snapshot = this.Snapshot;
            inputLayer.AddSynapse(synapse);
            BasicNetwork network = new BasicNetwork();
            network.TagLayer(BasicNetwork.TAG_INPUT, inputLayer);
            network.TagLayer(BasicNetwork.TAG_OUTPUT, outputLayer);
            network.Structure.FinalizeStructure();

            return network;


        }

        /// <summary>
        /// The NEAT neurons.
        /// </summary>
        public IList<NEATNeuron> Neurons
        {
            get
            {
                return neurons;
            }
        }

    }
}
