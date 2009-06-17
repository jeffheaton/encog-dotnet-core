using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Persist
{
    /// <summary>
    /// An Encog persisted object, that can be written to XML.
    /// </summary>
    public interface IEncogPersistedObject : ICloneable
    {
        /// <summary>
        /// The description for this object.
        /// </summary>
        String Description
        {
            get;
            set;
        }

        /// <summary>
        /// The name of this object.
        /// </summary>
        String Name
        {
            get;
            set;
        }

        /// <summary>
        /// Create a persistor for this object.
        /// </summary>
        /// <returns>A persistor for this object.</returns>
        IPersistor CreatePersistor();

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>A cloned version of this object.</returns>
        new Object Clone();
    }
}
