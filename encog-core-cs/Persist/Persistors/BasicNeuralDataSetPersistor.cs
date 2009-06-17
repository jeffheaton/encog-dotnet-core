using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;
using Encog.Util;
using Encog.Neural.Data.Basic;
using Encog.Parse.Tags.Read;
using Encog.Parse.Tags.Write;
using Encog.Neural.Data;

namespace Encog.Persist.Persistors
{
    /// <summary>
    /// The Encog persistor used to persist the BasicNeuralDataSet class.
    /// </summary>
    public class BasicNeuralDataSetPersistor : IPersistor
    {

        /// <summary>
        /// The item tag.
        /// </summary>
        public const String TAG_ITEM = "Item";

        /// <summary>
        /// The input tag.
        /// </summary>
        public const String TAG_INPUT = "Input";

        /// <summary>
        /// THe ideal tag.
        /// </summary>
        public const String TAG_IDEAL = "Ideal";

        /// <summary>
        /// The current data set being loaded.
        /// </summary>
        private BasicNeuralDataSet currentDataSet;



        /// <summary>
        /// Handle reading an item tag.
        /// </summary>
        /// <param name="xmlIn">The XML reader.</param>
        private void HandleItem(ReadXML xmlIn)
        {
            IDictionary<String, String> properties = xmlIn.ReadPropertyBlock();
            INeuralDataPair pair = null;
            INeuralData input = new BasicNeuralData(ReadCSV
                   .FromCommas(properties
                           [BasicNeuralDataSetPersistor.TAG_INPUT]));

            if (properties.ContainsKey(BasicNeuralDataSetPersistor.TAG_IDEAL))
            {
                // supervised
                INeuralData ideal = new BasicNeuralData(ReadCSV
                       .FromCommas(properties
                               [BasicNeuralDataSetPersistor.TAG_IDEAL]));
                pair = new BasicNeuralDataPair(input, ideal);
            }
            else
            {
                // unsupervised
                pair = new BasicNeuralDataPair(input);
            }

            this.currentDataSet.Add(pair);
        }

        /// <summary>
        /// Load the specified Encog object from an XML reader.
        /// </summary>
        /// <param name="xmlIn">The XML reader to use.</param>
        /// <returns>The loaded object.</returns>
        public IEncogPersistedObject Load(ReadXML xmlIn)
        {

            String name = xmlIn.LastTag.Attributes[
                   EncogPersistedCollection.ATTRIBUTE_NAME];
            String description = xmlIn.LastTag.GetAttributeValue(
                   EncogPersistedCollection.ATTRIBUTE_DESCRIPTION);

            this.currentDataSet = new BasicNeuralDataSet();
            this.currentDataSet.Name = name;
            this.currentDataSet.Description = description;

            while (xmlIn.ReadToTag())
            {
                if (xmlIn.IsIt(BasicNeuralDataSetPersistor.TAG_ITEM, true))
                {
                    HandleItem(xmlIn);
                }
                else if (xmlIn.IsIt(BasicNeuralDataSetPersistor.TAG_ITEM, false))
                {
                    break;
                }

            }

            return this.currentDataSet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="xmlOut"></param>
        public void Save(IEncogPersistedObject obj, WriteXML xmlOut)
        {
            PersistorUtil.BeginEncogObject(EncogPersistedCollection.TYPE_TRAINING,
                    xmlOut, obj, true);
            INeuralDataSet set = (INeuralDataSet)obj;
            StringBuilder builder = new StringBuilder();

            foreach (INeuralDataPair pair in set)
            {
                xmlOut.BeginTag(BasicNeuralDataSetPersistor.TAG_ITEM);

                ReadCSV.ToCommas(builder, pair.Input.Data);
                xmlOut.AddProperty(BasicNeuralDataSetPersistor.TAG_INPUT, builder
                        .ToString());

                if (pair.Ideal != null)
                {
                    ReadCSV.ToCommas(builder, pair.Ideal.Data);
                    xmlOut.AddProperty(BasicNeuralDataSetPersistor.TAG_IDEAL, builder
                            .ToString());
                }
                xmlOut.EndTag();
            }
            xmlOut.EndTag();

        }

    }

}
