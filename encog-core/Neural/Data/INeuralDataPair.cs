using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Data
{
    public interface INeuralDataPair
    {
        /// <summary>
        /// The input that the neural network.
        /// </summary>
        INeuralData Input
        {
            get;
        }

        /// <summary>
        /// The ideal data that the neural network should produce
        /// for the specified input.
        /// </summary>
        INeuralData Ideal
        {
            get;
        }
    }
}
