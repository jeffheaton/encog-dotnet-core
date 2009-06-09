using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Matrix;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Data;
using Encog.Persist;
using Encog.Neural.Data.Basic;
using Encog.Persist.Persistors;

namespace Encog.Neural.Networks.Synapse
{

    /// <summary>
    /// A fully-connected weight based synapse. Inputs will be multiplied by the
    /// weight matrix and presented to the layer.
    /// 
    /// This synapse type is teachable.
    /// </summary>
    public class WeightedSynapse : BasicSynapse
    {

        /// <summary>
        /// The weight matrix.
        /// </summary>
        private Matrix.Matrix matrix;


        /// <summary>
        /// Simple default constructor.
        /// </summary>
        public WeightedSynapse()
        {

        }

        /// <summary>
        /// Construct a weighted synapse between the two layers.
        /// </summary>
        /// <param name="fromLayer">The starting layer.</param>
        /// <param name="toLayer">The ending layer.</param>
        public WeightedSynapse(ILayer fromLayer, ILayer toLayer)
        {
            this.FromLayer = fromLayer;
            this.ToLayer = toLayer;
            this.matrix = new Matrix.Matrix(this.FromNeuronCount, this.ToNeuronCount);
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public override Object Clone()
        {
            WeightedSynapse result = new WeightedSynapse();
            result.WeightMatrix = (Matrix.Matrix)this.WeightMatrix.Clone();
            return result;
        }


        /// <summary>
        /// Compute the weighted output from this synapse. Each neuron
        /// in the from layer has a weighted connection to each of the
        /// neurons in the next layer. 
        /// </summary>
        /// <param name="input">The input from the synapse.</param>
        /// <returns>The output from this synapse.</returns>
        public override INeuralData Compute(INeuralData input)
        {
            INeuralData result = new BasicNeuralData(this.ToNeuronCount);
            Matrix.Matrix inputMatrix = MatrixMath.CreateInputMatrix(input);

            for (int i = 0; i < this.ToNeuronCount; i++)
            {
                Matrix.Matrix col = this.WeightMatrix.GetCol(i);
                double sum = MatrixMath.DotProduct(col, inputMatrix);
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
            return new WeightedSynapsePersistor();
        }

        /// <summary>
        /// Get the weight and threshold matrix.
        /// </summary>
        public override Matrix.Matrix WeightMatrix
        {
            get
            {
                return this.matrix;
            }
            set
            {
                this.matrix = value;
            }
        }

        /// <summary>
        /// Get the size of the matrix, or zero if one is not defined.
        /// </summary>
        public override int MatrixSize
        {
            get
            {
                if (this.matrix == null)
                {
                    return 0;
                }

                return this.matrix.Size;
            }
        }

        /// <summary>
        /// The type of synapse this is.
        /// </summary>
        public override SynapseType SynapseType
        {
            get
            {
                return SynapseType.Weighted;
            }
        }

        /// <summary>
        /// True, this is a teachable synapse type.
        /// </summary>
        public override bool IsTeachable
        {
            get
            {
                return true;
            }
        }
    }
}
