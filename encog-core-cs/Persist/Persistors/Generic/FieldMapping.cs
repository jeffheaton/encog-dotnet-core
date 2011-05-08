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
    /// A simple mapping that holds the reference, field and target of an object.
    /// This is used internally by the object mapper to help resolve references.
    /// </summary>
    public struct FieldMapping
    {
        /// <summary>
        /// The field's reference.
        /// </summary>
        public int Reff 
        { 
            get 
            { 
                return this.reff; 
            } 
            set 
            { 
                this.reff = value; 
            } 
        }

        /// <summary>
        /// The field object.
        /// </summary>
        public FieldInfo Field 
        { 
            get 
            { 
                return this.field; 
            } 
            set 
            { 
                this.field = value; 
            } 
        }

        /// <summary>
        /// The target object, that holds the field.
        /// </summary>
        private Object target;

        /// <summary>
        /// The field's reference.
        /// </summary>
        public int reff;

        /// <summary>
        /// The field object.
        /// </summary>
        public FieldInfo field;

        /// <summary>
        /// The target object, that holds the field.
        /// </summary>
        public Object Target 
        { 
            get 
            { 
                return this.target; 
            } 
            set 
            { 
                this.target = value; 
            } 
        }

        /// <summary>
        /// Construct a field mapping.
        /// </summary>
        /// <param name="reff">The field reference.</param>
        /// <param name="field">The field.</param>
        /// <param name="target">The target that holds the field.</param>
        public FieldMapping(int reff, FieldInfo field, Object target)
        {       
            this.reff = reff;
            this.field = field;
            this.target = target;
        }

    }
}
