using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Data;
using Encog.Persist;
using Encog.Persist.Persistors;

namespace Encog.Neural.Networks.Synapse
{
    /// <summary>
    /// A one-to-one synapse requires that the from and to layers have exactly the
    /// same number of neurons. A one-to-one synapse can be useful, when used in
    /// conjunction with a ContextLayer.
    /// </summary>
    public class OneToOneSynapse : BasicSynapse
    {

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(BasicNetwork));

        /// <summary>
        /// Simple default constructor.
        /// </summary>
        public OneToOneSynapse()
        {

        }

        /// <summary>
        /// Construct a one-to-one synapse between the two layers.
        /// </summary>
        /// <param name="fromLayer">The starting layer.</param>
        /// <param name="toLayer">The ending layer.</param>
        public OneToOneSynapse(ILayer fromLayer, ILayer toLayer)
        {
            if (fromLayer.NeuronCount != toLayer.NeuronCount)
            {
                String str =
                   "From and to layers must have the same number of "
                   + "neurons.";
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }

                throw new NeuralNetworkError(str);
            }
            this.FromLayer = fromLayer;
            this.ToLayer = toLayer;
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public override Object Clone()
        {
            OneToOneSynapse result = new OneToOneSynapse(FromLayer,
                   ToLayer);
            return result;
        }

        /// <summary>
        /// Compute the output from this synapse.
        /// </summary>
        /// <param name="input">The input to this synapse.</param>
        /// <returns>The output is the same as the input.</returns>
        public override INeuralData Compute(INeuralData input)
        {
            return input;
        }

        /// <summary>
        /// null, this synapse type has no matrix.
        /// </summary>
        /// <returns>A persistor for this object.</returns>
        public override IPersistor CreatePersistor()
        {
            return new OneToOneSynapsePersistor();
        }

        /// <summary>
        /// null, this synapse type has no matrix.
        /// </summary>
        public override Matrix.Matrix WeightMatrix
        {
            get
            {
                return null;
            }
            set
            {
                throw new NeuralNetworkError(
                    "Can't set the matrix for a OneToOneSynapse");
            }
        }

        /// <summary>
        /// 0, this synapse type has no matrix.
        /// </summary>
        public override int MatrixSize
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// The type of synapse that this is.
        /// </summary>
        public override SynapseType SynapseType
        {
            get
            {
                return SynapseType.OneToOne;
            }
        }

        /// <summary>
        /// False, because this type of synapse is not teachable.
        /// </summary>
        public override bool IsTeachable
        {
            get
            {
                return false;
            }
        }
    }
}
