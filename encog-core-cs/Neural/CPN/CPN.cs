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
using Encog.MathUtil.Matrices;
using Encog.MathUtil.Randomize;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Util.Simple;

namespace Encog.Neural.CPN
{
    /// <summary>
    /// Counterpropagation Neural Networks (CPN) were developed by Professor 
    /// Robert Hecht-Nielsen in 1987. CPN neural networks are a hybrid neural 
    /// network, employing characteristics of both a feedforward neural 
    /// network and a self-organzing map (SOM). The CPN is composed of 
    /// three layers, the input, the instar and the outstar. The connection 
    /// from the input to the instar layer is competitive, with only one 
    /// neuron being allowed to win. The connection between the instar and 
    /// outstar is feedforward. The layers are trained separately, 
    /// using instar training and outstar training. The CPN network is 
    /// good at regression.
    /// </summary>
    ///
    [Serializable]
    public class CPNNetwork : BasicML, IMLRegression, IMLResettable, IMLError
    {
        /// <summary>
        /// The number of neurons in the input layer.
        /// </summary>
        ///
        private readonly int _inputCount;

        /// <summary>
        /// The number of neurons in the instar, or hidden, layer.
        /// </summary>
        ///
        private readonly int _instarCount;

        /// <summary>
        /// The number of neurons in the outstar, or output, layer.
        /// </summary>
        ///
        private readonly int _outstarCount;

        /// <summary>
        /// The weights from the input to the instar layer.
        /// </summary>
        ///
        private readonly Matrix _weightsInputToInstar;

        /// <summary>
        /// The weights from the instar to the outstar layer.
        /// </summary>
        ///
        private readonly Matrix _weightsInstarToOutstar;

        /// <summary>
        /// The number of winning neurons.
        /// </summary>
        ///
        private readonly int _winnerCount;

        /// <summary>
        /// Construct the counterpropagation neural network.
        /// </summary>
        ///
        /// <param name="theInputCount">The number of input neurons.</param>
        /// <param name="theInstarCount">The number of instar neurons.</param>
        /// <param name="theOutstarCount">The number of outstar neurons.</param>
        /// <param name="theWinnerCount">The winner count.</param>
        public CPNNetwork(int theInputCount, int theInstarCount,
                          int theOutstarCount, int theWinnerCount)
        {
            _inputCount = theInputCount;
            _instarCount = theInstarCount;
            _outstarCount = theOutstarCount;

            _weightsInputToInstar = new Matrix(_inputCount, _instarCount);
            _weightsInstarToOutstar = new Matrix(_instarCount, _outstarCount);
            _winnerCount = theWinnerCount;
        }


        /// <value>The instar count, same as the input count.</value>
        public int InstarCount
        {
            get { return _instarCount; }
        }


        /// <value>The outstar count, same as the output count.</value>
        public int OutstarCount
        {
            get { return _outstarCount; }
        }


        /// <value>The weights between the input and instar.</value>
        public Matrix WeightsInputToInstar
        {
            get { return _weightsInputToInstar; }
        }


        /// <value>The weights between the instar and outstar.</value>
        public Matrix WeightsInstarToOutstar
        {
            get { return _weightsInstarToOutstar; }
        }


        /// <value>The winner count.</value>
        public int WinnerCount
        {
            get { return _winnerCount; }
        }

        #region MLError Members

        /// <summary>
        /// Calculate the error for this neural network.
        /// </summary>
        ///
        /// <param name="data">The training set.</param>
        /// <returns>The error percentage.</returns>
        public double CalculateError(IMLDataSet data)
        {
            return EncogUtility.CalculateRegressionError(this, data);
        }

        #endregion

        #region MLRegression Members

        /// <inheritdoc/>
        public IMLData Compute(IMLData input)
        {
            IMLData temp = ComputeInstar(input);
            return ComputeOutstar(temp);
        }

        /// <inheritdoc/>
        public int InputCount
        {
            get { return _inputCount; }
        }

        /// <inheritdoc/>
        public int OutputCount
        {
            get { return _outstarCount; }
        }

        #endregion

        #region MLResettable Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public void Reset()
        {
            Reset(0);
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public void Reset(int seed)
        {
            var randomize = new ConsistentRandomizer(-1, 1,
                                                     seed);
            randomize.Randomize(_weightsInputToInstar);
            randomize.Randomize(_weightsInstarToOutstar);
        }

        #endregion

        /// <summary>
        /// Compute the instar layer.
        /// </summary>
        ///
        /// <param name="input">The input.</param>
        /// <returns>The output.</returns>
        public IMLData ComputeInstar(IMLData input)
        {
            var result = new BasicMLData(_instarCount);
            int w, i;
            int winner = 0;
            var winners = new bool[_instarCount];

            for (i = 0; i < _instarCount; i++)
            {
                double sum = 0;
                int j;
                for (j = 0; j < _inputCount; j++)
                {
                    sum += _weightsInputToInstar[j, i]*input[j];
                }
                result[i] = sum;
                winners[i] = false;
            }
            double sumWinners = 0;
            for (w = 0; w < _winnerCount; w++)
            {
                double maxOut = Double.MinValue;
                for (i = 0; i < _instarCount; i++)
                {
                    if (!winners[i] && (result[i] > maxOut))
                    {
                        winner = i;
                        maxOut = result[winner];
                    }
                }
                winners[winner] = true;
                sumWinners += result[winner];
            }
            for (i = 0; i < _instarCount; i++)
            {
                if (winners[i]
                    && (Math.Abs(sumWinners) > EncogFramework.DefaultDoubleEqual))
                {
                    result[i] /= sumWinners;
                }
                else
                {
                    result[i] = 0;
                }
            }

            return result;
        }

        /// <summary>
        /// Compute the outstar layer.
        /// </summary>
        ///
        /// <param name="input">The input.</param>
        /// <returns>The output.</returns>
        public IMLData ComputeOutstar(IMLData input)
        {
            var result = new BasicMLData(_outstarCount);

            for (int i = 0; i < _outstarCount; i++)
            {
                double sum = 0;
                for (int j = 0; j < _instarCount; j++)
                {
                    sum += _weightsInstarToOutstar[j, i]*input[j];
                }
                result[i] = sum;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override void UpdateProperties()
        {
            // unneeded
        }
    }
}
