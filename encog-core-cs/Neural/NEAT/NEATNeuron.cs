//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.Collections.Generic;
using System.Text;

namespace Encog.Neural.NEAT
{
    /// <summary>
    /// Implements a NEAT neuron. Neat neurons are of a specific type, defined by the
    /// NEATNeuronType enum. Usually NEAT uses a sigmoid activation function. The
    /// activation response is used to allow the slope of the sigmoid to be evolved.
    /// NeuroEvolution of Augmenting Topologies (NEAT) is a genetic algorithm for the
    /// generation of evolving artificial neural networks. It was developed by Ken
    /// Stanley while at The University of Texas at Austin.
    /// http://www.cs.ucf.edu/~kstanley/
    /// </summary>
    ///
    [Serializable]
    public class NEATNeuron
    {
        /// <summary>
        /// The neuron id property.
        /// </summary>
        public const String NEURON_ID = "neuronID";

        /// <summary>
        /// The activation response property.
        /// </summary>
        public const String ACTIVATION_RESPONSE = "aresp";

        /// <summary>
        /// The activation response. This is evolved to allow NEAT to scale the slope
        /// of the activation function.
        /// </summary>
        ///
        private readonly double activationResponse;

        /// <summary>
        /// Inbound links to this neuron.
        /// </summary>
        ///
        private readonly IList<NEATLink> inboundLinks;

        /// <summary>
        /// The neuron id.
        /// </summary>
        ///
        private readonly long neuronID;

        /// <summary>
        /// The type of neuron this is.
        /// </summary>
        ///
        private readonly NEATNeuronType neuronType;

        /// <summary>
        /// The outbound links for this neuron.
        /// </summary>
        ///
        private readonly IList<NEATLink> outputboundLinks;

        /// <summary>
        /// The x-position of this neuron. Used to split links, as well as display.
        /// </summary>
        ///
        private readonly int posX;

        /// <summary>
        /// The y-position of this neuron. Used to split links, as well as display.
        /// </summary>
        ///
        private readonly int posY;

        /// <summary>
        /// The split value for X. Used to track splits.
        /// </summary>
        ///
        private readonly double splitX;

        /// <summary>
        /// The split value for Y. Used to track splits.
        /// </summary>
        ///
        private readonly double splitY;

        /// <summary>
        /// The sum activation.
        /// </summary>
        ///
        private readonly double sumActivation;

        /// <summary>
        /// The output from the neuron.
        /// </summary>
        ///
        private double output;

        /// <summary>
        /// Default constructor, used for persistance.
        /// </summary>
        ///
        public NEATNeuron()
        {
            inboundLinks = new List<NEATLink>();
            outputboundLinks = new List<NEATLink>();
        }

        /// <summary>
        /// Construct a NEAT neuron.
        /// </summary>
        ///
        /// <param name="neuronType_0">The type of neuron.</param>
        /// <param name="neuronID_1">The id of the neuron.</param>
        /// <param name="splitY_2">The split for y.</param>
        /// <param name="splitX_3">THe split for x.</param>
        /// <param name="activationResponse_4">The activation response.</param>
        public NEATNeuron(NEATNeuronType neuronType_0, long neuronID_1,
                          double splitY_2, double splitX_3,
                          double activationResponse_4)
        {
            inboundLinks = new List<NEATLink>();
            outputboundLinks = new List<NEATLink>();
            neuronType = neuronType_0;
            neuronID = neuronID_1;
            splitY = splitY_2;
            splitX = splitX_3;
            activationResponse = activationResponse_4;
            posX = 0;
            posY = 0;
            output = 0;
            sumActivation = 0;
        }


        /// <value>the activation response.</value>
        public double ActivationResponse
        {
            get { return activationResponse; }
        }


        /// <value>the inbound links.</value>
        public IList<NEATLink> InboundLinks
        {
            get { return inboundLinks; }
        }


        /// <value>The neuron id.</value>
        public long NeuronID
        {
            get { return neuronID; }
        }


        /// <value>the neuron type.</value>
        public NEATNeuronType NeuronType
        {
            get { return neuronType; }
        }


        /// <value>The output of the neuron.</value>
        public double Output
        {
            get { return output; }
            set { output = value; }
        }


        /// <value>The outbound links.</value>
        public IList<NEATLink> OutputboundLinks
        {
            get { return outputboundLinks; }
        }


        /// <value>The x position.</value>
        public int PosX
        {
            get { return posX; }
        }


        /// <value>The y position.</value>
        public int PosY
        {
            get { return posY; }
        }


        /// <value>The split x.</value>
        public double SplitX
        {
            get { return splitX; }
        }


        /// <value>The split y.</value>
        public double SplitY
        {
            get { return splitY; }
        }


        /// <value>The sum activation.</value>
        public double SumActivation
        {
            get { return sumActivation; }
        }


        /// <inheritdoc/>
        public override String ToString()
        {
            var result = new StringBuilder();
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
                default:
                    result.Append("Unknown");
                    break;
            }
            result.Append("]");
            return result.ToString();
        }

        /// <summary>
        /// Convert a string to a NEAT neuron type.
        /// </summary>
        /// <param name="t">The string.</param>
        /// <returns>The NEAT neuron type.</returns>
        public static NEATNeuronType String2NeuronType(String t)
        {
            String type = t.ToLower().Trim();

            if (type.Length > 0)
            {
                switch ((int) type[0])
                {
                    case 'i':
                        return NEATNeuronType.Input;
                    case 'o':
                        return NEATNeuronType.Output;
                    case 'h':
                        return NEATNeuronType.Hidden;
                    case 'b':
                        return NEATNeuronType.Bias;
                    case 'n':
                        return NEATNeuronType.None;
                }
            }

            return default(NEATNeuronType) /* was: null */;
        }

        /// <summary>
        /// Convert NEAT neuron type to string.
        /// </summary>
        /// <param name="t">The neuron type.</param>
        /// <returns>The string of the specified neuron type.</returns>
        public static String NeuronType2String(NEATNeuronType t)
        {
            switch (t)
            {
                case NEATNeuronType.Input:
                    return "I";
                case NEATNeuronType.Bias:
                    return "B";
                case NEATNeuronType.Hidden:
                    return "H";
                case NEATNeuronType.Output:
                    return "O";
                case NEATNeuronType.None:
                    return "N";
                default:
                    return null;
            }
        }
    }
}
