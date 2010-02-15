// Encog(tm) Artificial Intelligence Framework v2.3
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
using Encog.Parse.Tags.Write;
using System.IO;
using Encog.Persist.Location;

#if logging
using log4net;
#endif


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

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(PersistWriter));
#endif

        /// <summary>
        /// Create a writer for the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        public PersistWriter(IPersistenceLocation location)
        {
            this.fileOutput = location.CreateStream(FileMode.OpenOrCreate);
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
        public void MergeObjects(IPersistenceLocation location,
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
        public void ModifyObject(IPersistenceLocation location,
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
#if logging
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
#endif
                throw new PersistError(str);
            }
            persistor.Save(obj, this.xmlOut);
        }

    }

}
