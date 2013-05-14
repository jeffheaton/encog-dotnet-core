using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Train;
using Encog.ML.Data;
using Encog.ML.Prg.Train;
using Encog.Neural.Networks.Training;
using Encog.ML.EA.Train;
using Encog.ML.Prg.Train.Rewrite;
using Encog.ML.Prg;
using Encog.ML.Prg.Species;
using Encog.ML.Prg.Opp;
using Encog.ML.EA.Score.Adjust;

namespace Encog.ML.Factory.Train
{
    public class EPLGAFactory
    {
        /// <summary>
        /// Create an EPL GA trainer.
        /// </summary>
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                IMLDataSet training, String argsStr)
        {

            PrgPopulation pop = (PrgPopulation)method;

            ICalculateScore score = new TrainingSetScore(training);
            TrainEA train = new TrainEA(pop, score);
            train.Rules.AddRewriteRule(new RewriteConstants());
            train.Rules.AddRewriteRule(new RewriteAlgebraic());
            train.CODEC = new PrgCODEC();
            train.AddOperation(0.8, new SubtreeCrossover());
            train.AddOperation(0.1, new SubtreeMutation(pop.Context, 4));
            train.AddOperation(0.1, new ConstMutation(pop.Context, 0.5, 1.0));
            train.AddScoreAdjuster(new ComplexityAdjustedScore());
            train.Speciation = new PrgSpeciation();
            return train;
        }
    }
}
