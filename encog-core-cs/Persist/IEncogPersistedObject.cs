using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Persist
{
    public interface IEncogPersistedObject : ICloneable
    {
        String Description
        {
            get;
            set;
        }

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
