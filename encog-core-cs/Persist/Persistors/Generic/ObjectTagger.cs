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
using System.Collections;
using Encog.Util;
using System.Reflection;
using Encog.Persist.Attributes;

namespace Encog.Persist.Persistors.Generic
{
    /// <summary>
    /// The object tagger is used in generic persistence to tag objects with a
    /// reference number.
    /// </summary>
    public class ObjectTagger
    {
        /// <summary>
        /// The map of object to reference number.
        /// </summary>
        private IDictionary<Object, int> map = new Dictionary<Object, int>();

        /// <summary>
        /// The current reference ID.
        /// </summary>
        private int currentID = 1;

        /// <summary>
        /// The current depth.
        /// </summary>
        private int depth;

        /// <summary>
        /// Analyze the specified object and build a reference map.
        /// </summary>
        /// <param name="encogObject">The object to analyze.</param>
        public void Analyze(IEncogPersistedObject encogObject)
        {

            this.depth = 0;
            AssignObjectTag(encogObject);
            foreach (FieldInfo childField in ReflectionUtil
                    .GetAllFields(encogObject.GetType()))
            {
                if (ReflectionUtil.ShouldAccessField(childField, true))
                {
                    Object childValue = childField.GetValue(encogObject);
                    TagField(childValue);
                }
            }

        }

        /// <summary>
        /// Assign a reference number to the specified object.
        /// </summary>
        /// <param name="obj">The object to "tag".</param>
        private void AssignObjectTag(Object obj)
        {
            if ( ReflectionUtil.HasAttribute(obj.GetType(), typeof(EGReferenceable)))
            {
                this.map[obj] = this.currentID;
                this.currentID++;
            }
        }

        /// <summary>
        /// Clear the map and current id.
        /// </summary>
        public void Clear()
        {
            this.map.Clear();
            this.currentID = 1;
        }

        /// <summary>
        /// Get the reference for the specified object.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>-1 for no reference, otherwise the reference numebr.</returns>
        public int GetReference(Object obj)
        {
            if (obj == null)
            {
                return -1;
            }

            if (!this.map.ContainsKey(obj))
            {
                return -1;
            }

            return this.map[obj];
        }

        /// <summary>
        /// Returns true if the object has a reference.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the object has a reference.</returns>
        public bool HasReference(Object obj)
        {
            return this.map.ContainsKey(obj);
        }

        /// <summary>
        /// Tag a collection, every object in the collection will be a reference.
        /// </summary>
        /// <param name="value">The collection to tag.</param>
        private void TagCollection(ICollection value)
        {

            foreach (Object obj in value)
            {
                TagObject(obj);
            }
        }

        /// <summary>
        /// Tag a field.
        /// </summary>
        /// <param name="fieldObject">The field to tag.</param>
        private void TagField(Object fieldObject)
        {
            this.depth++;

            if (this.map.ContainsKey(fieldObject))
            {
                return;
            }
            if (fieldObject != null)
            {
                if (fieldObject is ICollection)
                {
                    TagCollection((ICollection)fieldObject);
                }
                else
                {
                    TagObject(fieldObject);
                }
            }
            this.depth--;
        }

        /// <summary>
        /// Tag an object.
        /// </summary>
        /// <param name="parentObject">The object to tag.</param>
        private void TagObject(Object parentObject)
        {

            ICollection<FieldInfo> allFields = ReflectionUtil
                   .GetAllFields(parentObject.GetType());

            AssignObjectTag(parentObject);

            // handle actual fields
            foreach (FieldInfo childField in allFields)
            {
                if (ReflectionUtil.ShouldAccessField(childField, false))
                {

                    Object childValue = childField.GetValue(parentObject);

                    if (!ReflectionUtil.IsSimple(childField.FieldType))
                    {
                        if (this.depth > 50)
                        {
                            throw new PersistError(
                                    "Encog persistence is greater than 50 levels deep, closed loop likely.  Consider adding @EGReference tag near attribute: "
                                            + parentObject.GetType().ToString());
                        }

                        if (!this.map.ContainsKey(childValue))
                        {
                            TagField(childValue);
                        }
                    }
                }
            }
        }
    }
}