//
// Encog(tm) Core v3.2 - .Net Version
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
using ConsoleExamples.Examples;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Util.Data;
using Encog.ML.Prg;
using Encog.ML.Prg.Ext;
using Encog.ML.Prg.Train;
using Encog.ML.Fitness;
using Encog.Neural.Networks.Training;
using Encog.ML.EA.Train;
using Encog.ML.Prg.Opp;
using Encog.ML.EA.Score.Adjust;
using Encog.ML.Prg.Species;
using Encog.ML.Prg.Train.Rewrite;
using Encog.ML.Prg.Generator;
using Encog.MathUtil.Randomize;

namespace Encog.Examples.EPL
{
    public class SimpleEPL : IExample
    {


        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(SimpleEPL),
                    "epl-simple",
                    "Simple EPL equation solve.",
                    "This example shows how to fit a simple equation with Genetic Programming (EPL).");
                return info;
            }
        }

        #region IExample Members

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="app">Holds arguments and other info.</param>
        public void Execute(IExampleInterface app)
        {
            IMLDataSet trainingData = GenerationUtil.GenerateSingleDataRange(
                (x) => (3 * Math.Pow(x, 2) + (12 * x) + 4)
                , 0, 100, 1);

            EncogProgramContext context = new EncogProgramContext();
            context.DefineVariable("x");

            StandardExtensions.CreateNumericOperators(context);

            PrgPopulation pop = new PrgPopulation(context, 1000);

            MultiObjectiveFitness score = new MultiObjectiveFitness();
            score.AddObjective(1.0, new TrainingSetScore(trainingData));

            TrainEA genetic = new TrainEA(pop, score);
            genetic.ValidationMode = true;
            genetic.CODEC = new PrgCODEC();
            genetic.AddOperation(0.5, new SubtreeCrossover());
            genetic.AddOperation(0.25, new ConstMutation(context, 0.5, 1.0));
            genetic.AddOperation(0.25, new SubtreeMutation(context, 4));
            genetic.AddScoreAdjuster(new ComplexityAdjustedScore(10, 20, 10, 20.0));
            genetic.Rules.AddRewriteRule(new RewriteConstants());
            genetic.Rules.AddRewriteRule(new RewriteAlgebraic());
            genetic.Speciation = new PrgSpeciation();

            (new RampedHalfAndHalf(context, 1, 6)).Generate(new EncogRandom(), pop);

            genetic.ShouldIgnoreExceptions = false;

            EncogProgram best = null;
            genetic.ThreadCount = 1;

            try
            {

                for (int i = 0; i < 1000; i++)
                {
                    genetic.Iteration();
                    best = (EncogProgram)genetic.BestGenome;
                    Console.Out.WriteLine(genetic.IterationNumber + ", Error: "
                            + best.Score + ",Best Genome Size:" + best.Size
                            + ",Species Count:" + pop.Species.Count + ",best: " + best.DumpAsCommonExpression());
                }

                //EncogUtility.evaluate(best, trainingData);

                Console.Out.WriteLine("Final score:" + best.Score
                        + ", effective score:" + best.AdjustedScore);
                Console.Out.WriteLine(best.DumpAsCommonExpression());
                //pop.dumpMembers(Integer.MAX_VALUE);
                //pop.dumpMembers(10);

            }
            catch (Exception t)
            {
                Console.Out.WriteLine(t.ToString());
            }
            finally
            {
                genetic.FinishTraining();
                EncogFramework.Instance.Shutdown();
            }
        }

        #endregion
    }
}
