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
using System.IO;
using Encog.ML.EA.Genome;
using Encog.ML.EA.Species;
using Encog.ML.Prg.ExpValue;
using Encog.ML.Prg.Ext;
using Encog.ML.Prg.Train;
using Encog.Persist;
using Encog.Util;
using Encog.Util.CSV;

namespace Encog.ML.Prg
{
    /// <summary>
    ///     Persist a population of Encog programs.
    /// </summary>
    public class PersistPrgPopulation : IEncogPersistor
    {
        /// <inheritdoc />
        public int FileVersion
        {
            get { return 1; }
        }

        /// <inheritdoc />
        public String PersistClassString
        {
            get { return "PrgPopulation"; }
        }

        /// <inheritdoc />
        public Object Read(Stream istream)
        {
            var context = new EncogProgramContext();

            var result = new PrgPopulation(context, 0);

            var reader = new EncogReadHelper(istream);
            EncogFileSection section;

            int count = 0;
            ISpecies lastSpecies = null;
            while ((section = reader.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("BASIC")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    IDictionary<string, string> prms = section.ParseParams();
                    EngineArray.PutAll(prms, result.Properties);
                }
                else if (section.SectionName.Equals("BASIC")
                         && section.SubSectionName.Equals("EPL-POPULATION"))
                {
                    foreach (string line in section.Lines)
                    {
                        IList<String> cols = EncogFileSection.SplitColumns(line);

                        if (String.Compare(cols[0], "s", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            lastSpecies = new BasicSpecies
                                {
                                    Age = int.Parse(cols[1]),
                                    BestScore = CSVFormat.EgFormat.Parse(cols[2]),
                                    Population = result,
                                    GensNoImprovement = int.Parse(cols[3])
                                };
                            result.Species.Add(lastSpecies);
                        }
                        else if (cols[0].Equals("p"))
                        {
                            double score;
                            double adjustedScore;

                            if (String.Compare(cols[1], "nan", StringComparison.OrdinalIgnoreCase) == 0
                                || String.Compare(cols[2], "nan", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                score = Double.NaN;
                                adjustedScore = Double.NaN;
                            }
                            else
                            {
                                score = CSVFormat.EgFormat.Parse(cols[1]);
                                adjustedScore = CSVFormat.EgFormat.Parse(cols[2]);
                            }

                            String code = cols[3];
                            var prg = new EncogProgram(context);
                            prg.CompileEPL(code);
                            prg.Score = score;
                            prg.Species = lastSpecies;
                            prg.AdjustedScore = adjustedScore;
                            if (lastSpecies == null)
                            {
                                throw new EncogError(
                                    "Have not defined a species yet");
                            }
                            lastSpecies.Add(prg);
                            count++;
                        }
                    }
                }
                else if (section.SectionName.Equals("BASIC")
                         && section.SubSectionName.Equals("EPL-OPCODES"))
                {
                    foreach (String line in section.Lines)
                    {
                        IList<string> cols = EncogFileSection.SplitColumns(line);
                        String name = cols[0];
                        int args = int.Parse(cols[1]);
                        result.Context.Functions.AddExtension(name, args);
                    }
                }
                else if (section.SectionName.Equals("BASIC")
                         && section.SubSectionName.Equals("EPL-SYMBOLIC"))
                {
                    bool first = true;
                    foreach (string line in section.Lines)
                    {
                        if (!first)
                        {
                            IList<String> cols = EncogFileSection.SplitColumns(line);
                            String name = cols[0];
                            String t = cols[1];
                            var vt = EPLValueType.Unknown;

                            if (string.Compare(t, "f", true) == 0)
                            {
                                vt = EPLValueType.FloatingType;
                            }
                            else if (string.Compare(t, "b", true) == 0)
                            {
                                vt = EPLValueType.BooleanType;
                            }
                            else if (string.Compare(t, "i", true) == 0)
                            {
                                vt = EPLValueType.IntType;
                            }
                            else if (string.Compare(t, "s", true) == 0)
                            {
                                vt = EPLValueType.StringType;
                            }
                            else if (string.Compare(t, "e", true) == 0)
                            {
                                vt = EPLValueType.EnumType;
                            }

                            int enumType = int.Parse(cols[2]);
                            int enumCount = int.Parse(cols[3]);
                            var mapping = new VariableMapping(
                                name, vt, enumType, enumCount);
                            if (mapping.Name.Length > 0)
                            {
                                result.Context.DefineVariable(mapping);
                            }
                            else
                            {
                                result.Context.Result = mapping;
                            }
                        }
                        else
                        {
                            first = false;
                        }
                    }
                }
            }
            result.PopulationSize = count;

            // set the best genome, should be the first genome in the first species
            if (result.Species.Count > 0)
            {
                ISpecies species = result.Species[0];
                if (species.Members.Count > 0)
                {
                    result.BestGenome = species.Members[0];
                }

                // set the leaders
                foreach (ISpecies sp in result.Species)
                {
                    if (sp.Members.Count > 0)
                    {
                        sp.Leader = sp.Members[0];
                    }
                }
            }
            return result;
        }

        /// <inheritdoc />
        public void Save(Stream ostream, Object obj)
        {
            var writer = new EncogWriteHelper(ostream);
            var pop = (PrgPopulation) obj;

            writer.AddSection("BASIC");
            writer.AddSubSection("PARAMS");
            writer.AddProperties(pop.Properties);
            writer.AddSubSection("EPL-OPCODES");
            foreach (IProgramExtensionTemplate temp in pop.Context
                                                          .Functions.OpCodes)
            {
                writer.AddColumn(temp.Name);
                writer.AddColumn(temp.ChildNodeCount);
                writer.WriteLine();
            }
            writer.AddSubSection("EPL-SYMBOLIC");
            writer.AddColumn("name");
            writer.AddColumn("type");
            writer.AddColumn("enum");
            writer.AddColumn("enum_type");
            writer.AddColumn("enum_count");
            writer.WriteLine();

            // write the first line, the result
            writer.AddColumn("");
            writer.AddColumn(GetType(pop.Context.Result));
            writer.AddColumn(pop.Context.Result.EnumType);
            writer.AddColumn(pop.Context.Result.EnumValueCount);
            writer.WriteLine();

            // write the next lines, the variables
            foreach (VariableMapping mapping in pop.Context.DefinedVariables)
            {
                writer.AddColumn(mapping.Name);
                writer.AddColumn(GetType(mapping));
                writer.AddColumn(mapping.EnumType);
                writer.AddColumn(mapping.EnumValueCount);
                writer.WriteLine();
            }
            writer.AddSubSection("EPL-POPULATION");
            foreach (ISpecies species in pop.Species)
            {
                if (species.Members.Count > 0)
                {
                    writer.AddColumn("s");
                    writer.AddColumn(species.Age);
                    writer.AddColumn(species.BestScore);
                    writer.AddColumn(species.GensNoImprovement);
                    writer.WriteLine();
                    foreach (IGenome genome in species.Members)
                    {
                        var prg = (EncogProgram) genome;
                        writer.AddColumn("p");
                        if (Double.IsInfinity(prg.Score)
                            || Double.IsNaN(prg.Score))
                        {
                            writer.AddColumn("NaN");
                            writer.AddColumn("NaN");
                        }
                        else
                        {
                            writer.AddColumn(prg.Score);
                            writer.AddColumn(prg.AdjustedScore);
                        }

                        writer.AddColumn(prg.GenerateEPL());
                        writer.WriteLine();
                    }
                }
            }

            writer.Flush();
        }

        /// <inheritdoc />
        public Type NativeType
        {
            get { return typeof (PrgPopulation); }
        }

        /// <summary>
        ///     Get the type string for the specified variable mapping.
        /// </summary>
        /// <param name="mapping">The mapping.</param>
        /// <returns>The value.</returns>
        private string GetType(VariableMapping mapping)
        {
            switch (mapping.VariableType)
            {
                case EPLValueType.FloatingType:
                    return "f";
                case EPLValueType.StringType:
                    return "s";
                case EPLValueType.BooleanType:
                    return "b";
                case EPLValueType.IntType:
                    return "i";
                case EPLValueType.EnumType:
                    return "e";
            }
            throw new EncogError("Unknown type: "
                                 + mapping.VariableType.ToString());
        }
    }
}
