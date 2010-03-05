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
using System.IO;
#if SILVERLIGHT
using Encog.Persist;
using Encog.Parse.Tags.Write;
using Encog.Parse.Tags.Read;
#else
using System.Runtime.Serialization.Formatters.Binary;
#endif

namespace Encog.Util
{
    /// <summary>
    /// A simple Object cloner that uses serialization. Actually works really well
    /// for the somewhat complex nature of BasicNetwork. Performs a deep copy without
    /// all the headache of programming a custom clone.
    /// 
    /// From a Java example at:
    /// 
    /// http://www.javaworld.com/javaworld/javatips/jw-javatip76.html?page=2
    /// 
    /// </summary>
    public class ObjectCloner
    {

        /// <summary>
        /// Private constructor.
        /// </summary>
        private ObjectCloner()
        {
        }

#if !SILVERLIGHT
        /// <summary>
        /// Perform a deep copy.
        /// </summary>
        /// <param name="oldObj">The old object.</param>
        /// <returns>The new object.</returns>
        static public Object DeepCopy(Object oldObj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream memory = null;

            try
            {
                memory = new MemoryStream(); 
                
                // serialize and pass the object
                formatter.Serialize(memory, oldObj);
                memory.Flush();
                memory.Position = 0;
                                
                // return the new object
                return formatter.Deserialize(memory);
            }
            catch (Exception e)
            {
                throw new EncogError(e);
            }
            finally
            {
                try
                {
                    memory.Close();
                }
                catch (Exception e)
                {
                    throw new EncogError(e);
                }
            }
        }
#else
        /// <summary>
        /// Perform a deep copy.
        /// Silverlight version.
        /// </summary>
        /// <param name="oldObj">The old object.</param>
        /// <returns>The new object.</returns>
        static public IEncogPersistedObject DeepCopy(IEncogPersistedObject oldObj)
        {
            bool replacedName = false;

            // encog objects won't save without a name
            if (oldObj.Name == null)
            {
                replacedName = true;
                oldObj.Name = "temp";
            }

            // now make the copy
            MemoryStream mstream = new MemoryStream();
            WriteXML xmlOut = new WriteXML(mstream);
            IPersistor persistor = oldObj.CreatePersistor();
            xmlOut.BeginDocument();
            persistor.Save(oldObj, xmlOut);
            xmlOut.EndDocument();
            // now read it back
            mstream.Position = 0;
            ReadXML xmlIn = new ReadXML(mstream);
            xmlIn.ReadToTag();
            IEncogPersistedObject result = persistor.Load(xmlIn);
            mstream.Close();

            // put the name back to null if we changed it
            if (replacedName)
            {
                oldObj.Name = null;
                result.Name = null;
            }
            return result;        }
#endif
    }
}
