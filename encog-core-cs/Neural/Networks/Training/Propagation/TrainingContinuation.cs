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
using Encog.Persist;
using Encog.Persist.Persistors;

namespace Encog.Neural.Networks.Training.Propagation
{
    /// <summary>
    /// Allows training to contune.
    /// </summary>
    public class TrainingContinuation : IEncogPersistedObject
    {
        /// <summary>
        /// The name of this object.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The description of this object.
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// The contents of this object.
        /// </summary>
        private IDictionary<String, Object> contents = new Dictionary<String, Object>();

        /// <summary>
        /// Obtain a persistor for this object.
        /// </summary>
        /// <returns>A persistor for this object.</returns>
        public IPersistor CreatePersistor()
        {
            return new TrainingContinuationPersistor();
        }

        /// <summary>
        /// Get the specified object.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The object.</returns>
        public Object this[String key]
        {
            get
            {
                return this.contents[key];
            }
            set
            {
                this.contents[key] = value;
            }
        }

        /// <summary>
        /// The contents.
        /// </summary>
        public IDictionary<String, Object> Contents
        {
            get
            {
                return this.contents;
            }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <returns>Not supported.</returns>
        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
