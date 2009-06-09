using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Parse.Tags.Write;
using System.IO;
using log4net;
using Encog.Persist.Location;

namespace Encog.Persist
{
    /// <summary>
    /// Utility class for writing Encog persisted class files.
    /// </summary>
    public class PersistWriter
    {

        /// <summary>
        /// The XML writer.
        /// </summary>
        private WriteXML xmlOut;

        /// <summary>
        /// The output stream.
        /// </summary>
        private Stream fileOutput;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(PersistWriter));

        /// <summary>
        /// Create a writer for the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        public PersistWriter(PersistenceLocation location)
        {
            this.fileOutput = location.CreateStream();
            this.xmlOut = new WriteXML(this.fileOutput);
        }

        /// <summary>
        /// Begin an Encog document.
        /// </summary>
        public void Begin()
        {
            this.xmlOut.BeginDocument();
            this.xmlOut.BeginTag("Document");
        }

        /// <summary>
        /// Begin the objects collection.
        /// </summary>
        public void BeginObjects()
        {
            this.xmlOut.BeginTag("Objects");
        }

        /// <summary>
        /// Close the writer.
        /// </summary>
        public void Close()
        {
            this.xmlOut.Close();
        }

        /// <summary>
        /// End the document.
        /// </summary>
        public void End()
        {
            this.xmlOut.EndTag();
            this.xmlOut.EndDocument();
        }

        /// <summary>
        /// End the objects collection.
        /// </summary>
        public void EndObjects()
        {
            this.xmlOut.EndTag();
        }

        /// <summary>
        /// Merge the objects from this collection into the new one.
        /// Skip the specified object.
        /// </summary>
        /// <param name="location">The location to merge to.</param>
        /// <param name="skip">The object to skip.</param>
        public void MergeObjects(PersistenceLocation location,
                 String skip)
        {
            PersistReader reader = new PersistReader(location);
            reader.SaveTo(this.xmlOut, skip);
            reader.Close();
        }

        /// <summary>
        /// Modify the specified object, such as changing its name or
        /// description.
        /// </summary>
        /// <param name="location">The location of the object being modified.</param>
        /// <param name="name">The old name of the object being modified.</param>
        /// <param name="newName">The new name of the object being modified.</param>
        /// <param name="newDesc">The new description of the object being modified.</param>
        public void ModifyObject(PersistenceLocation location,
                 String name, String newName, String newDesc)
        {

            PersistReader reader = new PersistReader(location);
            reader.SaveModified(this.xmlOut, name, newName, newDesc);
            reader.Close();

        }

        /// <summary>
        /// Write the header for the Encog file.
        /// </summary>
        public void WriteHeader()
        {
            this.xmlOut.BeginTag("Header");
            this.xmlOut.AddProperty("platform", ".Net");
            this.xmlOut.AddProperty("fileVersion", Encog.Instance.Properties[Encog.ENCOG_FILE_VERSION]);
            this.xmlOut.AddProperty("encogVersion", Encog.Instance.Properties[Encog.ENCOG_VERSION]);
            this.xmlOut.AddProperty("modified", DateTime.Now.ToString());
            this.xmlOut.EndTag();
        }

        /// <summary>
        /// Write an object.
        /// </summary>
        /// <param name="obj">The object to write.</param>
        public void WriteObject(IEncogPersistedObject obj)
        {
            IPersistor persistor = obj.CreatePersistor();
            if (persistor == null)
            {
                String str = "Can't find a persistor for object of type "
                       + obj.GetType().Name;
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
                throw new PersistError(str);
            }
            persistor.Save(obj, this.xmlOut);
        }

    }

}
