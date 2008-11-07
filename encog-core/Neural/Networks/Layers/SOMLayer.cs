using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util;
using Encog.Neural.Data;
using Encog.Neural.Data.Basic;
using Encog.Matrix;

namespace Encog.Neural.Networks.Layers
{
    class SOMLayer : BasicLayer
    {

        /// <summary>
        /// Do not allow patterns to go below this very small number.
        /// </summary>
        public const double VERYSMALL = 1.0e-30;

        /// <summary>
        /// The normalization type.
        /// </summary>
        private NormalizationType normalizationType;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="inputCount">Number of input neurons.</param>
        /// <param name="normalizationType">The normalization type to use.</param>
        public SOMLayer(int inputCount,
                 NormalizationType normalizationType)
            : base(inputCount)
        {
            this.normalizationType = normalizationType;
        }

        /// <summary>
        /// Compute the output from this layer.
        /// </summary>
        /// <param name="pattern">The pattern to compute for.</param>
        /// <returns>The output from the layer.</returns>
        public new INeuralData Compute(INeuralData pattern)
        {
            NormalizeInput input = new NormalizeInput(pattern.Data,
                   this.normalizationType);

            INeuralData output = new BasicNeuralData(this.Next.NeuronCount);

            for (int i = 0; i < this.Next.NeuronCount; i++)
            {
                Matrix.Matrix optr = this.WeightMatrix.GetRow(i);
                output[i] = MatrixMath.DotProduct(input.InputMatrix,
                        optr)
                        * input.Normfac;

                double d = (output[i] + 1.0) / 2.0;

                if (d < 0)
                {
                    output[i] = 0.0;
                }

                if (d > 1)
                {
                    output[i] = 1.0;
                }

                this.Next.Fire[i] = output[i];
            }

            return output;
        }

        /// <summary>
        /// Get the normalization type. 
        /// </summary>
        /// <returns>The normalization type.</returns>
        public NormalizationType NormalizationTypeUsed 
        {
            get
            {
                return this.normalizationType;
            }
            set
            {
                this.normalizationType = value;
            }
    
        }



        /// <summary>
        /// Set the next layer.
        /// </summary>
        public new ILayer Next
        {
            set
            {
                base.Next = value;

                if (!HasMatrix())
                {
                    this.WeightMatrix = new Matrix.Matrix(this.Next.NeuronCount, this.NeuronCount + 1);
                }
            }
            get
            {
                return base.Next;
            }
        }
    }
}
