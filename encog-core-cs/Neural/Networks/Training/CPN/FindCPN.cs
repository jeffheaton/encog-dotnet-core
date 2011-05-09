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
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Synapse;
using Encog.Neural.Networks.Pattern;

#if logging
using log4net;
#endif

namespace Encog.Neural.Networks.Training.CPN
{
    /// <summary>
    /// Find the parts of a CPN network.
    /// </summary>
    public class FindCPN
    {

        /// <summary>
        /// The input layer.
        /// </summary>
        private ILayer inputLayer;

        /// <summary>
        /// The instar layer.
        /// </summary>
        private ILayer instarLayer;

        /// <summary>
        /// The outstar layer.
        /// </summary>
        private ILayer outstarLayer;

        /// <summary>
        /// The synapse from the input to instar layer.
        /// </summary>
        private ISynapse instarSynapse;

        /// <summary>
        /// The synapse from the instar to the outstar layer.
        /// </summary>
        private ISynapse outstarSynapse;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private static readonly ILog logger = LogManager.GetLogger(typeof(FindCPN));
#endif
        /// <summary>
        /// Construct the object and find the parts of the network.
        /// </summary>
        /// <param name="network">The network to train.</param>
        public FindCPN(BasicNetwork network)
        {
            if (network.Structure.Layers.Count != 3)
            {
                String str = "A CPN network must have exactly 3 layers";
#if logging
                if (logger.IsErrorEnabled)
                {
                    logger.Error(str);
                }
#endif
                throw new TrainingError(str);
            }

            this.inputLayer = network.GetLayer(BasicNetwork.TAG_INPUT);
            this.outstarLayer = network.GetLayer(CPNPattern.TAG_OUTSTAR);
            this.instarLayer = network.GetLayer(CPNPattern.TAG_INSTAR);

            if (this.outstarLayer == null)
            {
                String str = "Can't find an OUTSTAR layer, this is required.";
#if logging
                if (logger.IsErrorEnabled)
                {
                    logger.Error(str);
                }
#endif
                throw new TrainingError(str);
            }

            if (this.instarLayer == null)
            {
                String str = "Can't find an OUTSTAR layer, this is required.";
#if logging
                if (logger.IsErrorEnabled)
                {
                    logger.Error(str);
                }
#endif
                throw new TrainingError(str);
            }

            this.instarSynapse = this.inputLayer.Next[0];
            this.outstarSynapse = this.instarLayer.Next[0];
        }

        /// <summary>
        /// The input layer.
        /// </summary>
        public ILayer InputLayer
        {
            get
            {
                return inputLayer;
            }
        }

        /// <summary>
        /// The instar layer.
        /// </summary>
        public ILayer InstarLayer
        {
            get
            {
                return instarLayer;
            }
        }

        /// <summary>
        /// The outstar layer.
        /// </summary>
        public ILayer OutstarLayer
        {
            get
            {
                return outstarLayer;
            }
        }

        /// <summary>
        /// The instar synapse.
        /// </summary>
        public ISynapse InstarSynapse
        {
            get
            {
                return instarSynapse;
            }
        }

        /// <summary>
        /// The outstar synapse.
        /// </summary>
        public ISynapse OutstarSynapse
        {
            get
            {
                return outstarSynapse;
            }
        }

        /// <summary>
        /// Calculate the winning neuron from the data, this is the neuron
        /// that has the highest output.
        /// </summary>
        /// <param name="data">The data to use to determine the winning neuron.</param>
        /// <returns>The winning neuron index, or -1 if no winner.</returns>
        public int Winner(MLData data)
        {
            int winner = -1;

            for (int i = 0; i < data.Count; i++)
            {
                if (winner == -1 || data[i] > data[winner])
                {
                    winner = i;
                }
            }

            return winner;
        }
    }
}
