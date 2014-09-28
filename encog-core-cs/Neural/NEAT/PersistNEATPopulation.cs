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
using Encog.Persist;
using System.IO;
using Encog.Neural.NEAT.Training;
using Encog.ML.EA.Species;
using Encog.Util.CSV;
using Encog.Engine.Network.Activation;
using Encog.Neural.HyperNEAT;
using Encog.ML.EA.Genome;

namespace Encog.Neural.NEAT
{
    /// <summary>
    /// Persist a NEAT or HyperNEAT network.
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
    public class PersistNEATPopulation : IEncogPersistor
    {
        /// <summary>
        /// Type for the Compositional pattern-producing networks used by HyperNEAT.
        /// </summary>
        public const String TypeCppn = "cppn";

        /// <summary>
        /// Convert a NEATNeuronType enum to a string.
        /// </summary>
        /// <param name="t">The type.</param>
        /// <returns>The string type.</returns>
        public static String NeuronTypeToString(NEATNeuronType t)
        {
            switch (t)
            {
                case NEATNeuronType.Bias:
                    return "b";
                case NEATNeuronType.Hidden:
                    return "h";
                case NEATNeuronType.Input:
                    return "i";
                case NEATNeuronType.None:
                    return "n";
                case NEATNeuronType.Output:
                    return "o";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Convert a string to a neuron type.
        /// </summary>
        /// <param name="t">The string.</param>
        /// <returns>The neuron type.</returns>
        public static NEATNeuronType StringToNeuronType(String t)
        {
            if (t.Equals("b"))
            {
                return NEATNeuronType.Bias;
            }
            if (t.Equals("h"))
            {
                return NEATNeuronType.Hidden;
            }
            if (t.Equals("i"))
            {
                return NEATNeuronType.Input;
            }
            if (t.Equals("n"))
            {
                return NEATNeuronType.None;
            }
            if (t.Equals("o"))
            {
                return NEATNeuronType.Output;
            }
            return NEATNeuronType.Unknown;
        }

        /// <inheritdoc/>
        public int FileVersion
        {
            get
            {
                return 1;
            }
        }

        /// <inheritdoc/>
        public String PersistClassString
        {
            get
            {
                return typeof(NEATPopulation).Name;
            }
        }


        /// <inheritdoc/>
        public Object Read(Stream istream)
        {
            long nextInnovationId = 0;
            long nextGeneId = 0;

            var result = new NEATPopulation();
            var innovationList = new NEATInnovationList {Population = result};
            result.Innovations = innovationList;
            var reader = new EncogReadHelper(istream);
            EncogFileSection section;

            while ((section = reader.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("NEAT-POPULATION")
                        && section.SubSectionName.Equals("INNOVATIONS"))
                {
                    foreach (String line in section.Lines)
                    {
                        IList<String> cols = EncogFileSection
                                .SplitColumns(line);
                        var innovation = new NEATInnovation();
                        var innovationId = int.Parse(cols[1]);
                        innovation.InnovationId = innovationId;
                        innovation.NeuronId = int.Parse(cols[2]);
                        result.Innovations.Innovations[cols[0]] = innovation;
                        nextInnovationId = Math.Max(nextInnovationId, innovationId + 1);
                    }
                }
                else if (section.SectionName.Equals("NEAT-POPULATION")
                      && section.SubSectionName.Equals("SPECIES"))
                {
                    NEATGenome lastGenome = null;
                    BasicSpecies lastSpecies = null;

                    foreach (String line in section.Lines)
                    {
                        IList<String> cols = EncogFileSection.SplitColumns(line);

                        if (String.Compare(cols[0], "s", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            lastSpecies = new BasicSpecies
                                {
                                    Population = result,
                                    Age = int.Parse(cols[1]),
                                    BestScore = CSVFormat.EgFormat.Parse(cols[2]),
                                    GensNoImprovement = int.Parse(cols[3])
                                };
                            result.Species.Add(lastSpecies);
                        }
                        else if (String.Compare(cols[0], "g", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            bool isLeader = lastGenome == null;
                            lastGenome = new NEATGenome
                                {
                                    InputCount = result.InputCount,
                                    OutputCount = result.OutputCount,
                                    Species = lastSpecies,
                                    AdjustedScore = CSVFormat.EgFormat.Parse(cols[1]),
                                    Score = CSVFormat.EgFormat.Parse(cols[2]),
                                    BirthGeneration = int.Parse(cols[3])
                                };
                            lastSpecies.Add(lastGenome);
                            if (isLeader)
                            {
                                lastSpecies.Leader = lastGenome;
                            }
                        }
                        else if (String.Compare(cols[0], "n", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var neuronGene = new NEATNeuronGene();
                            int geneId = int.Parse(cols[1]);
                            neuronGene.Id = geneId;

                            IActivationFunction af = EncogFileSection.ParseActivationFunction(cols[2]);
                            neuronGene.ActivationFunction = af;

                            neuronGene.NeuronType = PersistNEATPopulation.StringToNeuronType(cols[3]);
                            neuronGene.InnovationId = int.Parse(cols[4]);
                            lastGenome.NeuronsChromosome.Add(neuronGene);
                            nextGeneId = Math.Max(geneId + 1, nextGeneId);
                        }
                        else if (String.Compare(cols[0], "l", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var linkGene = new NEATLinkGene
                                {
                                    Id = int.Parse(cols[1]),
                                    Enabled = (int.Parse(cols[2]) > 0),
                                    FromNeuronId = int.Parse(cols[3]),
                                    ToNeuronId = int.Parse(cols[4]),
                                    Weight = CSVFormat.EgFormat.Parse(cols[5]),
                                    InnovationId = int.Parse(cols[6])
                                };
                            lastGenome.LinksChromosome.Add(linkGene);
                        }
                    }

                }
                else if (section.SectionName.Equals("NEAT-POPULATION")
                      && section.SubSectionName.Equals("CONFIG"))
                {
                    IDictionary<string, string> prm = section.ParseParams();

                    string afStr = prm[NEATPopulation.PropertyNEATActivation];

                    if (String.Compare(afStr, TypeCppn, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        HyperNEATGenome.BuildCPPNActivationFunctions(result.ActivationFunctions);
                    }
                    else
                    {
                        result.NEATActivationFunction = EncogFileSection.ParseActivationFunction(prm,
                                        NEATPopulation.PropertyNEATActivation);
                    }

                    result.ActivationCycles = EncogFileSection.ParseInt(prm,
                            PersistConst.ActivationCycles);
                    result.InputCount = EncogFileSection.ParseInt(prm,
                            PersistConst.InputCount);
                    result.OutputCount = EncogFileSection.ParseInt(prm,
                            PersistConst.OutputCount);
                    result.PopulationSize = EncogFileSection.ParseInt(prm,
                            NEATPopulation.PropertyPopulationSize);
                    result.SurvivalRate = EncogFileSection.ParseDouble(prm,
                            NEATPopulation.PropertySurvivalRate);
                    result.ActivationCycles = EncogFileSection.ParseInt(prm,
                            NEATPopulation.PropertyCycles);
                }
            }

            // set factories
            if (result.IsHyperNEAT)
            {
                result.GenomeFactory = new FactorHyperNEATGenome();
                result.CODEC = new HyperNEATCODEC();
            }
            else
            {
                result.GenomeFactory = new FactorNEATGenome();
                result.CODEC = new NEATCODEC();
            }

            // set the next ID's
            result.InnovationIDGenerate.CurrentID = nextInnovationId;
            result.GeneIdGenerate.CurrentID = nextGeneId;

            // find first genome, which should be the best genome
            if (result.Species.Count > 0)
            {
                ISpecies species = result.Species[0];
                if (species.Members.Count > 0)
                {
                    result.BestGenome = species.Members[0];
                }
            }

            return result;
        }


        /// <inheritdoc/>
        public void Save(Stream os, Object obj)
        {
            var output = new EncogWriteHelper(os);
            var pop = (NEATPopulation)obj;
            output.AddSection("NEAT-POPULATION");
            output.AddSubSection("CONFIG");
            output.WriteProperty(PersistConst.ActivationCycles, pop.ActivationCycles);

            if (pop.IsHyperNEAT)
            {
                output.WriteProperty(NEATPopulation.PropertyNEATActivation,TypeCppn);
            }
            else
            {
                IActivationFunction af = pop.ActivationFunctions.Contents[0].obj;
                output.WriteProperty(NEATPopulation.PropertyNEATActivation, af);
            }

            output.WriteProperty(PersistConst.InputCount, pop.InputCount);
            output.WriteProperty(PersistConst.OutputCount, pop.OutputCount);
            output.WriteProperty(NEATPopulation.PropertyCycles, pop.ActivationCycles);
            output.WriteProperty(NEATPopulation.PropertyPopulationSize, pop.PopulationSize);
            output.WriteProperty(NEATPopulation.PropertySurvivalRate, pop.SurvivalRate);
            output.AddSubSection("INNOVATIONS");
            if (pop.Innovations != null)
            {
                foreach (string key in pop.Innovations.Innovations.Keys)
                {
                    NEATInnovation innovation = pop.Innovations.Innovations[key];
                    output.AddColumn(key);
                    output.AddColumn(innovation.InnovationId);
                    output.AddColumn(innovation.NeuronId);
                    output.WriteLine();
                }
            }

            output.AddSubSection("SPECIES");

            // make sure the best species goes first
            ISpecies bestSpecies = pop.DetermineBestSpecies();
            if (bestSpecies != null)
            {
                SaveSpecies(output, bestSpecies);
            }

            // now write the other species, other than the best one
            foreach (ISpecies species in pop.Species)
            {
                if (species != bestSpecies)
                {
                    SaveSpecies(output, species);
                }
            }
            output.Flush();
        }

        private void SaveSpecies(EncogWriteHelper output, ISpecies species)
        {
            output.AddColumn("s");
            output.AddColumn(species.Age);
            output.AddColumn(species.BestScore);
            output.AddColumn(species.GensNoImprovement);
            output.WriteLine();

            foreach (IGenome genome in species.Members)
            {
                var neatGenome = (NEATGenome)genome;
                output.AddColumn("g");
                output.AddColumn(neatGenome.AdjustedScore);
                output.AddColumn(neatGenome.Score);
                output.AddColumn(neatGenome.BirthGeneration);
                output.WriteLine();

                foreach (NEATNeuronGene neatNeuronGene in neatGenome.NeuronsChromosome)
                {
                    output.AddColumn("n");
                    output.AddColumn(neatNeuronGene.Id);
                    output.AddColumn(neatNeuronGene.ActivationFunction);
                    output.AddColumn(NeuronTypeToString(neatNeuronGene.NeuronType));
                    output.AddColumn(neatNeuronGene.InnovationId);
                    output.WriteLine();
                }
                foreach (NEATLinkGene neatLinkGene in neatGenome.LinksChromosome)
                {
                    output.AddColumn("l");
                    output.AddColumn(neatLinkGene.Id);
                    output.AddColumn(neatLinkGene.Enabled);
                    output.AddColumn(neatLinkGene.FromNeuronId);
                    output.AddColumn(neatLinkGene.ToNeuronId);
                    output.AddColumn(neatLinkGene.Weight);
                    output.AddColumn(neatLinkGene.InnovationId);
                    output.WriteLine();
                }

            }

        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(NEATPopulation); }
        }
    }
}
