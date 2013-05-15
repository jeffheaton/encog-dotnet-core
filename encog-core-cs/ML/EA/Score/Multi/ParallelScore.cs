//
// Encog(tm) Core v3.2 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2013 Heaton Research, Inc.
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
using System.Linq;
using System.Text;
using Encog.ML.EA.Population;
using Encog.ML.EA.Codec;
using Encog.Neural.Networks.Training;
using Encog.ML.EA.Species;
using Encog.ML.EA.Genome;
using System.Threading.Tasks;

namespace Encog.ML.EA.Score.Multi
{
    /// <summary>
    /// This class is used to calculate the scores for an entire population. This is
    /// typically done when a new population must be scored for the first time.
    /// </summary>
    public class ParallelScore
    {
        /// <summary>
        /// The thread count.
        /// </summary>
        public int ThreadCount { get; set; }

        /// <summary>
        /// The population to score.
        /// </summary>
        private IPopulation population;

        /// <summary>
        /// The CODEC used to create genomes.
        /// </summary>
        private IGeneticCODEC codec;

        /// <summary>
        /// The scoring function.
        /// </summary>
        private ICalculateScore scoreFunction;

        /// <summary>
        /// The score adjuster.
        /// </summary>
        private IList<IAdjustScore> adjusters;

        /// <summary>
        /// The number of requested threads.
        /// </summary>
        private int threads;

        /// <summary>
        /// The actual number of threads.
        /// </summary>
        private int actualThreads;

        /// <summary>
        /// Construct the parallel score calculation object. 
        /// </summary>
        /// <param name="thePopulation">The population to score.</param>
        /// <param name="theCODEC">The CODEC to use.</param>
        /// <param name="theAdjusters">The score adjusters to use.</param>
        /// <param name="theScoreFunction">The score function.</param>
        /// <param name="theThreadCount">The requested thread count.</param>
        public ParallelScore(IPopulation thePopulation, IGeneticCODEC theCODEC,
                IList<IAdjustScore> theAdjusters, ICalculateScore theScoreFunction,
                int theThreadCount)
        {
            this.codec = theCODEC;
            this.population = thePopulation;
            this.scoreFunction = theScoreFunction;
            this.adjusters = theAdjusters;
            this.actualThreads = 0;
        }

        /// <summary>
        /// The population.
        /// </summary>
        public IPopulation Population
        {
            get
            {
                return population;
            }
        }

        /// <summary>
        /// The score function.
        /// </summary>
        public ICalculateScore ScoreFunction
        {
            get
            {
                return scoreFunction;
            }
        }

        /// <summary>
        /// The CODEC.
        /// </summary>
        public IGeneticCODEC CODEC
        {
            get
            {
                return codec;
            }
        }

        /// <summary>
        /// Calculate the scores.
        /// </summary>
        public void Process()
        {

            // calculate workload
            IList<ParallelScoreTask> tasks = new List<ParallelScoreTask>();

            foreach (ISpecies species in this.population.Species)
            {
                foreach (IGenome genome in species.Members)
                {
                    tasks.Add(new ParallelScoreTask(genome, this));
                }
            }

            // determine thread usage
            if (ScoreFunction.RequireSingleThreaded || this.threads == 1)
            {
                // single
                foreach (ParallelScoreTask task in tasks)
                {
                    task.PerformTask();
                }

            }
            else
            {
                // parallel
                Parallel.ForEach(tasks, currentTask =>
                {
                    currentTask.PerformTask();
                });
            }

        }

        /// <summary>
        /// The score adjusters.
        /// </summary>
        public IList<IAdjustScore> Adjusters
        {
            get
            {
                return this.adjusters;
            }
        }

    }
}
