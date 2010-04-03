using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks.Layers;
using Encog.Util;
using Encog.Neural.Data;
using Encog.Neural.Data.Basic;

namespace Encog.Neural.Networks.Synapse
{
    /// <summary>
    /// A synapse where only some of the neurons are connected. Works like a normal
    /// weighted synapse, except that a map is kept to determine if connections are
    /// enabled between the two layers.
    /// </summary>
    public class PartialSynapse : WeightedSynapse
    {
        /// <summary>
        /// A map of which connections are enabled.
        /// </summary>
        private bool[][] EnabledMap { get; set; }


        /// <summary>
        /// Construct a partial synapse.
        /// </summary>
        /// <param name="inputLayer">The input layer.</param>
        /// <param name="outputLayer">The output layer.</param>
        public PartialSynapse(ILayer inputLayer, ILayer outputLayer)
            : base(inputLayer, outputLayer)
        {

            EnabledMap = EncogArray.AllocateBool2D(inputLayer.NeuronCount, outputLayer.NeuronCount);

            // default everything to "enabled"
            for (int i = 0; i < EnabledMap.Length; i++)
            {
                for (int j = 0; j < EnabledMap[0].Length; i++)
                {
                    this.EnabledMap[i][j] = true;
                }
            }
        }

    
        /// <summary>
        /// Enable or disable a connection.
        /// </summary>
        /// <param name="fromNeuron">The from neuron.</param>
        /// <param name="toNeuron">The to neuron.</param>
        /// <param name="enabled">True to enable, false to disable.</param>
        public void EnableConnection(int fromNeuron, int toNeuron, bool enabled)
        {
            EnabledMap[fromNeuron][toNeuron] = enabled;
        }


        /// <summary>
        /// Compute the weighted output from this synapse. Each neuron in the from
        /// layer has a weighted connection to each of the neurons in the next layer.
        /// 
        /// Only connections that are "enabled" between the two layers are used.  If
        /// a connection is not "enabled", the weight will be set to zero.
        /// </summary>
        /// <param name="input">The input from the synapse.</param>
        /// <returns>The output from this synapse.</returns>
        public override INeuralData Compute(INeuralData input)
        {
            INeuralData result = new BasicNeuralData(ToNeuronCount);

            double[] inputArray = input.Data;
            double[][] matrixArray = WeightMatrix.Data;
            double[] resultArray = result.Data;

            for (int i = 0; i < ToNeuronCount; i++)
            {
                double sum = 0;
                for (int j = 0; j < inputArray.Length; j++)
                {
                    if (this.EnabledMap[j][i])
                        sum += inputArray[j] * matrixArray[j][i];
                    else
                        matrixArray[j][i] = 0;
                }
                resultArray[i] = sum;
            }
            return result;
        }

        /// <summary>
        /// The type of synapse this is.
        /// </summary>
        public SynapseType Type
        {
            get
            {
                return SynapseType.Partial;
            }
        }

    }
}
