using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.NEAT.Training
{
    /// <summary>
    /// Implements a NEAT innovation list.
    /// 
    /// NeuroEvolution of Augmenting Topologies (NEAT) is a genetic algorithm for the
    /// generation of evolving artificial neural networks. It was developed by Ken
    /// Stanley while at The University of Texas at Austin.
    /// 
    /// -----------------------------------------------------------------------------
    /// http://www.cs.ucf.edu/~kstanley/
    /// Encog's NEAT implementation was drawn from the following three Journal
    /// Articles. For more complete BibTeX sources, see NEATNetwork.java.
    /// 
    /// Evolving Neural Networks Through Augmenting Topologies
    /// 
    /// Generating Large-Scale Neural Networks Through Discovering Geometric
    /// Regularities
    /// 
    /// Automatic feature selection in neuroevolution
    [Serializable]
    public class NEATInnovationList
    {
        /// <summary>
        /// The population.
        /// </summary>
        public NEATPopulation Population { get; set; }

        /// <summary>
        /// The list of innovations.
        /// </summary>
        private IDictionary<string, NEATInnovation> list = new Dictionary<String, NEATInnovation>();

        /// <summary>
        /// The default constructor, used mainly for persistance.
        /// </summary>
        public NEATInnovationList()
        {

        }

        /// <summary>
        /// Produce an innovation key for a neuron.
        /// </summary>
        /// <param name="id">The neuron id.</param>
        /// <returns>The newly created key.</returns>
        public static String ProduceKeyNeuron(long id)
        {
            StringBuilder result = new StringBuilder();
            result.Append("n:");
            result.Append(id);
            return result.ToString();
        }

        /// <summary>
        /// Produce a key for a split neuron.
        /// </summary>
        /// <param name="fromID"></param>
        /// <param name="toID"></param>
        /// <returns></returns>
        public static String ProduceKeyNeuronSplit(long fromID, long toID)
        {
            StringBuilder result = new StringBuilder();
            result.Append("ns:");
            result.Append(fromID);
            result.Append(":");
            result.Append(toID);
            return result.ToString();
        }

        /// <summary>
        /// Produce a key for a link.
        /// </summary>
        /// <param name="fromID">The from id.</param>
        /// <param name="toID">The to id.</param>
        /// <returns>The key for the link.</returns>
        public static String ProduceKeyLink(long fromID, long toID)
        {
            StringBuilder result = new StringBuilder();
            result.Append("l:");
            result.Append(fromID);
            result.Append(":");
            result.Append(toID);
            return result.ToString();
        }

        /// <summary>
        /// Construct an innovation list, that includes the initial innovations.
        /// </summary>
        /// <param name="population">The population to base this innovation list on.</param>
        public NEATInnovationList(NEATPopulation population)
        {

            this.Population = population;

            this.FindInnovation(Population.AssignGeneID()); // bias

            // input neurons
            for (int i = 0; i < Population.InputCount; i++)
            {
                this.FindInnovation(Population.AssignGeneID());
            }

            // output neurons
            for (int i = 0; i < Population.OutputCount; i++)
            {
                this.FindInnovation(Population.AssignGeneID());
            }

            // connections
            for (long fromID = 0; fromID < Population.InputCount + 1; fromID++)
            {
                for (long toID = 0; toID < Population.OutputCount; toID++)
                {
                    FindInnovation(fromID, toID);
                }
            }



        }

        /// <summary>
        /// Find an innovation for a hidden neuron that split a existing link. This
        /// is the means by which hidden neurons are introduced in NEAT.
        /// </summary>
        /// <param name="fromID">The source neuron ID in the link.</param>
        /// <param name="toID">The target neuron ID in the link.</param>
        /// <returns>The newly created innovation, or the one that matched the search.</returns>
        public NEATInnovation FindInnovationSplit(long fromID, long toID)
        {
            String key = NEATInnovationList.ProduceKeyNeuronSplit(fromID, toID);

            lock (this.list)
            {
                if (this.list.ContainsKey(key))
                {
                    return this.list[key];
                }
                else
                {
                    long neuronID = Population.AssignGeneID();
                    NEATInnovation innovation = new NEATInnovation();
                    innovation.InnovationID = Population.AssignInnovationID();
                    innovation.NeuronID = neuronID;
                    list[key] = innovation;

                    // create other sides of split, if needed
                    FindInnovation(fromID, neuronID);
                    FindInnovation(neuronID, toID);
                    return innovation;
                }
            }
        }

        /// <summary>
        /// Find an innovation for a single neuron. Single neurons were created
        /// without producing a split. This means, the only single neurons are the
        /// input, bias and output neurons.
        /// </summary>
        /// <param name="neuronID">The neuron ID to find.</param>
        /// <returns>The newly created innovation, or the one that matched the search.</returns>
        public NEATInnovation FindInnovation(long neuronID)
        {
            String key = NEATInnovationList.ProduceKeyNeuron(neuronID);

            lock (this.list)
            {
                if (this.list.ContainsKey(key))
                {
                    return this.list[key];
                }
                else
                {
                    NEATInnovation innovation = new NEATInnovation();
                    innovation.InnovationID = Population.AssignInnovationID();
                    innovation.NeuronID = neuronID;
                    list[key] = innovation;
                    return innovation;
                }
            }
        }

        /// <summary>
        /// Find an innovation for a new link added between two existing neurons.
        /// </summary>
        /// <param name="fromID">The source neuron ID in the link.</param>
        /// <param name="toID">The target neuron ID in the link.</param>
        /// <returns>The newly created innovation, or the one that matched the search.</returns>
        public NEATInnovation FindInnovation(long fromID, long toID)
        {
            String key = NEATInnovationList.ProduceKeyLink(fromID, toID);

            lock (this.list)
            {
                if (this.list.ContainsKey(key))
                {
                    return this.list[key];
                }
                else
                {
                    NEATInnovation innovation = new NEATInnovation();
                    innovation.InnovationID = Population.AssignInnovationID();
                    innovation.NeuronID = -1;
                    list[key] = innovation;
                    return innovation;
                }
            }
        }

        /// <summary>
        /// A list of innovations.
        /// </summary>
        public IDictionary<String, NEATInnovation> Innovations
        {
            get
            {
                return list;
            }
        }
    }
}
