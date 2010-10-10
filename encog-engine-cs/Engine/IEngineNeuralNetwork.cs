using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Engine
{
    /// <summary>
    /// An interface that defines a neural network. Mainly adds the ability to
    /// encode/decode weights to/from a double array.
    /// </summary>
    public interface IEngineNeuralNetwork
    {     
        /// <summary>
        /// Decode an array to the neural network weights. 
        /// </summary>
        /// <param name="data">The data to decode.</param>
        void DecodeNetwork(double[] data);

        /// <summary>
        /// Encode the neural network weights to an array. 
        /// </summary>
        /// <returns>The encoded neural network.</returns>
        double[] EncodeNetwork();

        /// <summary>
        /// The length of the encoded array.
        /// </summary>
        int EncodeLength { get; }
    }
}
