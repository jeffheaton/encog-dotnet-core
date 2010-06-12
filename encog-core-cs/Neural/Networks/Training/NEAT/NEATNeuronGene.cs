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
using Encog.Neural.Networks.Synapse.NEAT;
using Encog.Solve.Genetic.Genes;
using Encog.Persist.Attributes;

namespace Encog.Neural.Networks.Training.NEAT
{
    /// <summary>
    /// Implements a NEAT neuron gene.  
    /// 
    /// NeuroEvolution of Augmenting Topologies (NEAT) is a genetic algorithm for the
    /// generation of evolving artificial neural networks. It was developed by Ken
    /// Stanley while at The University of Texas at Austin.
    /// 
    /// http://www.cs.ucf.edu/~kstanley/
    /// </summary>
    public class NEATNeuronGene: BasicGene
    {

        /// <summary>
        /// The activation response, the slope of the activation function.
        /// </summary>
        [@EGAttribute]
        private double activationResponse;

        /// <summary>
        /// The neuron type.
        /// </summary>
        [@EGAttribute]
        private NEATNeuronType neuronType;

        /// <summary>
        /// True if this is recurrent.
        /// </summary>
        [@EGAttribute]
        private bool recurrent;

        /// <summary>
        /// The x-split.
        /// </summary>
        [@EGAttribute]
        private double splitX;

        /// <summary>
        /// The y-split.
        /// </summary>
        [@EGAttribute]
        private double splitY;

        
        /// <summary>
        /// Construct a gene. 
        /// </summary>
        /// <param name="type">The type of neuron.</param>
        /// <param name="id">The id of this gene.</param>
        /// <param name="splitY">The split y.</param>
        /// <param name="splitX">The split x.</param>
        public NEATNeuronGene(NEATNeuronType type, long id,
                double splitY, double splitX)
            : this(type, id, splitY, splitX, false, 1.0)
        {

        }


        /// <summary>
        /// Construct a neuron gene. 
        /// </summary>
        /// <param name="type">The type of neuron.</param>
        /// <param name="id">The id of this gene.</param>
        /// <param name="splitY">The split y.</param>
        /// <param name="splitX">The split x.</param>
        /// <param name="recurrent">True if this is a recurrent link.</param>
        /// <param name="act">The activation response.</param>
        public NEATNeuronGene(NEATNeuronType type, long id,
                double splitY, double splitX, bool recurrent,
                double act)
        {
            neuronType = type;
            this.Id = id;
            this.splitX = splitX;
            this.splitY = splitY;
            this.recurrent = recurrent;
            activationResponse = act;
        }

        /// <summary>
        /// Copy another gene to this one. 
        /// </summary>
        /// <param name="gene">The other gene.</param>
        public override void Copy(IGene gene)
        {
            NEATNeuronGene other = (NEATNeuronGene)gene;
            activationResponse = other.activationResponse;
            Id = other.Id;
            neuronType = other.neuronType;
            recurrent = other.recurrent;
            splitX = other.splitX;
            splitY = other.splitY;

        }

        /// <summary>
        /// The activation response.
        /// </summary>
        public double ActivationResponse
        {
            get
            {
                return activationResponse;
            }
            set
            {
                this.activationResponse = value;
            }
        }


        /// <summary>
        /// The type for this neuron.
        /// </summary>
        public NEATNeuronType NeuronType
        {
            get
            {
                return neuronType;
            }
            set
            {
                this.neuronType = value;
            }
        }

        /// <summary>
        /// The split x value.
        /// </summary>
        public double SplitX
        {
            get
            {
                return splitX;
            }
            set
            {
                this.splitX = value;
            }
        }

        /// <summary>
        /// The split y value.
        /// </summary>
        public double SplitY
        {
            get
            {
                return splitY;
            }
            set
            {
                this.SplitY = value;
            }
        }

        /// <summary>
        /// True if this is recurrent.
        /// </summary>
        public bool Recurrent
        {
            get
            {
                return recurrent;
            }
            set
            {
                this.recurrent = value;
            }
        }
    }
}
