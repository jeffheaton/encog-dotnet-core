using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Persist
{
    /// <summary>
    /// An object that is not stored at the top level of an EG file.  Therefore the name 
    /// and description attributes are not needed.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public abstract class BasicPersistedSubObject : IEncogPersistedObject
    {
        /// <summary>
        /// The description.
        /// </summary>
        public string Description
        {
            get
            {
                return null;
            }
            set
            {
                // ignore
            }
        }

        /// <summary>
        /// The name of this object.
        /// </summary>
        public string Name
        {
            get
            {
                return null;
            }
            set
            {
                // ignore
            }
        }

        /// <summary>
        /// The collection that this object belongs to.
        /// </summary>
        public IEncogCollection Collection
        {
            get
            {
                return null;
            }
            set
            {
                // ignore
            }
        }

        /// <summary>
        /// Subclasses should override this and provide a persistor.
        /// </summary>
        /// <returns></returns>
        public abstract IPersistor CreatePersistor();

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public virtual object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
