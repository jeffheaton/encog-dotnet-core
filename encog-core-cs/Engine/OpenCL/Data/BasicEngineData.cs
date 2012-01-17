/*
 * Encog(tm) Core v2.5
 * http://www.heatonresearch.com/encog/
 * http://code.google.com/p/encog-java/
 *
 * Copyright 2008-2010 by Heaton Research Inc.
 *
 * Released under the LGPL.
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 *
 * This software is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this software; if not, write to the Free
 * Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
 * 02110-1301 USA, or see the FSF site: http://www.fsf.org.
 *
 * Encog and Heaton Research are Trademarks of Heaton Research, Inc.
 * For information on Heaton Research trademarks, visit:
 *
 * http://www.heatonresearch.com/copyright.html
 */

namespace Encog.Engine.Data
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// A basic implementation of the EngineData interface. This implementation
    /// simply holds and input and ideal NeuralData object.
    /// For supervised training both input and ideal should be specified.
    /// For unsupervised training the input property should be valid, however the
    /// ideal property should contain null.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class BasicEngineData : IEngineData
    {

        /// <summary>
        /// Create a new neural data pair object of the correct size for the neural
        /// network that is being trained. This object will be passed to the getPair
        /// method to allow the neural data pair objects to be copied to it.
        /// </summary>
        ///
        /// <param name="inputSize">The size of the input data.</param>
        /// <param name="idealSize">The size of the ideal data.</param>
        /// <returns>A new neural data pair object.</returns>
        public static IEngineData CreatePair(int inputSize, int idealSize)
        {
            IEngineData result;

            if (idealSize > 0)
            {
                result = new BasicEngineData(new double[inputSize],
                        new double[idealSize]);
            }
            else
            {
                result = new BasicEngineData(new double[inputSize]);
            }

            return result;
        }

        /// <summary>
        /// The the expected output from the neural network, or null for unsupervised
        /// training.
        /// </summary>
        ///
        private double[] ideal;

        /// <summary>
        /// The training input to the neural network.
        /// </summary>
        ///
        private double[] input;

        /// <summary>
        /// Construct the object with only input. If this constructor is used, then
        /// unsupervised training is being used.
        /// </summary>
        ///
        /// <param name="input">The input to the neural network.</param>
        public BasicEngineData(double[] input)
        {
            this.input = input;
            this.ideal = null;
        }

        /// <summary>
        /// Construct a BasicNeuralDataPair class with the specified input and ideal
        /// values.
        /// </summary>
        ///
        /// <param name="input">The input to the neural network.</param>
        /// <param name="ideal">The expected results from the neural network.</param>
        public BasicEngineData(double[] input, double[] ideal)
        {
            this.input = input;
            this.ideal = ideal;
        }

        /// <summary>
        /// The ideal array.
        /// </summary>
        public virtual double[] IdealArray
        {
            get
            {
                return this.ideal;
            }
            set
            {
                this.ideal = value;

            }
        }


        /// <summary>
        /// The input array.
        /// </summary>
        public virtual double[] InputArray
        {
            get
            {
                return this.input;
            }
            set
            {
                this.input = value;
            }
        }


        /// <summary>
        /// Determine if this data pair is supervised.
        /// </summary>
        public virtual bool Supervised
        {
            get
            {
                return this.ideal != null;
            }
        }


        /// <inheritdoc />
        public override System.String ToString()
        {
            StringBuilder builder = new StringBuilder("[NeuralDataPair:");
            builder.Append("Input:");
            builder.Append(InputArray);
            builder.Append("Ideal:");
            builder.Append(IdealArray);
            builder.Append("]");
            return builder.ToString();
        }

    }
}
