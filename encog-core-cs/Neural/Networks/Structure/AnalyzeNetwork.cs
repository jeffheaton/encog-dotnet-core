// Encog(tm) Artificial Intelligence Framework v2.5
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
using Encog.MathUtil;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Synapse;
using Encog.Util;
using Encog.Engine.Util;

namespace Encog.Neural.Networks.Structure
{
    /// <summary>
    /// Allows the weights and bias values of the neural network to be analyzed.
    /// </summary>
    public class AnalyzeNetwork
    {
        /// <summary>
        /// The ranges of the weights.
        /// </summary>
        private NumericRange weights;

        /// <summary>
        /// The ranges of the bias values.
        /// </summary>
        private NumericRange bias;

        /// <summary>
        /// The ranges of both the weights and biases.
        /// </summary>
        private NumericRange weightsAndBias;

        /// <summary>
        /// The number of disabled connections.
        /// </summary>
        private int disabledConnections;

        /// <summary>
        /// The total number of connections.
        /// </summary>
        private int totalConnections;

        /// <summary>
        /// All of the values in the neural network.
        /// </summary>
        private double[] allValues;

        /// <summary>
        /// The weight values in the neural network.
        /// </summary>
        private double[] weightValues;

        /// <summary>
        /// The bias values in the neural network.
        /// </summary>
        private double[] biasValues;

        /// <summary>
        /// Construct a network analyze class.  Analyze the specified network.
        /// </summary>
        /// <param name="network">The network to analyze.</param>
        public AnalyzeNetwork(BasicNetwork network)
        {
            int assignDisabled = 0;
            int assignedTotal = 0;
            IList<double> biasList = new List<double>();
            IList<double> weightList = new List<double>();
            IList<double> allList = new List<double>();

            foreach (ILayer layer in network.Structure.Layers)
            {
                if (layer.HasBias)
                {
                    for (int i = 0; i < layer.NeuronCount; i++)
                    {
                        biasList.Add(layer.BiasWeights[i]);
                        allList.Add(layer.BiasWeights[i]);
                    }
                }
            }

            foreach (ISynapse synapse in network.Structure.Synapses)
            {
                if (synapse.MatrixSize > 0)
                {
                    for (int from = 0; from < synapse.FromNeuronCount; from++)
                    {
                        for (int to = 0; to < synapse.ToNeuronCount; to++)
                        {
                            if (network.IsConnected(synapse, from, to))
                            {
                                double d = synapse.WeightMatrix[from, to];
                                weightList.Add(d);
                                allList.Add(d);
                            }
                            else
                            {
                                assignDisabled++;
                            }
                            assignedTotal++;
                        }
                    }
                }
            }

            this.disabledConnections = assignDisabled;
            this.totalConnections = assignedTotal;
            this.weights = new NumericRange(weightList);
            this.bias = new NumericRange(biasList);
            this.weightsAndBias = new NumericRange(allList);
            this.weightValues = EngineArray.ListToDouble(weightList);
            this.allValues = EngineArray.ListToDouble(allList);
            this.biasValues = EngineArray.ListToDouble(biasList);
        }

        /// <summary>
        /// The network analysis as a string.
        /// </summary>
        /// <returns>The network analysis as a string.</returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("All Values : ");
            result.Append(this.weightsAndBias.ToString());
            result.Append("\n");
            result.Append("Bias : ");
            result.Append(this.bias.ToString());
            result.Append("\n");
            result.Append("Weights    : ");
            result.Append(this.weights.ToString());
            result.Append("\n");
            result.Append("Disabled   : ");
            result.Append(Format.FormatInteger(this.disabledConnections));
            result.Append("\n");
            return result.ToString();
        }

        /// <summary>
        /// The numeric range of the weights values.
        /// </summary>
        public NumericRange Weights
        {
            get
            {
                return weights;
            }
        }


        /// <summary>
        /// The numeric range of the bias values.
        /// </summary>
        public NumericRange Bias
        {
            get
            {
                return bias;
            }
        }

        /// <summary>
        /// The numeric range of the weights and bias values.
        /// </summary>
        public NumericRange WeightsAndBias
        {
            get
            {
                return weightsAndBias;
            }
        }

        /// <summary>
        /// The number of disabled connections in the network.
        /// </summary>
        public int DisabledConnections
        {
            get
            {
                return disabledConnections;
            }
        }

        /// <summary>
        /// The total number of connections in the network.
        /// </summary>
        public int TotalConnections
        {
            get
            {
                return totalConnections;
            }
        }

        /// <summary>
        /// All of the values in the neural network.
        /// </summary>
        public double[] AllValues
        {
            get
            {
                return allValues;
            }
        }

        /// <summary>
        /// The weight values in the neural network.
        /// </summary>
        public double[] WeightValues
        {
            get
            {
                return weightValues;
            }
        }

        /// <summary>
        /// The bias values in the neural network.
        /// </summary>
        public double[] BiasValues
        {
            get
            {
                return biasValues;
            }
        }
    }
}
