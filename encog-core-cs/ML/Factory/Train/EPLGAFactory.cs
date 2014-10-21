//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
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
