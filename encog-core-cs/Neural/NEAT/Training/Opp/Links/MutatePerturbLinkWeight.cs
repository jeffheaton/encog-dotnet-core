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
using System.Text;
using Encog.ML.EA.Train;
using Encog.MathUtil.Randomize;

namespace Encog.Neural.NEAT.Training.Opp.Links
{
    /// <summary>
    /// Mutate weight links by perturbing their weights. This will be done by adding
    /// a Gaussian random number with the specified sigma. The sigma specifies the
    /// standard deviation of the random number. Because the random numbers are
    /// clustered at zero, this can be either an increase or decrease.
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
    public class MutatePerturbLinkWeight : IMutateLinkWeight
    {
        /// <summary>
        /// The trainer being used.
        /// </summary>
        private IEvolutionaryAlgorithm _trainer;

        /// <summary>
        /// The sigma (standard deviation) of the Gaussian random numbers.
        /// </summary>
        private readonly double _sigma;

        /// <summary>
        /// Construct the perturbing mutator.
        /// </summary>
        /// <param name="theSigma">The sigma (standard deviation) for all random numbers.</param>
        public MutatePerturbLinkWeight(double theSigma)
        {
            _sigma = theSigma;
        }

        /// <inheritdoc/>
        public IEvolutionaryAlgorithm Trainer
        {
            get
            {
                return _trainer;
            }
        }

        /// <inheritdoc/>
        public void Init(IEvolutionaryAlgorithm theTrainer)
        {
            _trainer = theTrainer;
        }

        /// <inheritdoc/>
        public void MutateWeight(EncogRandom rnd, NEATLinkGene linkGene,
                double weightRange)
        {
            double delta = rnd.NextGaussian() * _sigma;
            double w = linkGene.Weight + delta;
            w = NEATPopulation.ClampWeight(w, weightRange);
            linkGene.Weight = w;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append("[");
            result.Append(GetType().Name);
            result.Append(":sigma=");
            result.Append(_sigma);
            result.Append("]");
            return result.ToString();
        }
    }
}
