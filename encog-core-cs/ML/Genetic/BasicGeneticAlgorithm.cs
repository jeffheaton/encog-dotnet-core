using System;
using Encog.ML.Genetic.Genome;
using Encog.Util.Concurrency;

namespace Encog.ML.Genetic
{
    /// <summary>
    /// Provides a basic implementation of a genetic algorithm.
    /// </summary>
    ///
    public class BasicGeneticAlgorithm : GeneticAlgorithm
    {
        /// <summary>
        /// Is this the first iteration.
        /// </summary>
        ///
        private bool first;

        public BasicGeneticAlgorithm()
        {
            first = true;
        }

        /// <summary>
        /// Modify the weight matrix and bias values based on the last call to
        /// calcError.
        /// </summary>
        ///
        /// <exception cref="NeuralNetworkException"></exception>
        public override sealed void Iteration()
        {
            if (first)
            {
                Population.Claim(this);
                first = false;
            }

            var countToMate = (int) (Population.PopulationSize*PercentToMate);
            int offspringCount = countToMate*2;
            int offspringIndex = Population.PopulationSize
                                 - offspringCount;
            var matingPopulationSize = (int) (Population.PopulationSize*MatingPopulation);

            TaskGroup group = EngineConcurrency.Instance
                .CreateTaskGroup();

            // mate and form the next generation
            for (int i = 0; i < countToMate; i++)
            {
                IGenome mother = Population.Genomes[i];
                int fatherInt = ((new Random()).Next()*matingPopulationSize);
                IGenome father = Population.Genomes[fatherInt];
                IGenome child1 = Population.Genomes[offspringIndex];
                IGenome child2 = Population.Genomes[offspringIndex + 1];

                var worker = new MateWorker(mother, father, child1,
                                            child2);

                EngineConcurrency.Instance.ProcessTask(worker, group);

                offspringIndex += 2;
            }

            group.WaitForComplete();

            // sort the next generation
            Population.Sort();
        }
    }
}