// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist.Location;
using Encog.Parse.Tags.Read;
using Encog.Persist.Persistors;

namespace Encog.Persist
{
    /// <summary>
    /// A memory based collection of Encog objects.  Does not use the more complex temp
    /// file structure like EncogPersistedCollection, but also can't handle gigantic files.
    /// This class can load and save from/to Encog EG files.
    /// 
    /// This class can be very useful in the Silverlight version of Encog, which does not
    /// support the EncogPersistedCollection class.
    /// </summary>
    public class EncogMemoryCollection : IEncogCollection
    {
        /// <summary>
        /// The contents of this collection.
        /// </summary>
        public IDictionary<String, IEncogPersistedObject> Contents = new Dictionary<String, IEncogPersistedObject>();

        /// <summary>
        /// Populated during a load, the file version.
        /// </summary>
        public int FileVersion { get; set; }

        /// <summary>
        /// Populated during a load, the Encog version that created this file.
        /// </summary>
        public String EncogVersion { get; set; }

        /// <summary>
        /// Populated during a load, the platform that this was loaded to.
        /// </summary>
        public String Platform { get; set; }

        /// <summary>
        /// The location this collection is saved at.
        /// </summary>
        private IPersistenceLocation location;

        /// <summary>
        /// The elements in this collection.
        /// </summary>
	    private IList<DirectoryEntry> directory = new List<DirectoryEntry>();


        /// <summary>
        /// Load the contents of a location.
        /// </summary>
        /// <param name="location">The location to load from.</param>
        public void Load(IPersistenceLocation location)
        {
            PersistReader reader = null;
            this.location = location;

            try
            {
                reader = new PersistReader(location);
                IDictionary<String, String> header = reader.ReadHeader();
                if (header != null)
                {
                    this.FileVersion = int.Parse(header["fileVersion"]);
                    this.EncogVersion = header["encogVersion"];
                    this.Platform = header["platform"];
                }
                reader.AdvanceObjectsCollection();
                ReadXML xmlIn = reader.XMLInput;
                this.Contents.Clear();

                while (xmlIn.ReadToTag())
                {
                    if (xmlIn.IsIt(PersistReader.TAG_OBJECTS, false))
                    {
                        break;
                    }

                    String type = xmlIn.LastTag.Name;
                    String name = xmlIn.LastTag.Attributes["name"];

                    IPersistor persistor = PersistorUtil
                            .CreatePersistor(type);

                    if (persistor == null)
                    {
                        throw new PersistError("Do not know how to load: " + type);
                    }
                    IEncogPersistedObject obj = persistor.Load(xmlIn);
                    this.Contents[name] = obj;
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                BuildDirectory();
            }

        }

        /// <summary>
        /// Save the contents of this collection to a location.
        /// </summary>
        /// <param name="location">The location to save to.</param>
        public void Save(IPersistenceLocation location)
        {
            PersistWriter writer = null;

            writer = new PersistWriter(location);
            try
            {
                writer.Begin();
                writer.WriteHeader();
                writer.BeginObjects();
                foreach (IEncogPersistedObject obj in this.Contents.Values)
                {
                    writer.WriteObject(obj);
                }
                writer.EndObjects();
            }
            finally
            {
                writer.End();
                writer.Close();
            }
        }

        /// <inheritdoc/>
        public void Add(string name, IEncogPersistedObject obj)
        {
            this.Contents[name] = obj;
            BuildDirectory();
        }

        /// <inheritdoc/>
        public void BuildDirectory()
        {
            this.directory.Clear();
		    foreach ( IEncogPersistedObject obj in this.Contents.Values) 
            {
			    DirectoryEntry entry = new DirectoryEntry(obj);
			    this.directory.Add(entry);
		    }

        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.Contents.Clear();
            BuildDirectory();
        }

        /// <inheritdoc/>
        public void Delete(DirectoryEntry o)
        {
            this.Contents.Remove(o.Name);
            BuildDirectory();
        }

        /// <inheritdoc/>
        public void Delete(string key)
        {
            this.Contents.Remove(key);
            BuildDirectory();
        }

        /// <inheritdoc/>
        public bool Exists(string key)
        {
            return this.Contents.ContainsKey(key);
        }

        /// <inheritdoc/>
        public IEncogPersistedObject Find(DirectoryEntry entry)
        {
            return this.Contents[entry.Name];
        }

        /// <inheritdoc/>
        public IEncogPersistedObject Find(string key)
        {
            return this.Contents[key];
        }

        /// <inheritdoc/>
        public IList<DirectoryEntry> Directory
        {
            get
            {
                return this.directory;
            }
        }

        /// <inheritdoc/>
        public void UpdateProperties(string name, string newName, string desc)
        {
            IEncogPersistedObject obj = this.Contents[name];
		    obj.Name  = newName;
		    obj.Description = desc;
		    this.Contents.Remove(name);
		    this.Contents[newName] = obj;
		    BuildDirectory();
        }

        /// <inheritdoc/>
        public IPersistenceLocation Location
        {
            get 
            {
                return this.location ;
            }
        }
    }
}
