using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic.Innovation;
using Encog.Neural.Networks.Synapse.NEAT;

namespace Encog.Neural.Networks.Training.NEAT
{
    public class NEATInnovation : BasicInnovation
    {
        /// <summary>
        /// The from neuron id.
        /// </summary>
        private long fromNeuronID;

        /// <summary>
        /// The type of innovation.
        /// </summary>
        private NEATInnovationType innovationType;

        /// <summary>
        /// The neuron id.
        /// </summary>
        private long neuronID;

        /// <summary>
        /// The type of neuron, or none, if this is a link innovation.
        /// </summary>
        private NEATNeuronType neuronType;

        /// <summary>
        /// The split x property.
        /// </summary>
        private double splitX;

        /// <summary>
        /// The split y property.
        /// </summary>
        private double splitY;

        /// <summary>
        /// The to neuron's id.
        /// </summary>
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
    }
}
