using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Parse.Tags.Read;
using Encog.Neural.Networks.Synapse;
using Encog.Parse.Tags.Write;

namespace Encog.Persist.Persistors
{
    /// <summary>
    /// The Encog persistor used to persist the OneToOneSynapse class.
    /// </summary>
    public class OneToOneSynapsePersistor : IPersistor
    {
        /// <summary>
        /// Load the specified Encog object from an XML reader.
        /// </summary>
        /// <param name="xmlIn">The XML reader to use.</param>
        /// <returns>The loaded object.</returns>
        public IEncogPersistedObject Load(ReadXML xmlIn)
        {

            OneToOneSynapse synapse = new OneToOneSynapse();
            return synapse;
        }

        /// <summary>
        /// Save the specified Encog object to an XML writer.
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <param name="xmlOut">The XML writer to save to.</param>
        public void Save(IEncogPersistedObject obj, WriteXML xmlOut)
        {
            PersistorUtil.BeginEncogObject(
                    EncogPersistedCollection.TYPE_ONE2ONE_SYNAPSE, xmlOut, obj, false);
            xmlOut.EndTag();
        }

    }

}
