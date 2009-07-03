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
using log4net;
using System.IO;

namespace Encog.Persist.Location
{
    /// <summary>
    /// A persistence location based on a file.
    /// </summary>
    public class FilePersistence : PersistenceLocation
    {

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(FilePersistence));

        /// <summary>
        /// The file to persist to/from.
        /// </summary>
        private String file;

        /// <summary>
        /// Construct a persistance location based on a file.
        /// </summary>
        /// <param name="file">The file to use.</param>
        public FilePersistence(String file)
        {
            this.file = file;
        }

        /// <summary>
        /// Create a stream to a access the file.
        /// </summary>
        /// <returns>A new InputStream for this file.</returns>
        public Stream CreateStream(FileMode mode)
        {
            try
            {
                return new FileStream(this.file, mode);
            }
            catch (IOException e)
            {
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error("Exception", e);
                }
                throw new PersistError(e);
            }
        }

        /// <summary>
        /// Attempt to delete the file.
        /// </summary>
        public void Delete()
        {
            File.Delete(this.file);
        }

        /// <summary>
        /// Does the file exist?
        /// </summary>
        /// <returns>True if the file exists.</returns>
        public bool Exists()
        {
            return File.Exists(this.file);
        }

        /// <summary>
        /// The file this location is based on.
        /// </summary>
        public String FileName
        {
            get
            {
                return this.file;
            }
        }

        /// <summary>
        /// Rename this file to a different location.
        /// </summary>
        /// <param name="toLocation">What to rename to.</param>
        public void RenameTo(PersistenceLocation toLocation)
        {
            if (!(toLocation is FilePersistence))
            {
                String str =
                   "Can only rename from one FilePersistence location to another";
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
                throw new PersistError(str);
            }

            String toFile = ((FilePersistence)toLocation).FileName;

            File.Move(this.file, toFile);
        }
    }
}
