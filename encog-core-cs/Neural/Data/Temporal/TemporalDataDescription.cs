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
using Encog.Engine.Network.Activation;

namespace Encog.Neural.NeuralData.Temporal
{
    /// <summary>
    /// This class describes one unit of input, or output, to a temporal neural
    /// network. Data can be both an input and output. Inputs are used to attempt
    /// predict the output.
    /// </summary>
    public class TemporalDataDescription
    {

        /// <summary>
        /// The type of data requested.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// Data in its raw, unmodified form.
            /// </summary>
            RAW,
            /// <summary>
            /// The percent change.
            /// </summary>
            PERCENT_CHANGE,
            /// <summary>
            /// The difference change.
            /// </summary>
            DELTA_CHANGE,
        }

        /// <summary>
        /// The lowest allowed number.
        /// </summary>
        private double low;

        /// <summary>
        /// The highest allowed number.
        /// </summary>
        private double high;

        /// <summary>
        /// Is this data item used for input to prediction?
        /// </summary>
        private bool input;

        /// <summary>
        /// Should this data item be predicted?
        /// </summary>
        private bool predict;

        /// <summary>
        /// What type of data is requested?
        /// </summary>
        private Type type;

        /// <summary>
        /// What is the index of this data item in relation to the others.
        /// </summary>
        private int index;

        /// <summary>
        /// The lowest allowed data.
        /// </summary>
        public double Low
        {
            get
            {
                return this.low;
            }
            set
            {
                this.low = value;
            }
        }

        /// <summary>
        /// The highest allowed value.
        /// </summary>
        public double High
        {
            get
            {
                return this.high;
            }
            set
            {
                this.high = value;
            }
        }

        /// <summary>
        /// Is this data input?  Or is it to be predicted.
        /// </summary>
        public bool IsInput
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
        /// Determine if this is a predicted value.
        /// </summary>
        public bool IsPredict
        {
            get
            {
                return this.predict;
            }
            set
            {
                this.predict = value;
            }
        }

        /// <summary>
        /// Get the index.
        /// </summary>
        public int Index
        {
            get
            {
                return this.index;
            }
            set
            {
                this.index = value;
            }
        }

        /// <summary>
        /// The type of data this is.
        /// </summary>
        public Type DescriptionType
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>
        /// The activation function for this layer.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            get
            {
                return this.activationFunction;
            }
        }

        /// <summary>
        /// Should an activation function be used?
        /// </summary>
        private IActivationFunction activationFunction;

        /// <summary>
        /// Construct a data description item. Set both low and high to zero for
        /// unbounded.
        /// </summary>
        /// <param name="activationFunction">What activation function should be used?</param>
        /// <param name="low">What is the lowest allowed value.</param>
        /// <param name="high">What is the highest allowed value.</param>
        /// <param name="type">What type of data is this.</param>
        /// <param name="input">Used for input?</param>
        /// <param name="predict">Used for prediction?</param>
        public TemporalDataDescription(IActivationFunction activationFunction,
                 double low, double high, Type type,
                 bool input, bool predict)
        {
            this.low = low;
            this.type = type;
            this.high = high;
            this.input = input;
            this.predict = predict;
            this.activationFunction = activationFunction;
        }

        /// <summary>
        /// Construct a data description with an activation function, but no range.
        /// </summary>
        /// <param name="activationFunction">The activation function.</param>
        /// <param name="type">The type of data.</param>
        /// <param name="input">Used for input?</param>
        /// <param name="predict">Used for prediction?</param>
        public TemporalDataDescription(IActivationFunction activationFunction,
                 Type type, bool input, bool predict)
            : this(activationFunction, 0, 0, type, input, predict)
        {

        }

        /// <summary>
        /// Construct a data description with no activation function or range.
        /// </summary>
        /// <param name="type">The type of data.</param>
        /// <param name="input">Used for input?</param>
        /// <param name="predict">Used for prediction?</param>
        public TemporalDataDescription(Type type, bool input,
                 bool predict)
            : this(null, 0, 0, type, input, predict)
        {

        }
    }
}
