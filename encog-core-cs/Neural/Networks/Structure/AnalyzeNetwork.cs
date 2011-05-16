using System;
using System.Collections.Generic;
using System.Text;
using Encog.MathUtil;
using Encog.Neural.Networks.Layers;
using Encog.Util;

namespace Encog.Neural.Networks.Structure
{
    /// <summary>
    /// Allows the weights and bias values of the neural network to be analyzed.
    /// </summary>
    ///
    public class AnalyzeNetwork
    {
        /// <summary>
        /// All of the values in the neural network.
        /// </summary>
        ///
        private readonly double[] allValues;

        /// <summary>
        /// The ranges of the bias values.
        /// </summary>
        ///
        private readonly NumericRange bias;

        /// <summary>
        /// The bias values in the neural network.
        /// </summary>
        ///
        private readonly double[] biasValues;

        /// <summary>
        /// The number of disabled connections.
        /// </summary>
        ///
        private readonly int disabledConnections;

        /// <summary>
        /// The total number of connections.
        /// </summary>
        ///
        private readonly int totalConnections;

        /// <summary>
        /// The weight values in the neural network.
        /// </summary>
        ///
        private readonly double[] weightValues;

        /// <summary>
        /// The ranges of the weights.
        /// </summary>
        ///
        private readonly NumericRange weights;

        /// <summary>
        /// The ranges of both the weights and biases.
        /// </summary>
        ///
        private readonly NumericRange weightsAndBias;

        /// <summary>
        /// Construct a network analyze class. Analyze the specified network.
        /// </summary>
        ///
        /// <param name="network">The network to analyze.</param>
        public AnalyzeNetwork(BasicNetwork network)
        {
            int assignDisabled = 0;
            int assignedTotal = 0;
            IList<Double> biasList = new List<Double>();
            IList<Double> weightList = new List<Double>();
            IList<Double> allList = new List<Double>();

            for (int layerNumber = 0; layerNumber < network.LayerCount - 1; layerNumber++)
            {
                int fromCount = network.GetLayerNeuronCount(layerNumber);
                int fromBiasCount = network
                    .GetLayerTotalNeuronCount(layerNumber);
                int toCount = network.GetLayerNeuronCount(layerNumber + 1);

                // weights
                for (int fromNeuron = 0; fromNeuron < fromCount; fromNeuron++)
                {
                    for (int toNeuron = 0; toNeuron < toCount; toNeuron++)
                    {
                        double v = network.GetWeight(layerNumber, fromNeuron,
                                                     toNeuron);
                        weightList.Add(v);
                        allList.Add(v);
                    }
                }

                // bias
                if (fromCount != fromBiasCount)
                {
                    int biasNeuron = fromCount;
                    for (int toNeuron_0 = 0; toNeuron_0 < toCount; toNeuron_0++)
                    {
                        double v_1 = network.GetWeight(layerNumber, biasNeuron,
                                                       toNeuron_0);
                        biasList.Add(v_1);
                        allList.Add(v_1);
                    }
                }
            }


            foreach (Layer layer  in  network.Structure.Layers)
            {
                if (layer.HasBias())
                {
                    for (int i = 0; i < layer.NeuronCount; i++)
                    {
                    }
                }
            }

            disabledConnections = assignDisabled;
            totalConnections = assignedTotal;
            weights = new NumericRange(weightList);
            bias = new NumericRange(biasList);
            weightsAndBias = new NumericRange(allList);
            weightValues = EngineArray.ListToDouble(weightList);
            allValues = EngineArray.ListToDouble(allList);
            biasValues = EngineArray.ListToDouble(biasList);
        }


        /// <value>All of the values in the neural network.</value>
        public double[] AllValues
        {
            /// <returns>All of the values in the neural network.</returns>
            get { return allValues; }
        }


        /// <value>The numeric range of the bias values.</value>
        public NumericRange Bias
        {
            /// <returns>The numeric range of the bias values.</returns>
            get { return bias; }
        }


        /// <value>The bias values in the neural network.</value>
        public double[] BiasValues
        {
            /// <returns>The bias values in the neural network.</returns>
            get { return biasValues; }
        }


        /// <value>The number of disabled connections in the network.</value>
        public int DisabledConnections
        {
            /// <returns>The number of disabled connections in the network.</returns>
            get { return disabledConnections; }
        }


        /// <value>The total number of connections in the network.</value>
        public int TotalConnections
        {
            /// <returns>The total number of connections in the network.</returns>
            get { return totalConnections; }
        }


        /// <value>The numeric range of the weights values.</value>
        public NumericRange Weights
        {
            /// <returns>The numeric range of the weights values.</returns>
            get { return weights; }
        }


        /// <value>The numeric range of the weights and bias values.</value>
        public NumericRange WeightsAndBias
        {
            /// <returns>The numeric range of the weights and bias values.</returns>
            get { return weightsAndBias; }
        }


        /// <value>The weight values in the neural network.</value>
        public double[] WeightValues
        {
            /// <returns>The weight values in the neural network.</returns>
            get { return weightValues; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed String ToString()
        {
            var result = new StringBuilder();
            result.Append("All Values : ");
            result.Append((weightsAndBias.ToString()));
            result.Append("\n");
            result.Append("Bias : ");
            result.Append((bias.ToString()));
            result.Append("\n");
            result.Append("Weights    : ");
            result.Append((weights.ToString()));
            result.Append("\n");
            result.Append("Disabled   : ");
            result.Append(Format.FormatInteger(disabledConnections));
            result.Append("\n");
            return result.ToString();
        }
    }
}