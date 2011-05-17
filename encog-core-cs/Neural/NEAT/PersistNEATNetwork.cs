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
                    IDictionary<String, String> params_0 = section.ParseParams();

                    result.InputCount = EncogFileSection.ParseInt(params_0,
                                                                  PersistConst.INPUT_COUNT);
                    result.OutputCount = EncogFileSection.ParseInt(params_0,
                                                                   PersistConst.OUTPUT_COUNT);
                    result.ActivationFunction = EncogFileSection
                        .ParseActivationFunction(params_0,
                                                 PersistConst.ACTIVATION_FUNCTION);
                    result.OutputActivationFunction = EncogFileSection
                        .ParseActivationFunction(params_0,
                                                 NEATPopulation.PROPERTY_OUTPUT_ACTIVATION);
                    result.NetworkDepth = EncogFileSection.ParseInt(params_0,
                                                                    PersistConst.DEPTH);
                    result.Snapshot = EncogFileSection.ParseBoolean(params_0,
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
                    foreach (String line_1  in  section.Lines)
                    {
                        IList<String> cols_2 = EncogFileSection.SplitColumns(line_1);
                        int fromID = Int32.Parse(cols_2[0]);
                        int toID = Int32.Parse(cols_2[1]);
                        bool recurrent = Int32.Parse(cols_2[2]) > 0;
                        double weight = CSVFormat.EG_FORMAT.Parse(cols_2[3]);
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
            xout.WriteProperty(NEATPopulation.PROPERTY_OUTPUT_ACTIVATION,
                               neat.OutputActivationFunction);
            xout.WriteProperty(PersistConst.DEPTH, neat.NetworkDepth);
            xout.WriteProperty(PersistConst.SNAPSHOT, neat.Snapshot);

            xout.AddSubSection("NEURONS");

            foreach (NEATNeuron neatNeuron  in  neat.Neurons)
            {
                xout.AddColumn(neatNeuron.NeuronID);
                xout.AddColumn(PersistNEATPopulation.NeuronTypeToString(neatNeuron.NeuronType));
                xout.AddColumn(neatNeuron.ActivationResponse);
                xout.AddColumn(neatNeuron.SplitX);
                xout.AddColumn(neatNeuron.SplitY);
                xout.WriteLine();
            }

            xout.AddSubSection("LINKS");

            foreach (NEATNeuron neatNeuron_0  in  neat.Neurons)
            {
                foreach (NEATLink link  in  neatNeuron_0.OutputboundLinks)
                {
                    WriteLink(xout, link);
                }
            }

            xout.Flush();
        }

        #endregion

        private void WriteLink(EncogWriteHelper xout, NEATLink link)
        {
            xout.AddColumn(link.FromNeuron.NeuronID);
            xout.AddColumn(link.ToNeuron.NeuronID);
            xout.AddColumn(link.Recurrent);
            xout.AddColumn(link.Weight);
            xout.WriteLine();
        }
    }
}