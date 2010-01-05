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

#if SILVERLIGHT
using Encog;
#endif 

namespace Encog.Persist
{
    /// <summary>
    /// An Encog persisted object, that can be written to XML.
    /// </summary>
    public interface IEncogPersistedObject : ICloneable
    {
        /// <summary>
        /// The description for this object.
        /// </summary>
        String Description
        {
            get;
            set;
        }

        /// <summary>
        /// The name of this object.
        /// </summary>
        String Name
        {
            get;
            set;
        }

        /// <summary>
        /// Create a persistor for this object.
        /// </summary>
        /// <returns>A persistor for this object.</returns>
        IPersistor CreatePersistor();

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>A cloned version of this object.</returns>
        new Object Clone();
    }
}
