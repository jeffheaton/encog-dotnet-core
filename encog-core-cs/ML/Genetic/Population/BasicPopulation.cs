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
using System.Collections.Generic;
using Encog.ML.Genetic.Genome;
using Encog.ML.Genetic.Innovation;
using Encog.ML.Genetic.Species;
using Encog.Util.Identity;

namespace Encog.ML.Genetic.Population
{
    /// <summary>
    /// Defines the basic functionality for a population of genomes.
    /// </summary>
    [Serializable]
    public class BasicPopulation : IPopulation
    {
        /// <summary>
        /// Thed default old age penalty.
        /// </summary>
        ///
        public const double DefaultOldAgePenalty = 0.3d;

        /// <summary>
        /// The default old age threshold.
        /// </summary>
        ///
        public const int DefaultOldAgeThreshold = 50;

        /// <summary>
        /// The default survival rate.
        /// </summary>
        ///
        public const double DefaultSurvivalRate = 0.2d;

        /// <summary>
        /// The default youth penalty.
        /// </summary>
        ///
        public const double DefaultYouthBonus = 0.3d;

        /// <summary>
        /// The default youth threshold.
        /// </summary>
        ///
        public const int DefaultYouthThreshold = 10;

        /// <summary>
        /// Generate gene id's.
        /// </summary>
        ///
        private readonly IGenerateID _geneIDGenerate;

        /// <summary>
        /// Generate genome id's.
        /// </summary>
        ///
        private readonly IGenerateID _genomeIDGenerate;

        /// <summary>
        /// The population.
        /// </summary>
        ///
        private readonly List<IGenome> _genomes;

        /// <summary>
        /// Generate innovation id's.
        /// </summary>
        ///
        private readonly IGenerateID _innovationIDGenerate;
       
        /// <summary>
        /// Generate species id's.
        /// </summary>
        ///
        private readonly IGenerateID _speciesIDGenerate;

        /// <summary>
        /// The young threshold.
        /// </summary>
        ///
        private int _youngBonusAgeThreshold;

        /// <summary>
        /// Construct an empty population.
        /// </summary>
        ///
        public BasicPopulation()
        {
            _geneIDGenerate = new BasicGenerateID();
            _genomeIDGenerate = new BasicGenerateID();
            _genomes = new List<IGenome>();
            _innovationIDGenerate = new BasicGenerateID();
            OldAgePenalty = DefaultOldAgePenalty;
            OldAgeThreshold = DefaultOldAgeThreshold;
            Species = new List<ISpecies>();
            _speciesIDGenerate = new BasicGenerateID();
            SurvivalRate = DefaultSurvivalRate;
            _youngBonusAgeThreshold = DefaultYouthThreshold;
            YoungScoreBonus = DefaultYouthBonus;
            PopulationSize = 0;
        }

        /// <summary>
        /// Construct a population.
        /// </summary>
        /// <param name="thePopulationSize">The population size.</param>
        public BasicPopulation(int thePopulationSize)
        {
            _geneIDGenerate = new BasicGenerateID();
            _genomeIDGenerate = new BasicGenerateID();
            _genomes = new List<IGenome>();
            _innovationIDGenerate = new BasicGenerateID();
            OldAgePenalty = DefaultOldAgePenalty;
            OldAgeThreshold = DefaultOldAgeThreshold;
            Species = new List<ISpecies>();
            _speciesIDGenerate = new BasicGenerateID();
            SurvivalRate = DefaultSurvivalRate;
            _youngBonusAgeThreshold = DefaultYouthThreshold;
            YoungScoreBonus = DefaultYouthBonus;
            PopulationSize = thePopulationSize;
        }

        /// <value>the geneIDGenerate</value>
        public IGenerateID GeneIDGenerate
        {
            get { return _geneIDGenerate; }
        }


        /// <value>the genomeIDGenerate</value>
        public IGenerateID GenomeIDGenerate
        {
            get { return _genomeIDGenerate; }
        }

        /// <value>the innovationIDGenerate</value>
        public IGenerateID InnovationIDGenerate
        {
            get { return _innovationIDGenerate; }
        }

        /// <summary>
        /// Set the name.
        /// </summary>
        public String Name { get; set; }

        /// <value>the speciesIDGenerate</value>
        public IGenerateID SpeciesIDGenerate
        {
            get { return _speciesIDGenerate; }
        }

        #region IPopulation Members


        /// <inheritdoc/>
        public void Add(IGenome genome)
        {
            _genomes.Add(genome);
            genome.Population = this;
        }

        /// <inheritdoc/>
        public long AssignGeneID()
        {
            return _geneIDGenerate.Generate();
        }

        /// <inheritdoc/>
        public long AssignGenomeID()
        {
            return _genomeIDGenerate.Generate();
        }

        /// <inheritdoc/>
        public long AssignInnovationID()
        {
            return _innovationIDGenerate.Generate();
        }

        /// <inheritdoc/>
        public long AssignSpeciesID()
        {
            return _speciesIDGenerate.Generate();
        }

        /// <inheritdoc/>
        public void Claim(GeneticAlgorithm ga)
        {
            foreach (IGenome genome  in  _genomes)
            {
                genome.GA = ga;
            }
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _genomes.Clear();
        }

        /// <inheritdoc/>
        public IGenome Get(int i)
        {
            return _genomes[i];
        }

        /// <inheritdoc/>
        public IGenome Best
        {
            get { return _genomes.Count == 0 ? null : _genomes[0]; }
        }


        /// <inheritdoc/>
        public IList<IGenome> Genomes
        {
            get { return _genomes; }
        }


        /// <inheritdoc/>
        public IInnovationList Innovations { get; set; }


        /// <inheritdoc/>
        public double OldAgePenalty { get; set; }


        /// <inheritdoc/>
        public int OldAgeThreshold { get; set; }


        /// <inheritdoc/>
        public int PopulationSize { get; set; }


        /// <inheritdoc/>
        public IList<ISpecies> Species { get; set; }


        /// <inheritdoc/>
        public double SurvivalRate { get; set; }


        /// <value>the youngBonusAgeThreshold to set</value>
        public int YoungBonusAgeThreshold
        {
            get { return _youngBonusAgeThreshold; }
            set { _youngBonusAgeThreshold = value; }
        }


        /// <inheritdoc/>
        public double YoungScoreBonus { get; set; }


        /// <inheritdoc/>
        public int YoungBonusAgeThreshhold
        {
            set { _youngBonusAgeThreshold = value; }
        }


        /// <inheritdoc/>
        public int Size()
        {
            return _genomes.Count;
        }

        /// <inheritdoc/>
        public void Sort()
        {
            _genomes.Sort();
        }

        #endregion
    }
}
