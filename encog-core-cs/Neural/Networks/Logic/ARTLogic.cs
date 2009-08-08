using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data;

namespace Encog.Neural.Networks.Logic
{
    /// <summary>
    /// Neural logic for all ART type networks.
    /// </summary>
    public abstract class ARTLogic : INeuralLogic
    {
        /// <summary>
        /// Neural network property, the A1 parameter.
        /// </summary>
        public const String PROPERTY_A1 = "A1";

        /// <summary>
        /// Neural network property, the B1 parameter.
        /// </summary>
        public const String PROPERTY_B1 = "B1";

        /// <summary>
        /// Neural network property, the C1 parameter.
        /// </summary>
        public const String PROPERTY_C1 = "C1";

        /// <summary>
        /// Neural network property, the D1 parameter.
        /// </summary>
        public const String PROPERTY_D1 = "D1";

        /// <summary>
        /// Neural network property, the L parameter.
        /// </summary>
        public const String PROPERTY_L = "L";

        /// <summary>
        /// Neural network property, the vigilance parameter.
        /// </summary>
        public const String PROPERTY_VIGILANCE = "VIGILANCE";

        /// <summary>
        /// The network.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// Setup the network logic, read parameters from the network.
        /// </summary>
        /// <param name="network">The network that this logic class belongs to.</param>
        public virtual void Init(BasicNetwork network)
        {
            this.network = network;
        }

        /// <summary>
        /// The network in use.
        /// </summary>
        public BasicNetwork Network
        {
            get
            {
                return this.network;
            }
        }

        /// <summary>
        /// Compute the output for the BasicNetwork class.
        /// </summary>
        /// <param name="input">The input to the network.</param>
        /// <param name="useHolder">The NeuralOutputHolder to use.</param>
        /// <returns>The output from the network.</returns>
        public abstract INeuralData Compute(INeuralData input,
                NeuralOutputHolder useHolder);
    }
}
