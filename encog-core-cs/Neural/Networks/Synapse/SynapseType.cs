using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Synapse
{
    /// <summary>
    /// Specifies the type of synapse to be created.
    /// </summary>
    public enum SynapseType
    {
        /// <summary>
        /// OneToOne - Each neuron is connected to the same neuron number
        /// in the next layer.  The two layers must have the same number
        /// of neurons.
        /// </summary>
        OneToOne,

        /// <summary>
        /// Weighted - The neurons are connected between the two levels
        /// with weights.  These weights change during training.
        /// </summary>
        Weighted,

        /// <summary>
        /// Weightless - Every neuron is connected to every other neuron
        /// in the next layer, but there are no weights.
        /// </summary>
        Weightless,

        /// <summary>
        /// Direct - Input is simply passed directly to the next layer.
        /// </summary>
        Direct,

        /// <summary>
        /// Normalize - A synapse that normalizes the data.  Used to implement
        /// a SOM.
        /// </summary>
        Normalize
    }

}
