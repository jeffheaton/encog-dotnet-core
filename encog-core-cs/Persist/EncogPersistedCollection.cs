using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Encog.Persist.Location;
using System.IO;

namespace Encog.Persist
{
    
    /// <summary>
    /// An EncogPersistedCollection holds a collection of EncogPersistedObjects. This
 /// allows the various neural networks and some data sets to be persisted. They
 /// are persisted to an XML form.
 /// 
 /// The EncogPersistedCollection does not load the object into memory at once.
 /// This allows it to manage large files.
    /// </summary>
    public class EncogPersistedCollection
    {

        /// <summary>
        /// Generic error message for bad XML.
        /// </summary>
        public const String GENERAL_ERROR = "Malformed XML near tag: ";

        /// <summary>
        /// The type is TextData.
        /// </summary>
        public const String TYPE_TEXT = "TextData";

        /// <summary>
        /// The type is PropertyData.
        /// </summary>
        public const String TYPE_PROPERTY = "PropertyData";

        /// <summary>
        /// The type is BasicNetwork.
        /// </summary>
        public const String TYPE_BASIC_NET = "BasicNetwork";

        /// <summary>
        /// The type is BasicLayer.
        /// </summary>
        public const String TYPE_BASIC_LAYER = "BasicLayer";

        /// <summary>
        /// The type is ContextLayer.
        /// </summary>
        public const String TYPE_CONTEXT_LAYER = "ContextLayer";

        /// <summary>
        /// The type is RadialBasisFunctionLayer.
        /// </summary>
        public const String TYPE_RADIAL_BASIS_LAYER =
            "RadialBasisFunctionLayer";

        /// <summary>
        /// The type is TrainingData.
        /// </summary>
        public const String TYPE_TRAINING = "TrainingData";

        /// <summary>
        ///  The type is WeightedSynapse.
        /// </summary>
        public const String TYPE_WEIGHTED_SYNAPSE = "WeightedSynapse";

        /// <summary>
        /// The type is WeightlessSynapse.
        /// </summary>
        public const String TYPE_WEIGHTLESS_SYNAPSE = "WeightlessSynapse";

        /// <summary>
        /// The type is DirectSynapse.
        /// </summary>
        public const String TYPE_DIRECT_SYNAPSE = "DirectSynapse";

        /// <summary>
        /// The type is OneToOneSynapse.
        /// </summary>
        public const String TYPE_ONE2ONE_SYNAPSE = "OneToOneSynapse";

        /// <summary>
        /// The type is ParseTemplate.
        /// </summary>
        public const String TYPE_PARSE_TEMPLATE = "ParseTemplate";

        /// <summary>
        /// The name attribute.
        /// </summary>
        public const String ATTRIBUTE_NAME = "name";

        /// <summary>
        /// The description attribute.
        /// </summary>
        public const String ATTRIBUTE_DESCRIPTION = "description";

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly static ILog LOGGER = LogManager.GetLogger(typeof(EncogPersistedCollection));

        /// <summary>
        /// Throw and log an error.
        /// </summary>
        /// <param name="tag">The tag this error is for.</param>
        public static void ThrowError(String tag)
        {
            String str = EncogPersistedCollection.GENERAL_ERROR + tag;
            if (EncogPersistedCollection.LOGGER.IsErrorEnabled)
            {
                EncogPersistedCollection.LOGGER.Error(str);
            }
            throw new PersistError(str);
        }

        /// <summary>
        /// The primary file being persisted to.
        /// </summary>
        private PersistenceLocation filePrimary;

        /// <summary>
        /// The temp file, to be used for merges.
        /// </summary>
        private PersistenceLocation fileTemp;

        /// <summary>
        /// The platform this collection was created on.
        /// </summary>
        private String platform;

        /// <summary>
        /// The version of the persisted file.
        /// </summary>
        private int fileVersion;

        /// <summary>
        /// Directory entries for all of the objects in the current file.
        /// </summary>
        private IDictionary<DirectoryEntry, Object> directory =
                new Dictionary<DirectoryEntry, Object>();

        /// <summary>
        /// The version of Encog.
        /// </summary>
        private String encogVersion;

        /// <summary>
        /// Create a persistance collection for the specified file.
        /// </summary>
        /// <param name="file">The file to load/save.</param>
        /// <param name="mode">The file mode</param>
        public EncogPersistedCollection(String file, FileMode mode)
            : this(new FilePersistence(file, mode), mode)
        {

        }

        /// <summary>
        /// Create an object based on the specified location.
        /// </summary>
        /// <param name="location">The location to load/save from.</param>
        /// <param name="mode">The file mode.</param>
        public EncogPersistedCollection(PersistenceLocation location, FileMode mode)
        {
            this.filePrimary = location;

            if (this.filePrimary is FilePersistence)
            {
                String file = ((FilePersistence)this.filePrimary).FileName;

                int index = file.LastIndexOf('.');
                if (index != -1)
                {
                    file = file.Substring(0, index);
                }
                file += ".tmp";
                this.fileTemp = new FilePersistence(file, mode);

                if (this.filePrimary.Exists())
                {
                    BuildDirectory();
                }
                else
                {
                    Create();
                }
            }
            else
            {
                this.fileTemp = null;
            }
        }

