
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data;

namespace Encog.Neural.NeuralData
{
    /// <summary>
    /// A neural data pair holds both the input and ideal data.  If this
    /// is an unsupervised data element, then only input is provided.
    /// </summary>
    public interface INeuralDataPair: ICloneable
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

        /// <summary>
        /// True if this training pair is supervised.  That is, it has 
	    /// both input and ideal data.
        /// </summary>
        bool IsSupervised
        {
            get;
        }
    }
}
