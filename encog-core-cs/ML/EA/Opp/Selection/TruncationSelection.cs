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
using Encog.ML.EA.Train;
using System;
using Encog.ML.EA.Species;
namespace Encog.ML.EA.Opp.Selection
{
    public class TruncationSelection : ISelectionOperator
    {
        /// <summary>
        /// The trainer being used.
        /// </summary>
        public IEvolutionaryAlgorithm Trainer { get; set; }

        /// <summary>
        /// The number of rounds.
        /// </summary>
        public int Rounds { get; set; }

        /**
         * The trainer.
         */
        private IEvolutionaryAlgorithm trainer;

        /**
         * The percent to select from.
         */
        private double percent;

        /**
         * Construct the truncation selector.
         * @param theTrainer The trainer.
         * @param thePercent The top percent to select from.
         */
        public TruncationSelection(IEvolutionaryAlgorithm theTrainer,
                double thePercent)
        {
            this.trainer = theTrainer;
            this.percent = thePercent;
        }

        /**
         * {@inheritDoc}
         */
        public int PerformSelection(Random rnd, ISpecies species)
        {
            int top = Math.Max((int)(species.Members.Count * this.percent),
                    1);
            int result = rnd.Next(top);
            return result;
        }

        /**
         * {@inheritDoc}
         */
        public int PerformAntiSelection(Random rnd, ISpecies species)
        {
            return species.Members.Count - PerformSelection(rnd, species);
        }

    }
}
