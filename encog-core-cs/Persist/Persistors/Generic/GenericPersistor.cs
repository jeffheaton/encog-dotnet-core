// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009-2010, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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