        /// <summary>
        /// Add an EncogPersistedObject to the collection.
        /// </summary>
        /// <param name="name">The name of the object to load.</param>
        /// <param name="obj">The object to add.</param>
        public void Add(String name, IEncogPersistedObject obj)
        {
            obj.Name = name;
            PersistWriter writer = new PersistWriter(this.fileTemp);
            writer.Begin();
            writer.WriteHeader();
            writer.BeginObjects();
            writer.WriteObject(obj);
            writer.MergeObjects(this.filePrimary, name);
            writer.EndObjects();
            writer.End();
            writer.Close();
            MergeTemp();
            BuildDirectory();
        }

        /// <summary>
        /// Build a directory of objects.
        /// </summary>
        public void BuildDirectory()
        {
            PersistReader reader = new PersistReader(this.filePrimary);
            IDictionary<DirectoryEntry, Object> d = reader.BuildDirectory();
            this.directory.Clear();
            foreach (DirectoryEntry de in d.Keys)
            {
                this.directory.Add(de, null);
            }


            reader.Close();
        }

        /// <summary>
        /// Clear the collection.
        /// </summary>
        public void Clear()
        {

        }

        /// <summary>
        /// Create the file.
        /// </summary>
        public void Create()
        {
            PersistWriter writer = new PersistWriter(this.filePrimary);
            writer.Begin();
            writer.WriteHeader();
            writer.BeginObjects();
            writer.EndObjects();
            writer.End();
            writer.Close();

            this.directory.Clear();
        }

       /// <summary>
        /// Delete the specified object, use a directory entry.
       /// </summary>
        /// <param name="d">The object to delete.</param>
        public void Delete(DirectoryEntry d)
        {
            this.Delete(d.Name);

        }

        /// <summary>
        /// Delete the specified object.
        /// </summary>
        /// <param name="obj">The object to delete.</param>
        public void Delete(IEncogPersistedObject obj)
        {
            Delete(obj.Name);
        }

        /// <summary>
        /// Delete the specified object.
        /// </summary>
        /// <param name="name">The object name.</param>
        public void Delete(String name)
        {
            PersistWriter writer = new PersistWriter(this.fileTemp);
            writer.Begin();
            writer.WriteHeader();
            writer.BeginObjects();
            writer.MergeObjects(this.filePrimary, name);
            writer.EndObjects();
            writer.End();
            writer.Close();
            MergeTemp();
            foreach (DirectoryEntry d in this.directory.Keys)
            {
                if (d.Name.Equals(name))
                {
                    this.directory.Remove(d);
                    break;
                }
            }
        }

        /// <summary>
        /// Find the specified object, using a DirectoryEntry.
        /// </summary>
        /// <param name="d">The directory entry to find.</param>
        /// <returns>The loaded object.</returns>
        public IEncogPersistedObject Find(DirectoryEntry d)
        {
            return Find(d.Name);
        }

        /// <summary>
        /// Called to search all Encog objects in this collection for one with a name
        /// that passes what was passed in.
        /// </summary>
        /// <param name="name">The name we are searching for.</param>
        /// <returns>The Encog object with the correct name.</returns>
        public IEncogPersistedObject Find(String name)
        {

            PersistReader reader = new PersistReader(this.filePrimary);
            IEncogPersistedObject result = reader.ReadObject(name);
            reader.Close();
            return result;
        }

        /// <summary>
        /// The directory entries for the objects in this file.
        /// </summary>
        public IDictionary<DirectoryEntry, Object> Directory
        {
            get
            {
                return this.directory;
            }
        }

        /// <summary>
        /// The version of Encog this file was created with.
        /// </summary>
        public String EncogVersion
        {
            get
            {
                return this.encogVersion;
            }
        }

        /// <summary>
        /// The file version.
        /// </summary>
        public int FileVersion
        {
            get
            {
                return this.fileVersion;
            }
        }

        /// <summary>
        /// The platform this file was created on.
        /// </summary>
        public String Platform
        {
            get
            {
                return this.platform;
            }
        }

        /// <summary>
        /// Merge the temp file with the main one, call this to make any
        /// changes permanent.
        /// </summary>
        public void MergeTemp()
        {
            this.filePrimary.Delete();
            this.fileTemp.RenameTo(this.filePrimary);
        }

        /// <summary>
        /// Update any header properties for an Encog object, for example,
        /// a rename.
        /// </summary>
        /// <param name="name">The name of the object to change. </param>
        /// <param name="newName">The new name of this object.</param>
        /// <param name="newDesc">The description for this object.</param>
        public void UpdateProperties(String name, String newName,
                 String newDesc)
        {
            PersistWriter writer = new PersistWriter(this.fileTemp);
            writer.Begin();
            writer.WriteHeader();
            writer.BeginObjects();
            writer.ModifyObject(this.filePrimary, name, newName, newDesc);
            writer.EndObjects();
            writer.End();
            writer.Close();
            MergeTemp();
            BuildDirectory();

        }

    }
}
