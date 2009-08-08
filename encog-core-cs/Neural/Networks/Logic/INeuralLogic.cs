using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Encog.Neural.Data;

namespace Encog.Neural.Networks.Logic
{
    /// <summary>
    /// Neural logic classes implement neural network logic for a variety
    /// of neural network setups.
    /// </summary>
    public interface INeuralLogic 
    {
        /// <summary>
        /// Compute the output for the BasicNetwork class.
        /// </summary>
        /// <param name="input">The input to the network.</param>
        /// <param name="useHolder">The NeuralOutputHolder to use.</param>
        /// <returns>The output from the network.</returns>
        INeuralData Compute(INeuralData input,
                NeuralOutputHolder useHolder);

        /// <summary>
        /// Setup the network logic, read parameters from the network.
        /// </summary>
        /// <param name="network">The network that this logic class belongs to.</param>
        void Init(BasicNetwork network);
    }
}
