// Encog Neural Network and Bot Library v1.x (DotNet)
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2008, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util;
using Encog.Neural.NeuralData;
using Encog.Neural.Data.Basic;
using Encog.Matrix;
using Encog.Neural.Data;

namespace Encog.Neural.Networks.Layers
{
    /// <summary>
    /// SelfOrganizingMap: The Self Organizing Map, or Kohonen Neural Network, is a
    /// special type of neural network that is used to classify input into groups.
    /// The SOM makes use of unsupervised training.
    /// </summary>
    [Serializable]
    public class SOMLayer : BasicLayer
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
        public override INeuralData Compute(INeuralData pattern)
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
        public override ILayer Next
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
