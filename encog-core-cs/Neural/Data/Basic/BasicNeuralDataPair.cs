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
using Encog.Neural.NeuralData;

namespace Encog.Neural.Data.Basic
{
    /// <summary>
    /// Basic implementation of a data pair.  Holds both input and ideal data.
    /// If this is unsupervised training then ideal should be null.
    /// </summary>
    public class BasicNeuralDataPair : INeuralDataPair
    {
        /// <summary>
        /// The the expected output from the neural network, or null
        /// for unsupervised training.
        /// </summary>
        private INeuralData ideal;



        /// <summary>
        /// The training input to the neural network.
        /// </summary>
        private INeuralData input;

        /// <summary>
        /// Construct a BasicNeuralDataPair class with the specified input
        /// and ideal values.
        /// </summary>
        /// <param name="input">The input to the neural network.</param>
        /// <param name="ideal">The expected results from the neural network.</param>
        public BasicNeuralDataPair(INeuralData input, INeuralData ideal)
        {
            this.input = input;
            this.ideal = ideal;
        }

        /// <summary>
        /// Construct a data pair that only includes input. (unsupervised)
        /// </summary>
        /// <param name="input">The input data.</param>
        public BasicNeuralDataPair(INeuralData input)
        {
            this.input = input;
            this.ideal = null;
        }

        /// <summary>
        /// The input data.
        /// </summary>
        public virtual INeuralData Input
        {
            get
            {
                return this.input;
            }
        }

        /// <summary>
        /// The ideal data.
        /// </summary>
        public virtual INeuralData Ideal
        {
            get
            {
                return this.ideal;
            }
        }

        /// <summary>
        /// Convert object to a string.
        /// </summary>
        /// <returns>The object as a string.</returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append('[');
            result.Append("Input:");
            result.Append(this.Input);
            result.Append(",Ideal:");
            result.Append(this.Ideal);
            result.Append(']');
            return result.ToString();
        }

        /// <summary>
        /// Deterimine if this pair is supervised or unsupervised.
        /// </summary>
        /// <returns>True if this is a supervised pair.</returns>
        public bool IsSupervised
        {
            get
            {
                return this.ideal != null;
            }
        }

        public Object Clone()
        {
            Object result;

            if( this.Ideal == null )
                result = new BasicNeuralDataPair((INeuralData)this.input.Clone());
            else
                result = new BasicNeuralDataPair((INeuralData)this.input.Clone(), 
                    (INeuralData)this.ideal.Clone());

            return result;
        }
    }
}
