using System;
using System.Collections.Generic;
using System.IO;
using Encog.ML.Genetic.Genes;
using Encog.ML.Genetic.Genome;
using Encog.ML.Genetic.Innovation;
using Encog.ML.Genetic.Population;
using Encog.ML.Genetic.Species;
using Encog.Neural.NEAT;
using Encog.Neural.Neat.Training;
using Encog.Neural.NEAT.Training;
using Encog.Persist;
using Encog.Util.CSV;

namespace Encog.Neural.Neat
{
    /// <summary>
    /// Persist the NEAT population.
    /// </summary>
    [Serializable]
    public class PersistNEATPopulation : EncogPersistor
    {
        #region EncogPersistor Members

        /// <summary>
        /// The persistence class string.
        /// </summary>
        public virtual String PersistClassString
        {
            get { return typeof (NEATPopulation).Name; }
        }


        /// <summary>
        /// Read the object.
        /// </summary>
        /// <param name="mask0">The stream to read the object from.</param>
        /// <returns>The object that was loaded.</returns>
        public virtual Object Read(Stream mask0)
        {
            var result = new NEATPopulation();
            var innovationList = new NEATInnovationList();
            innovationList.Population = result;
            result.Innovations = innovationList;
            var ins0 = new EncogReadHelper(mask0);
            IDictionary<Int32, ISpecies> speciesMap = new Dictionary<Int32, ISpecies>();
            IDictionary<ISpecies, Int32> leaderMap = new Dictionary<ISpecies, Int32>();
            IDictionary<Int32, IGenome> genomeMap = new Dictionary<Int32, IGenome>();
            EncogFileSection section;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("NEAT-POPULATION")
                    && section.SubSectionName.Equals("INNOVATIONS"))
                {
                    foreach (String line  in  section.Lines)
                    {
                        IList<String> cols = EncogFileSection.SplitColumns(line);
                        var innovation = new NEATInnovation();
                        innovation.InnovationID = Int32.Parse(cols[0]);
                        innovation.InnovationType = StringToInnovationType(cols[1]);
                        innovation.NeuronType = StringToNeuronType(cols[2]);
                        innovation.SplitX = CSVFormat.EG_FORMAT.Parse(cols[3]);
                        innovation.SplitY = CSVFormat.EG_FORMAT.Parse(cols[4]);
                        innovation.NeuronID = Int32.Parse(cols[5]);
                        innovation.FromNeuronID = Int32.Parse(cols[6]);
                        innovation.ToNeuronID = Int32.Parse(cols[7]);
                        result.Innovations.Add(innovation);
                    }
                }
                else if (section.SectionName.Equals("NEAT-POPULATION")
                         && section.SubSectionName.Equals("SPECIES"))
                {
                    foreach (String line_0  in  section.Lines)
                    {
                        String[] cols_1 = line_0.Split(',');
                        var species = new BasicSpecies();

                        species.SpeciesID = Int32.Parse(cols_1[0]);
                        species.Age = Int32.Parse(cols_1[1]);
                        species.BestScore = CSVFormat.EG_FORMAT.Parse(cols_1[2]);
                        species.GensNoImprovement = Int32.Parse(cols_1[3]);
                        species.SpawnsRequired = CSVFormat.EG_FORMAT
                            .Parse(cols_1[4]);
                        species.SpawnsRequired = CSVFormat.EG_FORMAT
                            .Parse(cols_1[5]);
                        leaderMap[(species)] = (Int32.Parse(cols_1[6]));
                        result.Species.Add(species);
                        speciesMap[((int) species.SpeciesID)] = (species);
                    }
                }
                else if (section.SectionName.Equals("NEAT-POPULATION")
                         && section.SubSectionName.Equals("GENOMES"))
                {
                    NEATGenome lastGenome = null;

                    foreach (String line_2  in  section.Lines)
                    {
                        IList<String> cols_3 = EncogFileSection.SplitColumns(line_2);
                        if (cols_3[0].Equals("g", StringComparison.InvariantCultureIgnoreCase))
                        {
                            lastGenome = new NEATGenome();
                            lastGenome.NeuronsChromosome = new Chromosome();
                            lastGenome.LinksChromosome = new Chromosome();
                            lastGenome.Chromosomes.Add(lastGenome.NeuronsChromosome);
                            lastGenome.Chromosomes.Add(lastGenome.LinksChromosome);
                            lastGenome.GenomeID = Int32.Parse(cols_3[1]);
                            lastGenome.SpeciesID = Int32.Parse(cols_3[2]);
                            lastGenome.AdjustedScore = CSVFormat.EG_FORMAT
                                .Parse(cols_3[3]);
                            lastGenome.AmountToSpawn = CSVFormat.EG_FORMAT
                                .Parse(cols_3[4]);
                            lastGenome.NetworkDepth = Int32.Parse(cols_3[5]);
                            lastGenome.Score = CSVFormat.EG_FORMAT.Parse(cols_3[6]);
                            result.Add(lastGenome);
                            genomeMap[(int) lastGenome.GenomeID] = lastGenome;
                        }
                        else if (cols_3[0].Equals("n", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var neuronGene = new NEATNeuronGene();
                            neuronGene.Id = Int32.Parse(cols_3[1]);
                            neuronGene.NeuronType = StringToNeuronType(cols_3[2]);
                            neuronGene.Enabled = Int32.Parse(cols_3[3]) > 0;
                            neuronGene.InnovationId = Int32.Parse(cols_3[4]);
                            neuronGene.ActivationResponse = CSVFormat.EG_FORMAT
                                .Parse(cols_3[5]);
                            neuronGene.SplitX = CSVFormat.EG_FORMAT.Parse(cols_3[6]);
                            neuronGene.SplitY = CSVFormat.EG_FORMAT.Parse(cols_3[7]);
                            lastGenome.Neurons.Add(neuronGene);
                        }
                        else if (cols_3[0].Equals("l", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var linkGene = new NEATLinkGene();
                            linkGene.Id = Int32.Parse(cols_3[1]);
                            linkGene.Enabled = Int32.Parse(cols_3[2]) > 0;
                            linkGene.Recurrent = Int32.Parse(cols_3[3]) > 0;
                            linkGene.FromNeuronID = Int32.Parse(cols_3[4]);
                            linkGene.ToNeuronID = Int32.Parse(cols_3[5]);
                            linkGene.Weight = CSVFormat.EG_FORMAT.Parse(cols_3[6]);
                            linkGene.InnovationId = Int32.Parse(cols_3[7]);
                            lastGenome.Links.Add(linkGene);
                        }
                    }
                }
                else if (section.SectionName.Equals("NEAT-POPULATION")
                         && section.SubSectionName.Equals("CONFIG"))
                {
                    IDictionary<String, String> paras = section.ParseParams();

                    result.NeatActivationFunction = EncogFileSection
                        .ParseActivationFunction(paras,
                                                 NEATPopulation.PROPERTY_NEAT_ACTIVATION);
                    result.OutputActivationFunction = EncogFileSection
                        .ParseActivationFunction(paras,
                                                 NEATPopulation.PROPERTY_OUTPUT_ACTIVATION);
                    result.Snapshot = EncogFileSection.ParseBoolean(paras,
                                                                    PersistConst.SNAPSHOT);
                    result.InputCount = EncogFileSection.ParseInt(paras,
                                                                  PersistConst.INPUT_COUNT);
                    result.OutputCount = EncogFileSection.ParseInt(paras,
                                                                   PersistConst.OUTPUT_COUNT);
                    result.OldAgePenalty = EncogFileSection.ParseDouble(paras,
                                                                        PopulationConst.PROPERTY_OLD_AGE_PENALTY);
                    result.OldAgeThreshold = EncogFileSection.ParseInt(paras,
                                                                       PopulationConst.PROPERTY_OLD_AGE_THRESHOLD);
                    result.PopulationSize = EncogFileSection.ParseInt(paras,
                                                                      PopulationConst.PROPERTY_POPULATION_SIZE);
                    result.SurvivalRate = EncogFileSection.ParseDouble(paras,
                                                                       PopulationConst.PROPERTY_SURVIVAL_RATE);
                    result.YoungBonusAgeThreshhold = EncogFileSection.ParseInt(
                        paras, PopulationConst.PROPERTY_YOUNG_AGE_THRESHOLD);
                    result.YoungScoreBonus = EncogFileSection.ParseDouble(paras,
                                                                          PopulationConst.PROPERTY_YOUNG_AGE_BONUS);
                    result.GenomeIDGenerate.CurrentID = EncogFileSection.ParseInt(paras,
                                                                                  PopulationConst.
                                                                                      PROPERTY_NEXT_GENOME_ID);
                    result.InnovationIDGenerate.CurrentID = EncogFileSection.ParseInt(paras,
                                                                                      PopulationConst.
                                                                                          PROPERTY_NEXT_INNOVATION_ID);
                    result.GeneIDGenerate.CurrentID = EncogFileSection.ParseInt(paras,
                                                                                PopulationConst.
                                                                                    PROPERTY_NEXT_GENE_ID);
                    result.SpeciesIDGenerate.CurrentID = EncogFileSection.ParseInt(paras,
                                                                                   PopulationConst.
                                                                                       PROPERTY_NEXT_SPECIES_ID);
                }
            }

            // now link everything up


            // first put all the genomes into correct species
            foreach (IGenome genome  in  result.Genomes)
            {
                var neatGenome = (NEATGenome) genome;
                var speciesId = (int) neatGenome.SpeciesID;
                if( speciesMap.ContainsKey(speciesId))
                {
                    ISpecies s = speciesMap[speciesId];
                    s.Members.Add(neatGenome);
                }
                
                neatGenome.InputCount = result.InputCount;
                neatGenome.OutputCount = result.OutputCount;
            }


            // set the species leader links
            foreach (ISpecies species_5  in  leaderMap.Keys)
            {
                int leaderID = leaderMap[species_5];
                IGenome leader = genomeMap[leaderID];
                species_5.Leader = leader;
                ((BasicSpecies) species_5).Population = result;
            }

            return result;
        }

