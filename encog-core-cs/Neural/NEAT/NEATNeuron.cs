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
        public const String NEURON_ID = "neuronID";
        public const String ACTIVATION_RESPONSE = "aresp";

        /// <summary>
        /// The serial id.
        /// </summary>
        ///
        private const long serialVersionUID = -2815145950124389743L;

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
            /// <returns>the activation response.</returns>
            get { return activationResponse; }
        }


        /// <value>the inbound links.</value>
        public IList<NEATLink> InboundLinks
        {
            /// <returns>the inbound links.</returns>
            get { return inboundLinks; }
        }


        /// <value>The neuron id.</value>
        public long NeuronID
        {
            /// <returns>The neuron id.</returns>
            get { return neuronID; }
        }


        /// <value>the neuron type.</value>
        public NEATNeuronType NeuronType
        {
            /// <returns>the neuron type.</returns>
            get { return neuronType; }
        }


        /// <summary>
        /// Set the output.
        /// </summary>
        ///
        /// <value>The output of the neuron.</value>
        public double Output
        {
            /// <returns>The output from this neuron.</returns>
            get { return output; }
            /// <summary>
            /// Set the output.
            /// </summary>
            ///
            /// <param name="output_0">The output of the neuron.</param>
            set { output = value; }
        }


        /// <value>The outbound links.</value>
        public IList<NEATLink> OutputboundLinks
        {
            /// <returns>The outbound links.</returns>
            get { return outputboundLinks; }
        }


        /// <value>The x position.</value>
        public int PosX
        {
            /// <returns>The x position.</returns>
            get { return posX; }
        }


        /// <value>The y position.</value>
        public int PosY
        {
            /// <returns>The y position.</returns>
            get { return posY; }
        }


        /// <value>The split x.</value>
        public double SplitX
        {
            /// <returns>The split x.</returns>
            get { return splitX; }
        }


        /// <value>The split y.</value>
        public double SplitY
        {
            /// <returns>The split y.</returns>
            get { return splitY; }
        }


        /// <value>The sum activation.</value>
        public double SumActivation
        {
            /// <returns>The sum activation.</returns>
            get { return sumActivation; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
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