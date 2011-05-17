using System;
using System.Collections.Generic;
using Encog.ML.Genetic.Population;

namespace Encog.ML.Genetic.Genome
{
    /// <summary>
    /// A genome is the basic blueprint for creating an organism in Encog. A genome
    /// is made up of one or more chromosomes, which are in turn made up of genes.
    /// </summary>
    ///
    public interface IGenome : IComparable<IGenome>
    {
        /// <summary>
        /// Set the adjusted score.
        /// </summary>
        ///
        /// <value>The adjusted score.</value>
        double AdjustedScore
        { /// <summary>
            /// Get the adjusted score, this considers old-age penalties and youth
            /// bonuses. If there are no such bonuses or penalties, this is the same as
            /// the score.
            /// </summary>
            ///
            /// <returns>The adjusted score.</returns>
            get;
            /// <summary>
            /// Set the adjusted score.
            /// </summary>
            ///
            /// <param name="adjustedScore">The adjusted score.</param>
            set;
        }


        /// <summary>
        /// Set the amount to spawn.
        /// </summary>
        ///
        /// <value>The amount to spawn.</value>
        double AmountToSpawn
        { /// <returns>The amount of offspring this genome will have.</returns>
            get;
            /// <summary>
            /// Set the amount to spawn.
            /// </summary>
            ///
            /// <param name="amountToSpawn">The amount to spawn.</param>
            set;
        }


        /// <value>The chromosomes that make up this genome.</value>
        IList<Chromosome> Chromosomes
        { /// <returns>The chromosomes that make up this genome.</returns>
            get;
        }


        /// <summary>
        /// Set the GA used by this genome. This is normally a transient field and
        /// only used during training.
        /// </summary>
        ///
        /// <value>The GA.</value>
        GeneticAlgorithm GeneticAlgorithm
        { /// <returns>Get the GA used by this genome. This is normally a transient
            /// field and only used during training.</returns>
            get;
            /// <summary>
            /// Set the GA used by this genome. This is normally a transient field and
            /// only used during training.
            /// </summary>
            ///
            /// <param name="ga">The GA.</param>
            set;
        }


        /// <summary>
        /// Set the genome ID.
        /// </summary>
        ///
        /// <value>The genome id.</value>
        long GenomeID
        { /// <returns>The genome ID.</returns>
            get;
            /// <summary>
            /// Set the genome ID.
            /// </summary>
            ///
            /// <param name="genomeID">The genome id.</param>
            set;
        }


        /// <value>The organism produced by this genome.</value>
        Object Organism
        { /// <returns>The organism produced by this genome.</returns>
            get;
        }


        /// <summary>
        /// Set the population that this genome belongs to.
        /// </summary>
        ///
        /// <value>The population that this genome belongs to.</value>
        IPopulation Population
        { /// <returns>The population that this genome belongs to.</returns>
            get;
            /// <summary>
            /// Set the population that this genome belongs to.
            /// </summary>
            ///
            /// <param name="population">The population that this genome belongs to.</param>
            set;
        }


        /// <summary>
        /// Set the score.
        /// </summary>
        ///
        /// <value>The new score.</value>
        double Score
        { /// <returns>The score for this genome.</returns>
            get;
            /// <summary>
            /// Set the score.
            /// </summary>
            ///
            /// <param name="score">The new score.</param>
            set;
        }

        /// <returns>The number of genes in this genome.</returns>
        int CalculateGeneCount();

        /// <summary>
        /// Use the genes to update the organism.
        /// </summary>
        ///
        void Decode();

        /// <summary>
        /// Use the organism to update the genes.
        /// </summary>
        ///
        void Encode();


        /// <summary>
        /// Mate with another genome and produce two children.
        /// </summary>
        ///
        /// <param name="father">The father genome.</param>
        /// <param name="child1">The first child.</param>
        /// <param name="child2">The second child.</param>
        void Mate(IGenome father, IGenome child1, IGenome child2);
    }
}