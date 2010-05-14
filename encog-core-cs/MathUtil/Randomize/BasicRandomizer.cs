// Encog(tm) Artificial Intelligence Framework v2.3
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
using Encog.Neural.Networks;
using Encog.Neural.Networks.Synapse;
using Encog.Neural.Networks.Layers;
#if logging
using log4net;
using Encog.MathUtil.Matrices;
#endif
namespace Encog.MathUtil.Randomize
{
    /// <summary>
    /// Provides basic functionality that most randomizers will need.
    /// </summary>
    public abstract class BasicRandomizer : IRandomizer
    {
#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(BasicRandomizer));
#endif
        /// <summary>
        /// Randomize the synapses and thresholds in the basic network based on an
        /// array, modify the array. Previous values may be used, or they may be
        /// discarded, depending on the randomizer.
        /// </summary>
        /// <param name="network">A network to randomize.</param>
        public virtual void Randomize(BasicNetwork network)
        {

            // randomize the weight matrix
            foreach (ISynapse synapse in network.Structure.Synapses)
            {
                if (synapse.WeightMatrix != null)
                {
                    Randomize(network, synapse);
                }
            }

            // randomize the bias
            foreach (ILayer layer in network.Structure.Layers)
            {
                if (layer.HasBias)
                {
                    Randomize(layer.BiasWeights);
                }
            }
        }

        /// <summary>
        /// Randomize the array based on an array, modify the array. Previous values
        /// may be used, or they may be discarded, depending on the randomizer.
        /// </summary>
        /// <param name="d">An array to randomize.</param>
        public virtual void Randomize(double[] d)
        {
            for (int i = 0; i < d.Length; i++)
            {
                d[i] = Randomize(d[i]);
            }

        }



        /// <summary>
        /// Randomize the 2d array based on an array, modify the array. Previous 
        /// values may be used, or they may be discarded, depending on the 
        /// randomizer.
        /// </summary>
        /// <param name="d">An array to randomize.</param>
        public virtual void Randomize(double[][] d)
        {
            for (int r = 0; r < d.Length; r++)
            {
                for (int c = 0; c < d[0].Length; c++)
                {
                    d[r][c] = Randomize(d[r][c]);
                }
            }

        }

        /// <summary>
        /// Randomize the matrix based on an array, modify the array. Previous values
        /// may be used, or they may be discarded, depending on the randomizer.
        /// </summary>
        /// <param name="m">A matrix to randomize.</param>
        public virtual void Randomize(Matrix m)
        {
            double[][] mData = m.Data;
            for (int r = 0; r < m.Rows; r++)
            {
                for (int c = 0; c < m.Cols; c++)
                {
                    mData[r][c] = Randomize(mData[r][c]);
                }
            }
        }

        /// <summary>
        /// Starting with the specified number, randomize it to the degree specified
        /// by this randomizer. This could be a totally new random number, or it
        /// could be based on the specified number.
        /// </summary>
        /// <param name="d">The number to randomize.</param>
        /// <returns>A randomized number.</returns>
        abstract public double Randomize(double d);


        /// <summary>
        /// Randomize a synapse, only randomize those connections that are actually connected.
        /// </summary>
        /// <param name="network">The network the synapse belongs to.</param>
        /// <param name="synapse">The synapse to randomize.</param>
        public void Randomize(BasicNetwork network, ISynapse synapse)
        {
            if (synapse.WeightMatrix != null)
            {
                bool limited = network.Structure.IsConnectionLimited;
                double[][] d = synapse.WeightMatrix.Data;
                for (int fromNeuron = 0; fromNeuron < synapse.WeightMatrix.Rows; fromNeuron++)
                {
                    for (int toNeuron = 0; toNeuron < synapse.WeightMatrix.Cols; toNeuron++)
                    {
                        if (!limited || network.IsConnected(synapse, fromNeuron, toNeuron))
                            d[fromNeuron][toNeuron] = Randomize(d[fromNeuron][toNeuron]);
                    }
                }

            }
        }

    }

}
