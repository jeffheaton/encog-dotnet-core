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
using Encog.Parse.Tags.Read;
using System.IO;
using Encog.Persist.Location;
using Encog.Parse.Tags.Write;
using Encog.Parse.Tags;
using Encog.Persist.Persistors;
using Encog.MathUtil;
using System.Reflection;

#if logging
using log4net;
using Encog.Util;
#endif

namespace Encog.Persist
{
    /// <summary>
    /// Utility class for reading Encog persited object files.
    /// </summary>
    public class PersistReader
    {

        /// <summary>
        /// The name attribute.
        /// </summary>
        public const String ATTRIBUTE_NAME = "name";

        /// <summary>
        /// The objects tag.
        /// </summary>
        public const String TAG_OBJECTS = "Objects";

        /// <summary>
        /// The XML reader.
        /// </summary>
        private ReadXML xmlIn;

        /// <summary>
        /// The input stream.
        /// </summary>
        private Stream fileInput;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(PersistReader));
#endif

        /// <summary>
        /// Construct a persist reader.
        /// </summary>
        /// <param name="location">The location to use.</param>
        public PersistReader(IPersistenceLocation location)
        {
            this.fileInput = location.CreateStream(FileMode.Open);
            this.xmlIn = new ReadXML(this.fileInput);
        }

        /// <summary>
        /// Advance to the specified object.
        /// </summary>
        /// <param name="name">The name of the object looking for.</param>
        /// <returns>The beginning element of the object found.</returns>
        public bool Advance(String name)
        {
            AdvanceObjectsCollection();
            return AdvanceObjects(name);
        }

