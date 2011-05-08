// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
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
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;
using System.Runtime.Serialization;

namespace Encog.Neural.Data.Basic
{
    /// <summary>
    /// Basic implementation of a data pair.  Holds both input and ideal data.
    /// If this is unsupervised training then ideal should be null.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class BasicMLDataPair : MLDataPair
    {
        /// <summary>
        /// The the expected output from the neural network, or null
        /// for unsupervised training.
        /// </summary>
        private MLData ideal;

        /// <summary>
        /// The training input to the neural network.
        /// </summary>
        private MLData input;

        /// <summary>
        /// Construct a BasicMLDataPair class with the specified input
        /// and ideal values.
        /// </summary>
        /// <param name="input">The input to the neural network.</param>
        /// <param name="ideal">The expected results from the neural network.</param>
        public BasicMLDataPair(MLData input, MLData ideal)
        {
            this.input = input;
            this.ideal = ideal;
        }

        /// <summary>
        /// Construct a data pair that only includes input. (unsupervised)
        /// </summary>
        /// <param name="input">The input data.</param>
        public BasicMLDataPair(MLData input)
        {
            this.input = input;
            this.ideal = null;
        }

        /// <summary>
        /// The input data.
        /// </summary>
        public virtual MLData Input
        {
            get
            {
                return this.input;
            }
        }

        /// <summary>
        /// The ideal data.
        /// </summary>
        public virtual MLData Ideal
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

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public object Clone()
        {
            Object result;

            if (this.Ideal == null)
                result = new BasicMLDataPair((MLData)this.input.Clone());
            else
                result = new BasicMLDataPair((MLData)this.input.Clone(),
                    (MLData)this.ideal.Clone());

            return result;
        }

        /// <summary>
        /// Create a new neural data pair object of the correct size for the neural
	    /// network that is being trained. This object will be passed to the getPair
	    /// method to allow the neural data pair objects to be copied to it.
        /// </summary>
        /// <param name="inputSize">The size of the input data.</param>
        /// <param name="idealSize">The size of the ideal data.</param>
        /// <returns>A new neural data pair object.</returns>
        public static MLDataPair CreatePair(int inputSize, int idealSize)
        {
            MLDataPair result;

            if (idealSize > 0)
            {
                result = new BasicMLDataPair(new BasicMLData(inputSize),
                        new BasicMLData(idealSize));
            }
            else
            {
                result = new BasicMLDataPair(new BasicMLData(inputSize));
            }

            return result;
        }

        /// <summary>
        /// The supervised ideal data.
        /// </summary>
        public double[] IdealArray
        {
            get
            {
                if (this.ideal == null)
                    return null;
                else
                    return this.ideal.Data;
            }
            set
            {
                this.ideal.Data = value;
            }
        }

        /// <summary>
        /// The input array.
        /// </summary>
        public double[] InputArray
        {
            get
            {
                return this.input.Data;
            }
            set
            {
                this.input.Data = value;
            }
        }

        /// <summary>
        /// Returns true, if supervised.
        /// </summary>
        public bool Supervised
        {
            get 
            {
                return this.ideal != null; 
            }
        }
    }
}
