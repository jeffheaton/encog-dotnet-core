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
using Encog.Neural.Networks.Training.Propagation;
using Encog.Util.CSV;

namespace Encog.Persist.Persistors
{
    /// <summary>
    /// Allow the TrainingContinuation class to persist.
    /// </summary>
    public class TrainingContinuationPersistor : IPersistor
    {
        /// <summary>
        /// The items tag.
        /// </summary>
        public const String TAG_ITEMS = "items";

        /// <summary>
        /// An item tag.
        /// </summary>
        public const String TAG_ITEM = "Item";

        /// <summary>
        /// The name attribute.
        /// </summary>
        public const String ATTRIBUTE_NAME = "name";

        /// <summary>
        /// The current training continuation object.
        /// </summary>
        private TrainingContinuation current;

        /// <summary>
        /// Handle an item.
        /// </summary>
        /// <param name="xmlin">The XML input object.</param>
        public void HandleItem(ReadXML xmlin)
        {
            String name = xmlin.LastTag.GetAttributeValue(
                   TrainingContinuationPersistor.ATTRIBUTE_NAME);
            String str = xmlin.ReadTextToTag();
            double[] list = NumberList.FromList(CSVFormat.EG_FORMAT, str);
            this.current[name] = list;

        }

        /// <summary>
        /// Handle loading the items.
        /// </summary>
        /// <param name="xmlin">The XML input object.</param>
        public void HandleItems(ReadXML xmlin)
        {
            while (xmlin.ReadToTag())
            {
                if (xmlin.IsIt(TrainingContinuationPersistor.TAG_ITEM, true))
                {
                    HandleItem(xmlin);
                }
                else if (xmlin.IsIt(TrainingContinuationPersistor.TAG_ITEM, false))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Load the object.
        /// </summary>
        /// <param name="xmlin">The XML object to load from.</param>
        /// <returns>The loaded object.</returns>
        public IEncogPersistedObject Load(ReadXML xmlin)
        {
            this.current = new TrainingContinuation();

            String name = xmlin.LastTag.Attributes[
                    EncogPersistedCollection.ATTRIBUTE_NAME];
            String description = xmlin.LastTag.Attributes[
                    EncogPersistedCollection.ATTRIBUTE_DESCRIPTION];

            this.current.Name = name;
            this.current.Description = description;

            while (xmlin.ReadToTag())
            {
                if (xmlin.IsIt(TrainingContinuationPersistor.TAG_ITEMS, true))
                {
                    HandleItems(xmlin);
                }
                else if (xmlin.IsIt(
                    EncogPersistedCollection.TYPE_TRAINING_CONTINUATION, false))
                {
                    break;
                }
            }

            return this.current;
        }

        /// <summary>
        /// Save the object.
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <param name="xmlout">The XML output object.</param>
        public void Save(IEncogPersistedObject obj, WriteXML xmlout)
        {

            PersistorUtil.BeginEncogObject(
                    EncogPersistedCollection.TYPE_TRAINING_CONTINUATION, xmlout, obj,
                    true);
            this.current = (TrainingContinuation)obj;

            // save the layers
            xmlout.BeginTag(BasicNetworkPersistor.TAG_LAYERS);
            SaveItems(xmlout);
            xmlout.EndTag();

            xmlout.EndTag();
        }

        /// <summary>
        /// Save items.
        /// </summary>
        /// <param name="xmlout">The XML output object.</param>
        public void SaveItems(WriteXML xmlout)
        {
            foreach (String key in this.current.Contents.Keys)
            {
                xmlout.AddAttribute(TrainingContinuationPersistor.ATTRIBUTE_NAME, key);
                xmlout.BeginTag(TrainingContinuationPersistor.TAG_ITEM);
                double[] value = (double[])this.current[key];
                StringBuilder result = new StringBuilder();
                NumberList.ToList(CSVFormat.EG_FORMAT, result, value);
                xmlout.AddText(result.ToString());
                xmlout.EndTag();
            }
        }

    }
}
