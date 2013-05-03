using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Train;
using Encog.ML.EA.Population;
using Encog.Neural.Networks.Training;
using Encog.ML.Data;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks.Training.Propagation;

namespace Encog.ML.EA.Train
{
    /// <summary>
    /// Provides a MLTrain compatible class that can be used to train genomes.
    /// </summary>
    public class TrainEA : BasicEA, IMLTrain
    {
        /// <summary>
        /// Not used, the error.
        /// </summary>
        public new double Error 
        {
            get
            {
                return base.Error;
            }
            set
            {
                // not needed
            }
        }

        /// <summary>
        /// Create a trainer for a score function.
        /// </summary>
        /// <param name="thePopulation">The population.</param>
        /// <param name="theScoreFunction">The score function.</param>
        public TrainEA(IPopulation thePopulation, ICalculateScore theScoreFunction)
            : base(thePopulation, theScoreFunction)
        {
        }

        /// <summary>
        /// Create a trainer for training data.
        /// </summary>
        /// <param name="thePopulation">The population.</param>
        /// <param name="trainingData">The training data.</param>
        public TrainEA(IPopulation thePopulation, IMLDataSet trainingData)
            : base(thePopulation, new TrainingSetScore(trainingData))
        {
        }


        /// <summary>
        /// True if training can progress no further.
        /// </summary>
        public bool TrainingDone
        {
            get
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public TrainingImplementationType ImplementationType
        {
            get
            {
                return TrainingImplementationType.Iterative;
            }
        }

        /// <summary>
        /// Perform the specified number of training iterations. This is a basic
        /// implementation that just calls iteration the specified number of times.
        /// However, some training methods, particularly with the GPU, benefit
        /// greatly by calling with higher numbers than 1.
        /// </summary>
        /// <param name="count">The number of training iterations.</param>
        public void Iteration(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Iteration();
            }
        }

        /// <inheritdoc/>
        public TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc/>
        public void Resume(TrainingContinuation state)
        {

        }

        /// <summary>
        /// Not supported, will throw an error.
        /// </summary>
        /// <param name="strategy">Not used.</param>
        public void AddStrategy(IStrategy strategy)
        {
            throw new TrainingError(
                    "Strategies are not supported by this training method.");
        }

        /// <inheritdoc/>
        public bool CanContinue
        {
            get
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public override void FinishTraining()
        {
            base.FinishTraining();
            this.Population.BestGenome = BestGenome;
        }

        /// <summary>
        /// A network created for the best genome.
        /// </summary>
        public IMLMethod Method
        {
            get
            {
                return Population;
            }
        }

        /// <summary>
        /// Returns null, does not use a training set, rather uses a score function.
        /// </summary>
        public IMLDataSet Training
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Returns an empty list, strategies are not supported.
        /// </summary>
        public IList<IStrategy> Strategies
        {
            get
            {
                return new List<IStrategy>();
            }
        }

        public int IterationNumber
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
