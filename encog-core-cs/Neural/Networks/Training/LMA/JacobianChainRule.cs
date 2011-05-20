//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Util;

namespace Encog.Neural.Networks.Training.Lma
{
    /// <summary>
    /// Calculate the Jacobian using the chain rule.
    /// ----------------------------------------------------
    /// This implementation of the Levenberg Marquardt algorithm is based heavily on code
    /// published in an article by Cesar Roberto de Souza.  The original article can be
    /// found here:
    /// http://crsouza.blogspot.com/2009/11/neural-network-learning-by-levenberg_18.html
    /// Portions of this class are under the following copyright/license.
    /// Copyright 2009 by Cesar Roberto de Souza, Released under the LGPL.
    /// </summary>
    ///
    public class JacobianChainRule : ComputeJacobian
    {
        /// <summary>
        /// THe training set to use. Must be indexable.
        /// </summary>
        ///
        private readonly MLDataSet indexableTraining;

        /// <summary>
        /// The number of training set elements.
        /// </summary>
        ///
        private readonly int inputLength;

        /// <summary>
        /// The Jacobian matrix that was calculated.
        /// </summary>
        ///
        private readonly double[][] jacobian;

        /// <summary>
        /// The network that is to be trained.
        /// </summary>
        ///
        private readonly BasicNetwork network;

        /// <summary>
        /// Used to read the training data.
        /// </summary>
        ///
        private readonly MLDataPair pair;

        /// <summary>
        /// The number of weights and bias values in the neural network.
        /// </summary>
        ///
        private readonly int parameterSize;

        /// <summary>
        /// The errors for each row in the Jacobian.
        /// </summary>
        ///
        private readonly double[] rowErrors;

        /// <summary>
        /// The current column in the Jacobian matrix.
        /// </summary>
        ///
        private int jacobianCol;

        /// <summary>
        /// The current row in the Jacobian matrix.
        /// </summary>
        ///
        private int jacobianRow;

        /// <summary>
        /// Construct the chain rule calculation.
        /// </summary>
        ///
        /// <param name="network_0">The network to use.</param>
        /// <param name="indexableTraining_1">The training set to use.</param>
        public JacobianChainRule(BasicNetwork network_0,
                                 MLDataSet indexableTraining_1)
        {
            indexableTraining = indexableTraining_1;
            network = network_0;
            parameterSize = network_0.Structure.CalculateSize();
            inputLength = (int) indexableTraining.Count;
            jacobian = EngineArray.AllocateDouble2D(inputLength, parameterSize);

            rowErrors = new double[inputLength];

            var input = new BasicMLData(
                indexableTraining.InputSize);
            var ideal = new BasicMLData(
                indexableTraining.IdealSize);
            pair = new BasicMLDataPair(input, ideal);
        }

        #region ComputeJacobian Members

        /// <summary>
        /// Calculate the Jacobian matrix.
        /// </summary>
        ///
        /// <param name="weights">The weights for the neural network.</param>
        /// <returns>The sum squared of the weights.</returns>
        public virtual double Calculate(double[] weights)
        {
            double result = 0.0d;

            for (int i = 0; i < inputLength; i++)
            {
                jacobianRow = i;
                jacobianCol = 0;

                indexableTraining.GetRecord(i, pair);

                double e = CalculateDerivatives(pair);
                rowErrors[i] = e;
                result += e*e;
            }

            return result/2.0d;
        }


        /// <value>The Jacobian matrix.</value>
        public virtual double[][] Jacobian
        {
            get { return jacobian; }
        }


        /// <value>The errors for each row of the Jacobian.</value>
        public virtual double[] RowErrors
        {
            get { return rowErrors; }
        }

        #endregion

        /// <summary>
        /// Calculate the derivative.
        /// </summary>
        ///
        /// <param name="a">The activation function.</param>
        /// <param name="d">The value to calculate for.</param>
        /// <returns>The derivative.</returns>
        private double CalcDerivative(IActivationFunction a, double d)
        {
            return a.DerivativeFunction(d);
        }

        /// <summary>
        /// Calculate the derivative.
        /// </summary>
        ///
        /// <param name="a">The activation function.</param>
        /// <param name="d">The value to calculate for.</param>
        /// <returns>The derivative.</returns>
        private double CalcDerivative2(IActivationFunction a, double d)
        {
            var temp = new double[1];
            temp[0] = d;
            a.ActivationFunction(temp, 0, temp.Length);
            temp[0] = a.DerivativeFunction(temp[0]);
            return temp[0];
        }

        /// <summary>
        /// Calculate the derivatives for this training set element.
        /// </summary>
        ///
        /// <param name="pair_0">The training set element.</param>
        /// <returns>The sum squared of errors.</returns>
        private double CalculateDerivatives(MLDataPair pair_0)
        {
            // error values
            double e = 0.0d;
            double sum = 0.0d;

            network.Compute(pair_0.Input);

            int fromLayer = network.LayerCount - 2;
            int toLayer = network.LayerCount - 1;
            int fromNeuronCount = network.GetLayerTotalNeuronCount(fromLayer);
            int toNeuronCount = network.GetLayerNeuronCount(toLayer);

            double output = network.Structure.Flat.LayerOutput[0];
            e = pair_0.Ideal[0] - output;

            for (int i = 0; i < fromNeuronCount; i++)
            {
                double lastOutput = network.GetLayerOutput(fromLayer, i);

                jacobian[jacobianRow][jacobianCol++] = CalcDerivative(
                    network.GetActivation(toLayer), output)*lastOutput;
            }

            while (fromLayer > 0)
            {
                fromLayer--;
                toLayer--;
                fromNeuronCount = network.GetLayerTotalNeuronCount(fromLayer);
                toNeuronCount = network.GetLayerNeuronCount(toLayer);

                // this.network.getLayerOutput(fromLayer, neuronNumber) holder.getResult().get(lastSynapse);

                // for each neuron in the input layer
                for (int neuron = 0; neuron < toNeuronCount; neuron++)
                {
                    output = network.GetLayerOutput(toLayer, neuron);

                    IActivationFunction function = network.GetActivation(toLayer);

                    double w = network.GetWeight(toLayer, neuron, 0);
                    double val = CalcDerivative(function, output)
                                 *CalcDerivative2(function, sum)*w;

                    // for each weight of the input neuron
                    for (int i_1 = 0; i_1 < fromNeuronCount; i_1++)
                    {
                        sum = 0.0d;
                        // for each neuron in the next layer
                        for (int j = 0; j < toNeuronCount; j++)
                        {
                            // for each weight of the next neuron
                            for (int k = 0; k < fromNeuronCount; k++)
                            {
                                sum += network.GetWeight(fromLayer, k, j)*output;
                            }
                        }

                        jacobian[jacobianRow][jacobianCol++] = val
                                                               *network.GetLayerOutput(fromLayer, i_1);
                    }
                }
            }

            // return error
            return e;
        }
    }
}
