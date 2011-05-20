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
using Encog.ML.Genetic.Genome;
using Encog.Util.Concurrency;
using Encog.MathUtil;

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

        /// <summary>
        /// Construct the object.
        /// </summary>
        public BasicGeneticAlgorithm()
        {
            first = true;
        }

        /// <summary>
        /// Modify the weight matrix and bias values based on the last call to
        /// calcError.
        /// </summary>
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
                int fatherInt = (int)(ThreadSafeRandom.NextDouble()*matingPopulationSize);
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
