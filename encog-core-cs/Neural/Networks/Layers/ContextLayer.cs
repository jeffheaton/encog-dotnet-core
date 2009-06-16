using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data.Basic;
using Encog.Persist;
using Encog.Persist.Persistors;
using log4net;
using Encog.Neural.Data;
using Encog.Neural.Activation;

namespace Encog.Neural.Networks.Layers
{
    /// <summary>
    /// Implements a context layer. A context layer is used to implement a simple
    /// recurrent neural network, such as an Elman or Jordan neural network. The
    /// context layer has a short-term memory. The context layer accept input, and
    /// provide the same data as output on the next cycle. This continues, and the
    /// context layer's output "one step" out of sync with the input.
    /// </summary>
    public class ContextLayer : BasicLayer
    {

        /// <summary>
        /// The context data that this layer will store.
        /// </summary>
        private INeuralData context;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(BasicNetwork));

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

            this.context = new BasicNeuralData(neuronCount);
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
        public INeuralData Context
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
        public override void Process(INeuralData pattern)
        {
            for (int i = 0; i < pattern.Count; i++)
            {
                this.context[i] = pattern[i];
            }

            if (this.logger.IsDebugEnabled)
            {
                this.logger.Debug("Updated ContextLayer to " + pattern);
            }
        }

        /// <summary>
        /// Called to get the output from this layer when called in a recurrent
        /// manor. Simply return the context that was kept from the last iteration.
        /// </summary>
        /// <returns></returns>
        public override INeuralData Recur()
        {
            return this.context;
        }
    }
}
