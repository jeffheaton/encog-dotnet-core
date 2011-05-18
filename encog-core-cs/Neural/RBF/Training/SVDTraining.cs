using Encog.MathUtil.RBF;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Train;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Neural.RBF;
using Encog.Neural.RBF.Training;
using Encog.Util;
using Encog.Util.Simple;

namespace Encog.Neural.Rbf.Training
{
    /// <summary>
    /// Train a RBF neural network using a SVD.
    /// Contributed to Encog By M.Fletcher and M.Dean University of Cambridge, Dept.
    /// of Physics, UK
    /// </summary>
    ///
    public class SVDTraining : BasicTraining
    {
        /// <summary>
        /// The network that is to be trained.
        /// </summary>
        ///
        private readonly RBFNetwork network;

        /// <summary>
        /// Construct the training object.
        /// </summary>
        ///
        /// <param name="network_0">The network to train. Must have a single output neuron.</param>
        /// <param name="training">The training data to use. Must be indexable.</param>
        public SVDTraining(RBFNetwork network_0, MLDataSet training) : base(TrainingImplementationType.OnePass)
        {
            if (network_0.OutputCount != 1)
            {
                throw new TrainingError(
                    "SVD requires an output layer with a single neuron.");
            }

            Training = training;
            network = network_0;
        }

        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override MLMethod Method
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return network; }
        }

        /// <summary>
        /// Convert a flat network to a matrix.
        /// </summary>
        /// <param name="flat">The flat network to convert.</param>
        /// <param name="start">The starting point.</param>
        /// <param name="matrix">The matrix to convert to.</param>
        public void FlatToMatrix(double[] flat, int start,
                                 double[][] matrix)
        {
            int rows = matrix.Length;
            int cols = matrix[0].Length;

            int index = start;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    matrix[r][c] = flat[index++];
                }
            }
        }


        /// <summary>
        /// Perform one iteration.
        /// </summary>
        ///
        public override sealed void Iteration()
        {
            int length = network.RBF.Length;

            var funcs = new IRadialBasisFunction[length];

            // Iteration over neurons and determine the necessaries
            for (int i = 0; i < length; i++)
            {
                IRadialBasisFunction basisFunc = network.RBF[i];

                funcs[i] = basisFunc;

                // This is the value that is changed using other training methods.
                // weights[i] =
                // network.Structure.Synapses[0].WeightMatrix.Data[i][j];
            }

            ObjectPair<double[][], double[][]> data = TrainingSetUtil
                .TrainingToArray(Training);

            double[][] matrix = EngineArray.AllocateDouble2D(length, network.OutputCount);

            FlatToMatrix(network.Flat.Weights, 0, matrix);
            Error = SVD.Svdfit(data.A, data.B, matrix, funcs);
            MatrixToFlat(matrix, network.Flat.Weights, 0);
        }

        /// <summary>
        /// Convert the matrix to flat.
        /// </summary>
        ///
        /// <param name="matrix">The matrix.</param>
        /// <param name="flat">Flat array.</param>
        /// <param name="start">WHere to start.</param>
        public void MatrixToFlat(double[][] matrix, double[] flat,
                                 int start)
        {
            int rows = matrix.Length;
            int cols = matrix[0].Length;

            int index = start;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    flat[index++] = matrix[r][c];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override TrainingContinuation Pause()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override void Resume(TrainingContinuation state)
        {
        }
    }
}