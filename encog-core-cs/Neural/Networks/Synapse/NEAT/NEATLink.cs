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
using Encog.Persist.Attributes;

namespace Encog.Neural.Networks.Synapse.NEAT
{
    /// <summary>
    /// Implements a link between two NEAT neurons.
    ///
    /// NeuroEvolution of Augmenting Topologies (NEAT) is a genetic algorithm for the
    /// generation of evolving artificial neural networks. It was developed by Ken
    /// Stanley while at The University of Texas at Austin.
    /// 
    /// http://www.cs.ucf.edu/~kstanley/
    /// </summary>

    public class NEATLink
    {
        /// <summary>
        /// The source neuron.
        /// </summary>
        [EGReference]
        private NEATNeuron fromNeuron;

        /// <summary>
        /// Is this link recurrent.
        /// </summary>
        [EGAttribute]
        private bool recurrent;

        /// <summary>
        /// The target neuron.
        /// </summary>
        [EGReference]
        private NEATNeuron toNeuron;

        /// <summary>
        /// The weight between the two neurons.
        /// </summary>
        [EGAttribute]
        private double weight;

        
        /// <summary>
        /// Construct a NEAT link. 
        /// </summary>
        /// <param name="weight">The weight between the two neurons.</param>
        /// <param name="fromNeuron">The source neuron.</param>
        /// <param name="toNeuron">The target neuron.</param>
        /// <param name="recurrent">Is this a recurrent link.</param>
        public NEATLink(double weight, NEATNeuron fromNeuron,
                NEATNeuron toNeuron, bool recurrent)
        {
            this.weight = weight;
            this.fromNeuron = fromNeuron;
            this.toNeuron = toNeuron;
            this.recurrent = recurrent;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NEATLink()
        {
        }

        /// <summary>
        /// The source neuron.
        /// </summary>
        public NEATNeuron FromNeuron
        {
            get
            {
                return fromNeuron;
            }
        }

        /// <summary>
        /// The target neuron.
        /// </summary>
        public NEATNeuron ToNeuron
        {
            get
            {
                return toNeuron;
            }
        }

        /// <summary>
        /// The weight of the link.
        /// </summary>
        public double Weight
        {
            get
            {
                return weight;
            }
        }

        /// <summary>
        /// True if this is a recurrent link.
        /// </summary>
        public bool IsRecurrent
        {
            get
            {
                return recurrent;
            }
        }
    }
}
