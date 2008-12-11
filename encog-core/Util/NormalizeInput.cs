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
using Encog.Matrix;

namespace Encog.Util
{
    /// <summary>
    /// This class support two normalization types. Z-AXIS is the most commonly
    /// used normalization type. Multiplicative is used over z-axis when the
    /// values are in very close range.
    /// </summary>
    public enum NormalizationType
    {
        /// <summary>
        /// Z-Axis normalization.
        /// </summary>
        Z_AXIS, 
        /// <summary>
        /// Multiplicative normalization.
        /// </summary>
        MULTIPLICATIVE
    }

    /// <summary>
    /// NormalizeInput: Input into a Self Organizing Map must be normalized.
    /// </summary>
    public class NormalizeInput
    {
        /// <summary>
        /// Get the resulting input matrix.
        /// </summary>
        public Matrix.Matrix InputMatrix
        {
            get
            {
                return this.inputMatrix;
            }
        }

        /// <summary>
        /// The normalization factor.
        /// </summary>
        public double Normfac
        {
            get
            {
                return this.normfac;
            }
        }

        /// <summary>
        /// The synthetic input.
        /// </summary>
        public double Synth
        {
            get
            {
                return this.synth;
            }
        }



        /// <summary>
        /// Do not allow patterns to go below this very small number.
        /// </summary>
        public double VERYSMALL = Math.Pow(10, -30);


        /// <summary>
        /// What type of normalization should be used.
        /// </summary>
        private NormalizationType type;

        /// <summary>
        /// The normalization factor.
        /// </summary>
        protected double normfac;

        /// <summary>
        /// The synthetic input.
        /// </summary>
        protected double synth;

        /// <summary>
        /// The input expressed as a matrix.
        /// </summary>
        protected Matrix.Matrix inputMatrix;

        /// <summary>
        /// Normalize an input array into a matrix. The resulting matrix will have
        /// one extra column that will be occupied by the synthetic input.
        /// </summary>
        /// <param name="input">The input array to be normalized.</param>
        /// <param name="type">What type of normalization to use.</param>
        public NormalizeInput(double[] input, NormalizationType type)
        {
            this.type = type;
            CalculateFactors(input);
            this.inputMatrix = this.CreateInputMatrix(input, this.synth);
        }



        /// <summary>
        /// Create an input matrix that has enough space to hold the extra synthetic
        /// input.
        /// </summary>
        /// <param name="pattern">The input pattern to create.</param>
        /// <param name="extra">The synthetic input.</param>
        /// <returns>A matrix that contains the input pattern and the synthetic input.</returns>
        protected Matrix.Matrix CreateInputMatrix(double[] pattern,
                 double extra)
        {
            Matrix.Matrix result = new Matrix.Matrix(1, pattern.Length + 1);
            for (int i = 0; i < pattern.Length; i++)
            {
                result[0, i] = pattern[i];
            }

            result[0, pattern.Length] = extra;

            return result;
        }

        /// <summary>
        /// Determine both the normalization factor and the synthetic input for the
        /// given input.
        /// </summary>
        /// <param name="input">The input to normalize.</param>
        protected void CalculateFactors(double[] input)
        {

            Matrix.Matrix inputMatrix = Matrix.Matrix.CreateColumnMatrix(input);
            double len = MatrixMath.VectorLength(inputMatrix);
            len = Math.Max(len, VERYSMALL);
            int numInputs = input.Length;

            if (this.type == NormalizationType.MULTIPLICATIVE)
            {
                this.normfac = 1.0 / len;
                this.synth = 0.0;
            }
            else
            {
                this.normfac = 1.0 / Math.Sqrt(numInputs);
                double d = numInputs - Math.Pow(len, 2);
                if (d > 0.0)
                {
                    this.synth = Math.Sqrt(d) * this.normfac;
                }
                else
                {
                    this.synth = 0;
                }
            }
        }
    }
}
