using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Reflection;
using Encog.Util;

namespace Encog.Neural.Persist
{
    /// <summary>
    /// An EncogPersistedCollection holds a collection of EncogPersistedObjects.
    /// This allows the various neural networks and some data sets to be peristed.
    /// They are persisted to an XML form.
    /// </summary>
    public class EncogPersistedCollection
    {
        /// <summary>
        /// Create a persistor object.  These objects know how to persist
        /// certain types of classes.
        /// </summary>
        /// <param name="className">The name of the class to create a persistor for.</param>
        /// <returns>The persistor for the specified class.</returns>
        public static IPersistor CreatePersistor(String className)
        {
            String name = "Encog.Neural.Persist.Persistors." + className + "Persistor";
            IPersistor persistor = (IPersistor)Assembly.GetExecutingAssembly().CreateInstance(name);
            return persistor;
        }

        /// <summary>
        /// The object to be persisted.
        /// </summary>
        private IList<IEncogPersistedObject> list =
            new List<IEncogPersistedObject>();

        /// <summary>
        /// The platform this collection was created on.
        /// </summary>
        private String platform;

        /// <summary>
        /// The version of the persisted file.
        /// </summary>
        private int fileVersion;

        /// <summary>
        /// The version of Encog.
        /// </summary>
        private String encogVersion;

        /// <summary>
        /// Add an EncogPersistedObject to the collection.
        /// </summary>
        /// <param name="obj">The object to add.</param>
        public void Add(IEncogPersistedObject obj)
        {
            this.list.Add(obj);
        }

        /// <summary>
        /// Clear the collection.
        /// </summary>
        public void Clear()
        {
            this.list.Clear();
        }

        public String EncogVersion
        {
            get
            {
                return this.encogVersion;
            }
        }

        public int FileVersion
        {
            get
            {
                return this.fileVersion;
            }
        }

        public IList<IEncogPersistedObject> List
        {
            get
            {
                return this.list;
            }
        }

        public String Platform
        {
            get
            {
                return this.platform;
            }
        }

        /// <summary>
        /// Load from an input stream.
        /// </summary>
        /// <param name="stream">The stream to load from.</param>
        public void Load(Stream stream)
        {

            // read in data
            XmlDocument doc = new XmlDocument();
            doc.Load(stream);
            XmlElement memory = doc.DocumentElement;

            for (XmlNode child = memory.FirstChild; child != null; child = child.NextSibling)
            {
                if (!(child is XmlElement))
                {
                    continue;
                }
                XmlElement node = (XmlElement)child;

                if (node.Name.Equals("Header"))
                {
                    LoadHeader(node);
                }
                else if (node.Name.Equals("objects"))
                {
                    LoadObjects(node);
                }
            }
        }

        /// <summary>
        /// Load from a file.
        /// </summary>
        /// <param name="filename">The filename to load from.</param>
        public void Load(String filename)
        {

            Stream istream = File.OpenRead(filename);
            Load(istream);
            istream.Close();

        }

        /// <summary>
        /// Load the XML header.
        /// </summary>
        /// <param name="node">The node to load from.</param>
        private void LoadHeader(XmlElement node)
        {
            this.platform = XMLUtil.FindElementAsString(node, "platform");
            this.encogVersion = XMLUtil.FindElementAsString(node,
                    "encogVersion");
            this.fileVersion = XMLUtil
                    .FindElementAsInt(node, "fileVersion", -1);
        }

        /// <summary>
        /// Load the objects list.
        /// </summary>
        /// <param name="objects">The node to load from.</param>
        private void LoadObjects(XmlElement objects)
        {
            for (XmlNode child = objects.FirstChild; child != null; child = child.NextSibling)
            {
                if (!(child is XmlElement))
                {
                    continue;
                }
                XmlElement node = (XmlElement)child;

                IPersistor persistor = EncogPersistedCollection.CreatePersistor(node.Name);
                if (persistor != null)
                {
                    IEncogPersistedObject obj = persistor.Load(node);
                    this.list.Add(obj);
                }
            }
        }

        /// <summary>
        /// Save to an output stream.
        /// </summary>
        /// <param name="os">The stream to save to.</param>
        public void Save(Stream os)
        {
            XmlTextWriter hd = new XmlTextWriter(os, Encoding.ASCII);

            hd.WriteStartDocument();
            hd.WriteStartElement("Document");

            SaveHeader(hd);
            SaveObjects(hd);

            hd.WriteEndElement();
            hd.WriteEndDocument();

            hd.Close();
        }

        /// <summary>
        /// Save to a file.
        /// </summary>
        /// <param name="filename">The filename to save to.</param>
        public void Save(String filename)
        {
            Stream os = File.OpenWrite(filename);
            Save(os);
            os.Close();
        }

        /// <summary>
        /// Save the XML header.
        /// </summary>
        /// <param name="hd">The object to save to.</param>
        private void SaveHeader(XmlTextWriter hd)
        {
            hd.WriteStartElement("Header");

            // platform
            hd.WriteStartElement("platform");
            hd.WriteString("DotNet");
            hd.WriteEndElement();

            hd.WriteStartElement("fileVersion");
            hd.WriteString("0");
            hd.WriteEndElement();

            hd.WriteStartElement("encogVersion");
            hd.WriteString("1.1.0");
            hd.WriteEndElement();

            hd.WriteEndElement();
        }

        /// <summary>
        /// Save the list of objects.
        /// </summary>
        /// <param name="hd">The persistance object.</param>
        private void SaveObjects(XmlTextWriter hd)
        {
            hd.WriteStartElement("objects");

            foreach (IEncogPersistedObject obj in this.list)
            {
                String name = obj.GetType().Name;
                IPersistor persistor = EncogPersistedCollection.CreatePersistor(name);
                persistor.Save(obj, hd);

            }

            hd.WriteEndElement();

        }

        public static void AddAttribute(XmlTextWriter hd, String name, String value)
        {
            String v = value;
            if (v == null)
                v = "";
            hd.WriteAttributeString(name, value);
        }

        public static void CreateAttributes(XmlTextWriter hd, IEncogPersistedObject obj)
        {
            EncogPersistedCollection.AddAttribute(hd, "native", "" + obj.GetType().Name);
            EncogPersistedCollection.AddAttribute(hd, "name", obj.Name);
            EncogPersistedCollection.AddAttribute(hd, "description", obj.Description);
        }

        /// <summary>
        /// Called to search all Encog objects in this collection for one with a name
        /// that passes what was passed in.
        /// </summary>
        /// <param name="name">The name we are searching for.</param>
        /// <returns>The Encog object with the correct name.</returns>
        public IEncogPersistedObject Find(String name)
        {
            foreach (IEncogPersistedObject obj in this.list)
            {
                if (name.Equals(obj.Name))
                {
                    return obj;
                }
            }
            return null;
        }
    }
}
