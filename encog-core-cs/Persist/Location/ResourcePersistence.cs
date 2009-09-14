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
using System.IO;
using System.Resources;
using System.Reflection;
#if logging
using log4net;
#endif
namespace Encog.Persist.Location
{
    /// <summary>
    /// A location that allows Encog objects to be read from a resource. This
    /// location only supports read operations, so the Encog resource is usually
    /// created first as a file and then embedded in the application as a resource.
    /// </summary>
    public class ResourcePersistence : PersistenceLocation
    {

        /// <summary>
        /// The name of the resource to read from.
        /// </summary>
        private String resource;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(ResourcePersistence));
#endif

        /// <summary>
        /// Construct a location to read from the specified resource. 
        /// </summary>
        /// <param name="resource">The resource to read from.</param>
        public ResourcePersistence(String resource)
        {
            this.resource = resource;
        }

        /// <summary>
        /// Create a stream to read the resource.
        /// </summary>
        /// <returns>A stream.</returns>
        public Stream CreateStream(FileMode mode)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(this.resource);            
            return stream;
        }

        /// <summary>
        /// Delete operations are not supported for resource persistence.
        /// </summary>
        public void Delete()
        {
            String str =
            "The ResourcePersistence location does not suppor delete operations.";
#if logging   
            if (this.logger.IsErrorEnabled)
            {
                this.logger.Error(str);
            }
#endif
            throw new PersistError(str);

        }

        /// <summary>
        /// Exist is not supported for resource persistence.
        /// </summary>
        /// <returns>Nothing.</returns>
        public bool Exists()
        {
            String str =
            "The ResourcePersistence location does not suppor exists.";
#if logging
            if (this.logger.IsErrorEnabled)
            {
                this.logger.Error(str);
            }
#endif
            throw new PersistError(str);
        }

        /// <summary>
        /// Rename is not supported for resource persistence.
        /// </summary>
        /// <param name="toLocation">Not used.</param>
        public void RenameTo(PersistenceLocation toLocation)
        {
            String str =
           "The ResourcePersistence location does not suppor rename operations.";
#if logging            
            if (this.logger.IsErrorEnabled)
            {
                this.logger.Error(str);
            }
#endif
            throw new PersistError(str);
        }

    }
}
