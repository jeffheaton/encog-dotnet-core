using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Parse.Tags.Read;
using Encog.Parse.Tags.Write;
using System.Reflection;
using Encog.Util;

namespace Encog.Persist.Persistors.Generic
{
    /// <summary>
    /// An Encog perisistor that can be used with any object that supports the Encog
    /// generic persistence. Simply provide the class to the constructor, and return
    /// an instance of this object in the getPersistor call.
    /// 
    /// When loading an object, Encog will attempt to use this class if no other
    /// suitable persistor can be found.
    /// </summary>
    public class GenericPersistor : IPersistor
    {
        /// <summary>
        /// The class that this persistor is used with.
        /// </summary>
        private Type clazz;


        /// <summary>
        /// Construct a generic persistor for the specified class.
        /// </summary>
        /// <param name="clazz">The class to construct a persistor for.</param>
        public GenericPersistor(Type clazz)
        {
            this.clazz = clazz;
        }

        /// <summary>
        /// Load from the specified node.
        /// </summary>
        /// <param name="xmlIn">The node to load from.</param>
        /// <returns>The EncogPersistedObject that was loaded.</returns>
        public IEncogPersistedObject Load(ReadXML xmlIn)
        {
            IEncogPersistedObject current;

            String c = ReflectionUtil.ResolveEncogClass(clazz.Name);
            current = (IEncogPersistedObject)Assembly.GetExecutingAssembly().CreateInstance(c);
            XML2Object conv = new XML2Object();
            conv.Load(xmlIn, current);
            return current;
        }

        /// <summary>
        /// Save the specified object.
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <param name="xmlOut">The XML object.</param>
        public void Save(IEncogPersistedObject obj, WriteXML xmlOut)
        {
            Object2XML conv = new Object2XML();
            conv.Save(obj, xmlOut);
        }
    }
}
