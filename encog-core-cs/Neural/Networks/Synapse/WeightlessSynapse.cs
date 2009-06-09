using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks.Layers;
using log4net;
using Encog.Neural.Data;
using Encog.Persist;
using Encog.Neural.Data.Basic;
using Encog.Persist.Persistors;

namespace Encog.Neural.Networks.Synapse
{
    /// <summary>
    /// A fully connected synapse that simply sums all input to each neuron, no
 /// weights are applied.
 /// 
 /// This synapse type is not teachable.
    /// </summary>
    public class WeightlessSynapse : BasicSynapse
    {


        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(WeightlessSynapse));


        /// <summary>
        /// Simple default constructor.
        /// </summary>
        public WeightlessSynapse()
        {
        }


        /// <summary>
        /// Construct a weighted synapse between the two layers.
        /// </summary>
        /// <param name="fromLayer">The starting layer.</param>
        /// <param name="toLayer">The ending layer.</param>
        public WeightlessSynapse(ILayer fromLayer, ILayer toLayer)
        {
            this.FromLayer = fromLayer;
            this.ToLayer = toLayer;
        }

        /// <summary>
        /// A clone of this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public override Object Clone()
        {
            WeightlessSynapse result = new WeightlessSynapse();
            result.WeightMatrix = (Matrix.Matrix)this.WeightMatrix.Clone();
            return result;
        }


        /// <summary>
        /// Compute the weightless output from this synapse. Each neuron
        /// in the from layer has a weightless connection to each of the
        /// neurons in the next layer. 
        /// </summary>
        /// <param name="input">The input from the synapse.</param>
        /// <returns>The output from this synapse.</returns>
        public override INeuralData Compute(INeuralData input)
        {
            INeuralData result = new BasicNeuralData(this.ToNeuronCount);
            // just sum the input
            double sum = 0;
            for (int i = 0; i < input.Count; i++)
            {
                sum += input[i];
            }

            for (int i = 0; i < this.ToNeuronCount; i++)
            {
                result[i] = sum;
            }
            return result;
        }

        /// <summary>
        /// Return a persistor for this object.
        /// </summary>
        /// <returns>A new persistor.</returns>
        public override IPersistor CreatePersistor()
        {
            return new WeightlessSynapsePersistor();
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
                String str = "Can't set the matrix for a WeightlessSynapse";
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
                throw new NeuralNetworkError(str);
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
                return SynapseType.Weighted;
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
