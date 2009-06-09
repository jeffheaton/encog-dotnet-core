using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Parse.Tags.Read;
using Encog.Parse.Tags.Write;

namespace Encog.Persist
{
    /// <summary>
    /// This interface defines a class that can load and save an
    /// EncogPersistedObject.
    /// </summary>
    public interface IPersistor
    {

        /// <summary>
        /// Load from the specified node.
        /// </summary>
        /// <param name="xmlin">The node to load from.</param>
        /// <returns>The EncogPersistedObject that was loaded.</returns>
        IEncogPersistedObject Load(ReadXML xmlin);

        /// <summary>
        /// Save the specified object.
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <param name="xmlout">The XML object.</param>
        void Save(IEncogPersistedObject obj, WriteXML xmlout);
    }

}