        /// <summary>
        /// Advance to the specified tag.
        /// </summary>
        /// <param name="tag">The tag to advance to.</param>
        /// <returns>True if the tag was found.</returns>
        private bool AdvanceToTag(String tag)
        {
            while (this.xmlIn.ReadToTag())
            {
                Tag.Type type = this.xmlIn.LastTag.TagType;
                if (type == Tag.Type.BEGIN)
                {
                    if (this.xmlIn.LastTag.Name.Equals(tag))
                    {
                        return true;
                    }
                    else
                    {
                        SkipObject();
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Once you are in the objects collection, advance to a specific object.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The beginning tag of that object if its found, null otherwise.</returns>
        private bool AdvanceObjects(String name)
        {
            while (this.xmlIn.ReadToTag())
            {
                Tag.Type type = this.xmlIn.LastTag.TagType;
                if (type == Tag.Type.BEGIN)
                {
                    String elementName = this.xmlIn.LastTag.GetAttributeValue(
                           "name");

                    if ((elementName != null) && elementName.Equals(name))
                    {
                        return true;
                    }
                    else
                    {
                        SkipObject();
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Advance to the objects collection.
        /// </summary>
        public void AdvanceObjectsCollection()
        {
            while (this.xmlIn.ReadToTag())
            {
                Tag.Type type = this.xmlIn.LastTag.TagType;
                if ((type == Tag.Type.BEGIN)
                        && this.xmlIn.LastTag.Name.Equals(
                                PersistReader.TAG_OBJECTS))
                {
                    return;
                }
            }

            String str = "Can't find objects collection, invalid file.";
#if logging
            if (this.logger.IsErrorEnabled)
            {
                this.logger.Error(str);
            }
#endif
            throw new PersistError(str);

        }

        /// <summary>
        /// Build a directory entry list for the file.
        /// </summary>
        /// <returns>A list of objects in the file.</returns>
        public IList<DirectoryEntry> BuildDirectory()
        {
            IList<DirectoryEntry> result = new List<DirectoryEntry>();
            AdvanceObjectsCollection();

            while (this.xmlIn.ReadToTag())
            {
                if (this.xmlIn.IsIt(PersistReader.TAG_OBJECTS, false))
                {
                    break;
                }

                String type = this.xmlIn.LastTag.Name;
                String name = this.xmlIn.LastTag.GetAttributeValue("name");
                String description = this.xmlIn.LastTag.GetAttributeValue(
                       "description");

                DirectoryEntry entry = new DirectoryEntry(type, name,
                       description);
                if (!result.Contains(entry))
                {
                    result.Add(entry);
                }

                SkipObject();
            }

            return result;
        }

        /// <summary>
        /// Close the file.
        /// </summary>
        public void Close()
        {
            try
            {
                this.fileInput.Close();
            }
            catch (IOException e)
            {
                throw new PersistError(e);
            }

        }

        /// <summary>
        /// Copy all of the attributes to the writer.
        /// </summary>
        /// <param name="xmlOut">The XML writer.</param>
        /// <param name="replace">A map of attributes to replace.  This allows
        /// new values to be specified for select attributes.</param>
        private void CopyAttributes(WriteXML xmlOut,
                 IDictionary<String, String> replace)
        {
            foreach (String key in this.xmlIn.LastTag.Attributes.Keys)
            {
                String value = this.xmlIn.LastTag.GetAttributeValue(key);
                if ((replace != null) && replace.ContainsKey(key))
                {
                    value = replace[key];
                }
                xmlOut.AddAttribute(key, value);
            }
        }

        /// <summary>
        /// Copy an XML object, no need to know what it contains, just
        /// copy it.  This way we will not damage unknown objects during
        /// a merge.
        /// </summary>
        /// <param name="xmlOut">The XML writer.</param>
        /// <param name="replace"></param>
        private void CopyXML(WriteXML xmlOut,
                 IDictionary<String, String> replace)
        {
            StringBuilder text = new StringBuilder();
            int depth = 0;
            int ch;
            CopyAttributes(xmlOut, replace);
            String contain = this.xmlIn.LastTag.Name;

            xmlOut.BeginTag(contain);

            while ((ch = this.xmlIn.Read()) != -1)
            {
                Tag.Type type = this.xmlIn.LastTag.TagType;

                if (ch == 0)
                {
                    if (type == Tag.Type.BEGIN)
                    {
                        if (text.Length > 0)
                        {
                            xmlOut.AddText(text.ToString());
                            text.Length = 0;
                        }

                        CopyAttributes(xmlOut, null);
                        xmlOut.BeginTag(this.xmlIn.LastTag.Name);
                        depth++;
                    }
                    else if (type == Tag.Type.END)
                    {
                        if (text.Length > 0)
                        {
                            xmlOut.AddText(text.ToString());
                            text.Length = 0;
                        }

                        if (depth == 0)
                        {
                            break;
                        }
                        else
                        {
                            xmlOut.EndTag(xmlIn.LastTag.Name);
                        }
                        depth--;
                    }
                }
                else
                {
                    text.Append((char)ch);
                }

            }

            xmlOut.EndTag(contain);
        }

        /// <summary>
        /// Read until the next tag of the specified name.
        /// </summary>
        /// <param name="name">The name searched for.</param>
        /// <returns>True if the tag was found.</returns>
        public bool ReadNextTag(String name)
        {

            while (this.xmlIn.ReadToTag())
            {
                Tag.Type type = this.xmlIn.LastTag.TagType;
                if (type == Tag.Type.BEGIN)
                {
                    if (this.xmlIn.LastTag.Name.Equals(name))
                    {
                        return true;
                    }
                    else
                    {
                        SkipObject();
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Read all text until the specified ending tag is found.
        /// </summary>
        /// <param name="name">The tag.</param>
        /// <returns>The text found.</returns>
        public String ReadNextText(String name)
        {
            StringBuilder result = new StringBuilder();
            return result.ToString();
        }

        /// <summary>
        /// Read the specific object, search through the objects until its found.
        /// </summary>
        /// <param name="name">The name of the object you are looking for.</param>
        /// <returns>The object found, null if not found.</returns>
        public IEncogPersistedObject ReadObject(String name)
        {

            // did we find the object?
            if (Advance(name))
            {
                String objectType = this.xmlIn.LastTag.Name;
                IPersistor persistor = PersistorUtil
                       .CreatePersistor(objectType);

                if (persistor == null)
                {
                    throw new PersistError(
                            "Do not know how to load: " + objectType);
                }
                return persistor.Load(this.xmlIn);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Read the value in a period delimited string. For example
        /// property.name.value.
        /// </summary>
        /// <param name="name">The property to read.</param>
        /// <returns>The value found at the specified property.</returns>
        public String ReadValue(String name)
        {

            String[] tok = name.Split('.');
            for (int i = 0; i < tok.Length; i++)
            {
                String subName = tok[i];
                if (!ReadNextTag(subName))
                {
                    return null;
                }
            }

            return ReadNextText(this.xmlIn.LastTag.Name);
        }

        /// <summary>
        /// Modify the properties of this object.
        /// </summary>
        /// <param name="xmlOut">The XML writer.</param>
        /// <param name="targetName">The name of the object to change.</param>
        /// <param name="newName">The new name of this object.</param>
        /// <param name="newDesc">The new description of this object.</param>
        public void SaveModified(WriteXML xmlOut, String targetName,
                 String newName, String newDesc)
        {
            AdvanceObjectsCollection();

            while (this.xmlIn.ReadToTag())
            {
                Tag.Type type = this.xmlIn.LastTag.TagType;
                if (type == Tag.Type.BEGIN)
                {
                    String name = this.xmlIn.LastTag.GetAttributeValue(
                           PersistReader.ATTRIBUTE_NAME);
                    if (name.Equals(targetName))
                    {
                        IDictionary<String, String> replace =
                           new Dictionary<String, String>();
                        replace.Add("name", newName);
                        replace.Add("description", newDesc);
                        CopyXML(xmlOut, replace);
                    }
                    else
                    {
                        CopyXML(xmlOut, null);
                    }
                }
            }
        }

        /// <summary>
        /// Save all objects to the specified steam, skip the one specified by the
        /// skip parameter. Do not attempt to understand the structure, just copy.
        /// </summary>
        /// <param name="xmlOut">The XML writer to save the objects to.</param>
        /// <param name="skip">The object to skip.</param>
        public void SaveTo(WriteXML xmlOut, String skip)
        {
            AdvanceObjectsCollection();

            while (this.xmlIn.ReadToTag())
            {
                Tag.Type type = this.xmlIn.LastTag.TagType;
                if (type == Tag.Type.BEGIN)
                {
                    String name = this.xmlIn.LastTag.GetAttributeValue(
                           PersistReader.ATTRIBUTE_NAME);
                    if (name.Equals(skip))
                    {
                        SkipObject();
                    }
                    else
                    {
                        CopyXML(xmlOut, null);
                    }
                }
            }
        }

        /// <summary>
        /// Skip the current object.
        /// </summary>
        private void SkipObject()
        {
            int depth = 0;
            while (this.xmlIn.ReadToTag())
            {
                Tag.Type type = this.xmlIn.LastTag.TagType;

                switch (type)
                {
                    case Tag.Type.END:
                        if (depth == 0)
                            return;
                        depth--;
                        break;
                    case Tag.Type.BEGIN:
                        depth++;
                        break;
                }
            }
        }

        /// <summary>
        /// Read an Encog header.
        /// </summary>
        /// <returns>The Encog header.</returns>
        public IDictionary<string, string> ReadHeader()
        {
            IDictionary<String, String> headers = null;
            if (AdvanceToTag("Document"))
            {
                if (AdvanceToTag("Header"))
                {
                    headers = this.xmlIn.ReadPropertyBlock();
                }
            }
            return headers;
        }

        /// <summary>
        /// The ReadXML object used to access XML.
        /// </summary>
        public ReadXML XMLInput
        {
            get
            {
                return this.xmlIn;
            }
        }
    }

}
