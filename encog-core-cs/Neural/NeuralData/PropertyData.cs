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
using Encog.Persist;
using Encog.Persist.Persistors;

namespace Encog.Neural.Data
{
    /// <summary>
    /// An Encog data object that can be used to hold property data. This is a
    /// collection of name-value pairs that can be saved in an Encog persisted file.
    /// </summary>
    public class PropertyData : BasicPersistedObject
    {
        /// <summary>
        /// The property data.
        /// </summary>
        private IDictionary<String, String> data = new Dictionary<String, String>();

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clonned version of this object.</returns>
        public override object Clone()
        {
            PropertyData result = new PropertyData();
            result.Name = this.Name;
            result.Description = this.Description;

            foreach (String key in this.data.Keys)
            {
                result[key] = this[key];
            }
            return result;
        }

        /// <summary>
        /// A persistor for the property data.
        /// </summary>
        /// <returns>The persistor.</returns>
        public override IPersistor CreatePersistor()
        {
            return new PropertyDataPersistor();
        }

        /// <summary>
        /// Access one of the property items.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The item accessed.</returns>
        public String this[String key]
        {
            get
            {
                return this.data[key];
            }
            set
            {
                this.data[key] = value;
            }
        }

        /// <summary>
        /// Determine of the specified property is defined.
        /// </summary>
        /// <param name="key">The property to check.</param>
        /// <returns>True if this property is defined.</returns>
        public bool IsDefined(String key)
        {
            return this.data.ContainsKey(key);
        }

        /// <summary>
        /// Remove the specified property.
        /// </summary>
        /// <param name="key">The property to remove.</param>
        public void Remove(String key)
        {
            this.data.Remove(key);
        }



        /// <summary>
        /// The number of properties defined.
        /// </summary>
        public int Count
        {
            get
            {
                return this.data.Count;
            }
        }

        /// <summary>
        /// Get a property as an integer.
        /// </summary>
        /// <param name="field">The name of the field.</param>
        /// <returns>The integer value.</returns>
        public int GetInteger(String field)
        {
            String str = this[field];
            try
            {
                return int.Parse(str);
            }
            catch (Exception e)
            {
                throw new EncogError(e);
            }
        }

        /// <summary>
        /// Get a property as a double.
        /// </summary>
        /// <param name="field">The name of the field.</param>
        /// <returns>The double value.</returns>
        public double GetDouble(String field)
        {
            String str = this[field];

            return Double.Parse(str);

        }
        
        /// <summary>
        /// Get a property as a date.
        /// </summary>
        /// <param name="field">The name of the field.</param>
        /// <returns>The date value.</returns>
        public DateTime GetDate(String field)
        {
            String str = this[field];
            IFormatProvider formatProvider = System.Globalization.CultureInfo.CurrentCulture;
            DateTime result = DateTime.ParseExact(str, "MM/dd/yyyy", formatProvider);
            return result;
        }

        /// <summary>
        /// The data held by this object.
        /// </summary>
        public IDictionary<String, String> Data
        {
            get
            {
                return this.data;
            }
        }
    }
}
