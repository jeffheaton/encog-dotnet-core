using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Encog.Neural.Activation;

namespace Encog.Neural.Persist.Persistors
{
    class ActivationLinearPersistor:IPersistor
    {
        /// <summary>
        /// Load from the specified node.
        /// </summary>
        /// <param name="node">The node to load from.</param>
        /// <returns>The EncogPersistedObject that was loaded.</returns>
        public IEncogPersistedObject Load(XmlElement node)
        {
            return new ActivationLinear();
        }

        /// <summary>
        /// Save the specified object.
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <param name="hd">The XML object.</param>
        public void Save(IEncogPersistedObject obj, XmlTextWriter hd)
        {
        }
    }
}
