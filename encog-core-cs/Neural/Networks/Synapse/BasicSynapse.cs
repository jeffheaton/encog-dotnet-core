using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Encog.Neural.Networks.Layers;
using Encog.Persist;
using Encog.Neural.Data;

namespace Encog.Neural.Networks.Synapse
{
    /// <summary>
    /// An abstract class that implements basic functionality that may be needed by
    /// the other synapse classes. Specifically this class handles processing the
    /// from and to layer, as well as providing a name and description for the
    /// EncogPersistedObject.
    /// </summary>
    public abstract class BasicSynapse : ISynapse
    {



        /// <summary>
        /// The from layer.
        /// </summary>
        private ILayer fromLayer;

        /// <summary>
        /// The to layer.
        /// </summary>
        private ILayer toLayer;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(BasicNetwork));

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public abstract Object Clone();

        /// <summary>
        /// The EncogPersistedObject requires a name and description, however, these
        /// are not used on synapses.
        /// </summary>
        public String Description
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        /// <summary>
        /// The from layer.
        /// </summary>
        public ILayer FromLayer
        {
            get
            {
                return this.fromLayer;
            }
            set
            {
                this.fromLayer = value;
            }
        }

        /// <summary>
        /// The neuron count from the "from layer".
        /// </summary>
        public int FromNeuronCount
        {
            get
            {
                return this.fromLayer.NeuronCount;
            }
        }

        /// <summary>
        /// The EncogPersistedObject requires a name and description, however, these
        /// are not used on synapses.
        /// </summary>
        public String Name
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        /// <summary>
        /// The "to layer".
        /// </summary>
        public ILayer ToLayer
        {
            get
            {
                return this.toLayer;
            }
            set
            {
                this.toLayer = value;
            }
        }

        /// <summary>
        /// The neuron count from the "to layer".
        /// </summary>
        public int ToNeuronCount
        {
            get
            {
                return this.toLayer.NeuronCount;
            }
        }

        /// <summary>
        /// True if this is a self-connected synapse. That is, the from and
        /// to layers are the same.
        /// </summary>
        public bool IsSelfConnected
        {
            get
            {
                return this.fromLayer == this.toLayer;
            }
        }

        /// <summary>
        /// Create a persistor that will be used to persist this synapse.
        /// </summary>
        /// <returns>The persistor.</returns>
        public abstract IPersistor CreatePersistor();

        /// <summary>
        /// What type of synapse is this?
        /// </summary>
        public abstract SynapseType SynapseType { get; }

        /// <summary>
        /// Get the size of the matrix, or zero if one is not defined.
        /// </summary>
        public abstract int MatrixSize { get; }

        /// <summary>
        /// Get the weight matrix.
        /// </summary>
        public abstract Matrix.Matrix WeightMatrix { get; set; }


        /// <summary>
        /// Compute the output from this synapse.
        /// </summary>
        /// <param name="input">The input to this synapse.</param>
        /// <returns>The output from this synapse.</returns>
        public abstract INeuralData Compute(INeuralData input);

        /// <summary>
        /// True if the weights for this synapse can be modified.
        /// </summary>
        public virtual bool IsTeachable
        {
            get
            {
                return false;
            }
        }



        /// <summary>
        /// Convert this layer to a string.
        /// </summary>
        /// <returns>The layer as a string.</returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[");
            result.Append(GetType().Name);
            result.Append(": from=");
            result.Append(this.FromNeuronCount);
            result.Append(",to=");
            result.Append(this.ToNeuronCount);
            result.Append("]");
            return result.ToString();
        }

    }

}
