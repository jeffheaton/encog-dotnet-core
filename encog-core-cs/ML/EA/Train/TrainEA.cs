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

using System.Collections.Generic;
using Encog.ML.Data;
using Encog.ML.EA.Population;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation;

namespace Encog.ML.EA.Train
{
    /// <summary>
    ///     Provides a MLTrain compatible class that can be used to train genomes.
    /// </summary>
    public class TrainEA : BasicEA, IMLTrain
    {
        /// <summary>
        ///     Create a trainer for a score function.
        /// </summary>
        /// <param name="thePopulation">The population.</param>
        /// <param name="theScoreFunction">The score function.</param>
        public TrainEA(IPopulation thePopulation, ICalculateScore theScoreFunction)
            : base(thePopulation, theScoreFunction)
        {
        }

        /// <summary>
        ///     Create a trainer for training data.
        /// </summary>
        /// <param name="thePopulation">The population.</param>
        /// <param name="trainingData">The training data.</param>
        public TrainEA(IPopulation thePopulation, IMLDataSet trainingData)
            : base(thePopulation, new TrainingSetScore(trainingData))
        {
        }

        /// <summary>
        ///     Not used, the error.
        /// </summary>
        public new double Error
        {
            get { return base.Error; }
            set
            {
                // not needed
            }
        }


        /// <summary>
        ///     True if training can progress no further.
        /// </summary>
        public bool TrainingDone
        {
            get { return false; }
        }

        /// <inheritdoc />
        public TrainingImplementationType ImplementationType
        {
            get { return TrainingImplementationType.Iterative; }
        }

        /// <summary>
        ///     Perform the specified number of training iterations. This is a basic
        ///     implementation that just calls iteration the specified number of times.
        ///     However, some training methods, particularly with the GPU, benefit
        ///     greatly by calling with higher numbers than 1.
        /// </summary>
        /// <param name="count">The number of training iterations.</param>
        public void Iteration(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Iteration();
            }
        }

        /// <inheritdoc />
        public TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc />
        public void Resume(TrainingContinuation state)
        {
        }

        /// <summary>
        ///     Not supported, will throw an error.
        /// </summary>
        /// <param name="strategy">Not used.</param>
        public void AddStrategy(IStrategy strategy)
        {
            throw new TrainingError(
                "Strategies are not supported by this training method.");
        }

        /// <inheritdoc />
        public bool CanContinue
        {
            get { return false; }
        }

        /// <inheritdoc />
        public override void FinishTraining()
        {
            base.FinishTraining();
            Population.BestGenome = BestGenome;
        }

        /// <summary>
        ///     A network created for the best genome.
        /// </summary>
        public IMLMethod Method
        {
            get { return Population; }
        }

        /// <summary>
        ///     Returns null, does not use a training set, rather uses a score function.
        /// </summary>
        public IMLDataSet Training
        {
            get { return null; }
        }

        /// <summary>
        ///     Returns an empty list, strategies are not supported.
        /// </summary>
        public IList<IStrategy> Strategies
        {
            get { return new List<IStrategy>(); }
        }
    }
}