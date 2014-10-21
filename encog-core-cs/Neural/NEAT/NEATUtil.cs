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
using Encog.ML.EA.Train;
using Encog.Neural.Networks.Training;
using Encog.Neural.NEAT.Training.Species;
using Encog.ML.EA.Opp.Selection;
using Encog.ML.EA.Opp;
using Encog.Neural.NEAT.Training.Opp;
using Encog.Neural.NEAT.Training.Opp.Links;
using Encog.Neural.HyperNEAT;

namespace Encog.Neural.NEAT
{
    /// <summary>
    /// NEAT does not make use of a special trainer. Typically the generic TrainEA
    /// trainer is used. This utility class creates a NEAT compatible TrainEA class.
    /// 
    /// -----------------------------------------------------------------------------
    /// http://www.cs.ucf.edu/~kstanley/ Encog's NEAT implementation was drawn from
    /// the following three Journal Articles. For more complete BibTeX sources, see
    /// NEATNetwork.java.
    /// 
    /// Evolving Neural Networks Through Augmenting Topologies
    /// 
    /// Generating Large-Scale Neural Networks Through Discovering Geometric
    /// Regularities
    /// 
    /// Automatic feature selection in neuroevolution
    /// </summary>
    public class NEATUtil
    {
        /// <summary>
        /// Construct a NEAT or HyperNEAT trainer.
        /// </summary>
        /// <param name="calculateScore">The score function.</param>
        /// <param name="inputCount">The input count.</param>
        /// <param name="outputCount">The output count.</param>
        /// <param name="populationSize">The population size.</param>
        /// <returns></returns>
        public static TrainEA ConstructNEATTrainer(
            ICalculateScore calculateScore, int inputCount,
            int outputCount, int populationSize)
        {
            var pop = new NEATPopulation(inputCount, outputCount,
                    populationSize);
            pop.Reset();
            return ConstructNEATTrainer(pop, calculateScore);
        }

        /// <summary>
        /// Construct a NEAT (or HyperNEAT trainer.
        /// </summary>
        /// <param name="population">The population.</param>
        /// <param name="calculateScore">The score function.</param>
        /// <returns>The NEAT EA trainer.</returns>
        public static TrainEA ConstructNEATTrainer(NEATPopulation population,
                ICalculateScore calculateScore)
        {
            var result = new TrainEA(population, calculateScore) {Speciation = new OriginalNEATSpeciation()};

            result.Selection = new TruncationSelection(result, 0.3);
            var weightMutation = new CompoundOperator();
            weightMutation.Components.Add(
                    0.1125,
                    new NEATMutateWeights(new SelectFixed(1),
                            new MutatePerturbLinkWeight(0.02)));
            weightMutation.Components.Add(
                    0.1125,
                    new NEATMutateWeights(new SelectFixed(2),
                            new MutatePerturbLinkWeight(0.02)));
            weightMutation.Components.Add(
                    0.1125,
                    new NEATMutateWeights(new SelectFixed(3),
                            new MutatePerturbLinkWeight(0.02)));
            weightMutation.Components.Add(
                    0.1125,
                    new NEATMutateWeights(new SelectProportion(0.02),
                            new MutatePerturbLinkWeight(0.02)));
            weightMutation.Components.Add(
                    0.1125,
                    new NEATMutateWeights(new SelectFixed(1),
                            new MutatePerturbLinkWeight(1)));
            weightMutation.Components.Add(
                    0.1125,
                    new NEATMutateWeights(new SelectFixed(2),
                            new MutatePerturbLinkWeight(1)));
            weightMutation.Components.Add(
                    0.1125,
                    new NEATMutateWeights(new SelectFixed(3),
                            new MutatePerturbLinkWeight(1)));
            weightMutation.Components.Add(
                    0.1125,
                    new NEATMutateWeights(new SelectProportion(0.02),
                            new MutatePerturbLinkWeight(1)));
            weightMutation.Components.Add(
                    0.03,
                    new NEATMutateWeights(new SelectFixed(1),
                            new MutateResetLinkWeight()));
            weightMutation.Components.Add(
                    0.03,
                    new NEATMutateWeights(new SelectFixed(2),
                            new MutateResetLinkWeight()));
            weightMutation.Components.Add(
                    0.03,
                    new NEATMutateWeights(new SelectFixed(3),
                            new MutateResetLinkWeight()));
            weightMutation.Components.Add(
                    0.01,
                    new NEATMutateWeights(new SelectProportion(0.02),
                            new MutateResetLinkWeight()));
            weightMutation.Components.FinalizeStructure();

            result.ChampMutation = weightMutation;
            result.AddOperation(0.5, new NEATCrossover());
            result.AddOperation(0.494, weightMutation);
            result.AddOperation(0.0005, new NEATMutateAddNode());
            result.AddOperation(0.005, new NEATMutateAddLink());
            result.AddOperation(0.0005, new NEATMutateRemoveLink());
            result.Operators.FinalizeStructure();

            if (population.IsHyperNEAT)
            {
                result.CODEC = new HyperNEATCODEC();
            }
            else
            {
                result.CODEC = new NEATCODEC();
            }

            return result;
        }
    }
}
