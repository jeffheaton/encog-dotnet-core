// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
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
using Encog.Neural.Data.XML;
using Encog.Parse.Tags.Read;
using Encog.Parse.Tags.Write;

namespace Encog.Persist.Persistors
{
    /// <summary>
    /// The Encog persistor used to persist the PropertyData class.
    /// </summary>
    public class PropertyDataPersistor : IPersistor
    {
        /// <summary>
        /// The properties tag.
        /// </summary>
        public const String TAG_PROPERTIES = "properties";

        /// <summary>
        /// The property tag.
        /// </summary>
        public const String TAG_PROPERTY = "Property";

        /// <summary>
        /// The name attribute.
        /// </summary>
        public const String ATTRIBUTE_NAME = "name";

        /// <summary>
        /// The value attribute.
        /// </summary>
        public const String ATTRIBUTE_VALUE = "value";

        /// <summary>
        /// The property data being loaed.
        /// </summary>
        private PropertyData propertyData;

        /// <summary>
        /// Handle the properties tag.
        /// </summary>
        /// <param name="xmlin">The XML reader.</param>
        private void HandleProperties(ReadXML xmlin)
        {
            while (xmlin.ReadToTag())
            {
                if (xmlin.IsIt(PropertyDataPersistor.TAG_PROPERTY, true))
                {
                    HandleProperty(xmlin);
                }
                else if (xmlin.IsIt(PropertyDataPersistor.TAG_PROPERTIES, false))
                {
                    break;
                }

            }

        }

        /// <summary>
        /// Handle loading an individual property.
        /// </summary>
        /// <param name="xmlin">The XML reader.</param>
        private void HandleProperty(ReadXML xmlin)
        {
            String name = xmlin.LastTag.GetAttributeValue(
                   PropertyDataPersistor.ATTRIBUTE_NAME);
            String value = xmlin.LastTag.GetAttributeValue(
                   PropertyDataPersistor.ATTRIBUTE_VALUE);
            this.propertyData[name] = value;
        }

        /// <summary>
        /// Load the specified Encog object from an XML reader.
        /// </summary>
        /// <param name="xmlin">The XML reader to use.</param>
        /// <returns>The loaded object.</returns>
        public IEncogPersistedObject Load(ReadXML xmlin)
        {
            String name = xmlin.LastTag.Attributes[
                   EncogPersistedCollection.ATTRIBUTE_NAME];
            String description = xmlin.LastTag.Attributes[
                   EncogPersistedCollection.ATTRIBUTE_DESCRIPTION];

            this.propertyData = new PropertyData();

            this.propertyData.Name = name;
            this.propertyData.Description = description;

            while (xmlin.ReadToTag())
            {
                if (xmlin.IsIt(PropertyDataPersistor.TAG_PROPERTIES, true))
                {
                    HandleProperties(xmlin);
                }
                else if (xmlin.IsIt(EncogPersistedCollection.TYPE_PROPERTY, false))
                {
                    break;
                }

            }

            return this.propertyData;
        }

        /// <summary>
        /// Save the specified Encog object to an XML writer.
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <param name="xmlout">The XML writer to save to.</param>
        public void Save(IEncogPersistedObject obj, WriteXML xmlout)
        {

            PropertyData pData = (PropertyData)obj;

            PersistorUtil.BeginEncogObject(EncogPersistedCollection.TYPE_PROPERTY,
                    xmlout, obj, true);
            xmlout.BeginTag(PropertyDataPersistor.TAG_PROPERTIES);
            foreach (String key in pData.Data.Keys)
            {
                xmlout.AddAttribute(PropertyDataPersistor.ATTRIBUTE_NAME, key);
                xmlout.AddAttribute(PropertyDataPersistor.ATTRIBUTE_VALUE, pData
                        [key]);
                xmlout.BeginTag(PropertyDataPersistor.TAG_PROPERTY);
                xmlout.EndTag();
            }
            xmlout.EndTag();
            xmlout.EndTag();

        }

    }

}
