using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist.Location;

namespace Encog.Persist
{
    /// <summary>
    /// An interface to either a memory or disk-based encog collection.
    /// </summary>
    public interface IEncogCollection
    {        
        /// <summary>
        /// Add an element to the collection.
        /// </summary>
        /// <param name="name">The name of the element being added.</param>
        /// <param name="obj">The object to be added.</param>
        void Add(String name, IEncogPersistedObject obj);


        /// <summary>
        /// Build the directory. This allows the contents of the collection to be
        /// listed.
        /// </summary>
        void BuildDirectory();

        /// <summary>
        /// Clear all elements from the collection.
        /// </summary>
        void Clear();

        /// <summary>
        /// Delete an object from the collection. 
        /// </summary>
        /// <param name="o">The object to be deleted.</param>
        void Delete(DirectoryEntry o);

        
        /// <summary>
        /// Delete a key from the collection. 
        /// </summary>
        /// <param name="key">The key to delete.</param>
        void Delete(String key);

        /// <summary>
        /// Determine if the specified key exists. 
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>True, if the key exists.</returns>
        bool Exists(String key);
     
        /// <summary>
        /// Find the object that corresponds to the specified directory entry, return
        /// null, if not found. 
        /// </summary>
        /// <param name="entry">The entry to find.</param>
        /// <returns>The object that was found, or null, if no object was found.</returns>
        IEncogPersistedObject Find(DirectoryEntry entry);
        
        /// <summary>
        /// Find the object that corresponds to the specified key. 
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <returns>The object that corresponds to the specified key.</returns>
        IEncogPersistedObject Find(String key);


        /// <summary>
        /// Get a list of all the objects in the collection, specified by
        /// DirectoryEntry objects.
        /// </summary>
        IList<DirectoryEntry> Directory { get; }

        /// <summary>
        /// The version of Encog that this file was created with.
        /// </summary>
        String EncogVersion { get; }

        /// <summary>
        /// The Encog file version.
        /// </summary>
        int FileVersion { get; }

        /// <summary>
        /// The platform that this file was created on.
        /// </summary>
        String Platform { get; }

        /// <summary>
        /// Update the properties of an element in the collection. This allows the
        /// element to be renamed, if needed. 
        /// </summary>
        /// <param name="name">The name of the object that is being updated.</param>
        /// <param name="newName">The new name, can be the same as the old name, if the
        /// description is to be updated only.</param>
        /// <param name="desc">The new description.</param>
        void UpdateProperties(String name, String newName,
                 String desc);

        /// <summary>
        /// The location this collection is stored at.
        /// </summary>
        IPersistenceLocation Location { get; }

    }
}