        /// <summary>
        /// Save the object.
        /// </summary>
        /// <param name="os">The stream to write to.</param>
        /// <param name="obj">The object to save.</param>
        public virtual void Save(Stream os, Object obj)
        {
            var xout = new EncogWriteHelper(os);
            var pop = (NEATPopulation) obj;
            xout.AddSection("NEAT-POPULATION");
            xout.AddSubSection("CONFIG");
            xout.WriteProperty(PersistConst.SNAPSHOT, pop.Snapshot);
            xout.WriteProperty(NEATPopulation.PROPERTY_OUTPUT_ACTIVATION,
                               pop.OutputActivationFunction);
            xout.WriteProperty(NEATPopulation.PROPERTY_NEAT_ACTIVATION,
                               pop.NeatActivationFunction);
            xout.WriteProperty(PersistConst.INPUT_COUNT, pop.InputCount);
            xout.WriteProperty(PersistConst.OUTPUT_COUNT, pop.OutputCount);
            xout.WriteProperty(PopulationConst.PROPERTY_OLD_AGE_PENALTY,
                               pop.OldAgePenalty);
            xout.WriteProperty(PopulationConst.PROPERTY_OLD_AGE_THRESHOLD,
                               pop.OldAgeThreshold);
            xout.WriteProperty(PopulationConst.PROPERTY_POPULATION_SIZE,
                               pop.PopulationSize);
            xout.WriteProperty(PopulationConst.PROPERTY_SURVIVAL_RATE,
                               pop.SurvivalRate);
            xout.WriteProperty(PopulationConst.PROPERTY_YOUNG_AGE_THRESHOLD,
                               pop.YoungBonusAgeThreshold);
            xout.WriteProperty(PopulationConst.PROPERTY_YOUNG_AGE_BONUS,
                               pop.YoungScoreBonus);
            xout.WriteProperty(PopulationConst.PROPERTY_NEXT_GENOME_ID, pop.GenomeIDGenerate.CurrentID);
            xout.WriteProperty(PopulationConst.PROPERTY_NEXT_INNOVATION_ID, pop.InnovationIDGenerate.CurrentID);
            xout.WriteProperty(PopulationConst.PROPERTY_NEXT_GENE_ID, pop.GeneIDGenerate.CurrentID);
            xout.WriteProperty(PopulationConst.PROPERTY_NEXT_SPECIES_ID, pop.SpeciesIDGenerate.CurrentID);
            xout.AddSubSection("INNOVATIONS");
            if (pop.Innovations != null)
            {
                foreach (IInnovation innovation  in  pop.Innovations.Innovations)
                {
                    var neatInnovation = (NEATInnovation) innovation;
                    xout.AddColumn(neatInnovation.InnovationID);
                    xout.AddColumn(InnovationTypeToString(neatInnovation.InnovationType));
                    xout.AddColumn(NeuronTypeToString(neatInnovation.NeuronType));
                    xout.AddColumn(neatInnovation.SplitX);
                    xout.AddColumn(neatInnovation.SplitY);
                    xout.AddColumn(neatInnovation.NeuronID);
                    xout.AddColumn(neatInnovation.FromNeuronID);
                    xout.AddColumn(neatInnovation.ToNeuronID);
                    xout.WriteLine();
                }
            }
            xout.AddSubSection("GENOMES");

            foreach (IGenome genome  in  pop.Genomes)
            {
                var neatGenome = (NEATGenome) genome;
                xout.AddColumn("g");
                xout.AddColumn(neatGenome.GenomeID);
                xout.AddColumn(neatGenome.SpeciesID);
                xout.AddColumn(neatGenome.AdjustedScore);
                xout.AddColumn(neatGenome.AmountToSpawn);
                xout.AddColumn(neatGenome.NetworkDepth);
                xout.AddColumn(neatGenome.Score);
                xout.WriteLine();


                foreach (IGene neuronGene  in  neatGenome.Neurons.Genes)
                {
                    var neatNeuronGene = (NEATNeuronGene) neuronGene;
                    xout.AddColumn("n");
                    xout.AddColumn(neatNeuronGene.Id);
                    xout.AddColumn(NeuronTypeToString(neatNeuronGene.NeuronType));
                    xout.AddColumn(neatNeuronGene.Enabled);
                    xout.AddColumn(neatNeuronGene.InnovationId);
                    xout.AddColumn(neatNeuronGene.ActivationResponse);
                    xout.AddColumn(neatNeuronGene.SplitX);
                    xout.AddColumn(neatNeuronGene.SplitY);
                    xout.WriteLine();
                }

                foreach (IGene linkGene  in  neatGenome.Links.Genes)
                {
                    var neatLinkGene = (NEATLinkGene) linkGene;
                    xout.AddColumn("l");
                    xout.AddColumn(neatLinkGene.Id);
                    xout.AddColumn(neatLinkGene.Enabled);
                    xout.AddColumn(neatLinkGene.Recurrent);
                    xout.AddColumn(neatLinkGene.FromNeuronID);
                    xout.AddColumn(neatLinkGene.ToNeuronID);
                    xout.AddColumn(neatLinkGene.Weight);
                    xout.AddColumn(neatLinkGene.InnovationId);
                    xout.WriteLine();
                }
            }
            xout.AddSubSection("SPECIES");

            foreach (ISpecies species  in  pop.Species)
            {
                xout.AddColumn(species.SpeciesID);
                xout.AddColumn(species.Age);
                xout.AddColumn(species.BestScore);
                xout.AddColumn(species.GensNoImprovement);
                xout.AddColumn(species.NumToSpawn);
                xout.AddColumn(species.SpawnsRequired);
                xout.AddColumn(species.Leader.GenomeID);
                xout.WriteLine();
            }
            xout.Flush();
        }

