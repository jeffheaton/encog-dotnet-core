using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Activation;
using Encog.Neural.Data;
using Encog.Matrix;
using Encog.Neural.Data.Basic;

namespace Encog.Neural.Networks.Layers
{
    [Serializable]
    public class FeedforwardLayer : BasicLayer
    {
        /// <summary>
        /// Which activation function to use for this layer.
        /// </summary>
        private IActivationFunction activationFunction;

        /// <summary>
        /// Construct this layer with a non-default threshold function.
        /// </summary>
        /// <param name="thresholdFunction">The threshold function to use.</param>
        /// <param name="neuronCount">How many neurons in this layer.</param>
        public FeedforwardLayer(IActivationFunction thresholdFunction,
                 int neuronCount)
            : base(neuronCount)
        {
            this.activationFunction = thresholdFunction;
        }

        /// <summary>
        /// Construct this layer with a sigmoid threshold function.
        /// </summary>
        /// <param name="neuronCount">How many neurons in this layer.</param>
        public FeedforwardLayer(int neuronCount)
            : this(new ActivationSigmoid(), neuronCount)
        {

        }

        /// <summary>
        /// Compute the outputs for this layer given the input pattern. The output is
        /// also stored in the fire instance variable.
        /// </summary>
        /// <param name="pattern">The input pattern.</param>
        /// <returns>The output from this layer.</returns>
        public new INeuralData Compute(INeuralData pattern)
        {
            int i;
            if (pattern != null)
            {
                for (i = 0; i < this.NeuronCount; i++)
                {
                    this.Fire[i] = pattern[i];
                }
            }

            Matrix.Matrix inputMatrix = CreateInputMatrix(this.Fire);

            for (i = 0; i < this.Next.NeuronCount; i++)
            {
                Matrix.Matrix col = this.WeightMatrix.GetCol(i);
                double sum = MatrixMath.DotProduct(col, inputMatrix);

                this.Next.Fire[i] =
                        this.activationFunction.ActivationFunction(sum);
            }

            return this.Fire;
        }


        /// <summary>
        /// Take a simple double array and turn it into a matrix that can be used to
        /// calculate the results of the input array. Also takes into account the
        /// threshold.
        /// </summary>
        /// <param name="pattern">The pattern to create the matrix for.</param>
        /// <returns>The input matrix.</returns>
        private Matrix.Matrix CreateInputMatrix(INeuralData pattern)
        {
            Matrix.Matrix result = new Matrix.Matrix(1, pattern.Count + 1);
            for (int i = 0; i < pattern.Count; i++)
            {
                result[0, i] = pattern[i];
            }

            // add a "fake" first column to the input so that the threshold is
            // always multiplied by one, resulting in it just being added.
            result[0, pattern.Count] = 1;

            return result;
        }

        public IActivationFunction ActivationFunction
        {
            get
            {
                return this.activationFunction;
            }
            set
            {
                this.activationFunction = value;
            }
        }

        /// <summary>
        /// Prune one of the neurons from this layer. Remove all entries in this
        /// weight matrix and other layers.
        /// </summary>
        /// <param name="neuron">The neuron to prune. Zero specifies the first neuron.</param>
        public void Prune(int neuron)
        {
            // delete a row on this matrix
            if (this.HasMatrix())
            {
                this.WeightMatrix = MatrixMath.DeleteRow(this.WeightMatrix, neuron);
            }

            // delete a column on the previous
            ILayer previous = this.Previous;
            if (previous != null)
            {
                if (previous.HasMatrix())
                {
                    previous.WeightMatrix = MatrixMath.DeleteCol(previous.WeightMatrix,
                            neuron);
                }
            }

        }


        public new Matrix.Matrix WeightMatrix
        {
            get
            {
                return base.WeightMatrix;
            }
            set
            {
                if (value.Rows < 2)
                {
                    throw new NeuralNetworkError(
                            "Weight matrix includes threshold values, "
                                    + "and must have at least 2 rows.");
                }
                this.Fire = new BasicNeuralData(value.Rows - 1);

                base.WeightMatrix = value;
            }
        }

        public new ILayer Next
        {
            set
            {
                base.Next = value;

                if (!HasMatrix())
                {
                    // add one to the neuron count to provide a threshold value in row 0
                    this.WeightMatrix = new Matrix.Matrix(this.NeuronCount + 1, this.Next.NeuronCount);
                }
            }
            get
            {
                return base.Next;
            }
        }

        /// <summary>
        /// The string form of the layer.
        /// </summary>
        /// <returns>The string form of the layer.</returns>
        public new String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[FeedforwardLayer: Neuron Count=");
            result.Append(this.NeuronCount);
            result.Append("]");
            return result.ToString();
        }

    }
}
