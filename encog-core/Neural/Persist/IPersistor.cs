using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Encog.Neural.Persist
{
    public interface IPersistor
    {
        /// <summary>
        /// Load from the specified node. 
        /// </summary>
        /// <param name="node">The node to load from.</param>
        /// <returns>The EncogPersistedObject that was loaded.</returns>
        IEncogPersistedObject Load(XmlElement node);



        /// <summary>
        /// Save the specified object.
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <param name="hd">The XML object.</param>
        void Save(IEncogPersistedObject obj, XmlTextWriter hd);
    }
}
