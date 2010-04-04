using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic.Innovation;
using Encog.Neural.Networks.Synapse.NEAT;
using Encog.Persist.Attributes;

namespace Encog.Neural.Networks.Training.NEAT
{
    public class NEATInnovation : BasicInnovation
    {
        /// <summary>
        /// The from neuron id.
        /// </summary>
        [EGAttribute]
        private long fromNeuronID;

        /// <summary>
        /// The type of innovation.
        /// </summary>
        [EGAttribute]
        private NEATInnovationType innovationType;

        /// <summary>
        /// The neuron id.
        /// </summary>
        [EGAttribute]
        private long neuronID;

        /// <summary>
        /// The type of neuron, or none, if this is a link innovation.
        /// </summary>
        [EGAttribute]
        private NEATNeuronType neuronType;

        /// <summary>
        /// The split x property.
        /// </summary>
        [EGAttribute]
        private double splitX;

        /// <summary>
        /// The split y property.
        /// </summary>
        [EGAttribute]
        private double splitY;

        /// <summary>
        /// The to neuron's id.
        /// </summary>
        [EGAttribute]
        private long toNeuronID;

        /// <summary>
        /// Construct an innovation. 
        /// </summary>
        /// <param name="fromNeuronID">The from neuron.</param>
        /// <param name="toNeuronID">The two neuron.</param>
        /// <param name="innovationType">The innovation type.</param>
        /// <param name="innovationID">The innovation id.</param>
        public NEATInnovation(long fromNeuronID, long toNeuronID,
                NEATInnovationType innovationType, long innovationID)
        {

            this.fromNeuronID = fromNeuronID;
            this.toNeuronID = toNeuronID;
            this.innovationType = innovationType;
            InnovationID = innovationID;

            neuronID = -1;
            splitX = 0;
            splitY = 0;
            neuronType = NEATNeuronType.None;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NEATInnovation()
        {
        }


        /// <summary>
        /// Construct an innovation. 
        /// </summary>
        /// <param name="fromNeuronID">The from neuron.</param>
        /// <param name="toNeuronID">The to neuron.</param>
        /// <param name="innovationType">The innovation type.</param>
        /// <param name="innovationID">The innovation id.</param>
        /// <param name="neuronType">The neuron type.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public NEATInnovation(long fromNeuronID, long toNeuronID,
                NEATInnovationType innovationType, long innovationID,
                NEATNeuronType neuronType, double x, double y)
        {
            this.fromNeuronID = fromNeuronID;
            this.toNeuronID = toNeuronID;
            this.innovationType = innovationType;
            InnovationID = innovationID;
            this.neuronType = neuronType;
            splitX = x;
            splitY = y;

            neuronID = 0;
        }

        /// <summary>
        /// Construct an innovation. 
        /// </summary>
        /// <param name="neuronGene">The neuron gene.</param>
        /// <param name="innovationID">The innovation id.</param>
        /// <param name="neuronID">The neuron id.</param>
        public NEATInnovation(NEATNeuronGene neuronGene,
                long innovationID, long neuronID)
        {

            this.neuronID = neuronID;
            InnovationID = innovationID;
            splitX = neuronGene.SplitX;
            splitY = neuronGene.SplitY;

            neuronType = neuronGene.NeuronType;
            innovationType = NEATInnovationType.NewNeuron;
            fromNeuronID = -1;
            toNeuronID = -1;
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
        /// The innovation type.
        /// </summary>
        public NEATInnovationType InnovationType
        {
            get
            {
                return innovationType;
            }
        }

        /// <summary>
        /// The neuron ID.
        /// </summary>
        public long NeuronID
        {
            get
            {
                return neuronID;
            }
            set
            {
                this.neuronID = value;
            }
        }

        /// <summary>
        /// The neuron type.
        /// </summary>
        public NEATNeuronType NeuronType
        {
            get
            {
                return neuronType;
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
        /// The to neuron id.
        /// </summary>
        public long ToNeuronID
        {
            get
            {
                return toNeuronID;
            }
        }

        public String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[NeatInnovation:type=");
            switch (this.innovationType)
            {
                case NEATInnovationType.NewLink:
                    result.Append("link");
                    break;
                case NEATInnovationType.NewNeuron:
                    result.Append("neuron");
                    break;
            }
            result.Append(",from=");
            result.Append(this.fromNeuronID);
            result.Append(",to=");
            result.Append(this.toNeuronID);
            result.Append(",splitX=");
            result.Append(this.splitX);
            result.Append(",splitY=");
            result.Append(this.splitY);
            result.Append("]");
            return result.ToString();
        }
    }
}
