/*
 * Encog(tm) Core v2.5 - Java Version
 * http://www.heatonresearch.com/encog/
 * http://code.google.com/p/encog-java/
 
 * Copyright 2008-2010 Heaton Research, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *   
 * For more information on Heaton Research copyrights, licenses 
 * and trademarks visit:
 * http://www.heatonresearch.com/copyright
 */

namespace Encog.Engine.Network.Flat
{

    using Encog.Engine.Network.Activation;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Text;

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
        private readonly IActivationFunction activation;

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
        public FlatLayer(IActivationFunction activation_0, int count_1,
                double biasActivation_2, double[] paras)
        {
            this.activation = activation_0;
            this.count = count_1;
            this.biasActivation = biasActivation_2;
            this.contextFedBy = null;
        }


        /// <returns>the activation</returns>
        public IActivationFunction Activation
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
