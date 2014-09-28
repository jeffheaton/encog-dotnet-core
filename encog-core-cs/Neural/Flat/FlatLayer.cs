//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.Text;
using Encog.Engine.Network.Activation;

namespace Encog.Neural.Flat
{
    /// <summary>
    /// Used to configure a flat layer. Flat layers are not kept by a Flat Network,
    /// beyond setup.
    /// </summary>
    ///
    public class FlatLayer
    {
        /// <summary>
        /// The neuron count.
        /// </summary>
        ///
        private readonly int _count;

        /// <summary>
        /// The bias activation, usually 1 for bias or 0 for no bias.
        /// </summary>
        ///
        private double _biasActivation;

        /// <summary>
        /// The layer that feeds this layer's context.
        /// </summary>
        ///
        private FlatLayer _contextFedBy;

        /// <summary>
        /// Construct a flat layer.
        /// </summary>
        ///
        /// <param name="activation">The activation function.</param>
        /// <param name="count">The neuron count.</param>
        /// <param name="biasActivation">The bias activation.</param>
        public FlatLayer(IActivationFunction activation, int count,
                         double biasActivation)
        {
            Activation = activation;
            _count = count;
            _biasActivation = biasActivation;
            _contextFedBy = null;
        }


        /// <value>the activation to set</value>
        public IActivationFunction Activation { get; set; }


        /// <summary>
        /// Set the bias activation.
        /// </summary>
        public double BiasActivation
        {
            get
            {
                if (HasBias())
                {
                    return _biasActivation;
                }
                return 0;
            }
            set { _biasActivation = value; }
        }


        /// <value>The number of neurons our context is fed by.</value>
        public int ContextCount
        {
            get
            {
                if (_contextFedBy == null)
                {
                    return 0;
                }
                return _contextFedBy.Count;
            }
        }


        /// <summary>
        /// Set the layer that this layer's context is fed by.
        /// </summary>
        public FlatLayer ContextFedBy
        {
            get { return _contextFedBy; }
            set { _contextFedBy = value; }
        }


        /// <value>the count</value>
        public int Count
        {
            get { return _count; }
        }


        /// <value>The total number of neurons on this layer, includes context, bias
        /// and regular.</value>
        public int TotalCount
        {
            get
            {
                if (_contextFedBy == null)
                {
                    return Count + ((HasBias()) ? 1 : 0);
                }
                return Count + ((HasBias()) ? 1 : 0)
                       + _contextFedBy.Count;
            }
        }


        /// <returns>the bias</returns>
        public bool HasBias()
        {
            return Math.Abs(_biasActivation) > EncogFramework.DefaultDoubleEqual;
        }

        /// <inheritdoc/>
        public override sealed String ToString()
        {
            var result = new StringBuilder();
            result.Append("[");
            result.Append(GetType().Name);
            result.Append(": count=");
            result.Append(_count);
            result.Append(",bias=");

            if (HasBias())
            {
                result.Append(_biasActivation);
            }
            else
            {
                result.Append("false");
            }
            if (_contextFedBy != null)
            {
                result.Append(",contextFed=");
                if (_contextFedBy == this)
                {
                    result.Append("itself");
                }
                else
                {
                    result.Append(_contextFedBy);
                }
            }
            result.Append("]");
            return result.ToString();
        }
    }
}
