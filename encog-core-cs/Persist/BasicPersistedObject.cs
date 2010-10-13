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
    public abstract class BasicPersistedObject : IEncogPersistedObject
    {

	/**
	 * The name of the object.
	 */
	public String Name { get; set; }

	/**
	 * The description of the object.
	 */
    public String Description { get; set; }

	/**
	 * The collection the object belongs to.
	 */
    [NonSerialized]
    public IEncogCollection collection;


    /**
 * The collection the object belongs to.
 */
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

	/**
	 * {@inheritDoc}
	 */
	public IPersistor CreatePersistor() {
		return new GenericPersistor(this.GetType());
	}


    /**
 * {@inheritDoc}
 */
    public abstract object Clone();    
    }
}
