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
    /// Implements a NEAT neuron.  Neat neurons are of a specific type, defined by the
    /// NEATNeuronType enum.  Usually NEAT uses a sigmoid activation function.  The
    /// activation response is used to allow the slope of the sigmoid to be evolved.
    /// 
    /// NeuroEvolution of Augmenting Topologies (NEAT) is a genetic algorithm for the
    /// generation of evolving artificial neural networks. It was developed by Ken
    /// Stanley while at The University of Texas at Austin.
    /// 
    /// http://www.cs.ucf.edu/~kstanley/
    /// </summary>
    [EGReferenceable]
    public class NEATNeuron
    {
        /// <summary>
        /// The activation response. This is evolved to allow NEAT to scale the slope
        /// of the activation function.
        /// </summary>
        [EGAttribute]
        private double activationResponse;

        /// <summary>
        /// Inbound links to this neuron.
        /// </summary>
        private IList<NEATLink> inboundLinks = new List<NEATLink>();

        /// <summary>
        /// The neuron id.
        /// </summary>
        [EGAttribute]
        private long neuronID;

        /// <summary>
        /// The type of neuron this is.
        /// </summary>
        [EGAttribute]
        private NEATNeuronType neuronType;

        /// <summary>
        /// The output from the neuron.
        /// </summary>
        [EGAttribute]
        private double output;

        /// <summary>
        /// The outbound links for this neuron.
        /// </summary>
        private IList<NEATLink> outputboundLinks = new List<NEATLink>();

        /// <summary>
        /// The x-position of this neuron.  Used to split links, as well as display.
        /// </summary>
        [EGAttribute]
        private int posX;

        /// <summary>
        /// The y-position of this neuron.  Used to split links, as well as display.
        /// </summary>
        [EGAttribute]
        private int posY;

        /// <summary>
        /// The split value for X.  Used to track splits.
        /// </summary>
        [EGAttribute]
        private double splitX;

        /// <summary>
        /// The split value for Y.  Used to track splits.
        /// </summary>
        [EGAttribute]
        private double splitY;

        /// <summary>
        /// The sum activation.
        /// </summary>
        [EGAttribute]
        private double sumActivation;

        /// <summary>
        /// Construct a NEAT neuron. 
        /// </summary>
        /// <param name="neuronType">The type of neuron.</param>
        /// <param name="neuronID">The id of the neuron.</param>
        /// <param name="splitY">The split for y.</param>
        /// <param name="splitX">THe split for x.</param>
        /// <param name="activationResponse">The activation response.</param>
        public NEATNeuron(NEATNeuronType neuronType, long neuronID,
                 double splitY, double splitX,
                 double activationResponse)
        {
            this.neuronType = neuronType;
            this.neuronID = neuronID;
            this.splitY = splitY;
            this.splitX = splitX;
            this.activationResponse = activationResponse;
            posX = 0;
            posY = 0;
            output = 0;
            sumActivation = 0;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NEATNeuron()
        {
        }

        /// <summary>
        /// the activation response.
        /// </summary>
        public double ActivationResponse
        {
            get
            {
                return activationResponse;
            }
        }

        /// <summary>
        /// the inbound links.
        /// </summary>
        public IList<NEATLink> InboundLinks
        {
            get
            {
                return inboundLinks;
            }
        }

        /// <summary>
        /// The neuron id.
        /// </summary>
        public long NeuronID
        {
            get
            {
                return neuronID;
            }
        }

        /// <summary>
        /// The neuron type.
        /// </summary>
        /// <returns>The neuron type.</returns>
        public NEATNeuronType NeuronType
        {
            get
            {
                return neuronType;
            }
        }

        /// <summary>
        /// The output from this neuron.
        /// </summary>
        public double Output
        {
            get
            {
                return output;
            }
            set
            {
                this.output = value;
            }
        }

        /// <summary>
        /// The outbound links.
        /// </summary>
        public IList<NEATLink> OutputboundLinks
        {
            get
            {
                return outputboundLinks;
            }
        }

        /// <summary>
        /// The x position.
        /// </summary>
        public int PosX
        {
            get
            {
                return posX;
            }
        }

        /// <summary>
        /// The y position.
        /// </summary>
        public int PosY
        {
            get
            {
                return posY;
            }
        }

        /// <summary>
        /// The split x.
        /// </summary>
        public double SplitX
        {
            get
            {
            return splitX;
            }
        }

        /// <summary>
        /// The split y.
        /// </summary>
        public double SplitY
        {
            get
            {
                return splitY;
            }
        }

        /// <summary>
        /// Get the sum activation.
        /// </summary>
        /// <returns>The sum activation.</returns>
        public double getSumActivation()
        {
            return sumActivation;
        }

        /// <summary>
        /// Convert the string to a neuron.
        /// </summary>
        /// <returns>A string representation of the neuron.</returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[NEATNeuron:id=");
            result.Append(neuronID);
            result.Append(",type=");
            switch (neuronType)
            {
                case NEATNeuronType.Input:
                    result.Append("I");
                    break;
                case NEATNeuronType.Output:
                    result.Append("O");
                    break;
                case NEATNeuronType.Bias:
                    result.Append("B");
                    break;
                case NEATNeuronType.Hidden:
                    result.Append("H");
                    break;
            }
            result.Append("]");
            return result.ToString();
        }
    }
}
