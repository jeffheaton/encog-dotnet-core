using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Synapse;

namespace Encog.MathUtil.Randomize
{
    /// <summary>
    /// Implementation of <i>Nguyen-Widrow</i> weight initialization. This is the
    /// default weight initialization used by Encog, as it generally provides the
    /// most trainable neural network.
    /// 
    /// 
    /// author(from Java Version) StŽphan Corriveau
    /// </summary>
    public class NguyenWidrowRandomizer : RangeRandomizer
    {
        /// <summary>
        /// Construct a Nguyen-Widrow randomizer.
        /// </summary>
        /// <param name="min">The min of the range.</param>
        /// <param name="max">The max of the range.</param>
        public NguyenWidrowRandomizer(double min, double max)
            : base(min, max)
        {

        }



        /// <summary>
        /// * The <i>Nguyen-Widrow</i> initialization algorithm is the following :
        // <br>
        // 1. Initialize all weight of hidden layers with (ranged) random values<br>
        // 2. For each hidden layer<br>
        // 2.1 calculate beta value, 0.7 * Nth(#neurons of input layer) root of
        // #neurons of current layer <br>
        // 2.2 for each synapse<br>
        // 2.1.1 for each weight <br>
        // 2.1.2 Adjust weight by dividing by norm of weight for neuron and
        // multiplying by beta value
        /// </summary>
        /// <param name="network">The network to randomize.</param>
        public void Randomize(BasicNetwork network)
        {
            base.Randomize(network);
            int neuronCount = 0;

            foreach (ILayer layer in network.Structure.Layers)
            {
                neuronCount += layer.NeuronCount;
            }

            ILayer inputLayer = network.GetLayer(BasicNetwork.TAG_INPUT);
            ILayer outputLayer = network.GetLayer(BasicNetwork.TAG_OUTPUT);

            if (inputLayer == null)
                throw new EncogError("Must have an input layer for Nguyen-Widrow.");

            if (outputLayer == null)
                throw new EncogError("Must have an output layer for Nguyen-Widrow.");

            int hiddenNeurons = neuronCount - inputLayer.NeuronCount
                    - outputLayer.NeuronCount;

            if (hiddenNeurons < 1)
                throw new EncogError("Must have hidden neurons for Nguyen-Widrow.");

            double beta = 0.7 * Math.Pow(hiddenNeurons, 1.0 / inputLayer
                    .NeuronCount);

            foreach (ISynapse synapse in network.Structure.Synapses)
            {
                Randomize(beta, synapse);
            }

        }

        /// <summary>
        /// Randomize the specified synapse.
        /// </summary>
        /// <param name="beta">The beta value.</param>
        /// <param name="synapse">The synapse to modify.</param>
        private void Randomize(double beta, ISynapse synapse)
        {
            for (int j = 0; j < synapse.ToNeuronCount; j++)
            {
                double norm = 0.0;

                // Calculate the Euclidean Norm for the weights
                for (int k = 0; k < synapse.FromNeuronCount; k++)
                {
                    double v = synapse.WeightMatrix[k, j];
                    norm += v * v;
                }

                double value = synapse.ToLayer.BiasWeights[j];
                norm += value * value;
                norm = Math.Sqrt(norm);

                // Rescale the weights using beta and the norm
                for (int k = 0; k < synapse.FromNeuronCount; k++)
                {
                    value = synapse.WeightMatrix[k, j];
                    synapse.WeightMatrix[k, j] = beta * value / norm;
                }

                value = synapse.ToLayer.BiasWeights[j];
                synapse.ToLayer.BiasWeights[j] = beta * value / norm;
            }
        }
    }
}