        /// <summary>
        /// The file version.
        /// </summary>
        public virtual int FileVersion
        {
            get { return 1; }
        }

        #endregion

        /// <summary>
        /// Convert the neuron type to a string.
        /// </summary>
        /// <param name="t">The neuron type.</param>
        /// <returns>The string.</returns>
        public static String NeuronTypeToString(NEATNeuronType t)
        {
            switch (t)
            {
                case NEATNeuronType.Bias:
                    return ("b");
                case NEATNeuronType.Hidden:
                    return ("h");
                case NEATNeuronType.Input:
                    return ("i");
                case NEATNeuronType.None:
                    return ("n");
                case NEATNeuronType.Output:
                    return ("o");
                default:
                    return null;
            }
        }

        /// <summary>
        /// Convert the innovation type to a string.
        /// </summary>
        /// <param name="t">The innovation type.</param>
        /// <returns>The string.</returns>
        public static String InnovationTypeToString(NEATInnovationType t)
        {
            switch (t)
            {
                case NEATInnovationType.NewLink:
                    return "l";
                case NEATInnovationType.NewNeuron:
                    return "n";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Convert a string to an innovation type.
        /// </summary>
        /// <param name="t">The string to convert.</param>
        /// <returns>The innovation type.</returns>
        public static NEATInnovationType StringToInnovationType(String t)
        {
            if (t.Equals("l", StringComparison.InvariantCultureIgnoreCase))
            {
                return NEATInnovationType.NewLink;
            }
            else if (t.Equals("n", StringComparison.InvariantCultureIgnoreCase))
            {
                return NEATInnovationType.NewNeuron;
            }
            else
            {
                return default(NEATInnovationType) /* was: null */;
            }
        }

        /// <summary>
        /// Convert a string to a neuron type.
        /// </summary>
        /// <param name="t">The string.</param>
        /// <returns>The resulting neuron type.</returns>
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
                return default(NEATNeuronType) /* was: null */;
            }
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(NEATPopulation); }
        }
    }
}