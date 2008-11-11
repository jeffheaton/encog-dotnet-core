using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;

namespace Encog.Neural.Networks
{
    public interface ILayer
    {
        /// <summary>
        /// Compute the output for this layer.
        /// </summary>
        /// <param name="pattern">The input pattern.</param>
        /// <returns>The output from this layer.</returns>
        INeuralData Compute(INeuralData pattern);

        ILayer Next
        {
            get;
            set;
        }

        ILayer Previous
        {
            get;
            set;
        }

        INeuralData Fire
        {
            get;
            set;
        }

        int NeuronCount
        {
            get;
        }

        Matrix.Matrix WeightMatrix
        {
            get;
            set;
        }

        int MatrixSize
        {
            get;
        }

        /// <summary>
        /// Is this an input layer.
        /// </summary>
        /// <returns>True if this is an input layer.</returns>
        bool IsInput();

        /// <summary>
        /// Is this a hidden layer.
        /// </summary>
        /// <returns>True if this is a hidden layer.</returns>
        bool IsHidden();



        /// <summary>
        /// Reset the weight matrix to random values.
        /// </summary>
        void Reset();


        /// <summary>
        /// Is this an output layer.
        /// </summary>
        /// <returns>True if this is an output layer.</returns>
        bool IsOutput();

        bool HasMatrix();
    }
}
