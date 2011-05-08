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
        /// <param name="activation">The activation function.</param>
        /// <param name="count">The neuron count.</param>
        /// <param name="biasActivation">The bias activation.</param>
        /// <param name="paras">The parameters.</param>
        public FlatLayer(IActivationFunction activation, int count,
                double biasActivation, double[] paras)
        {
            this.activation = activation;
            this.count = count;
            this.biasActivation = biasActivation;
            this.contextFedBy = null;
        }


        /// <summary>
        /// The activation.
        /// </summary>
        public IActivationFunction Activation
        {
            get
            {
                return this.activation;
            }
        }



        /// <summary>
        /// Get the bias activation.
        /// </summary>
        public double BiasActivation
        {
            get
            {
                return this.biasActivation;
            }
        }



        /// <summary>
        /// The number of neurons our context is fed by.
        /// </summary>
        public int ContectCount
        {
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
        public FlatLayer ContextFedBy
        {
            get
            {
                return this.contextFedBy;
            }
            set
            {
                this.contextFedBy = value;
            }
        }



        /// <summary>
        /// The count.
        /// </summary>
        public int Count
        {
            get
            {
                return this.count;
            }
        }



        /// <summary>
        /// The total number of neurons on this layer, includes context, biasand regular.
        /// </summary>
        public int TotalCount
        {
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



        /// <summary>
        /// The bias.
        /// </summary>
        public bool Bias
        {
            get
            {
                return Math.Abs(this.biasActivation) > EncogEngine.DEFAULT_ZERO_TOLERANCE;
            }
        }


        /// <inheritdoc/>
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
