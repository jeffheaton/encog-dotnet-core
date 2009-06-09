using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Parse.Tags.Read;
using Encog.Parse.Tags.Write;
using Encog.Neural.Networks.Synapse;

namespace Encog.Persist.Persistors
{
    /// <summary>
    /// The Encog persistor used to persist the ActivationBiPolar class.
    /// </summary>
    public class WeightedSynapsePersistor : IPersistor
    {

        /// <summary>
        /// The weights tag.
        /// </summary>
        public const String TAG_WEIGHTS = "weights";


        /// <summary>
        /// Load the specified Encog object from an XML reader.
        /// </summary>
        /// <param name="xmlIn">The XML reader to use.</param>
        /// <returns>The loaded object.</returns>
        public IEncogPersistedObject Load(ReadXML xmlIn)
        {
            WeightedSynapse synapse = new WeightedSynapse();

            String end = xmlIn.LastTag.Name;

            while (xmlIn.ReadToTag())
            {

                if (xmlIn.IsIt(WeightedSynapsePersistor.TAG_WEIGHTS, true))
                {
                    xmlIn.ReadToTag();
                    synapse.WeightMatrix = PersistorUtil.loadMatrix(xmlIn);
                }
                if (xmlIn.IsIt(end, false))
                {
                    break;
                }
            }

            return synapse;
        }

        /// <summary>
        /// Save the specified Encog object to an XML writer.
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <param name="xmlOut">The XML writer to save to.</param>
        public void Save(IEncogPersistedObject obj, WriteXML xmlOut)
        {
            PersistorUtil
                    .BeginEncogObject(
                            EncogPersistedCollection.TYPE_WEIGHTED_SYNAPSE, xmlOut,
                            obj, false);
            WeightedSynapse synapse = (WeightedSynapse)obj;

            xmlOut.BeginTag(WeightedSynapsePersistor.TAG_WEIGHTS);
            PersistorUtil.saveMatrix(synapse.WeightMatrix, xmlOut);
            xmlOut.EndTag();

            xmlOut.EndTag();
        }

    }

}
