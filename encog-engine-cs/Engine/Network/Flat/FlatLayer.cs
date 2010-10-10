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
namespace Encog.Engine.Network.Flat
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Encog.Engine.Util;
    using Org.Encog.Engine;


    /// <summary>
    /// Used to configure a flat layer. Flat layers are not kept by a Flat Network,
    /// beyond setup.
    /// </summary>
    ///
    public class FlatLayer
    {

        /// <summary>
        /// The activation function.
        /// </summary>
        ///
        private readonly int activation;

        /// <summary>
        /// The neuron count.
        /// </summary>
        ///
        private readonly int count;

        /// <summary>
        /// The bias activation, usually 1 for bias or 0 for no bias.
        /// </summary>
        ///
        private readonly double biasActivation;

        /// <summary>
        /// The params for the activation function.
        /// </summary>
        ///
        private readonly double[] paras;

        /// <summary>
        /// The layer that feeds this layer's context.
        /// </summary>
        ///
        private FlatLayer contextFedBy;

        /// <summary>
        /// Construct a flat layer.
        /// </summary>
        ///
        /// <param name="activation_0"/>The activation function.</param>
        /// <param name="count_1"/>The neuron count.</param>
        /// <param name="biasActivation_2"/>The bias activation.</param>
        /// <param name="params"/>The parameters.</param>
        public FlatLayer(int activation_0, int count_1,
                double biasActivation_2, double[] paras)
        {
            this.activation = activation_0;
            this.count = count_1;
            this.biasActivation = biasActivation_2;
            this.paras = EngineArray.ArrayCopy(paras);
            this.contextFedBy = null;
        }


        /// <returns>the activation</returns>
        public int Activation
        {

            /// <returns>the activation</returns>
            get
            {
                return this.activation;
            }
        }



        /// <returns>Get the bias activation.</returns>
        public double BiasActivation
        {

            /// <returns>Get the bias activation.</returns>
            get
            {
                return this.biasActivation;
            }
        }



        /// <returns>The number of neurons our context is fed by.</returns>
        public int ContectCount
        {

            /// <returns>The number of neurons our context is fed by.</returns>
            get
            {
                if (this.contextFedBy == null)
                {
                    return 0;
                }
                else
                {
                    return this.contextFedBy.Count;
                }
            }
        }


        /// <summary>
        /// Set the layer that this layer's context is fed by.
        /// </summary>
        ///
        /// <param name="from"/>The layer feeding.</param>
        public FlatLayer ContextFedBy
        {

            /// <returns>The layer that feeds this layer's context.</returns>
            get
            {
                return this.contextFedBy;
            }
            /// <summary>
            /// Set the layer that this layer's context is fed by.
            /// </summary>
            ///
            /// <param name="from"/>The layer feeding.</param>
            set
            {
                this.contextFedBy = value;
            }
        }



        /// <returns>the count</returns>
        public int Count
        {

            /// <returns>the count</returns>
            get
            {
                return this.count;
            }
        }



        /// <returns>The parameters that this activation uses.</returns>
        public double[] Params
        {

            /// <returns>The parameters that this activation uses.</returns>
            get
            {
                return this.paras;
            }
        }



        /// <returns>The total number of neurons on this layer, includes context, biasand regular.</returns>
        public int TotalCount
        {

            /// <returns>The total number of neurons on this layer, includes context, biasand regular.</returns>
            get
            {
                if (this.contextFedBy == null)
                {
                    return Count + ((Bias) ? 1 : 0);
                }
                else
                {
                    return Count + ((Bias) ? 1 : 0)
                            + this.contextFedBy.Count;
                }
            }
        }



        /// <returns>the bias</returns>
        public bool Bias
        {

            /// <returns>the bias</returns>
            get
            {
                return Math.Abs(this.biasActivation) > EncogEngine.DEFAULT_ZERO_TOLERANCE;
            }
        }


        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        ///
        public override System.String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[FlatLayer: count=");
            result.Append(this.count);
            result.Append(",bias=");

            if (Bias)
            {
                result.Append(this.biasActivation);
            }
            else
            {
                result.Append("false");
            }
            if (this.contextFedBy != null)
            {
                result.Append(",contextFed=");
                if (this.contextFedBy == this)
                {
                    result.Append("itself");
                }
                else
                {
                    result.Append(this.contextFedBy);
                }
            }
            result.Append("]");
            return result.ToString();
        }
    }
}
