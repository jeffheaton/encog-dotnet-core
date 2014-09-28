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
        private readonly double[] _allValues;

        /// <summary>
        /// The ranges of the bias values.
        /// </summary>
        ///
        private readonly NumericRange _bias;

        /// <summary>
        /// The bias values in the neural network.
        /// </summary>
        ///
        private readonly double[] _biasValues;

        /// <summary>
        /// The number of disabled connections.
        /// </summary>
        ///
        private readonly int _disabledConnections;

        /// <summary>
        /// The total number of connections.
        /// </summary>
        ///
        private readonly int _totalConnections;

        /// <summary>
        /// The weight values in the neural network.
        /// </summary>
        ///
        private readonly double[] _weightValues;

        /// <summary>
        /// The ranges of the weights.
        /// </summary>
        ///
        private readonly NumericRange _weights;

        /// <summary>
        /// The ranges of both the weights and biases.
        /// </summary>
        ///
        private readonly NumericRange _weightsAndBias;

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

                        if (network.Structure.ConnectionLimited )
                        {
                            if (Math.Abs(v) < network.Structure.ConnectionLimit )
                            {
                                assignDisabled++;
                            }
                        }

                        weightList.Add(v);
                        allList.Add(v);
                        assignedTotal++;
                    }
                }

                // bias
                if (fromCount != fromBiasCount)
                {
                    int biasNeuron = fromCount;
                    for (int toNeuron = 0; toNeuron < toCount; toNeuron++)
                    {
                        double v = network.GetWeight(layerNumber, biasNeuron,
                                                       toNeuron);
                        if (network.Structure.ConnectionLimited)
                        {
                            if (Math.Abs(v) < network.Structure.ConnectionLimit)
                            {
                                assignDisabled++;
                            }
                        }

                        biasList.Add(v);
                        allList.Add(v);
                        assignedTotal++;
                    }
                }
            }

            _disabledConnections = assignDisabled;
            _totalConnections = assignedTotal;
            _weights = new NumericRange(weightList);
            _bias = new NumericRange(biasList);
            _weightsAndBias = new NumericRange(allList);
            _weightValues = EngineArray.ListToDouble(weightList);
            _allValues = EngineArray.ListToDouble(allList);
            _biasValues = EngineArray.ListToDouble(biasList);
        }


        /// <value>All of the values in the neural network.</value>
        public double[] AllValues
        {
            get { return _allValues; }
        }


        /// <value>The numeric range of the bias values.</value>
        public NumericRange Bias
        {
            get { return _bias; }
        }


        /// <value>The bias values in the neural network.</value>
        public double[] BiasValues
        {
            get { return _biasValues; }
        }


        /// <value>The number of disabled connections in the network.</value>
        public int DisabledConnections
        {
            get { return _disabledConnections; }
        }


        /// <value>The total number of connections in the network.</value>
        public int TotalConnections
        {
            get { return _totalConnections; }
        }


        /// <value>The numeric range of the weights values.</value>
        public NumericRange Weights
        {
            get { return _weights; }
        }


        /// <value>The numeric range of the weights and bias values.</value>
        public NumericRange WeightsAndBias
        {
            get { return _weightsAndBias; }
        }


        /// <value>The weight values in the neural network.</value>
        public double[] WeightValues
        {
            get { return _weightValues; }
        }


        /// <inheritdoc/>
        public override sealed String ToString()
        {
            var result = new StringBuilder();
            result.Append("All Values : ");
            result.Append((_weightsAndBias.ToString()));
            result.Append("\n");
            result.Append("Bias : ");
            result.Append((_bias.ToString()));
            result.Append("\n");
            result.Append("Weights    : ");
            result.Append((_weights.ToString()));
            result.Append("\n");
            result.Append("Disabled   : ");
            result.Append(Format.FormatInteger(_disabledConnections));
            result.Append("\n");
            return result.ToString();
        }
    }
}
