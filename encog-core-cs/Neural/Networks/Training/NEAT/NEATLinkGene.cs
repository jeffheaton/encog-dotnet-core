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
using Encog.Solve.Genetic.Genes;
using Encog.Persist.Attributes;

namespace Encog.Neural.Networks.Training.NEAT
{
    /// <summary>
    /// Implements a NEAT link gene. This describes a way in which two neurons
    /// are linked. 
    /// 
    /// NeuroEvolution of Augmenting Topologies (NEAT) is a genetic algorithm for the
    /// generation of evolving artificial neural networks. It was developed by Ken
    /// Stanley while at The University of Texas at Austin.
    /// 
    /// http://www.cs.ucf.edu/~kstanley/
    /// </summary>
    public class NEATLinkGene : BasicGene
    {
        /// <summary>
        /// The from neuron id.
        /// </summary>
        [EGAttribute]
        private long fromNeuronID;

        /// <summary>
        /// Is this a recurrent connection.
        /// </summary>
        [EGAttribute]
        private bool recurrent;

        /// <summary>
        /// The to neuron id.
        /// </summary>
        [EGAttribute]
        private long toNeuronID;

        private double weight;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NEATLinkGene()
        {
        }

        /// <summary>
        /// Construct a link gene.
        /// </summary>
        /// <param name="fromNeuronID">The from neuron's id.</param>
        /// <param name="toNeuronID">The to neuron's id.</param>
        /// <param name="enabled">True, if this link is enabled.</param>
        /// <param name="innovationID">The innovation id.</param>
        /// <param name="weight">The weight of this link.</param>
        /// <param name="recurrent">True, if this is a recurrent link.</param>
        public NEATLinkGene(long fromNeuronID, long toNeuronID,
                bool enabled, long innovationID,
                double weight, bool recurrent)
        {
            this.fromNeuronID = fromNeuronID;
            this.toNeuronID = toNeuronID;
            this.Enabled = enabled;
            this.InnovationId = innovationID;
            this.weight = weight;
            this.recurrent = recurrent;
        }

        /// <summary>
        /// Copy from another gene. 
        /// </summary>
        /// <param name="gene">The other gene.</param>
        public override void Copy(IGene gene)
        {
            NEATLinkGene other = (NEATLinkGene)gene;
            this.Enabled = other.Enabled;
            fromNeuronID = other.fromNeuronID;
            toNeuronID = other.toNeuronID;
            this.InnovationId = other.InnovationId;
            recurrent = other.recurrent;
            weight = other.weight;
        }

        /// <summary>
        /// The from neuron id.
        /// </summary>
        public long FromNeuronID
        {
            get
            {
                return fromNeuronID;
            }
        }

        /// <summary>
        /// The to neuron id.
        /// </summary>
        public long ToNeuronID
        {
            get
            {
                return toNeuronID;
            }
        }

        /// <summary>
        /// The weight of this connection.
        /// </summary>
        public double Weight
        {
            get
            {
                return weight;
            }
            set
            {
                this.weight = value;
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


        /// <summary>
        /// This link as a string.
        /// </summary>
        /// <returns>This link as a string.</returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[NEATLinkGene:innov=");
            result.Append(InnovationId);
            result.Append(",enabled=");
            result.Append(Enabled);
            result.Append(",from=");
            result.Append(fromNeuronID);
            result.Append(",to=");
            result.Append(toNeuronID);
            result.Append("]");
            return result.ToString();
        }
    }
}
