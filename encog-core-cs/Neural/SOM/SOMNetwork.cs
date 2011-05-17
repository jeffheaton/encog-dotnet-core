using Encog.MathUtil.Matrices;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.SOM.Training.Neighborhood;
using Encog.Util;

namespace Encog.Neural.SOM
{
    /// <summary>
    /// A self organizing map neural network.
    /// </summary>
    ///
    public class SOMNetwork : BasicML, MLClassification, MLResettable,
                              MLError
    {
        /// <summary>
        /// Do not allow patterns to go below this very small number.
        /// </summary>
        ///
        public const double VERYSMALL = 1.0E-30d;

        /// <summary>
        /// Number of input neurons.
        /// </summary>
        ///
        protected internal int inputNeuronCount;

        /// <summary>
        /// Number of output neurons.
        /// </summary>
        ///
        protected internal int outputNeuronCount;

        /// <summary>
        /// The weights of the output neurons base on the input from the input
        /// neurons.
        /// </summary>
        ///
        private Matrix weights;

        /// <summary>
        /// Default constructor.
        /// </summary>
        ///
        public SOMNetwork()
        {
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        ///
        /// <param name="inputCount">Number of input neurons</param>
        /// <param name="outputCount">Number of output neurons</param>
        public SOMNetwork(int inputCount, int outputCount)
        {
            inputNeuronCount = inputCount;
            outputNeuronCount = outputCount;
            weights = new Matrix(inputCount, outputCount);
        }


        /// <summary>
        /// Get the input neuron count.
        /// </summary>
        ///
        /// <value>The input neuron count.</value>
        public int InputNeuronCount
        {
            /// <summary>
            /// Get the input neuron count.
            /// </summary>
            ///
            /// <returns>The input neuron count.</returns>
            get { return inputNeuronCount; }
        }


        /// <summary>
        /// Set the output count.
        /// </summary>
        ///
        /// <value>The output count.</value>
        public int OutputNeuronCount
        {
            /// <summary>
            /// Get the output neuron count.
            /// </summary>
            ///
            /// <returns>The output neuron count.</returns>
            get { return outputNeuronCount; }
            /// <summary>
            /// Set the output count.
            /// </summary>
            ///
            /// <param name="i">The output count.</param>
            set { outputNeuronCount = value; }
        }


        /// <value>the weights to set</value>
        public Matrix Weights
        {
            /// <returns>the weights</returns>
            get { return weights; }
            /// <param name="weights_0">the weights to set</param>
            set { weights = value; }
        }

        #region MLClassification Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public int Classify(MLData input)
        {
            MLData result = Compute(input);
            return EngineArray.MaxIndex(result.Data);
        }

        /// <summary>
        /// Set the input count.
        /// </summary>
        ///
        /// <value>The input count.</value>
        public virtual int InputCount
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return inputNeuronCount; }
            /// <summary>
            /// Set the input count.
            /// </summary>
            ///
            /// <param name="i">The input count.</param>
            set { inputNeuronCount = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual int OutputCount
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return 1; }
        }

        #endregion

        #region MLError Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public double CalculateError(MLDataSet data)
        {
            var bmu = new BestMatchingUnit(this);

            bmu.Reset();


            // Determine the BMU foreach each training element.
            foreach (MLDataPair pair  in  data)
            {
                MLData input = pair.Input;
                bmu.CalculateBMU(input);
            }

            // update the error
            return bmu.WorstDistance/100.0d;
        }

        #endregion

        #region MLResettable Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public void Reset()
        {
            weights.Randomize(-1, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public void Reset(int seed)
        {
            Reset();
        }

        #endregion

        /// <summary>
        /// Determine the winner for the specified input. This is the number of the
        /// winning neuron.
        /// </summary>
        ///
        /// <param name="input">The input pattern.</param>
        /// <returns>The winning neuron.</returns>
        public MLData Compute(MLData input)
        {
            MLData result = new BasicMLData(outputNeuronCount);

            for (int i = 0; i < outputNeuronCount; i++)
            {
                Matrix optr = weights.GetCol(i);
                Matrix inputMatrix = Matrix.CreateRowMatrix(input.Data);
                result[i] = MatrixMath.DotProduct(inputMatrix, optr);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed void UpdateProperties()
        {
            // unneeded
        }

        /// <summary>
        /// Determine the winner for the specified input. This is the number of the
        /// winning neuron.
        /// </summary>
        ///
        /// <param name="input">The input pattern.</param>
        /// <returns>The winning neuron.</returns>
        public int Winner(MLData input)
        {
            MLData output = Compute(input);
            int win = EngineArray.IndexOfLargest(output.Data);
            return win;
        }
    }
}