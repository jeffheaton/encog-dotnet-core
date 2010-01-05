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
using System.IO;

#if logging
using log4net;
#endif

namespace Encog.Persist.Location
{
    /// <summary>
    /// Create a location based on a Stream.
    /// </summary>
    public class StreamPersistence : IPersistenceLocation
    {
#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(FilePersistence));
#endif
        /// <summary>
        /// The stream to persist to/from.
        /// </summary>
        private Stream stream;

        /// <summary>
        /// Construct a persistance location based on a stream.
        /// </summary>
        /// <param name="stream">The stream to use.</param>
        public StreamPersistence(Stream stream)
        {
            this.stream = stream;
        }

        /// <summary>
        /// The stream to use.
        /// </summary>
        /// <returns>A new InputStream for this file.</returns>
        public Stream CreateStream(FileMode mode)
        {
            return this.stream;
        }

        /// <summary>
        /// Attempt to delete the file.  Will fail for this location type.
        /// </summary>
        public void Delete()
        {
        }

        /// <summary>
        /// Does the file exist?  This is always true for this location type.
        /// </summary>
        /// <returns>True if the file exists.</returns>
        public bool Exists()
        {
            return true;
        }


        /// <summary>
        /// Rename this file to a different location.
        /// </summary>
        /// <param name="toLocation">What to rename to.</param>
        public void RenameTo(IPersistenceLocation toLocation)
        {
                String str =
                   "Rename is not supported on this location type.";
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
