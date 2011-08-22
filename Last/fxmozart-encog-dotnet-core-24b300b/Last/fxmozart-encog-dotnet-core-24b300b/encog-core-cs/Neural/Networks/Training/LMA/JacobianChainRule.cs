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
    public class JacobianChainRule : IComputeJacobian
    {
        /// <summary>
        /// THe training set to use. Must be indexable.
        /// </summary>
        ///
        private readonly IMLDataSet _indexableTraining;

        /// <summary>
        /// The number of training set elements.
        /// </summary>
        ///
        private readonly int _inputLength;

        /// <summary>
        /// The Jacobian matrix that was calculated.
        /// </summary>
        ///
        private readonly double[][] _jacobian;

        /// <summary>
        /// The network that is to be trained.
        /// </summary>
        ///
        private readonly BasicNetwork _network;

        /// <summary>
        /// Used to read the training data.
        /// </summary>
        ///
        private readonly IMLDataPair _pair;

        /// <summary>
        /// The number of weights and bias values in the neural network.
        /// </summary>
        ///
        private readonly int _parameterSize;

        /// <summary>
        /// The errors for each row in the Jacobian.
        /// </summary>
        ///
        private readonly double[] _rowErrors;

        /// <summary>
        /// The current column in the Jacobian matrix.
        /// </summary>
        ///
        private int _jacobianCol;

        /// <summary>
        /// The current row in the Jacobian matrix.
        /// </summary>
        ///
        private int _jacobianRow;

        /// <summary>
        /// Construct the chain rule calculation.
        /// </summary>
        ///
        /// <param name="network">The network to use.</param>
        /// <param name="indexableTraining">The training set to use.</param>
        public JacobianChainRule(BasicNetwork network,
                                 IMLDataSet indexableTraining)
        {
            _indexableTraining = indexableTraining;
            _network = network;
            _parameterSize = network.Structure.CalculateSize();
            _inputLength = (int) _indexableTraining.Count;
            _jacobian = EngineArray.AllocateDouble2D(_inputLength, _parameterSize);

            _rowErrors = new double[_inputLength];

            var input = new BasicMLData(
                _indexableTraining.InputSize);
            var ideal = new BasicMLData(
                _indexableTraining.IdealSize);
            _pair = new BasicMLDataPair(input, ideal);
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

            for (int i = 0; i < _inputLength; i++)
            {
                _jacobianRow = i;
                _jacobianCol = 0;

                _indexableTraining.GetRecord(i, _pair);

                double e = CalculateDerivatives(_pair);
                _rowErrors[i] = e;
                result += e*e;
            }

            return result/2.0d;
        }


        /// <value>The Jacobian matrix.</value>
        public virtual double[][] Jacobian
        {
            get { return _jacobian; }
        }


        /// <value>The errors for each row of the Jacobian.</value>
        public virtual double[] RowErrors
        {
            get { return _rowErrors; }
        }

        #endregion

        /// <summary>
        /// Calculate the derivative.
        /// </summary>
        ///
        /// <param name="a">The activation function.</param>
        /// <param name="d">The value to calculate for.</param>
        /// <returns>The derivative.</returns>
        private static double CalcDerivative(IActivationFunction a, double d)
        {
            return a.DerivativeFunction(d,d);
        }

        /// <summary>
        /// Calculate the derivative.
        /// </summary>
        ///
        /// <param name="a">The activation function.</param>
        /// <param name="d">The value to calculate for.</param>
        /// <returns>The derivative.</returns>
        private static double CalcDerivative2(IActivationFunction a, double d)
        {
            var temp = new double[1];
            temp[0] = d;
            a.ActivationFunction(temp, 0, temp.Length);
            temp[0] = a.DerivativeFunction(temp[0],temp[0]);
            return temp[0];
        }

        /// <summary>
        /// Calculate the derivatives for this training set element.
        /// </summary>
        ///
        /// <param name="pair">The training set element.</param>
        /// <returns>The sum squared of errors.</returns>
        private double CalculateDerivatives(IMLDataPair pair)
        {
            // error values
            double e = 0.0d;
            double sum = 0.0d;

            _network.Compute(pair.Input);

            int fromLayer = _network.LayerCount - 2;
            int toLayer = _network.LayerCount - 1;
            int fromNeuronCount = _network.GetLayerTotalNeuronCount(fromLayer);
            int toNeuronCount = _network.GetLayerNeuronCount(toLayer);

            double output = _network.Structure.Flat.LayerOutput[0];
            e = pair.Ideal[0] - output;

            for (int i = 0; i < fromNeuronCount; i++)
            {
                double lastOutput = _network.GetLayerOutput(fromLayer, i);

                _jacobian[_jacobianRow][_jacobianCol++] = CalcDerivative(
                    _network.GetActivation(toLayer), output)*lastOutput;
            }

            while (fromLayer > 0)
            {
                fromLayer--;
                toLayer--;
                fromNeuronCount = _network.GetLayerTotalNeuronCount(fromLayer);
                toNeuronCount = _network.GetLayerNeuronCount(toLayer);

                // this.network.getLayerOutput(fromLayer, neuronNumber) holder.getResult().get(lastSynapse);

                // for each neuron in the input layer
                for (int neuron = 0; neuron < toNeuronCount; neuron++)
                {
                    output = _network.GetLayerOutput(toLayer, neuron);

                    IActivationFunction function = _network.GetActivation(toLayer);

                    double w = _network.GetWeight(toLayer, neuron, 0);
                    double val = CalcDerivative(function, output)
                                 *CalcDerivative2(function, sum)*w;

                    // for each weight of the input neuron
                    for (int i = 0; i < fromNeuronCount; i++)
                    {
                        sum = 0.0d;
                        // for each neuron in the next layer
                        for (int j = 0; j < toNeuronCount; j++)
                        {
                            // for each weight of the next neuron
                            for (int k = 0; k < fromNeuronCount; k++)
                            {
                                sum += _network.GetWeight(fromLayer, k, j)*output;
                            }
                        }

                        _jacobian[_jacobianRow][_jacobianCol++] = val
                                                               *_network.GetLayerOutput(fromLayer, i);
                    }
                }
            }

            // return error
            return e;
        }
    }
}
