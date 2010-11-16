using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist.Persistors.Generic;

namespace Encog.Persist
{
    /// <summary>
    /// A basic Encog persisted object. Provides the name, description and collection
    /// attributes. Also provides a generic persistor.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public abstract class BasicPersistedObject : IEncogPersistedObject
    {
        /// <summary>
        /// The name of the object.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The description of the object.
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// The collection the object belongs to.
        /// </summary>
#if !SILVERLIGHT
        [NonSerialized]
#endif
        public IEncogCollection collection;

        /// <summary>
        /// The collection the object belongs to.
        /// </summary>
        public IEncogCollection Collection
        {
            get
            {
                return this.collection;
            }
            set
            {
                this.collection = value;
            }
        }

        /// <inheritdoc/>
        public virtual IPersistor CreatePersistor()
        {
            return new GenericPersistor(this.GetType());
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A cloned object.</returns>
        public virtual object Clone()
        {
            throw new NotImplementedException();
        }

    }
}
