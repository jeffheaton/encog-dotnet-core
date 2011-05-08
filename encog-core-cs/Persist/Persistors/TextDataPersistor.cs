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
using Encog.Parse.Tags.Write;
using Encog.Neural.Data;

namespace Encog.Persist.Persistors
{
    /// <summary>
    /// The Encog persistor used to persist the TextData class.
    /// </summary>
    public class TextDataPersistor : IPersistor
    {
        /// <summary>
        /// The text of this object.
        /// </summary>
        public String Text { get; set; }

        /// <summary>
        /// Load the specified Encog object from an XML reader.
        /// </summary>
        /// <param name="xmlin">The XML reader to use.</param>
        /// <returns>The loaded object.</returns>
        public IEncogPersistedObject Load(ReadXML xmlin)
        {
            String name = xmlin.LastTag.GetAttributeValue("name");
            String description = xmlin.LastTag.GetAttributeValue("description");
            TextData result = new TextData();
            xmlin.ReadToTag();
            String text = xmlin.LastTag.Name;
            result.Name = name;
            result.Description = description;
            result.Text = text;
            xmlin.ReadToTag();
            return result;
        }

        /// <summary>
        /// Save the specified Encog object to an XML writer.
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <param name="xmlout">The XML writer to save to.</param>
        public void Save(IEncogPersistedObject obj, WriteXML xmlout)
        {
            PersistorUtil.BeginEncogObject(EncogPersistedCollection.TYPE_TEXT, xmlout,
                    obj, true);
            TextData text = (TextData)obj;
            xmlout.AddCDATA(text.Text);
            xmlout.EndTag();
        }
    }

}
