using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Data.Basic
{
    class BasicNeuralDataPair : INeuralDataPair
    {
        /// <summary>
        /// The the expected output from the neural network, or null
        /// for unsupervised training.
        /// </summary>
        private INeuralData ideal;



        /// <summary>
        /// The training input to the neural network.
        /// </summary>
        private INeuralData input;

        /// <summary>
        /// Construct a BasicNeuralDataPair class with the specified input
        /// and ideal values.
        /// </summary>
        /// <param name="input">The input to the neural network.</param>
        /// <param name="ideal">The expected results from the neural network.</param>
        public BasicNeuralDataPair(INeuralData input, INeuralData ideal)
        {
            this.input = input;
            this.ideal = ideal;
        }

        public BasicNeuralDataPair(INeuralData input)
        {
            this.input = input;
            this.ideal = null;
        }

        public INeuralData Input
        {
            get
            {
                return this.input;
            }
        }

        public INeuralData Ideal
        {
            get
            {
                return this.ideal;
            }
        }
    }
}
