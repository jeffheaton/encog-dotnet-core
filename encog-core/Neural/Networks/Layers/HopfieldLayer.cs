using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;
using Encog.Neural.NeuralData.Bipolar;
using Encog.Matrix;

namespace Encog.Neural.Networks.Layers
{
    [Serializable]
    public class HopfieldLayer : BasicLayer
    {
        /// <summary>
        /// Construct a hopfield layer of the specified size.
        /// </summary>
        /// <param name="size">The number of neurons in this layer.</param>
        public HopfieldLayer(int size)
            : base(size)
        {
            this.Fire = new BiPolarNeuralData(size);
            this.WeightMatrix = new Matrix.Matrix(size, size);
        }

        /// <summary>
        /// Present a pattern to the neural network and receive the result.
        /// </summary>
        /// <param name="pattern">The pattern to be presented to the neural network.</param>
        /// <returns>The output from the neural network.</returns>
        public override INeuralData Compute(INeuralData pattern)
        {
            // convert the input pattern into a matrix with a single row.
            // also convert the boolean values to bipolar(-1=false, 1=true)
            Matrix.Matrix inputMatrix = Matrix.Matrix.CreateRowMatrix(pattern.Data);

            // Process each value in the pattern
            for (int col = 0; col < pattern.Count; col++)
            {
                Matrix.Matrix columnMatrix = this.WeightMatrix.GetCol(col);
                columnMatrix = MatrixMath.Transpose(columnMatrix);

                // The output for this input element is the dot product of the
                // input matrix and one column from the weight matrix.
                double dotProduct = MatrixMath.DotProduct(inputMatrix,
                       columnMatrix);

                // Convert the dot product to either true or false.
                if (dotProduct > 0)
                {
                    this.Fire[col] = 1;
                }
                else
                {
                    this.Fire[col] = -1;
                }
            }

            return this.Fire;
        }

        public new BiPolarNeuralData Fire
        {
            get
            {
                return (BiPolarNeuralData)base.Fire;
            }
            set
            {
                base.Fire = value;
            }
        }
    }
}
