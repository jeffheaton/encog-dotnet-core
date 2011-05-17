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
    public class CPNNetwork : BasicML, MLRegression, MLResettable, MLError
    {
        /// <summary>
        /// The number of neurons in the input layer.
        /// </summary>
        ///
        private readonly int inputCount;

        /// <summary>
        /// The number of neurons in the instar, or hidden, layer.
        /// </summary>
        ///
        private readonly int instarCount;

        /// <summary>
        /// The number of neurons in the outstar, or output, layer.
        /// </summary>
        ///
        private readonly int outstarCount;

        /// <summary>
        /// The weights from the input to the instar layer.
        /// </summary>
        ///
        private readonly Matrix weightsInputToInstar;

        /// <summary>
        /// The weights from the instar to the outstar layer.
        /// </summary>
        ///
        private readonly Matrix weightsInstarToOutstar;

        /// <summary>
        /// The number of winning neurons.
        /// </summary>
        ///
        private readonly int winnerCount;

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
            inputCount = theInputCount;
            instarCount = theInstarCount;
            outstarCount = theOutstarCount;

            weightsInputToInstar = new Matrix(inputCount, instarCount);
            weightsInstarToOutstar = new Matrix(instarCount, outstarCount);
            winnerCount = theWinnerCount;
        }


        /// <value>The instar count, same as the input count.</value>
        public int InstarCount
        {
            /// <returns>The instar count, same as the input count.</returns>
            get { return instarCount; }
        }


        /// <value>The outstar count, same as the output count.</value>
        public int OutstarCount
        {
            /// <returns>The outstar count, same as the output count.</returns>
            get { return outstarCount; }
        }


        /// <value>The weights between the input and instar.</value>
        public Matrix WeightsInputToInstar
        {
            /// <returns>The weights between the input and instar.</returns>
            get { return weightsInputToInstar; }
        }


        /// <value>The weights between the instar and outstar.</value>
        public Matrix WeightsInstarToOutstar
        {
            /// <returns>The weights between the instar and outstar.</returns>
            get { return weightsInstarToOutstar; }
        }


        /// <value>The winner count.</value>
        public int WinnerCount
        {
            /// <returns>The winner count.</returns>
            get { return winnerCount; }
        }

        #region MLError Members

        /// <summary>
        /// Calculate the error for this neural network.
        /// </summary>
        ///
        /// <param name="data">The training set.</param>
        /// <returns>The error percentage.</returns>
        public double CalculateError(MLDataSet data)
        {
            return EncogUtility.CalculateRegressionError(this, data);
        }

        #endregion

        #region MLRegression Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public MLData Compute(MLData input)
        {
            MLData temp = ComputeInstar(input);
            return ComputeOutstar(temp);
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public int InputCount
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return inputCount; }
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public int OutputCount
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return outstarCount; }
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
            randomize.Randomize(weightsInputToInstar);
            randomize.Randomize(weightsInstarToOutstar);
        }

        #endregion

        /// <summary>
        /// Compute the instar layer.
        /// </summary>
        ///
        /// <param name="input">The input.</param>
        /// <returns>The output.</returns>
        public MLData ComputeInstar(MLData input)
        {
            MLData result = new BasicMLData(instarCount);
            int w, i, j;
            double sum, sumWinners, maxOut;
            int winner = 0;
            var winners = new bool[instarCount];

            for (i = 0; i < instarCount; i++)
            {
                sum = 0;
                for (j = 0; j < inputCount; j++)
                {
                    sum += weightsInputToInstar[j, i]*input[j];
                }
                result[i] = sum;
                winners[i] = false;
            }
            sumWinners = 0;
            for (w = 0; w < winnerCount; w++)
            {
                maxOut = Double.MinValue;
                for (i = 0; i < instarCount; i++)
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
            for (i = 0; i < instarCount; i++)
            {
                if (winners[i]
                    && (Math.Abs(sumWinners) > EncogFramework.DEFAULT_DOUBLE_EQUAL))
                {
                    result.Data[i] /= sumWinners;
                }
                else
                {
                    result.Data[i] = 0;
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
        public MLData ComputeOutstar(MLData input)
        {
            MLData result = new BasicMLData(outstarCount);

            double sum = 0;

            for (int i = 0; i < outstarCount; i++)
            {
                sum = 0;
                for (int j = 0; j < instarCount; j++)
                {
                    sum += weightsInstarToOutstar[j, i]*input[j];
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