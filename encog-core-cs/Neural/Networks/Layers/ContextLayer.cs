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
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Persist;
using Encog.Persist.Persistors;
using Encog.Engine.Network.Activation;
#if logging
using log4net;
#endif
namespace Encog.Neural.Networks.Layers
{
    /// <summary>
    /// Implements a context layer. A context layer is used to implement a simple
    /// recurrent neural network, such as an Elman or Jordan neural network. The
    /// context layer has a short-term memory. The context layer accept input, and
    /// provide the same data as output on the next cycle. This continues, and the
    /// context layer's output "one step" out of sync with the input.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class ContextLayer : BasicLayer, IContextClearable
    {       
        /// <summary>
        /// If this layer is part of a network that has been flattened, 
        /// this attribute holds the index to the context values.  Otherwise, 
        /// this property holds the value of -1.
        /// </summary>
        public int FlatContextIndex { get; set; }

        /// <summary>
        /// The context data that this layer will store.
        /// </summary>
        private MLData context;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private static readonly ILog logger = LogManager.GetLogger(typeof(BasicNetwork));
#endif

        /// <summary>
        /// Default constructor, mainly so the workbench can easily create a default
        /// layer.
        /// </summary>
        public ContextLayer()
            : this(1)
        {

        }

        /// <summary>
        /// Construct a context layer with the parameters specified.
        /// </summary>
        /// <param name="thresholdFunction">The threshold function to use.</param>
        /// <param name="hasThreshold">Does this layer have thresholds?</param>
        /// <param name="neuronCount">The neuron count to use.</param>
        public ContextLayer(IActivationFunction thresholdFunction,
                 bool hasThreshold, int neuronCount)
            : base(thresholdFunction, hasThreshold, neuronCount)
        {
            this.FlatContextIndex = -1;
            this.context = new BasicMLData(neuronCount);
        }

        /// <summary>
        /// Construct a default context layer that has the TANH activation function
        /// and the specified number of neurons. Use threshold values.
        /// </summary>
        /// <param name="neuronCount">The number of neurons on this layer.</param>
        public ContextLayer(int neuronCount)
            : this(new ActivationTANH(), true, neuronCount)
        {

        }

        /// <summary>
        /// Create a persistor for this layer.
        /// </summary>
        /// <returns>The new persistor.</returns>
        public override IPersistor CreatePersistor()
        {
            return new ContextLayerPersistor();
        }

        /// <summary>
        /// The context, or memory of this layer. These will be the values
        /// that were just output.
        /// </summary>
        public MLData Context
        {
            get
            {
                return this.context;
            }
        }

        /// <summary>
        /// Called to process input from the previous layer. Simply store the output
        /// in the context.
        /// </summary>
        /// <param name="pattern">The pattern to store in the context.</param>
        public override void Process(MLData pattern)
        {
            double[] target = this.context.Data;
            double[] source = pattern.Data;

            Array.Copy(source, target, source.Length);

#if logging
            if (ContextLayer.logger.IsDebugEnabled)
            {
                ContextLayer.logger.Debug("Updated ContextLayer to " + pattern);
            }
#endif
        }

        /// <summary>
        /// Called to get the output from this layer when called in a recurrent
        /// manor. Simply return the context that was kept from the last iteration.
        /// </summary>
        /// <returns></returns>
        public override MLData Recur()
        {
            return this.context;
        }

        /// <summary>
        /// Reset the context values back to zero.
        /// </summary>
        public void ClearContext()
        {
            for (int i = 0; i < this.context.Count; i++)
            {
                this.context[i] = 0;
            }

        }
    }
}
