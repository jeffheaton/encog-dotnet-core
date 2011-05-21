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
using System.IO;
using Encog.Neural.Neat;
using Encog.Persist;
using Encog.Util.CSV;

namespace Encog.Neural.NEAT
{
    /// <summary>
    /// Persist a NEAT network.
    /// </summary>
    [Serializable]
    public class PersistNEATNetwork : EncogPersistor
    {
        #region EncogPersistor Members

        /// <summary>
        /// The file version.
        /// </summary>
        public virtual int FileVersion
        {
            get { return 1; }
        }

        /// <summary>
        /// The persist class string.
        /// </summary>
        public virtual String PersistClassString
        {
            get { return "NEATNetwork"; }
        }

        /// <summary>
        /// Read the object.
        /// </summary>
        /// <param name="mask0">The stream to read from.</param>
        /// <returns>The loaded object.</returns>
        public virtual Object Read(Stream mask0)
        {
            var result = new NEATNetwork();
            var ins0 = new EncogReadHelper(mask0);
            EncogFileSection section;
            IDictionary<Int32, NEATNeuron> neuronMap = new Dictionary<Int32, NEATNeuron>();

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("NEAT")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    IDictionary<String, String> paras = section.ParseParams();

                    foreach (String key in paras.Keys)
                    {
                        result.Properties.Add(key, paras[key]);
                    }
                }
                if (section.SectionName.Equals("NEAT")
                    && section.SubSectionName.Equals("NETWORK"))
                {
                    IDictionary<String, String> p = section.ParseParams();

                    result.InputCount = EncogFileSection.ParseInt(p,
                                                                  PersistConst.INPUT_COUNT);
                    result.OutputCount = EncogFileSection.ParseInt(p,
                                                                   PersistConst.OUTPUT_COUNT);
                    result.ActivationFunction = EncogFileSection
                        .ParseActivationFunction(p,
                                                 PersistConst.ACTIVATION_FUNCTION);
                    result.OutputActivationFunction = EncogFileSection
                        .ParseActivationFunction(p,
                                                 NEATPopulation.PropertyOutputActivation);
                    result.NetworkDepth = EncogFileSection.ParseInt(p,
                                                                    PersistConst.DEPTH);
                    result.Snapshot = EncogFileSection.ParseBoolean(p,
                                                                    PersistConst.SNAPSHOT);
                }
                else if (section.SectionName.Equals("NEAT")
                         && section.SubSectionName.Equals("NEURONS"))
                {
                    foreach (String line  in  section.Lines)
                    {
                        IList<String> cols = EncogFileSection.SplitColumns(line);

                        long neuronID = Int32.Parse(cols[0]);
                        NEATNeuronType neuronType = PersistNEATPopulation
                            .StringToNeuronType(cols[1]);
                        double activationResponse = CSVFormat.EG_FORMAT
                            .Parse(cols[2]);
                        double splitY = CSVFormat.EG_FORMAT
                            .Parse(cols[3]);
                        double splitX = CSVFormat.EG_FORMAT
                            .Parse(cols[4]);

                        var neatNeuron = new NEATNeuron(neuronType,
                                                        neuronID, splitY, splitX, activationResponse);
                        result.Neurons.Add(neatNeuron);
                        neuronMap[((int) neuronID)] = (neatNeuron);
                    }
                }
                else if (section.SectionName.Equals("NEAT")
                         && section.SubSectionName.Equals("LINKS"))
                {
                    foreach (String line  in  section.Lines)
                    {
                        IList<String> cols = EncogFileSection.SplitColumns(line);
                        int fromID = Int32.Parse(cols[0]);
                        int toID = Int32.Parse(cols[1]);
                        bool recurrent = Int32.Parse(cols[2]) > 0;
                        double weight = CSVFormat.EG_FORMAT.Parse(cols[3]);
                        NEATNeuron fromNeuron = (neuronMap[fromID]);
                        NEATNeuron toNeuron = (neuronMap[toID]);
                        var neatLink = new NEATLink(weight, fromNeuron,
                                                    toNeuron, recurrent);
                        fromNeuron.OutputboundLinks.Add(neatLink);
                        toNeuron.InboundLinks.Add(neatLink);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Save the object.
        /// </summary>
        /// <param name="os">The output stream.</param>
        /// <param name="obj">The object to save.</param>
        public virtual void Save(Stream os, Object obj)
        {
            var xout = new EncogWriteHelper(os);
            var neat = (NEATNetwork) obj;
            xout.AddSection("NEAT");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(neat.Properties);
            xout.AddSubSection("NETWORK");

            xout.WriteProperty(PersistConst.INPUT_COUNT, neat.InputCount);
            xout.WriteProperty(PersistConst.OUTPUT_COUNT, neat.OutputCount);
            xout.WriteProperty(PersistConst.ACTIVATION_FUNCTION,
                               neat.ActivationFunction);
            xout.WriteProperty(NEATPopulation.PropertyOutputActivation,
                               neat.OutputActivationFunction);
            xout.WriteProperty(PersistConst.DEPTH, neat.NetworkDepth);
            xout.WriteProperty(PersistConst.SNAPSHOT, neat.Snapshot);

            xout.AddSubSection("NEURONS");

            foreach (NEATNeuron neatNeuron  in  neat.Neurons)
            {
                xout.AddColumn((int)neatNeuron.NeuronID);
                xout.AddColumn(PersistNEATPopulation.NeuronTypeToString(neatNeuron.NeuronType));
                xout.AddColumn(neatNeuron.ActivationResponse);
                xout.AddColumn(neatNeuron.SplitX);
                xout.AddColumn(neatNeuron.SplitY);
                xout.WriteLine();
            }

            xout.AddSubSection("LINKS");

            foreach (NEATNeuron neatNeuron  in  neat.Neurons)
            {
                foreach (NEATLink link  in  neatNeuron.OutputboundLinks)
                {
                    WriteLink(xout, link);
                }
            }

            xout.Flush();
        }

        #endregion

        /// <summary>
        /// Write a link.
        /// </summary>
        /// <param name="xout">The output file.</param>
        /// <param name="link">The link.</param>
        private static void WriteLink(EncogWriteHelper xout, NEATLink link)
        {
            xout.AddColumn((int)link.FromNeuron.NeuronID);
            xout.AddColumn((int)link.ToNeuron.NeuronID);
            xout.AddColumn(link.Recurrent);
            xout.AddColumn(link.Weight);
            xout.WriteLine();
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(NEATNetwork); }
        }
    }
}
