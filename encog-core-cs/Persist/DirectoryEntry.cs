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

namespace Encog.Persist
{
    /// <summary>
    /// The idea of the Encog persisted collection is that the entire file might be
    /// quite long and should not be read into memory in its entirity. Directory
    /// entry classes allow you to list the contents of a file without loading the
    /// entire file.
    /// </summary>
    public class DirectoryEntry : IComparable
    {

        /// <summary>
        /// The type of object that this is.
        /// </summary>
        private String type;

        /// <summary>
        /// The name of this object.
        /// </summary>
        private String name;

        /// <summary>
        /// The description for this object.
        /// </summary>
        private String description;

        /// <summary>
        /// Construct a directory entry for the specified object.
        /// </summary>
        /// <param name="obj">The Encog object.</param>
        public DirectoryEntry(IEncogPersistedObject obj)
        {
            String type = obj.GetType().Name;

            if (type.Equals("BasicNeuralDataSet"))
                type = EncogPersistedCollection.TYPE_TRAINING;

            this.type = type;
            this.description = obj.Description;
            this.name = obj.Name;
        }

        /// <summary>
        /// Construct a directory entry from strings.
        /// </summary>
        /// <param name="type">The type of object.</param>
        /// <param name="name">The name of this object.</param>
        /// <param name="description">The description for this object.</param>
        public DirectoryEntry(String type, String name,
                 String description)
        {
            this.type = type;
            this.name = name;
            this.description = description;
        }

        /// <summary>
        /// Compare the two objects.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>0 if equal.</returns>
        public int CompareTo(Object other)
        {
            if (other is DirectoryEntry)
            {
                DirectoryEntry other2 = (DirectoryEntry)other;
                if (other2.Type.Equals(other2.Type))
                {
                    String c = this.Name == null ? "" : this.Name;
                    return c.CompareTo(other2.Name);
                }
                else
                {
                    return CompareTo(other2.Type);
                }
            }
            else
            {
                return 1;
            }
        }


        /// <summary>
        /// Returns true if the two objects are equal.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>True if equal.</returns>
        public override bool Equals(Object other)
        {
            return CompareTo(other) == 0;
        }

        /// <summary>
        /// Create a simple hash code for this object.
        /// </summary>
        /// <returns>A simple hash code for this object.</returns>
        public override int GetHashCode()
        {
            return this.name.GetHashCode() + this.type.GetHashCode();
        }

        /// <summary>
        /// The description for this object.
        /// </summary>
        public String Description
        {
            get
            {
                return this.description;
            }
        }

        /// <summary>
        /// The name of this object.
        /// </summary>
        public String Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// The type of this object.
        /// </summary>
        public String Type
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>
        /// Convert the object to a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[DirectoryEntry:type=");
            result.Append(this.Type);
            result.Append(",name=");
            result.Append(this.Name);
            result.Append("]");
            return result.ToString();
        }

    }

}
