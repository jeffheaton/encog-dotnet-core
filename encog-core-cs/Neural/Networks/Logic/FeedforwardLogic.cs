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
using Encog.Neural.Networks.Synapse;
using Encog.Neural.Data;
using Encog.Neural.Networks.Layers;

#if logging
using log4net;
using Encog.Neural.Data.Basic;
#endif

namespace Encog.Neural.Networks.Logic
{
    /// <summary>
    /// Provides the neural logic for an Feedforward type network.  See FeedforwardPattern
    /// for more information on this type of network.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class FeedforwardLogic : INeuralLogic
    {

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private static readonly ILog logger = LogManager.GetLogger(typeof(FeedforwardLogic));
#endif

        /// <summary>
        /// The network to use.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// Compute the output for a given input to the neural network. This method
        /// provides a parameter to specify an output holder to use.  This holder
        /// allows propagation training to track the output from each layer.
        /// If you do not need this holder pass null, or use the other 
        /// compare method.
        /// </summary>
        /// <param name="input">The input provide to the neural network.</param>
        /// <param name="useHolder">Allows a holder to be specified, this allows
        /// propagation training to check the output of each layer.</param>
        /// <returns>The results from the output neurons.</returns>
        public virtual INeuralData Compute(INeuralData input,
                 NeuralOutputHolder useHolder)
        {
            NeuralOutputHolder holder;

            ILayer inputLayer = this.network.GetLayer(BasicNetwork.TAG_INPUT);

#if logging
            if (FeedforwardLogic.logger.IsDebugEnabled)
            {
                FeedforwardLogic.logger.Debug("Pattern " + input.ToString()
                    + " presented to neural network");
            }
#endif

            if (useHolder == null)
            {
                holder = new NeuralOutputHolder();
            }
            else
            {
                holder = useHolder;
            }

            if (holder == null && this.network.Structure.Flat != null)
            {
                this.network.Structure.UpdateFlatNetwork();
                INeuralData result = new BasicNeuralData(this.network.Structure.Flat.OutputCount);
                this.network.Structure.Flat.Compute(input.Data, result.Data);
                return result;
            }

            Compute(holder, inputLayer, input, null);
            return holder.Output;
        }

        /// <summary>
        /// Internal computation method for a single layer.  This is called, 
        /// as the neural network processes.
        /// </summary>
        /// <param name="holder">The output holder.</param>
        /// <param name="layer">The layer to process.</param>
        /// <param name="input">The input to this layer.</param>
        /// <param name="source">The source synapse.</param>
        private void Compute(NeuralOutputHolder holder, ILayer layer,
                 INeuralData input, ISynapse source)
        {
            try
            {
#if logging
                if (FeedforwardLogic.logger.IsDebugEnabled)
                {
                    FeedforwardLogic.logger.Debug("Processing layer: "
                        + layer.ToString()
                        + ", input= "
                        + input.ToString());
                }
#endif
                
                // typically used to process any recurrent layers that feed into this
                // layer.
                PreprocessLayer(layer, input, source);

                foreach (ISynapse synapse in layer.Next)
                {
                    if (!holder.Result.ContainsKey(synapse))
                    {
#if logging
                        if (FeedforwardLogic.logger.IsDebugEnabled)
                        {
                            FeedforwardLogic.logger.Debug("Processing synapse: " + synapse.ToString());
                        }
#endif
                        INeuralData pattern = synapse.Compute(input);
                        pattern = synapse.ToLayer.Compute(pattern);
                        synapse.ToLayer.Process(pattern);
                        holder.Result[synapse] = input;
                        Compute(holder, synapse.ToLayer, pattern, synapse);

                        ILayer outputLayer = this.network.GetLayer(BasicNetwork.TAG_OUTPUT);

                        // Is this the output from the entire network?
                        if (synapse.ToLayer == outputLayer)
                        {
                            holder.Output = pattern;
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new NeuralNetworkError("Size mismatch on input of size " + input.Count + " and layer: ", ex);
            }
        }

        /// <summary>
        /// Setup the network logic, read parameters from the network.
        /// </summary>
        /// <param name="network">The network that this logic class belongs to.</param>
        public virtual void Init(BasicNetwork network)
        {
            this.network = network;
        }

        /// <summary>
        /// The network in use.
        /// </summary>
        public BasicNetwork Network
        {
            get
            {
                return network;
            }
        }

        /// <summary>
        /// Can be overridden by subclasses.  Usually used to implement recurrent 
        /// layers. 
        /// </summary>
        /// <param name="layer">The layer to process.</param>
        /// <param name="input">The input to this layer.</param>
        /// <param name="source">The source from this layer.</param>
        virtual public void PreprocessLayer(ILayer layer, INeuralData input, ISynapse source)
        {
            // nothing to do		
        }
    }
}
