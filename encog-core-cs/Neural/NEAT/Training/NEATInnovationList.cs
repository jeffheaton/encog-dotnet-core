using System;
using Encog.ML.Genetic.Genes;
using Encog.ML.Genetic.Genome;
using Encog.ML.Genetic.Innovation;
using Encog.ML.Genetic.Population;
using Encog.Neural.Neat.Training;
using Encog.Neural.Networks.Training;

namespace Encog.Neural.NEAT.Training
{
    /// <summary>
    /// Implements a NEAT innovation list.
    /// NeuroEvolution of Augmenting Topologies (NEAT) is a genetic algorithm for the
    /// generation of evolving artificial neural networks. It was developed by Ken
    /// Stanley while at The University of Texas at Austin.
    /// http://www.cs.ucf.edu/~kstanley/
    /// </summary>
    ///
    [Serializable]
    public class NEATInnovationList : BasicInnovationList
    {
        /// <summary>
        /// The next neuron id.
        /// </summary>
        ///
        private long nextNeuronID;

        /// <summary>
        /// The population.
        /// </summary>
        ///
        private IPopulation population;

        /// <summary>
        /// The default constructor, used mainly for persistance.
        /// </summary>
        ///
        public NEATInnovationList()
        {
            nextNeuronID = 0;
        }

        /// <summary>
        /// Construct an innovation list.
        /// </summary>
        ///
        /// <param name="population_0">The population.</param>
        /// <param name="links">The links.</param>
        /// <param name="neurons">THe neurons.</param>
        public NEATInnovationList(IPopulation population_0,
                                  Chromosome links, Chromosome neurons)
        {
            nextNeuronID = 0;
            population = population_0;

            foreach (IGene gene  in  neurons.Genes)
            {
                var neuronGene = (NEATNeuronGene) gene;

                var innovation = new NEATInnovation(neuronGene,
                                                    population_0.AssignInnovationID(), AssignNeuronID());
                Add(innovation);
            }


            foreach (IGene gene_1  in  links.Genes)
            {
                var linkGene = (NEATLinkGene) gene_1;
                var innovation_2 = new NEATInnovation(
                    linkGene.FromNeuronID, linkGene.ToNeuronID,
                    NEATInnovationType.NewLink,
                    population.AssignInnovationID());
                Add(innovation_2);
            }
        }

        /// <summary>
        /// The population.
        /// </summary>
        public NEATPopulation Population
        {
            set { population = value; }
        }

        /// <summary>
        /// Assign a neuron ID.
        /// </summary>
        ///
        /// <returns>The neuron id.</returns>
        private long AssignNeuronID()
        {
            return nextNeuronID++;
        }

        /// <summary>
        /// Check to see if we already have an innovation.
        /// </summary>
        ///
        /// <param name="ins0">The input neuron.</param>
        /// <param name="xout">THe output neuron.</param>
        /// <param name="type">The type.</param>
        /// <returns>The innovation, either new or existing if found.</returns>
        public NEATInnovation CheckInnovation(long ins0, long xout,
                                              NEATInnovationType type)
        {
            foreach (IInnovation i  in  Innovations)
            {
                var innovation = (NEATInnovation) i;
                if ((innovation.FromNeuronID == ins0)
                    && (innovation.ToNeuronID == xout)
                    && (innovation.InnovationType == type))
                {
                    return innovation;
                }
            }

            return null;
        }

        /// <summary>
        /// Create a new neuron gene from an id.
        /// </summary>
        ///
        /// <param name="neuronID">The neuron id.</param>
        /// <returns>The neuron gene.</returns>
        public NEATNeuronGene CreateNeuronFromID(long neuronID)
        {
            var result = new NEATNeuronGene(NEATNeuronType.Hidden,
                                            0, 0, 0);


            foreach (IInnovation i  in  Innovations)
            {
                var innovation = (NEATInnovation) i;
                if (innovation.NeuronID == neuronID)
                {
                    result.NeuronType = innovation.NeuronType;
                    result.Id = innovation.NeuronID;
                    result.SplitY = innovation.SplitY;
                    result.SplitX = innovation.SplitX;

                    return result;
                }
            }

            throw new TrainingError("Failed to find innovation for neuron: "
                                    + neuronID);
        }

        /// <summary>
        /// Create a new innovation.
        /// </summary>
        ///
        /// <param name="ins0">The input neuron.</param>
        /// <param name="xout">The output neuron.</param>
        /// <param name="type">The type.</param>
        public void CreateNewInnovation(long ins0, long xout,
                                        NEATInnovationType type)
        {
            var newInnovation = new NEATInnovation(ins0, xout, type,
                                                   population.AssignInnovationID());

            if (type == NEATInnovationType.NewNeuron)
            {
                newInnovation.NeuronID = AssignNeuronID();
            }

            Add(newInnovation);
        }

        /// <summary>
        /// Create a new innovation.
        /// </summary>
        ///
        /// <param name="from">The from neuron.</param>
        /// <param name="to">The to neuron.</param>
        /// <param name="innovationType">THe innovation type.</param>
        /// <param name="neuronType">The neuron type.</param>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <returns>The new innovation.</returns>
        public long CreateNewInnovation(long from, long to,
                                        NEATInnovationType innovationType,
                                        NEATNeuronType neuronType, double x, double y)
        {
            var newInnovation = new NEATInnovation(from, to,
                                                   innovationType, population.AssignInnovationID(),
                                                   neuronType, x, y);

            if (innovationType == NEATInnovationType.NewNeuron)
            {
                newInnovation.NeuronID = AssignNeuronID();
            }

            Add(newInnovation);

            return (nextNeuronID - 1); // ??????? should it be innov?
        }
    }
}