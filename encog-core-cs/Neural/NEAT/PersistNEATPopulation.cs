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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        /**
         * Type for the Compositional pattern-producing networks used by HyperNEAT.
         */
        public const String TYPE_CPPN = "cppn";

        /**
         * Convert a NEATNeuronType enum to a string.
         * @param t The type.
         * @return The string type.
         */
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

        public static NEATNeuronType StringToNeuronType(String t)
        {
            if (t.Equals("b"))
            {
                return NEATNeuronType.Bias;
            }
            else if (t.Equals("h"))
            {
                return NEATNeuronType.Hidden;
            }
            else if (t.Equals("i"))
            {
                return NEATNeuronType.Input;
            }
            else if (t.Equals("n"))
            {
                return NEATNeuronType.None;
            }
            else if (t.Equals("o"))
            {
                return NEATNeuronType.Output;
            }
            else
            {
                return NEATNeuronType.Unknown;
            }
        }

        public int FileVersion
        {
            get
            {
                return 1;
            }
        }

        public String PersistClassString
        {
            get
            {
                return typeof(NEATPopulation).Name;
            }
        }

        public Object Read(Stream istream)
        {
            long nextInnovationID = 0;
            long nextGeneID = 0;

            NEATPopulation result = new NEATPopulation();
            NEATInnovationList innovationList = new NEATInnovationList();
            innovationList.Population = result;
            result.Innovations = innovationList;
            EncogReadHelper reader = new EncogReadHelper(istream);
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
                        NEATInnovation innovation = new NEATInnovation();
                        int innovationID = int.Parse(cols[1]);
                        innovation.InnovationID = innovationID;
                        innovation.NeuronID = int.Parse(cols[2]);
                        result.Innovations.Innovations[cols[0]] = innovation;
                        nextInnovationID = Math.Max(nextInnovationID, innovationID + 1);
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

                        if (string.Compare(cols[0], "s", true) == 0)
                        {
                            lastSpecies = new BasicSpecies();
                            lastSpecies.Population = result;
                            lastSpecies.Age = int.Parse(cols[1]);
                            lastSpecies.BestScore = CSVFormat.EgFormat.Parse(cols[2]);
                            lastSpecies.GensNoImprovement = int.Parse(cols[3]);
                            result.Species.Add(lastSpecies);
                        }
                        else if (string.Compare(cols[0], "g", true) == 0)
                        {
                            bool isLeader = lastGenome == null;
                            lastGenome = new NEATGenome();
                            lastGenome.InputCount = result.InputCount;
                            lastGenome.OutputCount = result.OutputCount;
                            lastGenome.Species = lastSpecies;
                            lastGenome.AdjustedScore = CSVFormat.EgFormat.Parse(cols[1]);
                            lastGenome.Score = CSVFormat.EgFormat.Parse(cols[2]);
                            lastGenome.BirthGeneration = int.Parse(cols[3]);
                            lastSpecies.Add(lastGenome);
                            if (isLeader)
                            {
                                lastSpecies.Leader = lastGenome;
                            }
                        }
                        else if (string.Compare(cols[0], "n", true) == 0)
                        {
                            NEATNeuronGene neuronGene = new NEATNeuronGene();
                            int geneID = int.Parse(cols[1]);
                            neuronGene.Id = geneID;

                            IActivationFunction af = EncogFileSection.ParseActivationFunction(cols[2]);
                            neuronGene.ActivationFunction = af;

                            neuronGene.NeuronType = PersistNEATPopulation.StringToNeuronType(cols[3]);
                            neuronGene.InnovationId = int.Parse(cols[4]);
                            lastGenome.NeuronsChromosome.Add(neuronGene);
                            nextGeneID = Math.Max(geneID + 1, nextGeneID);
                        }
                        else if (string.Compare(cols[0], "l", true) == 0)
                        {
                            NEATLinkGene linkGene = new NEATLinkGene();
                            linkGene.Id = int.Parse(cols[1]);
                            linkGene.Enabled = (int.Parse(cols[2]) > 0);
                            linkGene.FromNeuronID = int.Parse(cols[3]);
                            linkGene.ToNeuronID = int.Parse(cols[4]);
                            linkGene.Weight = CSVFormat.EgFormat.Parse(cols[5]);
                            linkGene.InnovationId = int.Parse(cols[6]);
                            lastGenome.LinksChromosome.Add(linkGene);
                        }
                    }

                }
                else if (section.SectionName.Equals("NEAT-POPULATION")
                      && section.SubSectionName.Equals("CONFIG"))
                {
                    IDictionary<string, string> prm = section.ParseParams();

                    string afStr = prm[NEATPopulation.PROPERTY_NEAT_ACTIVATION];

                    if (string.Compare(afStr, PersistNEATPopulation.TYPE_CPPN, true) == 0)
                    {
                        HyperNEATGenome.BuildCPPNActivationFunctions(result.ActivationFunctions);
                    }
                    else
                    {
                        result.NEATActivationFunction = EncogFileSection.ParseActivationFunction(prm,
                                        NEATPopulation.PROPERTY_NEAT_ACTIVATION);
                    }

                    result.ActivationCycles = EncogFileSection.ParseInt(prm,
                            PersistConst.ActivationCycles);
                    result.InputCount = EncogFileSection.ParseInt(prm,
                            PersistConst.InputCount);
                    result.OutputCount = EncogFileSection.ParseInt(prm,
                            PersistConst.OutputCount);
                    result.PopulationSize = EncogFileSection.ParseInt(prm,
                            NEATPopulation.PROPERTY_POPULATION_SIZE);
                    result.SurvivalRate = EncogFileSection.ParseDouble(prm,
                            NEATPopulation.PROPERTY_SURVIVAL_RATE);
                    result.ActivationCycles = EncogFileSection.ParseInt(prm,
                            NEATPopulation.PROPERTY_CYCLES);
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
            result.InnovationIDGenerate.CurrentID = nextInnovationID;
            result.GeneIDGenerate.CurrentID = nextGeneID;

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

        public void Save(Stream os, Object obj)
        {
            EncogWriteHelper output = new EncogWriteHelper(os);
            NEATPopulation pop = (NEATPopulation)obj;
            output.AddSection("NEAT-POPULATION");
            output.AddSubSection("CONFIG");
            output.WriteProperty(PersistConst.ActivationCycles, pop.ActivationCycles);

            if (pop.IsHyperNEAT)
            {
                output.WriteProperty(NEATPopulation.PROPERTY_NEAT_ACTIVATION,
                        PersistNEATPopulation.TYPE_CPPN);
            }
            else
            {
                IActivationFunction af = pop.ActivationFunctions.Contents[0].obj;
                output.WriteProperty(NEATPopulation.PROPERTY_NEAT_ACTIVATION, af);
            }

            output.WriteProperty(PersistConst.InputCount, pop.InputCount);
            output.WriteProperty(PersistConst.OutputCount, pop.OutputCount);
            output.WriteProperty(NEATPopulation.PROPERTY_CYCLES, pop.ActivationCycles);
            output.WriteProperty(NEATPopulation.PROPERTY_POPULATION_SIZE, pop.PopulationSize);
            output.WriteProperty(NEATPopulation.PROPERTY_SURVIVAL_RATE, pop.SurvivalRate);
            output.AddSubSection("INNOVATIONS");
            if (pop.Innovations != null)
            {
                foreach (string key in pop.Innovations.Innovations.Keys)
                {
                    NEATInnovation innovation = pop.Innovations.Innovations[key];
                    output.AddColumn(key);
                    output.AddColumn(innovation.InnovationID);
                    output.AddColumn(innovation.NeuronID);
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
                NEATGenome neatGenome = (NEATGenome)genome;
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
                    output.AddColumn(PersistNEATPopulation.NeuronTypeToString(neatNeuronGene.NeuronType));
                    output.AddColumn(neatNeuronGene.InnovationId);
                    output.WriteLine();
                }
                foreach (NEATLinkGene neatLinkGene in neatGenome.LinksChromosome)
                {
                    output.AddColumn("l");
                    output.AddColumn(neatLinkGene.Id);
                    output.AddColumn(neatLinkGene.Enabled);
                    output.AddColumn(neatLinkGene.FromNeuronID);
                    output.AddColumn(neatLinkGene.ToNeuronID);
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
