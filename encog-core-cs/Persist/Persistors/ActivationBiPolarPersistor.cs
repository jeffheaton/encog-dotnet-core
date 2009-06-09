using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Parse.Tags.Read;
using Encog.Parse.Tags.Write;
using Encog.Neural.Activation;

namespace Encog.Persist.Persistors
{
    /// <summary>
    /// The Encog persistor used to persist the ActivationBiPolar class.
    /// </summary>
    public class ActivationBiPolarPersistor : IPersistor
    {
        /// <summary>
        /// Load the specified Encog object from an XML reader.
        /// </summary>
        /// <param name="xmlIn">The XML reader to use.</param>
        /// <returns>The loaded object.</returns>
        public IEncogPersistedObject Load(ReadXML xmlIn)
        {
            return new ActivationBiPolar();
        }

        /// <summary>
        /// Save the specified Encog object to an XML writer.
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <param name="xmlOut">The XML writer to save to.</param>
        public void Save(IEncogPersistedObject obj, WriteXML xmlOut)
        {
            xmlOut.BeginTag(this.GetType().Name);
            xmlOut.EndTag();
        }
    }
}
