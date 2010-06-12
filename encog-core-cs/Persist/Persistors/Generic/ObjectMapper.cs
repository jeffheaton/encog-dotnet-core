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
using System.Reflection;

namespace Encog.Persist.Persistors.Generic
{
    /// <summary>
    /// Used to map objects to reference numbers. This is where reference numbers are
    /// resolved. This class is used by Encog generic persistence.
    /// </summary>
    public class ObjectMapper
    {
        /// <summary>
        /// A map from reference numbers to objects.
        /// </summary>
        private IDictionary<int, Object> objectMap = new Dictionary<int, Object>();

        /// <summary>
        /// A list of all of the field mappings.
        /// </summary>
        private IList<FieldMapping> list = new List<FieldMapping>();

        /// <summary>
        /// Add a field mapping to be resolved later. This builds a list of
        /// references to be resolved later when the resolve method is called.
        /// </summary>
        /// <param name="reff">The reference number.</param>
        /// <param name="field">The field to map.</param>
        /// <param name="target">The target object that holds the field.</param>
        public void AddFieldMapping(int reff, FieldInfo field,
                Object target)
        {
            this.list.Add(new FieldMapping(reff, field, target));
        }

        /// <summary>
        /// Add an object mapping to be resolved later.
        /// </summary>
        /// <param name="reff">The object reference.</param>
        /// <param name="obj">The object.</param>
        public void AddObjectMapping(int reff, Object obj)
        {
            this.objectMap[reff] = obj;
        }

        /// <summary>
        /// Clear the map and reference list.
        /// </summary>
        public void Clear()
        {
            this.objectMap.Clear();
            this.list.Clear();
        }

        /// <summary>
        /// Resolve all references and place the correct objects.
        /// </summary>
        public void Resolve()
        {

            foreach (FieldMapping field in this.list)
            {
                Object obj = this.objectMap[field.Reff];
                field.Field.SetValue(field.Target, obj);
            }
        }
    }
}
