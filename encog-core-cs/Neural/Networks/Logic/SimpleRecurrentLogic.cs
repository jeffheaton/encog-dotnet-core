using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Data;
using log4net;
using Encog.Neural.Networks.Synapse;

namespace Encog.Neural.Networks.Logic
{
    /// <summary>
    /// Provides the neural logic for an Simple Recurrent Network (SRN) type network.  
    /// This class is used for the Elman and Jordan networks.
    /// </summary>
    public class SimpleRecurrentLogic : FeedforwardLogic
    {
        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private static readonly ILog logger = LogManager.GetLogger(typeof(SimpleRecurrentLogic));

        /// <summary>
        /// Handle recurrent layers.  See if there are any recurrent layers before
        /// the specified layer that must affect the input.
        /// </summary>
        /// <param name="layer">The layer being processed, see if there are any recurrent
        /// connections to this.</param>
        /// <param name="input">The input to the layer, will be modified with the result
        /// from any recurrent layers.</param>
        /// <param name="source">The source synapse.</param>
        public override void PreprocessLayer(ILayer layer,
                 INeuralData input, ISynapse source)
        {
            foreach (ISynapse synapse in
                     this.Network.Structure.GetPreviousSynapses(layer))
            {
                if (synapse != source)
                {
                    if (SimpleRecurrentLogic.logger.IsDebugEnabled)
                    {
                        SimpleRecurrentLogic.logger.Debug("Recurrent layer from: " + input.ToString());
                    }
                    INeuralData recurrentInput = synapse.FromLayer.Recur();

                    if (recurrentInput != null)
                    {
                        INeuralData recurrentOutput = synapse
                               .Compute(recurrentInput);

                        for (int i = 0; i < input.Count; i++)
                        {
                            input[i] = input[i]
                                    + recurrentOutput[i];
                        }

                        if (SimpleRecurrentLogic.logger.IsDebugEnabled)
                        {
                            SimpleRecurrentLogic.logger.Debug("Recurrent layer to: " + input.ToString());
                        }
                    }
                }
            }
        }
    }
}
