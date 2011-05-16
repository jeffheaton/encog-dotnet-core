using System;
using System.Text;
using Encog.ML.Genetic.Innovation;
using Encog.Neural.Neat.Training;

namespace Encog.Neural.NEAT.Training
{
    /// <summary>
    /// Implements a NEAT innovation. This lets NEAT track what changes it has
    /// previously tried with a neural network.
    /// NeuroEvolution of Augmenting Topologies (NEAT) is a genetic algorithm for the
    /// generation of evolving artificial neural networks. It was developed by Ken
    /// Stanley while at The University of Texas at Austin.
    /// http://www.cs.ucf.edu/~kstanley/
    /// </summary>
    ///
    [Serializable]
    public class NEATInnovation : BasicInnovation
    {
        /// <summary>
        /// Serial id.
        /// </summary>
        ///
        private const long serialVersionUID = 1L;

        /// <summary>
        /// The from neuron id.
        /// </summary>
        ///
        private long fromNeuronID;

        /// <summary>
        /// The type of innovation.
        /// </summary>
        ///
        private NEATInnovationType innovationType;

        /// <summary>
        /// The neuron id.
        /// </summary>
        ///
        private long neuronID;

        /// <summary>
        /// The type of neuron, or none, if this is a link innovation.
        /// </summary>
        ///
        private NEATNeuronType neuronType;

        /// <summary>
        /// The split x property.
        /// </summary>
        ///
        private double splitX;

        /// <summary>
        /// The split y property.
        /// </summary>
        ///
        private double splitY;

        /// <summary>
        /// The to neuron's id.
        /// </summary>
        ///
        private long toNeuronID;

        /// <summary>
        /// Default constructor, used mainly for persistence.
        /// </summary>
        ///
        public NEATInnovation()
        {
        }

        /// <summary>
        /// Construct an innovation.
        /// </summary>
        ///
        /// <param name="fromNeuronID_0">The from neuron.</param>
        /// <param name="toNeuronID_1">The two neuron.</param>
        /// <param name="innovationType_2">The innovation type.</param>
        /// <param name="innovationID">The innovation id.</param>
        public NEATInnovation(long fromNeuronID_0, long toNeuronID_1,
                              NEATInnovationType innovationType_2, long innovationID)
        {
            fromNeuronID = fromNeuronID_0;
            toNeuronID = toNeuronID_1;
            innovationType = innovationType_2;
            InnovationID = innovationID;

            neuronID = -1;
            splitX = 0;
            splitY = 0;
            neuronType = NEATNeuronType.None;
        }

        /// <summary>
        /// Construct an innovation.
        /// </summary>
        ///
        /// <param name="fromNeuronID_0">The from neuron.</param>
        /// <param name="toNeuronID_1">The to neuron.</param>
        /// <param name="innovationType_2">The innovation type.</param>
        /// <param name="innovationID">The innovation id.</param>
        /// <param name="neuronType_3">The neuron type.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">THe y coordinate.</param>
        public NEATInnovation(long fromNeuronID_0, long toNeuronID_1,
                              NEATInnovationType innovationType_2, long innovationID,
                              NEATNeuronType neuronType_3, double x, double y)
        {
            fromNeuronID = fromNeuronID_0;
            toNeuronID = toNeuronID_1;
            innovationType = innovationType_2;
            InnovationID = innovationID;
            neuronType = neuronType_3;
            splitX = x;
            splitY = y;

            neuronID = 0;
        }

        /// <summary>
        /// Construct an innovation.
        /// </summary>
        ///
        /// <param name="neuronGene">The neuron gene.</param>
        /// <param name="innovationID">The innovation id.</param>
        /// <param name="neuronID_0">The neuron id.</param>
        public NEATInnovation(NEATNeuronGene neuronGene,
                              long innovationID, long neuronID_0)
        {
            neuronID = neuronID_0;
            InnovationID = innovationID;
            splitX = neuronGene.SplitX;
            splitY = neuronGene.SplitY;

            neuronType = neuronGene.NeuronType;
            innovationType = NEATInnovationType.NewNeuron;
            fromNeuronID = -1;
            toNeuronID = -1;
        }


        /// <value>the fromNeuronID to set</value>
        public long FromNeuronID
        {
            /// <returns>The from neuron id.</returns>
            get { return fromNeuronID; }
            /// <param name="fromNeuronID_0">the fromNeuronID to set</param>
            set { fromNeuronID = value; }
        }


        public NEATInnovationType InnovationType
        {
            /// <returns>The innovation type.</returns>
            get { return innovationType; }
            set { innovationType = value; }
        }


        /// <summary>
        /// Set the neuron id.
        /// </summary>
        ///
        /// <value>The neuron id.</value>
        public long NeuronID
        {
            /// <returns>The neuron ID.</returns>
            get { return neuronID; }
            /// <summary>
            /// Set the neuron id.
            /// </summary>
            ///
            /// <param name="neuronID_0">The neuron id.</param>
            set { neuronID = value; }
        }


        public NEATNeuronType NeuronType
        {
            /// <returns>The neuron type.</returns>
            get { return neuronType; }
            set { neuronType = value; }
        }


        public double SplitX
        {
            /// <returns>The split x.</returns>
            get { return splitX; }
            set { splitX = value; }
        }


        public double SplitY
        {
            /// <returns>The split y.</returns>
            get { return splitY; }
            set { splitY = value; }
        }


        /// <value>the toNeuronID to set</value>
        public long ToNeuronID
        {
            /// <returns>The to neuron id.</returns>
            get { return toNeuronID; }
            /// <param name="toNeuronID_0">the toNeuronID to set</param>
            set { toNeuronID = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[NeatInnovation:type=");
            switch (innovationType)
            {
                case NEATInnovationType.NewLink:
                    result.Append("link");
                    break;
                case NEATInnovationType.NewNeuron:
                    result.Append("neuron");
                    break;
            }
            result.Append(",from=");
            result.Append(fromNeuronID);
            result.Append(",to=");
            result.Append(toNeuronID);
            result.Append(",splitX=");
            result.Append(splitX);
            result.Append(",splitY=");
            result.Append(splitY);
            result.Append("]");
            return result.ToString();
        }
    }
}